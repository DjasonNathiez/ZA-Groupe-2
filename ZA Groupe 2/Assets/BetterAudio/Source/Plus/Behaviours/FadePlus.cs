using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    public partial class Fade : IColliderBehaviour, IEventBehaviour
    {
        public void OnEvent(BetterAudioSource caller, object argument = null)
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

        public void OnTrigger(BetterAudioSource caller, ColliderMode mode, GameObject other, object information)
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
    }
}