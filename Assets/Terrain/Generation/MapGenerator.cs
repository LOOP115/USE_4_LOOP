using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public enum DrawMode {NoiseMap, Mesh, Border};
    public DrawMode drawMode;

    public NoiseData noiseData;
    public TerrainData terrainData;
    public TextureData textureData;

    public Material terrianMaterial;

    const int mapChunkSize = 241;
    [Range(0,6)]
    public int levelDetail;
    
    public bool autoUpdate;


    float[,] border;

    public List<GameObject> objSpwan = new List<GameObject>();

    void borderCall()
    {
        border = Border.GenerateBorder(mapChunkSize);
    }

    private void Awake()
    {
        textureData.ApplyToMaterial(terrianMaterial);
        textureData.UpdateMeshHeight(terrianMaterial, terrainData.minHeight, terrainData.maxHeight);
    }

    private void Start()
    {
        GenerateMap();
   //     GameObject randomObj = objSpwan[Random.Range(0, objSpwan.Count)];
      //  Instantiate(randomObj, transform.position, Quaternion.identity);
    }
    private void Update()
    {
        
    }

    void OnValueUpdate()
    {
        if (!Application.isPlaying)
        {
            GenerateMap();
        }
    }

    void OnTextureValueUpdated()
    {
        textureData.ApplyToMaterial(terrianMaterial);
    }

    public void GenerateMap()
    {
        OnTextureValueUpdated();
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseData.seed, noiseData.noiseScale, noiseData.ocatves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                if (terrainData.useBorder)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - border[x, y]);
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap) 
        {
            display.DrawTexture(TextureGenerator.TextureHeightMap(noiseMap)); 
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelDetail));
        }
        else if (drawMode == DrawMode.Border)
        {
            display.DrawTexture(TextureGenerator.TextureHeightMap(Border.GenerateBorder(mapChunkSize)));
        }

        textureData.UpdateMeshHeight(terrianMaterial, terrainData.minHeight, terrainData.maxHeight);

    }

    private void OnValidate()
    {
        #if UNITY_EDITOR
        if (terrainData != null)
        {
            //subscribition count reset to avoid lots of mapgenerate times
            terrainData.OnValuesUpdated -= OnValueUpdate;
            terrainData.OnValuesUpdated += OnValueUpdate;
        }
        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValueUpdate;
            noiseData.OnValuesUpdated += OnValueUpdate;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnValueUpdate;
            textureData.OnValuesUpdated += OnValueUpdate;
        }
        #endif
        border = Border.GenerateBorder(mapChunkSize);
    }
}
