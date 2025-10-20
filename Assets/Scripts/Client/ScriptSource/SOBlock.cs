using Sirenix.OdinInspector;
using UnityEngine;

namespace MC.Configurations
{
    [CreateAssetMenu(fileName = "BlockDatabase", menuName = "Minecraft/Block Database")]
    public class SOBlock : SerializedScriptableObject
    {
        public BlockProperties blockProperties;
        public BlockTexture blockTexture;

    }
}