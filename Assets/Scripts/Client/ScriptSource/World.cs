using System.Collections.Generic;
using System.Data;
using MC.Configurations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using SimplexNoise;
using System.Collections;

namespace MC
{
    public class World : MonoBehaviour
    {
        //存储着世界中所有的Chunk
        public static List<ChunkData> chunks;
        public int ChunkWidth; //每个Chunk的长宽Size
        public int ChunkHeight;//每个Chunk的高度
        public int baseHeight = 9;//最小生成高度
        public int waterHeight = 10;//水面高度
        public int Seed;//随机种子
        public float frequency = 0.025f; //噪音频率（噪音采样时会用到）
        public float amplitude = 1; //噪音振幅（噪音采样时会用到）
        public Material WorldMaterial;



        //噪音采样时会用到的偏移
        Vector3 offset0;
        Vector3 offset1;
        Vector3 offset2;

        public static Dictionary<string, BlockType> types = new Dictionary<string, BlockType>();
        bool typesHasLoaded = false;
        public static World Instance { get; private set; }
        void Awake()
        {
            chunks = new List<ChunkData>();
            if (Instance == null)
                Instance = this;
        }
        #region 加载方块类型资源
        public void LoadAllAssetsByLabel()
        {
            // 开始异步加载所有带有该标签的资源
            AsyncOperationHandle<IList<BlockType>> handle =
                Addressables.LoadAssetsAsync<BlockType>("Block", null);

            handle.Completed += OnAllAssetsLoaded;
        }

        private void OnAllAssetsLoaded(AsyncOperationHandle<IList<BlockType>> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                IList<BlockType> assets = handle.Result;
                foreach (BlockType asset in assets)
                {
                    if (!types.ContainsKey(asset.blockProperties.Block_Name))
                        types.Add(asset.blockProperties.Block_Name, asset);
                    Debug.Log($"加载资源: {asset.name}");
                    // 实例化或处理资源
                }
            }
            else
            {
                Debug.LogError($"加载失败: {handle.OperationException}");
            }
            Addressables.Release(handle);
            typesHasLoaded = true;
        }
        #endregion
        IEnumerator Start()
        {
            LoadAllAssetsByLabel();
            yield return new WaitUntil(() => typesHasLoaded);
            InitOffset();

        }

