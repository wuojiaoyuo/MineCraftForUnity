using System;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Configurations
{
    public class BlockRender
    {
        public ShowDirection showDirection;
        public Material[] facesMaterial { get; private set; } = new Material[6];
        private SOBlock sOBlock;
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

        public BlockRender(MeshFilter meshFilter, MeshRenderer meshRenderer, SOBlock sOBlock,ShowDirection showDirection)
        {
            this.meshFilter = meshFilter;
            this.meshRenderer = meshRenderer;
            this.sOBlock = sOBlock;
            this.showDirection = showDirection;

            this.mesh = new Mesh();
            this.mesh.name = $"{sOBlock.blockProperties.Block_Name}_Mesh";
            meshFilter.mesh = mesh;
            GenerateCube();
        }


        public void GenerateCube()
        {
            if (mesh == null) return;

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
            if (showDirection.HasFlag(ShowDirection.foreward))
            {
                AddFace(vertices, normals, uvs,
                        new int[] { 0, 1, 2, 3 }, Vector3.forward,
                        faceTriangles[0], ref currentVertexIndex);

                CreateFaceMaterial(0);
                activeMaterials.Add(facesMaterial[0]);
                subMeshIndex++;
            }

            // 后面 (Z负方向)
            if (showDirection.HasFlag(ShowDirection.backwards))
            {
                AddFace(vertices, normals, uvs,
                        new int[] { 5, 4, 7, 6 }, Vector3.back,
                        faceTriangles[1], ref currentVertexIndex);

                CreateFaceMaterial(1);
                activeMaterials.Add(facesMaterial[1]);
                subMeshIndex++;
            }

            // 上面 (Y正方向)
            if (showDirection.HasFlag(ShowDirection.up))
            {
                AddFace(vertices, normals, uvs,
                        new int[] { 3, 2, 6, 7 }, Vector3.up,
                        faceTriangles[2], ref currentVertexIndex);

                CreateFaceMaterial(2);
                activeMaterials.Add(facesMaterial[2]);
                subMeshIndex++;
            }

            // 下面 (Y负方向)
            if (showDirection.HasFlag(ShowDirection.down))
            {
                AddFace(vertices, normals, uvs,
                        new int[] { 1, 0, 4, 5 }, Vector3.down,
                        faceTriangles[3], ref currentVertexIndex);

                CreateFaceMaterial(3);
                activeMaterials.Add(facesMaterial[3]);
                subMeshIndex++;
            }

            // 右面 (X正方向)
            if (showDirection.HasFlag(ShowDirection.right))
            {
                AddFace(vertices, normals, uvs,
                        new int[] { 1, 5, 6, 2 }, Vector3.right,
                        faceTriangles[4], ref currentVertexIndex);

                CreateFaceMaterial(4);
                activeMaterials.Add(facesMaterial[4]);
                subMeshIndex++;
            }

            // 左面 (X负方向)
            if (showDirection.HasFlag(ShowDirection.left))
            {
                AddFace(vertices, normals, uvs,
                        new int[] { 4, 0, 3, 7 }, Vector3.left,
                        faceTriangles[5], ref currentVertexIndex);

                CreateFaceMaterial(5);
                activeMaterials.Add(facesMaterial[5]);
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
                if (showDirection.Has(i) && faceTriangles[i].Count > 0)
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
            if (facesMaterial[faceIndex] != null) return;

            facesMaterial[faceIndex] = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            facesMaterial[faceIndex].mainTexture = sOBlock.blockTexture.Index(faceIndex);

        }

        private void AddFace(List<Vector3> vertices, List<Vector3> normals,
                            List<Vector2> uvs, int[] vertexIndices, Vector3 normal,
                            List<int> faceTriangles, ref int startIndex)
        {
            // 添加四个顶点
            for (int i = 0; i < 4; i++)
            {
                vertices.Add(baseVertices[vertexIndices[i]]);
                normals.Add(normal);
            }

            // 设置UV坐标
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));

            // 添加两个三角形
            faceTriangles.Add(startIndex);
            faceTriangles.Add(startIndex + 1);
            faceTriangles.Add(startIndex + 2);

            faceTriangles.Add(startIndex);
            faceTriangles.Add(startIndex + 2);
            faceTriangles.Add(startIndex + 3);

            startIndex += 4;
        }

        #region Public Methoes

        // 公共方法：设置面状态
        public void SetFaceEnabled(ShowDirection showDirection)
        {
            this.showDirection = showDirection;
            GenerateCube();
        }

        public Mesh GetMesh() => mesh;
        #endregion
    }
}