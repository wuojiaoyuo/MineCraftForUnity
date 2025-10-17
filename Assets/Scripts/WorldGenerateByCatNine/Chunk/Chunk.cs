using System;
using MineCraft;
using UnityEngine;

public static class Chunk
{
    /// <summary>
    /// 遍历一个Chunk中的所有Block
    /// </summary>
    /// <param name="chunkData"></param>
    /// <param name="actionToPerform"></param>
    public static void LoopThroughTheBlocks(ChunkData chunkData, Action<int, int, int> actionToPerform)
    {
        for (int index = 0; index < chunkData.blocks.Length; index++)
        {
            var position = GetPostitionFromIndex(chunkData, index);
            actionToPerform(position.x, position.y, position.z);//对方块进行的改变
        }
    }

    /// <summary>
    /// 根据Block的位置得到在Block数组中的位置
    /// 将一维索引转换为三维坐标
    /// </summary>
    /// <param name="chunkData"></param>
    /// <param name="index"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 根据Chunk坐标系的坐标得出该Block的类型
    /// </summary>
    /// <param name="chunkData"></param>
    /// <param name="chunkCoordinates"></param>
    /// <returns></returns>
    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates)
    {
        return GetBlockFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }

    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        if (InRange(chunkData, x) && InRangeHeight(chunkData, y) && InRange(chunkData, z))
        {
            int index = GetIndexFromPosition(chunkData, x, y, z);
            return chunkData.blocks[index];
        }

        return chunkData.worldReference.GetBlockFromChunkCoordinates(chunkData, chunkData.worldPosition.x + x, chunkData.worldPosition.y + y, chunkData.worldPosition.z + z);
    }

    /// <summary>
    /// 设置Block的类型
    /// </summary>
    /// <param name="chunkData"></param>
    /// <param name="localPosition"></param>
    /// <param name="block"></param>
    /// <exception cref="Exception"></exception>
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

    /// <summary>
    /// 从一个Block的坐标得到在数组中的位置 x + chunkSize * y + chunkSize * chunkHeight * z
    /// </summary>
    /// <param name="chunkData"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z)
    {
        return x + chunkData.chunkSize * y + chunkData.chunkSize * chunkData.chunkHeight * z;
    }

    /// <summary>
    /// 获得MeshData的方法
    /// </summary>
    /// <param name="chunkData"></param>
    /// <returns></returns>
    public static MeshData GetChunkMeshData(ChunkData chunkData)
    {
        MeshData meshData = new MeshData(true);

        LoopThroughTheBlocks(chunkData, (x, y, z) => meshData = BlockHelper.GetMeshData(chunkData, x, y, z, meshData, chunkData.blocks[GetIndexFromPosition(chunkData, x, y, z)]));


        return meshData;
    }

    internal static Vector3Int ChunkPositionFromBlockCoords(World world, int x, int y, int z)
    {
        Vector3Int pos = new Vector3Int
        {
            x = Mathf.FloorToInt(x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(y / (float)world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(z / (float)world.chunkSize) * world.chunkSize
        };
        return pos;
    }
}