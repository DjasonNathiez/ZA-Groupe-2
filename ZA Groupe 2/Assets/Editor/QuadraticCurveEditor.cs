using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(QuadraticCurve))]
public class QuadraticCurveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        QuadraticCurve script = (QuadraticCurve) target;
        script.bake = EditorGUILayout.Toggle("BAKING PLANKS", script.bake);
        if (script.bake)
        {
            script.bake = false;
            script.CreateQuad();
        }
        script.bakerail = EditorGUILayout.Toggle("BAKING RAILS", script.bakerail);
        if (script.bakerail)
        {
            script.bakerail = false;
            script.CreateMesh();
        }
    }
}
