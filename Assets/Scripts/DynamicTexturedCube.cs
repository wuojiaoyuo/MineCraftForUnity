using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DynamicMultiMaterialCube : MonoBehaviour
{
    [System.Serializable]
    public class CubeFace
    {
        public bool enabled = true;
        public Material material;
        public Texture2D texture;
        [Range(0.1f, 2f)] public float textureScale = 1f;
        public Vector2 textureOffset = Vector2.zero;
        
        [System.NonSerialized] public Material materialInstance;
    }

    [Header("方块设置")]
    public Vector3 centerPosition = Vector3.zero;
    public Vector3 size = Vector3.one;
    
    [Header("面设置")]
    public CubeFace[] faces = new CubeFace[6]
    {
        new CubeFace(), // 前面 (0)
        new CubeFace(), // 后面 (1)
        new CubeFace(), // 上面 (2)
        new CubeFace(), // 下面 (3)
        new CubeFace(), // 右面 (4)
        new CubeFace()  // 左面 (5)
    };

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private List<Material> activeMaterials = new List<Material>();

    // 基础顶点定义（局部坐标）
    private Vector3[] baseVertices = new Vector3[8]
    {
        new Vector3(-0.5f, -0.5f, 0.5f),  // 0: 前下左
        new Vector3(0.5f, -0.5f, 0.5f),   // 1: 前下右
        new Vector3(0.5f, 0.5f, 0.5f),    // 2: 前上右
        new Vector3(-0.5f, 0.5f, 0.5f),   // 3: 前上左
        new Vector3(-0.5f, -0.5f, -0.5f), // 4: 后下左
        new Vector3(0.5f, -0.5f, -0.5f),  // 5: 后下右
        new Vector3(0.5f, 0.5f, -0.5f),   // 6: 后上右
        new Vector3(-0.5f, 0.5f, -0.5f)   // 7: 后上左
    };

    void Start()
    {
        InitializeComponents();
        GenerateCube();
    }

    private void InitializeComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
        
        mesh = new Mesh();
        mesh.name = "DynamicMultiMaterialCube";
        meshFilter.mesh = mesh;
    }

    [ContextMenu("生成方块")]
    public void GenerateCube()
    {
        if (mesh == null) InitializeComponents();
        
        // 应用位置和尺寸
        transform.position = centerPosition;
        transform.localScale = size;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        activeMaterials.Clear();

        // 存储每个面的三角形索引
        List<int>[] faceTriangles = new List<int>[6];
        for (int i = 0; i < 6; i++) faceTriangles[i] = new List<int>();
        
        int currentVertexIndex = 0;
        int subMeshIndex = 0;

        // 前面 (Z正方向)
        if (faces[0].enabled)
        {
            AddFace(vertices, normals, uvs, 
                    new int[] { 0, 1, 2, 3 }, Vector3.forward, 
                    faces[0].textureScale, faces[0].textureOffset, 
                    faceTriangles[0], ref currentVertexIndex);
            
            CreateFaceMaterial(0);
            activeMaterials.Add(faces[0].materialInstance);
            subMeshIndex++;
        }

        // 后面 (Z负方向)
        if (faces[1].enabled)
        {
            AddFace(vertices, normals, uvs, 
                    new int[] { 5, 4, 7, 6 }, Vector3.back, 
                    faces[1].textureScale, faces[1].textureOffset, 
                    faceTriangles[1], ref currentVertexIndex);
            
            CreateFaceMaterial(1);
            activeMaterials.Add(faces[1].materialInstance);
            subMeshIndex++;
        }

        // 上面 (Y正方向)
        if (faces[2].enabled)
        {
            AddFace(vertices, normals, uvs, 
                    new int[] { 3, 2, 6, 7 }, Vector3.up, 
                    faces[2].textureScale, faces[2].textureOffset, 
                    faceTriangles[2], ref currentVertexIndex);
            
            CreateFaceMaterial(2);
            activeMaterials.Add(faces[2].materialInstance);
            subMeshIndex++;
        }

        // 下面 (Y负方向)
        if (faces[3].enabled)
        {
            AddFace(vertices, normals, uvs, 
                    new int[] { 1, 0, 4, 5 }, Vector3.down, 
                    faces[3].textureScale, faces[3].textureOffset, 
                    faceTriangles[3], ref currentVertexIndex);
            
            CreateFaceMaterial(3);
            activeMaterials.Add(faces[3].materialInstance);
            subMeshIndex++;
        }

        // 右面 (X正方向)
        if (faces[4].enabled)
        {
            AddFace(vertices, normals, uvs, 
                    new int[] { 1, 5, 6, 2 }, Vector3.right, 
                    faces[4].textureScale, faces[4].textureOffset, 
                    faceTriangles[4], ref currentVertexIndex);
            
            CreateFaceMaterial(4);
            activeMaterials.Add(faces[4].materialInstance);
            subMeshIndex++;
        }

        // 左面 (X负方向)
        if (faces[5].enabled)
        {
            AddFace(vertices, normals, uvs, 
                    new int[] { 4, 0, 3, 7 }, Vector3.left, 
                    faces[5].textureScale, faces[5].textureOffset, 
                    faceTriangles[5], ref currentVertexIndex);
            
            CreateFaceMaterial(5);
            activeMaterials.Add(faces[5].materialInstance);
            subMeshIndex++;
        }

        // 应用网格数据
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        
        // 设置子网格数量和三角形
        mesh.subMeshCount = subMeshIndex;
        int activeSubMesh = 0;
        
        for (int i = 0; i < 6; i++)
        {
            if (faces[i].enabled && faceTriangles[i].Count > 0)
            {
                mesh.SetTriangles(faceTriangles[i].ToArray(), activeSubMesh);
                activeSubMesh++;
            }
        }
        
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        // 设置材质
        meshRenderer.materials = activeMaterials.ToArray();
    }

    private void CreateFaceMaterial(int faceIndex)
    {
        if (faces[faceIndex].materialInstance != null) return;
        
        if (faces[faceIndex].material != null)
        {
            faces[faceIndex].materialInstance = new Material(faces[faceIndex].material);
        }
        else
        {
            faces[faceIndex].materialInstance = new Material(Shader.Find("Standard"));
        }
        
        if (faces[faceIndex].texture != null)
        {
            faces[faceIndex].materialInstance.mainTexture = faces[faceIndex].texture;
        }
    }

    private void AddFace(List<Vector3> vertices, List<Vector3> normals, 
                        List<Vector2> uvs, int[] vertexIndices, Vector3 normal, 
                        float textureScale, Vector2 textureOffset, 
                        List<int> faceTriangles, ref int startIndex)
    {
        // 添加四个顶点
        for (int i = 0; i < 4; i++)
        {
            vertices.Add(baseVertices[vertexIndices[i]]);
            normals.Add(normal);
        }
        
        // 设置UV坐标
        uvs.Add(new Vector2(0, 0) * textureScale + textureOffset);
        uvs.Add(new Vector2(1, 0) * textureScale + textureOffset);
        uvs.Add(new Vector2(1, 1) * textureScale + textureOffset);
        uvs.Add(new Vector2(0, 1) * textureScale + textureOffset);
        
        // 添加两个三角形
        faceTriangles.Add(startIndex);
        faceTriangles.Add(startIndex + 1);
        faceTriangles.Add(startIndex + 2);
        
        faceTriangles.Add(startIndex);
        faceTriangles.Add(startIndex + 2);
        faceTriangles.Add(startIndex + 3);
        
        startIndex += 4;
    }

    // 公共方法：设置面状态
    public void SetFaceEnabled(int faceIndex, bool enabled)
    {
        if (faceIndex >= 0 && faceIndex < faces.Length)
        {
            faces[faceIndex].enabled = enabled;
            GenerateCube();
        }
    }

    // 公共方法：设置面材质
    public void SetFaceMaterial(int faceIndex, Material material)
    {
        if (faceIndex >= 0 && faceIndex < faces.Length)
        {
            faces[faceIndex].material = material;
            
            if (faces[faceIndex].materialInstance != null)
            {
                Destroy(faces[faceIndex].materialInstance);
                faces[faceIndex].materialInstance = null;
            }
            
            GenerateCube();
        }
    }

    // 公共方法：设置面纹理
    public void SetFaceTexture(int faceIndex, Texture2D texture)
    {
        if (faceIndex >= 0 && faceIndex < faces.Length)
        {
            faces[faceIndex].texture = texture;
            
            if (faces[faceIndex].materialInstance != null)
            {
                faces[faceIndex].materialInstance.mainTexture = texture;
            }
        }
    }

    // 公共方法：设置面纹理缩放
    public void SetFaceTextureScale(int faceIndex, float scale)
    {
        if (faceIndex >= 0 && faceIndex < faces.Length)
        {
            faces[faceIndex].textureScale = scale;
            GenerateCube();
        }
    }

    // 公共方法：设置面纹理偏移
    public void SetFaceTextureOffset(int faceIndex, Vector2 offset)
    {
        if (faceIndex >= 0 && faceIndex < faces.Length)
        {
            faces[faceIndex].textureOffset = offset;
            GenerateCube();
        }
    }

    // 公共方法：设置方块位置
    public void SetPosition(Vector3 position)
    {
        centerPosition = position;
        transform.position = position;
    }

    // 公共方法：设置方块尺寸
    public void SetSize(Vector3 newSize)
    {
        size = newSize;
        transform.localScale = newSize;
    }

    // 在Inspector中修改时自动更新
    private void OnValidate()
    {
        if (mesh != null && Application.isPlaying)
        {
            GenerateCube();
        }
    }

    // 清理材质实例
    private void OnDestroy()
    {
        foreach (var face in faces)
        {
            if (face.materialInstance != null)
            {
                Destroy(face.materialInstance);
            }
        }
    }
}