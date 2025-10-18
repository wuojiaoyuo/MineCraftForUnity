using System.Collections.Generic;
using UnityEngine;
namespace WorldGenerateByCatNine
{
    public class World : MonoBehaviour
    {
        public int mapSizeInChunks = 6;//用Chunk来衡量地图的大小，每个边多少个Chunk
        public int chunkSize = 16, chunkHeight = 100;//每个Chunk的大小和高度
        public int waterThreshold = 50;//水面的高度
        public float noiseScale = 0.03f;//影响我们的噪声值
        public GameObject chunkPrefab;//chunkPrefab预制体
                                      //存储想要在地图上生成的ChunkData
        Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
        //存储地图上Chunk的实体，与上面数据区别
        Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();


        void Start()
        {
            GenerateWorld();
        }

        /// <summary>
        /// 地图生成
        /// </summary>
        public void GenerateWorld()
        {
            chunkDataDictionary.Clear();

            foreach (ChunkRenderer chunk in chunkDictionary.Values)
            {
                Destroy(chunk.gameObject);
            }

            chunkDictionary.Clear();

            /***开始生成地图****/

            for (int x = 0; x < mapSizeInChunks; x++)
            {
                for (int z = 0; z < mapSizeInChunks; z++)
                {
                    //初始化一个Chunk，然后在一个Chunk里面循环创建Voxel并添加到字典里面
                    ChunkData data = new ChunkData(chunkSize, chunkHeight, this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
                    GenerateVoxels(data);
                    chunkDataDictionary.Add(data.worldPosition, data);
                }
            }

            //根据前面循环生成的地形数据(每个Chunk的信息)生成地形
            foreach (ChunkData data in chunkDataDictionary.Values)
            {
                MeshData meshData = Chunk.GetChunkMeshData(data);
                GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
                ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
                chunkDictionary.Add(data.worldPosition, chunkRenderer);
                chunkRenderer.InitializeChunk(data);
                chunkRenderer.UpdateChunk(meshData);

            }
        }

        //这里是对Chunk细化到Chunk内部每一个Block处理
        private void GenerateVoxels(ChunkData data)
        {
            //遍历Chunk(本地坐标系)
            for (int x = 0; x < data.chunkSize; x++)
            {
                for (int z = 0; z < data.chunkSize; z++)
                {
                    //对每个坐标生成一个随机高度值(为什么使用噪声，第一篇文章已经讲过了)
                    float noiseValue = Mathf.PerlinNoise((data.worldPosition.x + x) * noiseScale, (data.worldPosition.z + z) * noiseScale);
                    int groundPosition = Mathf.RoundToInt(noiseValue * chunkHeight);
                    //知道该坐标的地面高度，循环遍历该坐标的所有Block
                    for (int y = 0; y < chunkHeight; y++)
                    {
                        BlockType voxelType = BlockType.Dirt;
                        //如果在地面上，进一步讨论
                        if (y > groundPosition)
                        {
                            //地面在水下，那么地面上和水下之间的Block都应该是Water
                            if (y < waterThreshold)
                            {
                                voxelType = BlockType.Water;
                            }
                            //在地面上由在水面上，自然都是空气
                            else
                            {
                                voxelType = BlockType.Air;
                            }

                        }
                        //水下的地面使用Sand类型的Block
                        else if (y == groundPosition && y < waterThreshold)
                        {
                            voxelType = BlockType.Sand;
                        }
                        //默认的地面是Grass_Dirt类型的Block
                        else if (y == groundPosition)
                        {
                            voxelType = BlockType.Grass_Dirt;
                        }

                        Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                    }
                }
            }
        }

        //辅助方法，在Chunk类中用到
        internal BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
        {
            Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, x, y, z);
            ChunkData containerChunk = null;

            chunkDataDictionary.TryGetValue(pos, out containerChunk);

            if (containerChunk == null)
                return BlockType.Nothing;
            // Vector3Int blockInCHunkCoordinates = Chunk.GetBlockFromChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
            // return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInCHunkCoordinates);
            return Chunk.GetBlockFromChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        }
    }
}