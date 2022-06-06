using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    public partial class RandomClip : IColliderBehaviour, IEventBehaviour
    {
        public void OnEvent(BetterAudioSource caller, object argument = null)
        {
            Action(caller);
        }

        public void OnTrigger(BetterAudioSource caller, ColliderMode mode, GameObject other, object information)
        {
            Action(caller);
        }
    }
}