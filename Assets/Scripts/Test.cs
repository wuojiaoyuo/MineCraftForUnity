using MC.Configurations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Test : MonoBehaviour
{
    public MeshFilter meshFilter;
    public BlockMeshData blockMeshData;
    public BlockType blockType;

    void Start()
    {
        Addressables.LoadAssetAsync<BlockType>("Assets/_minecraft/Block/Block/Grass_Dirt.asset").Completed += (handle) =>
       {
           blockType = handle.Result;
       };
    }

    [Button]
    public void Run()
    {
        meshFilter.mesh = blockMeshData.ToMesh();
    }
}
