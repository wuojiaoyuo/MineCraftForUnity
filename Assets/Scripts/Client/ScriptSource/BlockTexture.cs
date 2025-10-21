using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MC.Configurations
{
    [Serializable]
    public class BlockTexture
    {
        [HorizontalGroup("Split")]
        [PreviewField(55,ObjectFieldAlignment.Left), VerticalGroup("Split/Meta1")]
        public Texture forward;
        [PreviewField(55,ObjectFieldAlignment.Left), VerticalGroup("Split/Meta1")]
        public Texture backwards;
        [PreviewField(55,ObjectFieldAlignment.Left), VerticalGroup("Split/Meta2")]
        public Texture up;
        [PreviewField(55,ObjectFieldAlignment.Left), VerticalGroup("Split/Meta2")]
        public Texture down;
        [PreviewField(55,ObjectFieldAlignment.Left), VerticalGroup("Split/Meta3")]
        public Texture right;
        [PreviewField(55,ObjectFieldAlignment.Left), VerticalGroup("Split/Meta3")]
        public Texture left;
    }

    public static class BlockTextureExtensions
    {
        public static Texture Index(this BlockTexture blockTexture, int index)
        {
            return index switch
            {
                0 => blockTexture.forward,
                1 => blockTexture.backwards,
                2 => blockTexture.up,
                3 => blockTexture.down,
                4 => blockTexture.right,
                5 => blockTexture.left,
                _ => throw new Exception("None")
            };
        }
    }
}