using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataUpdater), true)]
public class UpdatableDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DataUpdater data = (DataUpdater)target;
        if (GUILayout.Button("Update"))
        {
            data.NotifyOfUpdatedValues();
            //Notify changes
            EditorUtility.SetDirty(target);
        }
    }
}
