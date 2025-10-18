using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldGenerateByCatNine
{
    //BlockDataSO只是把纹理数据保存下来
    //这个了才是把纹理对应到Block上(字典中一一对应)
    public class BlockDataManager : MonoBehaviour
    {
        public static float textureOffset = 0.001f;//一个很小的值用来防止贴图发生相互交错而出现伪影
        public static float tileSizeX, tileSizeY;
        public static Dictionary<BlockType, TextureData> blockTextureDataDictionary = new Dictionary<BlockType, TextureData>();
        public BlockData textureData;

        private void Awake()
        {
            foreach (var item in textureData.textureDataList)
            {
                if (blockTextureDataDictionary.ContainsKey(item.blockType) == false)
                {
                    blockTextureDataDictionary.Add(item.blockType, item);
                }
                ;
            }
            tileSizeX = textureData.textureSizeX;
            tileSizeY = textureData.textureSizeY;
        }
    }
}