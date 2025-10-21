using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MC.Configurations
{
    public static class WorldGenerate
    {
        public static void GenerateVoxels(this ChunkData chunkData, float noiseScale, float waterThreshold)
        {
            //遍历Chunk(本地坐标系)
            for (int x = 0; x < chunkData.chunkSize; x++)
            {
                for (int z = 0; z < chunkData.chunkSize; z++)
                {
                    //对每个坐标生成一个随机高度值(为什么使用噪声，第一篇文章已经讲过了)
                    float noiseValue = Mathf.PerlinNoise((chunkData.worldPosition.x + x) * noiseScale, (chunkData.worldPosition.z + z) * noiseScale);
                    int groundPosition = Mathf.RoundToInt(noiseValue * chunkData.chunkHeight);

                    //知道该坐标的地面高度，循环遍历该坐标的所有Block
                    for (int y = 0; y < chunkData.chunkHeight; y++)
                    {
                        BlockType voxelType = Chunk.types["Dirt"];
                        //如果在地面上，进一步讨论
                        if (y > groundPosition)
                        {
                            //地面在水下，那么地面上和水下之间的Block都应该是Water
                            if (y < waterThreshold)
                            {
                                voxelType = Chunk.types["Water"];
                            }
                            //在地面上由在水面上，自然都是空气
                            else
                            {
                                voxelType = Chunk.types["Air"];
                            }

                        }
                        //水下的地面使用Sand类型的Block
                        else if (y == groundPosition && y < waterThreshold)
                        {
                            voxelType = Chunk.types["Sand"];
                        }
                        //默认的地面是Grass_Dirt类型的Block
                        else if (y == groundPosition)
                        {
                            voxelType = Chunk.types["Grass_Dirt"];
                        }

                        Chunk.SetBlock(chunkData, new Vector3Int(x, y, z), voxelType);
                    }
                }
            }
        }
    }
}