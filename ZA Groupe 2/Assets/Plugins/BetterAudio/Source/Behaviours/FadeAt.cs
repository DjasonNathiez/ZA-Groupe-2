using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BetterAudio
{
    [System.Serializable]
    public partial class FadeAt : BetterAudioBehaviour, IUpdateBehaviour
    {
        [SerializeReference]
        public TimeProperty TriggerTime;

        [SerializeReference]
        public InheritVolume InitialVolume;
        [SerializeReference]
        public InheritVolume TargetVolume;


        [SerializeField]
        public RandomFloat FadeTime;

        public bool triggered = false;


        public override void GUI(Rect rect)
        {
#if UNITY_EDITOR
            TriggerTime.DrawGUI();

            InitialVolume.DrawGUI();
            TargetVolume.DrawGUI();

            FadeTime.DrawGUI();
#endif
        }

        public override bool canMultipleInstances()
        {
            return true;
        }

        public override void Init(BetterAudioSource m)
        {

            base.Init(m);
            Name = "Fade At";
            Priority = 100;
            TriggerTime = new TimeProperty("Trigger Time", m);
            TargetVolume = new InheritVolume("Target Volume");
            InitialVolume = new InheritVolume("Initial Volume");
            TargetVolume.SetCaller(Main.gameObject);
            InitialVolume.SetCaller(Main.gameObject);
            FadeTime = new RandomFloat("Fade Time", value: 2, sMin: 0, sMax: 9999999);
        }

        public void OnUpdate(BetterAudioSource caller, bool playing, bool clip)
        {
            if (playing && clip)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    caller.Error = "Cannot fade without playmode";
#endif
                Main = caller;
                TriggerTime.SetCaller(caller);
                TargetVolume.SetCaller(caller.gameObject);
                InitialVolume.SetCaller(caller.gameObject);

                if (caller.source.time > TriggerTime.percent * caller.source.clip.length)
                {
                    if (!triggered)
                    {
                        triggered = true;
                        caller.StartCoroutine(DoFade(InitialVolume.Value, TargetVolume.Value, FadeTime.Value));
                        return;
                    }
                }
                else
                {
                    triggered = false;
                }
            }
        }

        IEnumerator DoFade(float I, float D, float T)
        {
            Main.source.volume = I;
            float t = 0;
            while ((t += Time.deltaTime) < T)
            {
                yield return new WaitForEndOfFrame();
                Main.source.volume = Mathf.Lerp(I, D, t / T);
            }
            Main.source.volume = D;

        }


    }
}
