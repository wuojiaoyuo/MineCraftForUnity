using Sirenix.OdinInspector;
using UnityEngine;

namespace MC.Configurations
{
    [CreateAssetMenu(fileName = "BlockDatabase", menuName = "Minecraft/Mesh Database")]
    public class MeshData : SerializedScriptableObject
    {
        public string MeshName;
        public Vector3[] vertices = new Vector3[4]
{
        new Vector3(-0.5f, 0.5f, 0.5f),   // 3: 前上左
        new Vector3(0.5f, 0.5f, 0.5f),    // 2: 前上右
        new Vector3(0.5f, 0.5f, -0.5f),   // 6: 后上右
        new Vector3(-0.5f, 0.5f, -0.5f)   // 7: 后上左
};
        public int[] triangles = { 0, 1, 2, 0, 2, 3 };
    }
}