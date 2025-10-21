using Sirenix.OdinInspector;
using UnityEngine;

namespace MC.Configurations
{
    [CreateAssetMenu(fileName = "BlockDatabase", menuName = "Minecraft/BlockMesh")]
    public class BlockMeshData : SerializedScriptableObject
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uv;
        public Vector3[] normals;
        [HideLabel,InlineEditor(InlineEditorModes.LargePreview)]
        public Mesh Mesh;

        [Button("加载MESH")]
        public Mesh ToMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = name;
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.normals = normals;
            mesh.RecalculateBounds();
            this.Mesh = mesh;
            return mesh;
        }
    }
}