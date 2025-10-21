using MC.Configurations;
using Sirenix.OdinInspector;
using UnityEngine;
namespace MC
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Block : MonoBehaviour
    {
        [LabelText("方块类型")]
        public BlockType blockType;
        [LabelText("显示的面")]
        [EnumToggleButtons]
        public ShowDirection showDirection;
        BlockRender blockRender;
        void Start()
        {
            // blockRender = new BlockRender(GetComponent<MeshFilter>(), GetComponent<MeshRenderer>(), blockType, showDirection);
            //  gameObject.AddComponent<MeshCollider>().sharedMesh = blockRender.GetMesh();
        }
        public void Render(Vector3Int pos)
        {
            transform.position = pos;
            blockRender = new BlockRender(GetComponent<MeshFilter>(), GetComponent<MeshRenderer>(), blockType, showDirection);
            gameObject.AddComponent<MeshCollider>().sharedMesh = blockRender.GetMesh();
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (blockRender != null)
                blockRender.SetFaceEnabled(showDirection);
        }
#endif
        // 清理材质实例
        private void OnDestroy()
        {
            foreach (var face in blockRender.facesMaterial)
            {
                if (face != null)
                {
                    Destroy(face);
                }
            }
        }
    }
}