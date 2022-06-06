using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BetterAudio
{
    [System.Serializable]
    public partial class SetSourceProperty : BetterAudioBehaviour, IPlayBehaviour, IStartBehaviour, IEndBehaviour
    {
        [SerializeField]
        sourceProperties.sourcePropertiesList selectedProperty;
        [SerializeField]
        sourceProperty property = sourceProperties.properties[0];

        [SerializeField]
        int selectedIndex;

        [SerializeField]
        [SerializeReference]
        public RandomFloat val;


        public override void GUI(Rect rect)
        {
#if UNITY_EDITOR
            selectedProperty = (sourceProperties.sourcePropertiesList)EditorGUILayout.EnumPopup("Property", selectedProperty);
            if (selectedIndex != (int)selectedProperty)
            {
                selectedIndex = (int)selectedProperty;
                property = sourceProperties.properties[selectedIndex];

                val = new RandomFloat("Value", true, property.min, property.max, property.value);

                
            }

            val.DrawGUI();
#endif
        }

        public override void Init(BetterAudioSource m)
        {
            base.Init(m);
            Name = "Set source property (float)";
            Priority = 100;
            val = new RandomFloat("Value", true, property.min, property.max, property.value);

        }

        public override bool canMultipleInstances()
        {
            return true;
        }

        

        void setVal(BetterAudioSource caller)
        {
            switch (selectedProperty)
            {
                case sourceProperties.sourcePropertiesList.Volume:

                        caller.source.volume = val.Value;
                    
                    break;
                case sourceProperties.sourcePropertiesList.Pitch:

                        caller.source.pitch = val.Value;

                    break;
                case sourceProperties.sourcePropertiesList.Stereo_Pan:

                        caller.source.panStereo = val.Value;

                    break;
                case sourceProperties.sourcePropertiesList.Spatial_Blend:

                        caller.source.spatialBlend = val.Value;

                    break;
                case sourceProperties.sourcePropertiesList.Reverb_Zone:

                        caller.source.reverbZoneMix = val.Value;

                    break;
                case sourceProperties.sourcePropertiesList.Doppler_Level:

                        caller.source.dopplerLevel = val.Value;

                    break;
                case sourceProperties.sourcePropertiesList.Spread:

                        caller.source.spread = val.Value;

                    break;
                case sourceProperties.sourcePropertiesList.Min_Distance:

                        caller.source.minDistance = val.Value;

                    break;
                case sourceProperties.sourcePropertiesList.Max_Distance:

                        caller.source.maxDistance = val.Value;

                    break;
                default:
                    break;
            }


        }

        public void OnPlay(BetterAudioSource caller)
        {
            setVal(caller);
        }

        public void OnStart(BetterAudioSource caller)
        {
            setVal(caller);
        }

        public void OnEnd(BetterAudioSource caller)
        {
            setVal(caller);
        }
    }
}
