using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    public partial class SetSourceProperty : IColliderBehaviour, IEventBehaviour
    {
        public void OnEvent(BetterAudioSource caller, object argument = null)
        {
            setVal(caller);
        }

        public void OnTrigger(BetterAudioSource caller, ColliderMode mode, GameObject other, object information)
        {
            setVal(caller);
        }
    }
}