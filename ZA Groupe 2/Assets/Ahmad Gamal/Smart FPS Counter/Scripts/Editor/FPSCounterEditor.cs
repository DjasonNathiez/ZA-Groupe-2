using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[CustomEditor(typeof(FPSCounter))]
public class FPSCounterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FPSCounter counter = (FPSCounter)target;

        EditorGUILayout.LabelField("Base", EditorStyles.boldLabel);
        counter.GraphicsType = (TextType)EditorGUILayout.EnumPopup("Text Type", counter.GraphicsType);

        if (counter.GraphicsType == TextType.Text)
        {
            counter.framerateText = EditorGUILayout.ObjectField("Framerate", counter.framerateText, typeof(Text), true) as Text;
            counter.avgFramerateText = EditorGUILayout.ObjectField("Average Framerate", counter.avgFramerateText, typeof(Text), true) as Text;
            counter.maxFramerateText = EditorGUILayout.ObjectField("Max Framerate", counter.maxFramerateText, typeof(Text), true) as Text;
            counter.minFrameRateText = EditorGUILayout.ObjectField("Min Framerate", counter.minFrameRateText, typeof(Text), true) as Text;
            counter.advancedTransform = EditorGUILayout.ObjectField("Advanced Transform", counter.advancedTransform, typeof(Transform), true) as Transform;


            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            counter.useAdvanced = EditorGUILayout.Toggle(counter.useAdvanced, GUILayout.MaxWidth(15));
            EditorGUILayout.LabelField("Advanced", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            if (counter.useAdvanced)
            {
                counter.memorySizeUnit = (MemorySizeUnit)EditorGUILayout.EnumPopup("Memory Size Unit", counter.memorySizeUnit);
                counter.OSText = EditorGUILayout.ObjectField("OS", counter.OSText, typeof(Text), true) as Text;
                counter.CPUText = EditorGUILayout.ObjectField("CPU", counter.CPUText, typeof(Text), true) as Text;
                counter.GPUText = EditorGUILayout.ObjectField("GPU", counter.GPUText, typeof(Text), true) as Text;
                counter.VRAMText = EditorGUILayout.ObjectField("VRAM", counter.VRAMText, typeof(Text), true) as Text;
                counter.RAMText = EditorGUILayout.ObjectField("RAM", counter.RAMText, typeof(Text), true) as Text;
                counter.ScreenText = EditorGUILayout.ObjectField("Screen", counter.ScreenText, typeof(Text), true) as Text;
            }
        }

        if (counter.GraphicsType == TextType.TextMeshPro)
        {
            counter.framerateTextMPro = EditorGUILayout.ObjectField("Framerate", counter.framerateTextMPro, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
            counter.avgFramerateTextMPro = EditorGUILayout.ObjectField("Average Framerate", counter.avgFramerateTextMPro, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
            counter.maxFramerateTextMPro = EditorGUILayout.ObjectField("Max Framerate", counter.maxFramerateTextMPro, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
            counter.minFrameRateTextMPro = EditorGUILayout.ObjectField("Min Framerate", counter.minFrameRateTextMPro, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
            counter.advancedTransform = EditorGUILayout.ObjectField("Advanced Transform", counter.advancedTransform, typeof(Transform), true) as Transform;


            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            counter.useAdvanced = EditorGUILayout.Toggle(counter.useAdvanced, GUILayout.MaxWidth(15));
            EditorGUILayout.LabelField("Advanced", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            if (counter.useAdvanced)
            {
                counter.memorySizeUnit = (MemorySizeUnit)EditorGUILayout.EnumPopup("Memory Size Unit", counter.memorySizeUnit);
                counter.OSTextMPro = EditorGUILayout.ObjectField("OS", counter.OSTextMPro, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
                counter.CPUTextMPro = EditorGUILayout.ObjectField("CPU", counter.CPUTextMPro, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
                counter.GPUTextMPro = EditorGUILayout.ObjectField("GPU", counter.GPUTextMPro, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
                counter.VRAMTextMPro = EditorGUILayout.ObjectField("VRAM", counter.VRAMTextMPro, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
                counter.RAMTextMPro = EditorGUILayout.ObjectField("RAM", counter.RAMTextMPro, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
                counter.ScreenTextMPro = EditorGUILayout.ObjectField("Screen", counter.ScreenTextMPro, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
            }
        }

        if (counter.useAdvanced)
            EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        counter.useDynamicColors = EditorGUILayout.Toggle(counter.useDynamicColors, GUILayout.MaxWidth(15));
        EditorGUILayout.LabelField("Static Colors", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        if (!counter.useDynamicColors)
        {
            counter.highFPSColor = EditorGUILayout.ColorField("High", counter.highFPSColor);
            counter.mediumFPSColor = EditorGUILayout.ColorField("Medium", counter.mediumFPSColor);
            counter.lowFPSColor = EditorGUILayout.ColorField("Low", counter.lowFPSColor);
        }
        else
        {
            counter.staticInfoColor = EditorGUILayout.ColorField("Main Color", counter.staticInfoColor);
        }
    }
}
