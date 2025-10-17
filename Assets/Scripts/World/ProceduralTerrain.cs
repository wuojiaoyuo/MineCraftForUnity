using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界生成试验
/// </summary>
public class ProceduralTerrain : MonoBehaviour
{
    /// <summary>
    /// 噪声缩放
    /// </summary>
    [SerializeField] private float noiseScale = 0.05f;
    /// <summary>
    /// 世界尺寸
    /// </summary>
    [SerializeField] private int worldSize = 6;
    /// <summary>
    /// 世界高度
    /// </summary>
    [SerializeField] private int worldHeight = 10;
    /// <summary>
    /// 水面高度
    /// </summary>
    [SerializeField] private int waterHeight = 4;

    /// <summary>
    /// 保存每个BLock类型的颜色
    /// </summary>
    private Dictionary<t_BlockType, Color> colorDic = new Dictionary<t_BlockType, Color>();

    /// <summary>
    /// 方块数据缓存
    /// </summary>
    private List<t_BlockData> worldBlockData = new List<t_BlockData>();

    #region event function
    private void Awake()
    {
        //将Block的材质和颜色一一对应
        colorDic.Add(t_BlockType.Grass_Dirt, Color.green);//草方块对应绿色
        colorDic.Add(t_BlockType.Dirt, Color.yellow);//泥土对应黄色
        colorDic.Add(t_BlockType.Water, Color.blue);//水对应蓝色
        colorDic.Add(t_BlockType.Sand, Color.gray);//沙对应灰色
    }

    private void Start()
    {
        GenerateTerrainData();
    }
    #endregion

    #region FUNCTION

    /// <summary>
    /// 生成地图数据求出每个坐标的高度值
    /// </summary>
    private void GenerateTerrainData()
    {
        t_BlockType blockType;
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                //柏林噪声取 用水平坐标*噪声缩放 取值
                float noiseValue = Mathf.PerlinNoise((transform.position.x + i) * noiseScale, (transform.position.z + j) * noiseScale);

                int groundPostion = Mathf.RoundToInt(noiseValue * worldSize);

                for (int y = 0; y < worldHeight; y++)
                {
                    blockType = t_BlockType.Dirt;
                    if (y > groundPostion)
                    {
                        if (y < waterHeight)
                        {
                            blockType = t_BlockType.Water;
                        }
                        else
                        {
                            blockType = t_BlockType.Air;
                        }
                    }
                    else if (y == groundPostion && y < waterHeight)
                    {
                        blockType = t_BlockType.Sand;
                    }
                    else if (y == groundPostion)
                    {
                        blockType = t_BlockType.Grass_Dirt;
                    }

                    t_BlockData temp = new t_BlockData(new Vector3(transform.position.x + i, y, transform.position.z + j), blockType);
                    worldBlockData.Add(temp);
                }

            }
        }
    }

    /// <summary>
    /// 绘制地形预览
    /// </summary>
    private void OnDrawGizmos()
    {
        if (worldBlockData.Count > 0)
        {
            foreach (var block in worldBlockData)
            {
                if (block.blockType == t_BlockType.Air)
                {
                    continue;
                }
                Gizmos.color = colorDic[block.blockType];
                Gizmos.DrawCube(block.blockPosition, new Vector3(1f, 1f, 1f));
            }
        }

    }

    #endregion

    #region DATA

    internal class t_BlockData
    {
        public Vector3 blockPosition;
        public t_BlockType blockType;
        public t_BlockData(Vector3 postion, t_BlockType type)
        {
            blockPosition = postion;
            blockType = type;
        }
    }
    public enum t_BlockType
    {
        None,
        Air,
        Grass_Dirt,
        Dirt,
        Water,
        Sand,
    }
    #endregion
}
