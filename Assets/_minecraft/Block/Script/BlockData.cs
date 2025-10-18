using UnityEngine;

public class BlockData
{
    // 方块类型枚举
    public BlockType blockType;

    // 方块属性
    public Vector3Int worldPosition;  // 世界坐标
    public int chunkIndex;            // 所属区块索引
    public bool isActive = true;      // 是否激活（是否显示）

    // 物理属性
    public float hardness = 1.0f;     // 硬度
    public bool isTransparent = false; // 是否透明
    public bool isSolid = true;       // 是否实体方块
    public bool isLiquid = false;     // 是否是液体
}

/// <summary>
/// 方块类型
/// </summary>
public enum BlockType
{
    Air = 0,        // 空气
    Stone = 1,      // 石头
    Dirt = 2,       // 泥土
    Grass = 3,      // 草方块
    Wood = 4,       // 木头
    Leaves = 5,     // 树叶
    Water = 6,      // 水
    Sand = 7,       // 沙子
}

