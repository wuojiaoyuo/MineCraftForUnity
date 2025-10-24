using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace MC.Configurations
{
    public static class TextureAtlasGenerator
    {
        public static void GenerateTextureAtlas(this BlockTypeOverview blockTypeOverview)
        {
            List<Texture2D> textures = new List<Texture2D>();
            foreach (BlockType type in blockTypeOverview.AllBlockTypes)
            {
                if (type.blockProperties.BlockFlags.HasFlag(BlockFlags.AlwaysInvisible))
                    continue;
                textures.Add(type.blockTexture.up.texture);
                textures.Add(type.blockTexture.down.texture);
                textures.Add(type.blockTexture.forward.texture);
                textures.Add(type.blockTexture.backwards.texture);
                textures.Add(type.blockTexture.left.texture);
                textures.Add(type.blockTexture.right.texture);
            }

            string atlasFolderPath = "Assets/_minecraft/Block/TextureAtlases/";
            if (!Directory.Exists(atlasFolderPath))
            {
                Directory.CreateDirectory(atlasFolderPath);
            }

            string atlasName = "TextureAtlas";
            string atlasPath = atlasFolderPath + atlasName + ".png";
            string atlasDataPath = atlasFolderPath + atlasName + "_Data.asset";

            blockTypeOverview.CreateTextureAtlas(textures, atlasPath, atlasDataPath);
        }

        private static void CreateTextureAtlas(this BlockTypeOverview blockTypeOverview, List<Texture2D> textures, string atlasPath, string atlasDataPath)
        {
            Dictionary<string, Texture2D> uniqueTextures = new Dictionary<string, Texture2D>();
            List<Texture2D> uniqueList = new List<Texture2D>();

            foreach (Texture2D texture in textures)
            {
                string key = GetTextureHash(texture);
                if (!uniqueTextures.ContainsKey(key))
                {
                    uniqueTextures.Add(key, texture);
                    uniqueList.Add(texture);
                }
            }

            Debug.Log($"Original textures: {textures.Count}, Unique textures: {uniqueList.Count}");

            Texture2D[] texturesArray = uniqueList.ToArray();
            Texture2D atlas = new Texture2D(2048, 2048);
            Rect[] rects = atlas.PackTextures(texturesArray, 2, 2048, false);

            byte[] pngData = atlas.EncodeToPNG();
            File.WriteAllBytes(atlasPath, pngData);
            AssetDatabase.Refresh();

            TextureImporter importer = AssetImporter.GetAtPath(atlasPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                importer.sRGBTexture = true;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.isReadable = true;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }

            TextureAtlasData atlasData = ScriptableObject.CreateInstance<TextureAtlasData>();
            atlasData.atlasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(atlasPath);
            atlasData.textureRects = new List<TextureRectInfo>();

            for (int i = 0; i < texturesArray.Length; i++)
            {
                TextureRectInfo info = new TextureRectInfo
                {
                    originalTexture = texturesArray[i],
                    atlasRect = rects[i]
                };
                atlasData.textureRects.Add(info);
            }

            AssetDatabase.CreateAsset(atlasData, atlasDataPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            foreach (BlockType type in blockTypeOverview.AllBlockTypes)
            {
                type.blockTexture.up.uvRect = atlasData.GetRectForTexture(type.blockTexture.up.texture);
                type.blockTexture.down.uvRect = atlasData.GetRectForTexture(type.blockTexture.down.texture);
                type.blockTexture.forward.uvRect = atlasData.GetRectForTexture(type.blockTexture.forward.texture);
                type.blockTexture.backwards.uvRect = atlasData.GetRectForTexture(type.blockTexture.backwards.texture);
                type.blockTexture.left.uvRect = atlasData.GetRectForTexture(type.blockTexture.left.texture);
                type.blockTexture.right.uvRect = atlasData.GetRectForTexture(type.blockTexture.right.texture);
            }

            Debug.Log($"Texture atlas created at: {atlasPath}");
            Debug.Log($"Atlas data created at: {atlasDataPath}");
        }

        private static string GetTextureHash(Texture2D texture)
        {
            return $"{texture.name}_{texture.width}x{texture.height}";
        }
    }
    [Serializable]
    public struct TextureRectInfo
    {
        public Texture2D originalTexture;
        public Rect atlasRect;
    }

    public class TextureAtlasData : SerializedScriptableObject
    {
        public Texture2D atlasTexture;
        public List<TextureRectInfo> textureRects;

        public Rect GetRectForTexture(Texture2D texture)
        {
            foreach (TextureRectInfo info in textureRects)
            {
                if (info.originalTexture == texture)
                {
                    return info.atlasRect;
                }
            }
            return new Rect(0, 0, 0, 0);
        }
    }
}