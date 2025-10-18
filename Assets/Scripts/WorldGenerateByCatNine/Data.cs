using UnityEngine;

namespace WorldGenerateByCatNine
{
    /// <summary>
    /// 方向
    /// </summary>
    public enum Direction
    {
        up,
        down,
        right,
        left,
        foreward,
        backwards
    }
    /// <summary>
    /// 每一个Block的类型，Block即Minecraft中的一个方块
    /// </summary>
    public enum BlockType
    {
        Nothing,
        Air,
        Grass_Dirt,
        Dirt,
        Stone,
        Water,
        Sand
    }

    public class ChunkData
    {
        public BlockType[] blocks;
        public int chunkSize = 16;
        public int chunkHeight = 100;
        public World worldReference;
        public Vector3Int worldPosition;//每一个Chunk都有一个位置

        //FIXME:World还没有实现
        public ChunkData(int chunkSize, int chunkHeight, World world, Vector3Int worldPosition)
        {
            this.chunkHeight = chunkHeight;
            this.chunkSize = chunkSize;
            this.worldReference = world;
            this.worldPosition = worldPosition;
            blocks = new BlockType[chunkSize * chunkHeight * chunkSize];
        }
    }
}

