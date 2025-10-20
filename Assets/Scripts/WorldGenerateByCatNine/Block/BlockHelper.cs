using System;
using UnityEngine;

namespace WorldGenerateByCatNine
{
    public static class DirectionExtensions
    {
        public static Vector3Int GetVector(this Direction direction)
        {
            return direction switch
            {
                Direction.up => Vector3Int.up,
                Direction.down => Vector3Int.down,
                Direction.right => Vector3Int.right,
                Direction.left => Vector3Int.left,
                Direction.foreward => Vector3Int.forward,
                Direction.backwards => Vector3Int.back,
                _ => throw new Exception("Invalid input direction")
            };
        }
    }

    /// <summary>
    /// 填充MeshData(MeshData是面优化后的)
    /// </summary>
    public static class BlockHelper
    {
        private static Direction[] directions =
        {
        Direction.backwards,
        Direction.down,
        Direction.foreward,
        Direction.left,
        Direction.right,
        Direction.up
    };

        /// <summary>
        /// 获取MeshData同时还关心邻居方块的状态
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="meshData"></param>
        /// <param name="blockType"></param>
        /// <returns></returns>
        public static MeshData GetMeshData(ChunkData chunk, int x, int y, int z, MeshData meshData, BlockType blockType)
        {
            if (blockType == BlockType.Air || blockType == BlockType.Nothing)
                return meshData;
            foreach (Direction direction in directions)
            {
                Vector3Int neighbourBlockCoordinates = new Vector3Int(x, y, z) + direction.GetVector();//获取邻居Block的坐标
                BlockType neighbourBlockType = Chunk.GetBlockFromChunkCoordinates(chunk, neighbourBlockCoordinates);//获取邻居Block的类型

                //优化部分
                //if (neighbourBlockType != BlockType.Nothing && BlockDataManager.blockTextureDataDictionary[neighbourBlockType].isSolid == false)
                if (BlockDataManager.blockTextureDataDictionary[neighbourBlockType].isSolid == false)
                {
                    if (blockType == BlockType.Water)
                    {
                        if (neighbourBlockType == BlockType.Air)
                        {
                            meshData.waterMesh = GetFaceDataIn(direction, chunk, x, y, z, meshData.waterMesh, blockType);
                        }
                    }
                    else
                    {
                        meshData = GetFaceDataIn(direction, chunk, x, y, z, meshData, blockType);
                    }
                }
            }

            return meshData;
        }

        /// <summary>
        /// 填充MeshData,也就是给顶点，三角形，UV赋值
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="chunk"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="meshData"></param>
        /// <param name="blockType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static MeshData GetFaceDataIn(Direction direction, ChunkData chunk, int x, int y, int z, MeshData meshData, BlockType blockType)
        {
            GetFaceVertices(direction, x, y, z, meshData, blockType);
            meshData.AddQuadTriangles(BlockDataManager.blockTextureDataDictionary[blockType].generatesCollider);
            meshData.uv.AddRange(FaceUVs(direction, blockType));


            return meshData;
        }

        /// <summary>
        /// 正方体的顶点
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="meshData"></param>
        /// <param name="blockType"></param>
        public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData, BlockType blockType)
        {
            var generatesCollider = BlockDataManager.blockTextureDataDictionary[blockType].generatesCollider;

            switch (direction)
            {
                case Direction.backwards:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    break;
                case Direction.foreward:
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    break;
                case Direction.left:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    break;

                case Direction.right:
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    break;
                case Direction.down:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    break;
                case Direction.up:
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 给定direction就可以确定一个面，利用四个顶点来进行UV映射
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="blockType"></param>
        /// <returns></returns>
        public static Vector2[] FaceUVs(Direction direction, BlockType blockType)
        {
            Vector2[] UVs = new Vector2[4];
            var tilePos = TexturePosition(direction, blockType);

            UVs[0] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.tileSizeX - BlockDataManager.textureOffset,
                BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.textureOffset);

            UVs[1] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.tileSizeX - BlockDataManager.textureOffset,
                BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.tileSizeY - BlockDataManager.textureOffset);

            UVs[2] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.textureOffset,
                BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.tileSizeY - BlockDataManager.textureOffset);

            UVs[3] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.textureOffset,
                BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.textureOffset);

            return UVs;
        }

        /// <summary>
        /// 根据方向和Block类型确定纹理
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="blockType"></param>
        /// <returns></returns>
        public static Vector2Int TexturePosition(Direction direction, BlockType blockType)
        {
            return direction switch
            {
                Direction.up => BlockDataManager.blockTextureDataDictionary[blockType].up,
                Direction.down => BlockDataManager.blockTextureDataDictionary[blockType].down,
                _ => BlockDataManager.blockTextureDataDictionary[blockType].side
            };
        }
    }
}