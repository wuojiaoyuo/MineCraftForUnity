using System;
using Sirenix.OdinInspector;

namespace MC.Configurations
{
    [Flags]
    public enum BlockFlags
    {
        None = 0,
        [LabelText("忽略碰撞")]
        IgnoreCollisions = 1 << 0,
        [LabelText("忽略放置方块射线检测")]
        IgnorePlaceBlockRaycast = 1 << 1,
        [LabelText("忽略破坏方块射线检测")]
        IgnoreDestroyBlockRaycast = 1 << 2,
        [LabelText("保留位")]
        Reserved = 1 << 3,
        [LabelText("忽略爆炸")]
        IgnoreExplosions = 1 << 4,
        [LabelText("始终隐形")]
        AlwaysInvisible = 1 << 5
    }
}