using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio{
    public interface IColliderBehaviour : IBehaviour
    {
        void OnTrigger(BetterAudioSource caller, ColliderMode mode, GameObject other, object information);
    }


    public interface IEventBehaviour : IBehaviour
    {
        void OnEvent(BetterAudioSource caller, object argument = null);
    }
}
