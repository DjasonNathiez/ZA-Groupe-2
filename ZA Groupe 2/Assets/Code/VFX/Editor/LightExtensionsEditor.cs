using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Light)), CanEditMultipleObjects]
public class LightExtensionsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
 
        base.OnInspectorGUI();
        
        if (GUILayout.Button("CREATE LODs"))
        {
            Debug.Log("Réussi");
        }
        serializedObject.ApplyModifiedProperties();
    }
}
