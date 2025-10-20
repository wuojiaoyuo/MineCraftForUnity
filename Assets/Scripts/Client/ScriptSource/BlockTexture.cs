using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace MC.Configurations
{
    [Serializable]
    public class BlockTexture
    {
        [HorizontalGroup("Split")]
        [PreviewField(55), VerticalGroup("Split/Meta1")]
        public Texture2D forward;
        [PreviewField(55), VerticalGroup("Split/Meta2")]
        public Texture2D backwards;
        [PreviewField(55), VerticalGroup("Split/Meta3")]
        public Texture2D up;
        [PreviewField(55), VerticalGroup("Split/Meta4")]
        public Texture2D down;
        [PreviewField(55), VerticalGroup("Split/Meta5")]
        public Texture2D right;
        [PreviewField(55), VerticalGroup("Split/Meta6")]
        public Texture2D left;
    }

    public static class BlockTextureExtensions
    {
        public static Texture2D Index(this BlockTexture blockTexture, int index)
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