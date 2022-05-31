using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BetterAudio
{
    [System.Serializable]
    public partial class Fade : BetterAudioBehaviour, IPlayBehaviour
    {
        [SerializeReference]
        public InheritVolume InitialVolume;
        [SerializeReference]
        public InheritVolume TargetVolume;


        [SerializeField]
        public RandomFloat FadeTime;
        

        public override void GUI(Rect rect)
        {
#if UNITY_EDITOR
            InitialVolume.DrawGUI();
            TargetVolume.DrawGUI();
            
            FadeTime.DrawGUI();
#endif
        }

        public override void Init(BetterAudioSource m)
        {
            base.Init(m);
            Name = "Fade";
            Priority = 100;
            TargetVolume = new InheritVolume( "Target Volume");
            InitialVolume = new InheritVolume("Initial Volume");
            TargetVolume.SetCaller(Main.gameObject);
            InitialVolume.SetCaller(Main.gameObject);
            FadeTime = new RandomFloat("Fade Time", value: 2, sMin:0, sMax:9999999);
        }

        public void OnPlay(BetterAudioSource caller)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                caller.Error = "Cannot fade without playmode";
#endif
            Main = caller;
            TargetVolume.SetCaller(caller.gameObject);
            InitialVolume.SetCaller(caller.gameObject);
            caller.StartCoroutine(DoFade(InitialVolume.Value, TargetVolume.Value, FadeTime.Value));
        }

        IEnumerator DoFade(float I, float D, float T)
        {
            Main.source.volume = I;
            float t = 0;
            while((t+=Time.deltaTime) < T)
            {
                yield return new WaitForEndOfFrame();
                Main.source.volume = Mathf.Lerp(I, D, t / T);
            }
            Main.source.volume = D;

        }


    }
}
