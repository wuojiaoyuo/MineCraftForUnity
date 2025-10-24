using System;
using MC;
using MC.Configurations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ChunkPool : MonoBehaviour
{
    private ObjectPool<Chunk> pool;
    private int poolSize = 32;
    private int maxPoolSize = 256;
    private static ChunkPool _instance;
    public static ChunkPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ChunkPool>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("ChunkPool");
                    _instance = obj.AddComponent<ChunkPool>();
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        pool = new ObjectPool<Chunk>(
            CreateChunk,
            OnGetChunk,
            OnReleaseChunk,
            OnDestroyChunk,
            false,
            poolSize,
            maxPoolSize
        );
    }

    /// <summary>
    /// 从对象池取出对象
    /// </summary>
    /// <returns></returns>
    public Chunk GetChunk()
    {
        return pool.Get();
    }
    /// <summary>
    /// 将对象放回对象池
    /// </summary>
    /// <param name="item"></param>
    public void ReleaseChunk(Chunk chunk)
    {
        pool.Release(chunk);
    }

    private void OnDestroyChunk(Chunk chunk)
    {
        Destroy(chunk.gameObject);
    }

    private void OnReleaseChunk(Chunk chunk)
    {
        chunk.Release();
        chunk.gameObject.SetActive(false);
    }

    private void OnGetChunk(Chunk chunk)
    {
        chunk.gameObject.SetActive(true);
    }

    private Chunk CreateChunk()
    {
        GameObject ChunkObj = new GameObject("Chunk");
        return ChunkObj.AddComponent<Chunk>();
    }
}
