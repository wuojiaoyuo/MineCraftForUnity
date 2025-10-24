using System;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Configurations
{
    public static class  BlockRender
    {
        public static String GetBlockType(this ChunkData chunkData, int x, int y, int z)
        {
            if (y < 0 || y > chunkData.chunkHeight - 1)
            {
                return "Air";
            }

            //当前位置是否在Chunk内
            if ((x < 0) || (z < 0) || (x >= chunkData.chunkSize) || (z >= chunkData.chunkSize))
            {
                var id = World.Instance.GenerateBlockType(new Vector3(x, y, z) + chunkData.worldPosition);
                return id;
            }
            return chunkData.BlockIDs[x, y, z];
        }
        
         public static void BuildBlock(this ChunkData chunkData,int x, int y, int z, List<Vector3> verts, List<Vector2> uvs, List<int> tris)
        {
            if (chunkData.BlockIDs[x, y, z] == "Air") return;

            BlockType type = World.types[chunkData.BlockIDs[x, y, z]];

            //left
            if (chunkData.CheckNeedBuildFace(x - 1, y, z))
                BuildFace(type.blockTexture.left.uvRect, new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), Vector3.forward, Vector3.up, true, verts, uvs, tris);
            //right
            if (chunkData.CheckNeedBuildFace(x + 1, y, z))
                BuildFace(type.blockTexture.right.uvRect, new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), Vector3.forward, Vector3.up, false, verts, uvs, tris);

            //down
            if (chunkData.CheckNeedBuildFace(x, y - 1, z))
                BuildFace(type.blockTexture.down.uvRect, new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), Vector3.forward, Vector3.right, false, verts, uvs, tris);
            //up
            if (chunkData.CheckNeedBuildFace(x, y + 1, z))
                BuildFace(type.blockTexture.up.uvRect, new Vector3(x - 0.5f, y + 1.5f, z - 0.5f), Vector3.forward, Vector3.right, true, verts, uvs, tris);

            //backwards
            if (chunkData.CheckNeedBuildFace(x, y, z - 1))
                BuildFace(type.blockTexture.backwards.uvRect, new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), Vector3.right, Vector3.up, false, verts, uvs, tris);
            //forward
            if (chunkData.CheckNeedBuildFace(x, y, z + 1))
                BuildFace(type.blockTexture.forward.uvRect, new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), Vector3.right, Vector3.up, true, verts, uvs, tris);
        }

        private static bool CheckNeedBuildFace(this ChunkData chunkData,int x, int y, int z)
        {
            if (y < 0) return false;
            var type = chunkData.GetBlockType(x, y, z);
            switch (type)
            {
                case "Air":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 构建一个方块的面
        /// </summary>
        /// <param name="uvRect">UV贴图区域</param>
        /// <param name="corner">方块顶点的起始位置</param>
        /// <param name="up">向上的方向向量</param>
        /// <param name="right">向右的方向向量</param>
        /// <param name="reversed">是否翻转三角形顺序</param>
        /// <param name="verts">顶点列表</param>
        /// <param name="uvs">UV坐标列表</param>
        /// <param name="tris">三角形索引列表</param>
        private static void BuildFace(Rect uvRect, Vector3 corner, Vector3 up, Vector3 right, bool reversed, List<Vector3> verts, List<Vector2> uvs, List<int> tris)
        {
            int index = verts.Count;

            verts.Add(corner);
            verts.Add(corner + up);
            verts.Add(corner + up + right);
            verts.Add(corner + right);


            uvs.Add(new Vector2(uvRect.x, uvRect.y));
            uvs.Add(new Vector2(uvRect.x + uvRect.width, uvRect.y));
            uvs.Add(new Vector2(uvRect.x + uvRect.width, uvRect.y + uvRect.height));
            uvs.Add(new Vector2(uvRect.x, uvRect.y + uvRect.height));

            if (reversed)
            {
                tris.Add(index + 0);
                tris.Add(index + 1);
                tris.Add(index + 2);
                tris.Add(index + 2);
                tris.Add(index + 3);
                tris.Add(index + 0);
            }
            else
            {
                tris.Add(index + 1);
                tris.Add(index + 0);
                tris.Add(index + 2);
                tris.Add(index + 3);
                tris.Add(index + 2);
                tris.Add(index + 0);
            }
        }
    }
}