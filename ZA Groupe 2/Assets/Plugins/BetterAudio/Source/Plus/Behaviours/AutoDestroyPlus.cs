using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    public partial class AutoDestroy : IColliderBehaviour, IEventBehaviour
    {
        public void OnEvent(BetterAudioSource caller, object argument = null)
        {
            if (autoDestroy && Application.isPlaying)
            {
                caller.DoDestroy(autoDestroyFloat.Value);
            }
        }

        public void OnTrigger(BetterAudioSource caller, ColliderMode mode, GameObject other, object information)
        {
            if (autoDestroy && Application.isPlaying)
            {
                caller.DoDestroy(autoDestroyFloat.Value);
            }
        }
    }
}
