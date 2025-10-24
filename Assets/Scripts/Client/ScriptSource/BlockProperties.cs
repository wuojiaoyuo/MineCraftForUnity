
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MC.Configurations
{
    [Serializable]
    public class BlockProperties
    {
        [HorizontalGroup("Split", 55, LabelWidth = 70)]
        [HideLabel, PreviewField(55, ObjectFieldAlignment.Left)]
        public Texture Icon;

        [LabelText("方块名称")]
        [VerticalGroup("Split/Meta")]
        public String Block_Name;

        [LabelText("ID")]
        [VerticalGroup("Split/Meta")]
        public string ID;

        [LabelText("方块标签")]
        [VerticalGroup("Split/Meta")]
        public BlockFlags BlockFlags;
        
        [VerticalGroup("Split/Meta2")]
        [LabelText("阻力系数"), ProgressBar(0, 1)]
        public float MoveResistance;

        [VerticalGroup("Split/Meta2")]
        [LabelText("光线透射率"), ProgressBar(0, 1)]
        public float LightOpacity;

        [VerticalGroup("Split/Meta2")]
        [LabelText("光亮度"), ProgressBar(0, 10, 0, 1, 0, Segmented = true)]
        public int LightValue;

        [VerticalGroup("Split/Meta2")]
        [LabelText("硬度"), ProgressBar(-1, 10, 0, 1, 0, Segmented = true)]
        public int Hardness;

        // [LabelText("破坏效果颜色")]
        // public Color DestoryEffectColor;
        // //TODO:ExtraAsset
    }

}