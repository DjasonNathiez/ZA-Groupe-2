using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BetterAudio
{
    [System.Serializable]
    public partial class SourceCurves : BetterAudioBehaviour, IUpdateBehaviour 
    {
        [SerializeField]
        sourceProperties.sourcePropertiesList selectedProperty;
        [SerializeField]
        sourceProperty property;

        [SerializeField]
        int selectedIndex;

        [SerializeField]
        [SerializeReference]
        public AnimationCurve curve;

        [SerializeField]
        bool _multiply;
        [SerializeField]
        public bool Multiply
        {
            get
            {
                return _multiply;
            }
            set
            {
                _multiply = value;
                if(value)
                    curve = sourceProperties.MakeTwoPointsCurve(0, 1);
                else
                    curve = property.defaultCurve;
            }
        }

        bool init = false;

        public override void GUI(Rect rect)
        {
#if UNITY_EDITOR
            selectedProperty = (sourceProperties.sourcePropertiesList)EditorGUILayout.EnumPopup("Property", selectedProperty);
            if (selectedIndex != (int)selectedProperty)
            {
                selectedIndex = (int)selectedProperty;
                property = sourceProperties.properties[selectedIndex];

                curve = property.defaultCurve;
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Value by time");
            curve = EditorGUILayout.CurveField(curve, GUILayout.MinHeight(25));
            EditorGUILayout.Space();
            if( EditorGUILayout.Toggle("Multiply based value", Multiply) != _multiply)
            {
                Multiply = !Multiply;
            }
            EditorGUILayout.EndVertical();
#endif
        }

        public override void Init(BetterAudioSource m)
        {
            base.Init(m);
            Name = "Set source property (curve)";
            Priority = 100;
            property = sourceProperties.properties[selectedIndex];
            curve = property.defaultCurve;

        }

        public override bool canMultipleInstances()
        {
            return true;
        }

        float baseValue;
        public void OnUpdate(BetterAudioSource caller, bool playing, bool clip)
        {
            if (!playing || !clip)
                return;

            float time = caller.source.time / caller.source.clip.length;
            switch (selectedProperty)
            {
                case sourceProperties.sourcePropertiesList.Volume:
                    if (Multiply)
                    {
                        if (!init)
                            baseValue = caller.source.volume;
                        caller.source.volume = curve.Evaluate(time)*baseValue;
                    }
                    else
                    {
                        caller.source.volume = curve.Evaluate(time);
                    }
                    break;
                case sourceProperties.sourcePropertiesList.Pitch:
                    if (Multiply)
                    {
                        if (!init)
                            baseValue = caller.source.pitch;
                        caller.source.pitch = curve.Evaluate(time) * baseValue;
                    }
                    else
                    {
                        caller.source.pitch = curve.Evaluate(time);
                    }
                    break;
                case sourceProperties.sourcePropertiesList.Stereo_Pan:
                    if (Multiply)
                    {
                        if (!init)
                            baseValue = caller.source.panStereo;
                        caller.source.panStereo = curve.Evaluate(time) * baseValue;
                    }
                    else
                    {
                        caller.source.panStereo = curve.Evaluate(time);
                    }
                    break;
                case sourceProperties.sourcePropertiesList.Spatial_Blend:
                    if (Multiply)
                    {
                        if (!init)
                            baseValue = caller.source.spatialBlend;
                        caller.source.spatialBlend = curve.Evaluate(time) * baseValue;
                    }
                    else
                    {
                        caller.source.spatialBlend = curve.Evaluate(time);
                    }
                    break;
                case sourceProperties.sourcePropertiesList.Reverb_Zone:
                    if (Multiply)
                    {
                        if (!init)
                            baseValue = caller.source.reverbZoneMix;
                        caller.source.reverbZoneMix = curve.Evaluate(time) * baseValue;
                    }
                    else
                    {
                        caller.source.reverbZoneMix = curve.Evaluate(time);
                    }
                    break;
                case sourceProperties.sourcePropertiesList.Doppler_Level:
                    if (Multiply)
                    {
                        if (!init)
                            baseValue = caller.source.dopplerLevel;
                        caller.source.dopplerLevel = curve.Evaluate(time) * baseValue;
                    }
                    else
                    {
                        caller.source.dopplerLevel = curve.Evaluate(time);
                    }
                    break;
                case sourceProperties.sourcePropertiesList.Spread:
                    if (Multiply)
                    {
                        if (!init)
                            baseValue = caller.source.spread;
                        caller.source.spread = curve.Evaluate(time) * baseValue;
                    }
                    else
                    {
                        caller.source.spread = curve.Evaluate(time);
                    }
                    break;
                case sourceProperties.sourcePropertiesList.Min_Distance:
                    if (Multiply)
                    {
                        if (!init)
                            baseValue = caller.source.minDistance;
                        caller.source.minDistance = curve.Evaluate(time) * baseValue;
                    }
                    else
                    {
                        caller.source.minDistance = curve.Evaluate(time);
                    }
                    break;
                case sourceProperties.sourcePropertiesList.Max_Distance:
                    if (Multiply)
                    {
                        if (!init)
                            baseValue = caller.source.maxDistance;
                        caller.source.maxDistance = curve.Evaluate(time) * baseValue;
                    }
                    else
                    {
                        caller.source.maxDistance = curve.Evaluate(time);
                    }
                    break;
                default:
                    break;
            }
            if (!init)
                init = true;

        }

       
    }
}
