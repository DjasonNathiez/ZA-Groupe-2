using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BetterAudio
{
    [System.Serializable]
    public partial class Delay : BetterAudioBehaviour, IPlayBehaviour, IStartBehaviour, IEndBehaviour
    {

        public enum SwitchAudioSourceMode
        {
            Toggle,
            Play,
            Stop
        }

        [SerializeField]
        public SwitchAudioSourceMode switchAudio;

        [SerializeField]
        [SerializeReference]
        public RandomFloat delay;

        public override void GUI(Rect rect)
        {
#if UNITY_EDITOR
            switchAudio = (SwitchAudioSourceMode)EditorGUILayout.EnumPopup("Mode", switchAudio);

            delay.DrawGUI();
#endif

        }

        public override void Init(BetterAudioSource m)
        {
            base.Init(m);
            Name = "Delay";
            Priority = 100;

            delay = new RandomFloat("Delay time:", sMin: 0, sMax: 99999);
        }

        public enum Properties
        {
            String_Name,
            SwitchAudioSourceMode_Mode,
            Bool_DelayRandom,
            Float_Delay,
            Float_Min,
            Float_Max
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
                case Properties.SwitchAudioSourceMode_Mode:
                    if (value.GetType() == typeof(SwitchAudioSourceMode))
                    {
                        switchAudio = (SwitchAudioSourceMode)value;
                    }
                    else
                    {
                        if (Main)
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !", Main);
                        else
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !");
                    }
                    break;
                case Properties.Bool_DelayRandom:
                    if (value.GetType() == typeof(bool))
                    {
                        delay.isRandom = (bool)value;
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
                        delay.Value = (float)value;
                    }
                    else
                    {
                        if (Main)
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !", Main);
                        else
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !");
                    }
                    break;
                case Properties.Float_Min:
                    if (value.GetType() == typeof(float))
                    {
                        delay.min = (float)value;
                    }
                    else
                    {
                        if (Main)
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !", Main);
                        else
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !");
                    }
                    break;
                case Properties.Float_Max:
                    if (value.GetType() == typeof(float))
                    {
                        delay.max = (float)value;
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

        public void OnPlay(BetterAudioSource caller)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                caller.Error = "Cannot Delay without playmode";
#endif
            Main = caller;

            Main.StartCoroutine(DoDelay());
        }

        public void OnStart(BetterAudioSource caller)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            Main = caller;

            Main.StartCoroutine(DoDelay());
        }

        IEnumerator DoDelay()
        {
            float d = delay.Value;
            while (d > 0)
            {
                yield return new WaitForEndOfFrame();
                d -= Time.deltaTime;
            }

            if (switchAudio == SwitchAudioSourceMode.Play)
                Main.source.Play();
            else if (switchAudio == SwitchAudioSourceMode.Stop)
                Main.source.Stop();
            else
            {
                if (Main.source.isPlaying)
                    Main.source.Stop();
                else
                {
                    Main.source.Play();
                }
            }
        }

        public void OnEnd(BetterAudioSource caller)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                caller.Error = "Cannot Delay without playmode";
#endif
            Main = caller;
            if (switchAudio != SwitchAudioSourceMode.Stop)
                caller.source.time = 0;

            Main.StartCoroutine(DoDelay());
        }
    }
}
