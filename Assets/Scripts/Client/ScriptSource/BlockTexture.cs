using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MC.Configurations
{
    [Serializable]
    public class BlockTexture
    {
        [HorizontalGroup("Split")]
        [ VerticalGroup("Split/Meta1")]
        public TextureData forward;
        [VerticalGroup("Split/Meta1")]
        public TextureData backwards;
        [VerticalGroup("Split/Meta2")]
        public TextureData up;
        [VerticalGroup("Split/Meta2")]
        public TextureData down;
        [VerticalGroup("Split/Meta3")]
        public TextureData right;
        [VerticalGroup("Split/Meta3")]
        public TextureData left;
    }
    [Serializable]
    public struct TextureData
    {
        [PreviewField(55, ObjectFieldAlignment.Left)]
        public Texture2D texture;
        [ReadOnly]
        public Rect uvRect;
    }
    public static class BlockTextureExtensions
    {
      
    }
}