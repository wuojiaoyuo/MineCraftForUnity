using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class TextureAtlasGenerator : MonoBehaviour
{
    [MenuItem("Assets/Generate Texture Atlas")]
    public static void GenerateTextureAtlas()
    {
        // 1. 收集选中的贴图
        List<Texture2D> textures = new List<Texture2D>();
        foreach (Object obj in Selection.objects)
        {
            if (obj is Texture2D texture)
            {
                textures.Add(texture);
            }
        }

        if (textures.Count == 0)
        {
            Debug.LogError("No textures selected!");
            return;
        }

        // 2. 创建图集文件夹
        string atlasFolderPath = "Assets/TextureAtlases/";
        if (!Directory.Exists(atlasFolderPath))
        {
            Directory.CreateDirectory(atlasFolderPath);
        }

        // 3. 生成唯一图集名称
        string atlasName = "TextureAtlas_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string atlasPath = atlasFolderPath + atlasName + ".png";
        string atlasDataPath = atlasFolderPath + atlasName + "_Data.asset";

        // 4. 处理贴图并创建图集
        CreateTextureAtlas(textures, atlasPath, atlasDataPath);
    }

    private static void CreateTextureAtlas(List<Texture2D> textures, string atlasPath, string atlasDataPath)
    {
        // 5. 创建唯一贴图列表（去重）
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

        // 6. 准备贴图打包
        Texture2D[] texturesArray = uniqueList.ToArray();
        Texture2D atlas = new Texture2D(2048, 2048);
        Rect[] rects = atlas.PackTextures(texturesArray, 2, 2048, false);

        // 7. 保存图集贴图
        byte[] pngData = atlas.EncodeToPNG();
        File.WriteAllBytes(atlasPath, pngData);
        AssetDatabase.Refresh();

        // 8. 导入设置
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

        // 9. 创建图集数据资产
        TextureAtlasData atlasData = ScriptableObject.CreateInstance<TextureAtlasData>();
        atlasData.atlasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(atlasPath);
        atlasData.textureRects = new List<TextureRectInfo>();

        // 10. 映射原始贴图到图集位置
        for (int i = 0; i < texturesArray.Length; i++)
        {
            TextureRectInfo info = new TextureRectInfo
            {
                originalTexture = texturesArray[i],
                atlasRect = rects[i]
            };
            atlasData.textureRects.Add(info);
        }

        // 11. 保存图集数据
        AssetDatabase.CreateAsset(atlasData, atlasDataPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Texture atlas created at: {atlasPath}");
        Debug.Log($"Atlas data created at: {atlasDataPath}");
    }

    private static string GetTextureHash(Texture2D texture)
    {
        // 使用贴图名称和尺寸作为唯一标识
        // 对于更精确的去重，可以计算像素哈希值
        return $"{texture.name}_{texture.width}x{texture.height}";
    }
}

[System.Serializable]
public class TextureRectInfo
{
    public Texture2D originalTexture;
    public Rect atlasRect;
}

public class TextureAtlasData : ScriptableObject
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