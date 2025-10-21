using System;
using System.Collections;
using System.Collections.Generic;
using MC.Configurations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace MC
{
    public class Chunk : MonoBehaviour
    {
        public ChunkData chunkData;
        public static Dictionary<string, BlockType> types = new Dictionary<string, BlockType>();
        bool Loaded = false;
        public void LoadAllAssetsByLabel(string label)
        {
            // 开始异步加载所有带有该标签的资源
            AsyncOperationHandle<IList<BlockType>> handle =
                Addressables.LoadAssetsAsync<BlockType>(label, null);

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
            Loaded = true;
        }
        IEnumerator Start()
        {
            LoadAllAssetsByLabel("Block");
            yield return new WaitUntil(() => Loaded);
            chunkData = new ChunkData(16, 100, new Vector3Int(0, 0, 0));
            WorldGenerate.GenerateVoxels(chunkData, .03f, 50);
            for (int j = 0; j < chunkData.blocks.Length; j = j + 100)
            {
                for (int i = j; i < j + 100; i++)
                {
                    GameObject block = new GameObject(chunkData.blocks[i].blockProperties.Block_Name);
                    Block blockrender = block.AddComponent<Block>();
                    blockrender.showDirection = ShowDirection.All;
                    blockrender.blockType = chunkData.blocks[i];
                    blockrender.Render(GetPostitionFromIndex(chunkData, i));
                }
                yield return null;
            }
        }
        public static void SetBlock(ChunkData chunkData, Vector3Int localPosition, BlockType block)
        {
            if (InRange(chunkData, localPosition.x) && InRangeHeight(chunkData, localPosition.y) && InRange(chunkData, localPosition.z))
            {
                int index = GetIndexFromPosition(chunkData, localPosition.x, localPosition.y, localPosition.z);
                chunkData.blocks[index] = block;
            }
            else
            {
                throw new Exception("Need to ask World for appropiate chunk");
            }
        }

        private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z)
        {
            return x + chunkData.chunkSize * y + chunkData.chunkSize * chunkData.chunkHeight * z;
        }

        private static Vector3Int GetPostitionFromIndex(ChunkData chunkData, int index)
        {
            int x = index % chunkData.chunkSize;
            int y = (index / chunkData.chunkSize) % chunkData.chunkHeight;
            int z = index / (chunkData.chunkSize * chunkData.chunkHeight);
            return new Vector3Int(x, y, z);
        }

        /// <summary>
        /// 在Chunk坐标系下，判断Block是否超出所属Chunk的界限
        /// </summary>
        /// <param name="chunkData"></param>
        /// <param name="axisCoordinate"></param>
        /// <returns></returns>
        private static bool InRange(ChunkData chunkData, int axisCoordinate)
        {
            if (axisCoordinate < 0 || axisCoordinate >= chunkData.chunkSize)
                return false;

            return true;
        }

        /// <summary>
        /// 在Chunk坐标系下，判断Block是否超出所属Chunk的界限（高度）
        /// </summary>
        /// <param name="chunkData"></param>
        /// <param name="ycoordinate"></param>
        /// <returns></returns>
        private static bool InRangeHeight(ChunkData chunkData, int ycoordinate)
        {
            if (ycoordinate < 0 || ycoordinate >= chunkData.chunkHeight)
                return false;

            return true;
        }

    }
}
