using System;
using System.Collections;
using System.Collections.Generic;
using MineCraft;
using UnityEngine;

//决定每个Block的纹理等信息
[CreateAssetMenu(fileName = "Block Data", menuName = "Data/Block Data")]
public class BlockData : ScriptableObject
{
    public float textureSizeX, textureSizeY;
    public List<TextureData> textureDataList;
}

[Serializable]
public class TextureData
{
    public BlockType blockType;//是什么类型的方块
    public Vector2Int up, down, side;//up为Block上面的贴图side为旁边的
    public bool isSolid = true;//是否是固体，水就为false
    public bool generatesCollider = true;//是否生成碰撞体
}