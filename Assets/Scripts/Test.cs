using System.Collections.Generic;
using MC;
using MC.Configurations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Test : MonoBehaviour
{
    public float viewRange = 30;
    void Update()
    {
        UpdateWorld();
    }
    [Button]
    void UpdateWorld()
    {
        for (float x = transform.position.x - viewRange; x < transform.position.x + viewRange; x += World.Instance.ChunkWidth)
        {
            for (float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z += World.Instance.ChunkWidth)
            {
                Vector3 pos = new Vector3(x, 0, z);
                pos.x = Mathf.Floor(pos.x / (float)World.Instance.ChunkWidth) * World.Instance.ChunkWidth;
                pos.z = Mathf.Floor(pos.z / (float)World.Instance.ChunkWidth) * World.Instance.ChunkWidth;

                Chunk chunk = World.GetChunk(pos);
                if (chunk != null) continue;

                //chunk = (Chunk)Instantiate(chunkPrefab, pos, Quaternion.identity);

                World.Instance.AddChunk(pos);
            }
        }
    }
}
