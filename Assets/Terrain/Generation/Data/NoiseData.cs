using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : DataUpdater
{
    public float noiseScale;
    public int ocatves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;

    #if UNITY_EDITOR

    protected override void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (ocatves < 0)
        {
            ocatves = 0;
        }
        base.OnValidate();
    }

    #endif
}
