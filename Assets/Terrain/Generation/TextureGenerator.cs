using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);

        //Using point instead of linear to avoid blurring
        texture.filterMode = FilterMode.Point;
        //Solving wrap problem to make it clear
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }
    public static Texture2D TextureHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureColourMap(colourMap, width, height);
    }
}
