using UnityEngine;

public class DataUpdater : ScriptableObject
{
    #if UNITY_EDITOR

    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    protected virtual void OnValidate()
    {
        if (autoUpdate)
        {
            //Solving that after compiling nothing is updated
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
        }
    }

    public void NotifyOfUpdatedValues()
    {
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }
    #endif
}
