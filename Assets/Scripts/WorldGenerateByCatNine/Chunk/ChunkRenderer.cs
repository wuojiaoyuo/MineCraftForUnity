using System.Linq;
using UnityEngine;
namespace WorldGenerateByCatNine
{
    public class ChunkRenderer : MonoBehaviour
    {
        MeshFilter meshFilter;
        MeshCollider meshCollider;
        Mesh mesh;

        public bool showGizmo = false;
        public ChunkData ChunkData { get; private set; }

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
            mesh = meshFilter.mesh;
        }
        /// <summary>
        /// 初始化Chunk
        /// </summary>
        /// <param name="data"></param>
        public void InitializeChunk(ChunkData data)
        {
            this.ChunkData = data;
        }

        /// <summary>
        /// 根据meshData渲染
        /// </summary>
        /// <param name="meshData"></param>
        private void RenderMesh(MeshData meshData)
        {
            mesh.Clear();

            mesh.subMeshCount = 2; //subMesh也叫子网格区分水和地面等

            //将该Block的MeshData的顶点和MeshData里的waterMesh的顶点都加入到该Mesh的顶点数组
            mesh.vertices = meshData.vertices.Concat(meshData.waterMesh.vertices).ToArray();

            mesh.SetTriangles(meshData.triangles.ToArray(), 0);
            //由于该Mesh的顶点数组有两部分构成，所以我们应该加上数组的长度来获得waterMesh的顶点，最后的那个1代表是哪个subMesh
            mesh.SetTriangles(meshData.waterMesh.triangles.Select(val => val + meshData.vertices.Count).ToArray(), 1);

            mesh.uv = meshData.uv.Concat(meshData.waterMesh.uv).ToArray();
            mesh.RecalculateNormals();//重新计算网格的法线向量


            meshCollider.sharedMesh = null;
            Mesh collisionMesh = new Mesh();
            collisionMesh.vertices = meshData.colliderVertices.ToArray();
            collisionMesh.triangles = meshData.colliderTriangles.ToArray();
            collisionMesh.RecalculateNormals();

            meshCollider.sharedMesh = collisionMesh;
        }

        //公共方法来执行上面的RenderMesh
        public void UpdateChunk()
        {
            RenderMesh(Chunk.GetChunkMeshData(ChunkData));
        }

        public void UpdateChunk(MeshData data)
        {
            RenderMesh(data);
        }
    }
}
