using UnityEngine;
using System.Linq;

[CreateAssetMenu()]
public class TextureData : DataUpdater
{

    const int textureSize = 512;
    const TextureFormat textureFormat = TextureFormat.RGB565;

    public Layer[] layer;

    float savedMinHeight;
    float savedMaxHeight;

    public void ApplyToMaterial(Material material)
    {
        material.SetInt("layerCount", layer.Length);
        material.SetColorArray("baseColours", layer.Select(x => x.tint).ToArray());
        material.SetFloatArray("baseStartHeight", layer.Select(x => x.startHeight).ToArray());
        material.SetFloatArray("baseBlends", layer.Select(x => x.blendStrength).ToArray());
        material.SetFloatArray("baseColourStrength", layer.Select(x => x.tintStrength).ToArray());
        material.SetFloatArray("baseTextureScales", layer.Select(x => x.textureScale).ToArray());
        Texture2DArray texture2DArray = SetTexture2DArray(layer.Select(x => x.texture).ToArray());
        material.SetTexture("baseTextures", texture2DArray);

        UpdateMeshHeight(material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeight(Material material, float minHeight, float maxHeight)
    {

        savedMaxHeight = maxHeight;
        savedMinHeight = minHeight;

        material.SetFloat("maxHeight", maxHeight);
        material.SetFloat("minHeight", minHeight);
    }

    Texture2DArray SetTexture2DArray(Texture2D[] textures)
    {
        Texture2DArray texture2DArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            texture2DArray.SetPixels(textures[i].GetPixels(), i);
        }
        texture2DArray.Apply();
        return texture2DArray;
    }

    [System.Serializable]
    public class Layer
    {
        public Texture2D texture;
        public Color tint;
        [Range(0,1)]
        public float tintStrength;
        [Range(0, 1)]
        public float startHeight;
        [Range(0, 1)]
        public float blendStrength;
        public float textureScale;
    }

}
