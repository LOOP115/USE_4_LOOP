using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelDetail)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        //To make sure the mesh is centered x = (width - 1)/-2 
        float topLeftx = (width - 1) / -2f;
        float topLeftz = (height - 1) / -2f;

        int tempIncrement = (levelDetail == 0) ? 1 : levelDetail * 2;
        int verticesPerLine = (width - 1) / tempIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for(int y = 0; y < height; y += tempIncrement)
        {
            for (int x = 0; x < width; x += tempIncrement)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftx + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftz - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriagnle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriagnle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleNum;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        /*Total number of vertices to from grid is (width + 1) * (height + 1) * 6
         * https://www.youtube.com/watch?v=64NblGkAabk&t=619s */
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    //https://www.youtube.com/watch?v=64NblGkAabk&t=619s
    public void AddTriagnle(int a, int b, int c)
    {
        triangles[triangleNum] = a;
        triangles[triangleNum + 1] = b;
        triangles[triangleNum + 2] = c;
        triangleNum += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}