        public static void UpdateWorld(Vector3 PlayerPos)
        {
            int viewRange = 30;//TODO:全局配置
            for (float x = PlayerPos.x - viewRange; x < PlayerPos.x + viewRange; x += World.Instance.ChunkWidth)
            {
                for (float z = PlayerPos.z - viewRange; z < PlayerPos.z + viewRange; z += World.Instance.ChunkWidth)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    pos.x = Mathf.Floor(pos.x / (float)World.Instance.ChunkWidth) * World.Instance.ChunkWidth;
                    pos.z = Mathf.Floor(pos.z / (float)World.Instance.ChunkWidth) * World.Instance.ChunkWidth;

                    ChunkData chunkData = World.GetChunk(pos);
                    if (chunkData != null) continue;

                    //chunk = (Chunk)Instantiate(chunkPrefab, pos, Quaternion.identity);

                    World.Instance.AddChunk(pos);
                }
            }
        }

        public void AddChunk(Vector3 pos)
        {
            if (!typesHasLoaded) return;

            ChunkData chunkData = new ChunkData(ChunkWidth, ChunkHeight, pos);
            BuildChunk(chunkData);
            chunks.Add(chunkData);


            Chunk chunk = ChunkPool.Instance.GetChunk();
            chunk.gameObject.name = $"{chunks.Count}_Chunk_[{pos.x},{pos.z}]";
            chunk.transform.SetParent(this.transform);
            chunk.Init(chunkData);
            chunk.meshRenderer.material = WorldMaterial;
        }


        public void BuildChunk(ChunkData chunkData)
        {
            chunkData.ChunkMesh = new Mesh();
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int y = 0; y < ChunkHeight; y++)
                {
                    for (int z = 0; z < ChunkWidth; z++)
                    {
                        chunkData.BlockIDs[x, y, z] = GenerateBlockType(new Vector3(x, y, z) + chunkData.worldPosition);
                    }
                }
            }
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int y = 0; y < ChunkHeight; y++)
                {
                    for (int z = 0; z < ChunkWidth; z++)
                    {
                        chunkData.BuildBlock(x, y, z, verts, uvs, tris);
                    }
                }
            }
            chunkData.ChunkMesh.vertices = verts.ToArray();
            chunkData.ChunkMesh.uv = uvs.ToArray();
            chunkData.ChunkMesh.triangles = tris.ToArray();
            chunkData.ChunkMesh.RecalculateBounds();
            chunkData.ChunkMesh.RecalculateNormals();
        }

        private void InitOffset()
        {
            //初始化随机种子
            Random.InitState(Seed);

            offset0 = new Vector3(Random.value * 1000, Random.value * 1000, Random.value * 1000);
            offset1 = new Vector3(Random.value * 1000, Random.value * 1000, Random.value * 1000);
            offset2 = new Vector3(Random.value * 1000, Random.value * 1000, Random.value * 1000);
        }
        private int GenerateHeight(Vector3 wPos)
        {

            //让随机种子，振幅，频率，应用于我们的噪音采样结果
            float x0 = (wPos.x + offset0.x) * frequency;
            float y0 = (wPos.y + offset0.y) * frequency;
            float z0 = (wPos.z + offset0.z) * frequency;

            float x1 = (wPos.x + offset1.x) * frequency * 2;
            float y1 = (wPos.y + offset1.y) * frequency * 2;
            float z1 = (wPos.z + offset1.z) * frequency * 2;

            float x2 = (wPos.x + offset2.x) * frequency / 4;
            float y2 = (wPos.y + offset2.y) * frequency / 4;
            float z2 = (wPos.z + offset2.z) * frequency / 4;

            float noise0 = Noise.Generate(x0, y0, z0) * amplitude;
            float noise1 = Noise.Generate(x1, y1, z1) * amplitude / 2;
            float noise2 = Noise.Generate(x2, y2, z2) * amplitude / 4;

            //在采样结果上，叠加上baseHeight，限制随机生成的高度下限
            return Mathf.FloorToInt(noise0 + noise1 + noise2 + baseHeight);
        }

        public static ChunkData GetChunk(Vector3 wPos)
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                Vector3 tempPos = chunks[i].worldPosition;

                //wPos是否超出了Chunk的XZ平面的范围
                if ((wPos.x < tempPos.x) || (wPos.z < tempPos.z) || (wPos.x >= tempPos.x + chunks[i].chunkSize) || (wPos.z >= tempPos.z + chunks[i].chunkSize))
                    continue;

                return chunks[i];
            }
            return null;
        }

        public string GenerateBlockType(Vector3 wPos)
        {
            //y坐标是否在Chunk内
            if (wPos.y >= ChunkHeight)
            {
                return "Air";
            }

            //获取当前位置方块随机生成的高度值
            float genHeight = GenerateHeight(wPos);

            //当前方块位置高于随机生成的高度值时，当前方块类型为空
            if (wPos.y > genHeight)
            {
                return "Air";
            }
            //当前方块位置等于随机生成的高度值时，当前方块类型为草地
            else if (wPos.y == genHeight)
            {
                return "Grass_Dirt";
            }
            //当前方块位置小于随机生成的高度值 且 大于 genHeight - 5时，当前方块类型为泥土
            else if (wPos.y < genHeight && wPos.y > genHeight - 5)
            {
                return "Dirt";
            }
            else if (wPos.y == 0)
            {
                return "BedRock";
            }
            //其他情况，当前方块类型为碎石
            return "Stone";
        }

    }
}