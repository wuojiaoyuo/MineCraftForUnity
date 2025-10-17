using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 保存Face的Mesh数据信息
/// </summary>
public class MeshData
{
    //用List而不用数组是为了更容易添加新的顶点进去
    /// <summary>
    /// 顶点
    /// </summary>
    public List<Vector3> vertices = new List<Vector3>();
    /// <summary>
    /// 三角面
    /// </summary>
    public List<int> triangles = new List<int>();
    /// <summary>
    /// UV
    /// </summary>
    public List<Vector2> uv = new List<Vector2>();

    //单独出来是因为有些Face是需要渲染出来而不需要有碰撞，比如Water
    /// <summary>
    /// 碰撞盒顶点
    /// </summary>
    public List<Vector3> colliderVertices = new List<Vector3>();
    /// <summary>
    /// 碰撞盒三角面
    /// </summary>
    public List<int> colliderTriangles = new List<int>();

    /// <summary>
    /// 水面mesh
    /// </summary>
    public MeshData waterMesh;
    /// <summary>
    /// 非水面Mesh判断
    /// </summary>
    private bool isMainMesh = true;//不是水方块

    /// <summary>
    /// 为了确定每个Chunk上的Mesh，
    /// isMainMesh是true就执行waterMesh的构造函数，
    /// 但传入的是false,也就是waterMesh将会是空
    /// </summary>
    /// <param name="_isMainMesh"></param>
    public MeshData(bool _isMainMesh)
    {
        if (_isMainMesh)
        {
            waterMesh = new MeshData(false);
        }
    }

    /// <summary>
    /// 添加顶点
    /// </summary>
    /// <param name="vertex"></param>
    /// <param name="vertexGeneratesCollider"></param>
    public void AddVertex(Vector3 vertex, bool vertexGeneratesCollider)
    {
        vertices.Add(vertex);
        if (vertexGeneratesCollider)
        {
            colliderVertices.Add(vertex);
        }
    }

    /// <summary>
    /// 添加三角面，一个mesh实际就是很多个三角面
    /// </summary>
    /// <param name="quadGeneratesCollider"></param>
    public void AddQuadTriangles(bool quadGeneratesCollider)
    {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);

        //如果要生成碰撞，需要将顶点再加入到colliderVertices
        if (quadGeneratesCollider)
        {
            colliderTriangles.Add(colliderVertices.Count - 4);
            colliderTriangles.Add(colliderVertices.Count - 3);
            colliderTriangles.Add(colliderVertices.Count - 2);

            colliderTriangles.Add(colliderVertices.Count - 4);
            colliderTriangles.Add(colliderVertices.Count - 2);
            colliderTriangles.Add(colliderVertices.Count - 1);
        }
    }

}
