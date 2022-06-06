using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BetterAudio
{
    [System.Serializable]
    public partial class AutoDestroy : BetterAudioBehaviour, IEndBehaviour, IStartBehaviour, IPlayBehaviour
    {

        [SerializeField]
        public bool autoDestroy = true;
        [SerializeField]
        public RandomFloat autoDestroyFloat;

        public override void GUI(Rect rect)
        {
#if UNITY_EDITOR


            autoDestroy = EditorGUILayout.Toggle("Auto Destroy", autoDestroy);

            autoDestroyFloat.DrawGUI();

#endif
        }

        public override void Init(BetterAudioSource m)
        {
            base.Init(m);
            Name = "Auto Destroy";
            Priority = 100;

            autoDestroyFloat = new RandomFloat("Auto Destroy Delay", sMin: 0, sMax: 99999, value: 0);
        }

        public enum Properties
        {
            String_Name,
            Bool_AutoDestroy,
            Bool_IsDelayRandom,
            Float_Delay,
            Float_MinDelay,
            Float_MaxDelay
        }

        public void SetProperty(Properties property, object value)
        {
            switch (property)
            {
                case Properties.String_Name:
                    if (value.GetType() == typeof(string))
                    {
                        Name = (string)value;
                    }
                    else
                    {
                        if (Main)
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !", Main);
                        else
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !");
                    }
                    break;
                case Properties.Bool_AutoDestroy:
                    if (value.GetType() == typeof(bool))
                    {
                        autoDestroy = (bool)value;
                    }
                    else
                    {
                        if (Main)
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !", Main);
                        else
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !");
                    }
                    break;
                case Properties.Bool_IsDelayRandom:
                    if (value.GetType() == typeof(bool))
                    {
                        autoDestroyFloat.isRandom = (bool)value;
                    }
                    else
                    {
                        if (Main)
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !", Main);
                        else
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !");
                    }
                    break;
                case Properties.Float_Delay:
                    if (value.GetType() == typeof(float))
                    {
                        autoDestroyFloat.Value = (float)value;
                    }
                    else
                    {
                        if (Main)
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !", Main);
                        else
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !");
                    }
                    break;
                case Properties.Float_MinDelay:
                    if (value.GetType() == typeof(float))
                    {
                        autoDestroyFloat.min = (float)value;
                    }
                    else
                    {
                        if (Main)
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !", Main);
                        else
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !");
                    }
                    break;
                case Properties.Float_MaxDelay:
                    if (value.GetType() == typeof(float))
                    {
                        autoDestroyFloat.max = (float)value;
                    }
                    else
                    {
                        if (Main)
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !", Main);
                        else
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !");
                    }
                    break;
                default:
                    break;
            }
        }





        public void OnEnd(BetterAudioSource caller)
        {
                if (autoDestroy && Application.isPlaying)
                {
                    caller.DoDestroy(autoDestroyFloat.Value);
                }
        }

        public void OnPlay(BetterAudioSource caller)
        {
            if (autoDestroy && Application.isPlaying)
            {
                caller.DoDestroy(autoDestroyFloat.Value);
            }
        }

        public void OnStart(BetterAudioSource caller)
        {
            if (autoDestroy && Application.isPlaying)
            {
                caller.DoDestroy(autoDestroyFloat.Value);
            }
        }
    }



}
