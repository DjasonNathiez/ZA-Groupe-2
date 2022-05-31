using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    public partial class Delay : IColliderBehaviour, IEventBehaviour
    {
        public void OnEvent(BetterAudioSource caller, object argument = null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            Main = caller;

            Main.StartCoroutine(DoDelay());
        }

        public void OnTrigger(BetterAudioSource caller, ColliderMode mode, GameObject other, object information)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            Main = caller;

            Main.StartCoroutine(DoDelay());
        }
    }
}