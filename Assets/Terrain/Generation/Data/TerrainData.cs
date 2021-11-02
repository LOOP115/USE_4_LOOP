using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : DataUpdater
{
    public bool useBorder;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public float minHeight
    {
        get
        {
            return meshHeightMultiplier * meshHeightCurve.Evaluate(0);
        }
    }
    
    public float maxHeight
    {
        get
        {
            return meshHeightMultiplier * meshHeightCurve.Evaluate(1);
        }
    }
}
