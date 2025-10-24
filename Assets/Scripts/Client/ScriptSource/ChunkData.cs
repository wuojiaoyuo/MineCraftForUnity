using System;
using UnityEngine;

namespace MC.Configurations
{
    [Serializable]
    public class ChunkData
    {
        public string[,,] BlockIDs;
        public int chunkSize = 16;
        public int chunkHeight = 100;
        public Vector3 worldPosition;//每一个Chunk都有一个位置
        public Mesh ChunkMesh;
        public ChunkData(int chunkSize, int chunkHeight, Vector3 worldPosition)
        {
            this.chunkHeight = chunkHeight;
            this.chunkSize = chunkSize;
            this.worldPosition = worldPosition;
            BlockIDs = new string[chunkSize, chunkHeight, chunkSize];
        }

#if UNITY_EDITOR
        public void SaveMesh()
        {

        }
#endif
    }
}