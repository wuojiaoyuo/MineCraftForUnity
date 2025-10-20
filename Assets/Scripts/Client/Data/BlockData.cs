using System;
using Sirenix.OdinInspector;
using UnityEngine;

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

    [Flags]
    public enum ShowDirection
    {
        None = 0,
        foreward = 1 << 0,
        backwards = 1 << 1,
        up = 1 << 2,
        down = 1 << 3,
        right = 1 << 4,
        left = 1 << 5
    }

    public enum Direction
    {
        foreward = 0,
        backwards = 1,
        up = 2,
        down = 3,
        right = 4,
        left = 5
    }

    public static class Extensions
    {
        public static Vector3Int GetVector(this Direction direction)
        {
            return direction switch
            {
                Direction.up => Vector3Int.up,
                Direction.down => Vector3Int.down,
                Direction.right => Vector3Int.right,
                Direction.left => Vector3Int.left,
                Direction.foreward => Vector3Int.forward,
                Direction.backwards => Vector3Int.back,
                _ => throw new Exception("Invalid input direction")
            };
        }

        public static int[] ShowedIndex(this ShowDirection direction)
        {
            // 预分配最大可能长度的数组（6个方向）
            int[] indices = new int[6];
            int count = 0;

            // 检查每个方向并记录索引
            if (direction.HasFlag(ShowDirection.foreward)) indices[count++] = 0;
            if (direction.HasFlag(ShowDirection.backwards)) indices[count++] = 1;
            if (direction.HasFlag(ShowDirection.up)) indices[count++] = 2;
            if (direction.HasFlag(ShowDirection.down)) indices[count++] = 3;
            if (direction.HasFlag(ShowDirection.right)) indices[count++] = 4;
            if (direction.HasFlag(ShowDirection.left)) indices[count++] = 5;

            // 返回实际长度的数组
            int[] result = new int[count];
            Array.Copy(indices, result, count);
            return result;
        }

        public static bool Has(this ShowDirection direction, int index)
        {
            return index switch
            {
                0 => direction.HasFlag(ShowDirection.foreward),
                1 => direction.HasFlag(ShowDirection.backwards),
                2 => direction.HasFlag(ShowDirection.up),
                3 => direction.HasFlag(ShowDirection.down),
                4 => direction.HasFlag(ShowDirection.right),
                5 => direction.HasFlag(ShowDirection.left),
                _ => false
            };
        }
    }
}