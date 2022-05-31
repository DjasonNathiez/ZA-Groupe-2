using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    public partial class DestroyAt : BetterAudioBehaviour, IUpdateBehaviour
    {
        [SerializeReference]
        public TimeProperty TriggerTime;

        [SerializeField]
        public RandomFloat DestroyDelay;

        public bool triggered = false;


        public override void GUI(Rect rect)
        {
#if UNITY_EDITOR
            TriggerTime.DrawGUI();

            DestroyDelay.DrawGUI();
#endif
        }

        public override bool canMultipleInstances()
        {
            return true;
        }

        public override void Init(BetterAudioSource m)
        {

            base.Init(m);
            Name = "Destroy At";
            Priority = 100;
            TriggerTime = new TimeProperty("Trigger Time", m);
            DestroyDelay = new RandomFloat("Destroy Delay");

        }

        public enum Properties
        {
            String_Name,
            Float01_TriggerTime,
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
                case Properties.Float01_TriggerTime:
                    if (value.GetType() == typeof(float))
                    {
                        TriggerTime.percent = (float)value;
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
                        DestroyDelay.isRandom = (bool)value;
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
                        DestroyDelay.Value = (float)value;
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
                        DestroyDelay.min = (float)value;
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
                        DestroyDelay.max = (float)value;
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

        public void OnUpdate(BetterAudioSource caller, bool playing, bool clip)
        {
            Main = caller;
            TriggerTime.SetCaller(caller);
            if (playing && clip)
            {
                if (caller.source.time > TriggerTime.percent * caller.source.clip.length)
                {
                    if (!triggered)
                    {
                        triggered = true;
                        caller.DoDestroy(DestroyDelay.Value);
                        return;
                    }
                }
                else
                {
                    triggered = false;
                }
            }

        }

    }
}
