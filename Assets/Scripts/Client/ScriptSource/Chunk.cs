using MC.Configurations;
using UnityEngine;

namespace MC
{
    public class Chunk : MonoBehaviour
    {
        public ChunkData chunkData;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;
        public MeshFilter meshFilter;

        public void Init(ChunkData chunkData)
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();

            if (meshFilter == null)
                meshFilter = gameObject.AddComponent<MeshFilter>();
            if (meshRenderer == null)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            if (meshCollider == null)
                meshCollider = gameObject.AddComponent<MeshCollider>();

            this.chunkData = chunkData;
            this.transform.position = chunkData.worldPosition;

            meshFilter.mesh = chunkData.ChunkMesh;
            meshCollider.sharedMesh = chunkData.ChunkMesh;

            meshRenderer.enabled = true;
            meshCollider.enabled = true;
        }
        public void Release()
        {
            chunkData = null;

            meshRenderer.enabled = false;
            meshCollider.enabled = false;
        }
    }
}
