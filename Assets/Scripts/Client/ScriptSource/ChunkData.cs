using System;
using UnityEngine;

namespace MC.Configurations
{
    [Serializable]
    public class ChunkData
    {
        public BlockType[] blocks;
        public int chunkSize = 16;
        public int chunkHeight = 100;
        public Vector3Int worldPosition;//每一个Chunk都有一个位置

        public ChunkData(int chunkSize, int chunkHeight, Vector3Int worldPosition)
        {
            this.chunkHeight = chunkHeight;
            this.chunkSize = chunkSize;
            this.worldPosition = worldPosition;
            blocks = new BlockType[chunkSize * chunkHeight * chunkSize];
        }
    }
}