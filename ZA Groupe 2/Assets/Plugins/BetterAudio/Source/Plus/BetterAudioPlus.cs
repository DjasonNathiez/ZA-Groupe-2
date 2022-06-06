using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    public static partial class BetterAudio
    {
        public static Behaviour[] colliderBehaviours = { AutoDestroy, ClipSerie, Delay, Fade, Follower, RandomClip, SetSource };
        public static Behaviour[] eventBehaviours = { AutoDestroy, ClipSerie, Delay, Fade, Follower, RandomClip, SetSource };
    }

    public abstract partial class BetterAudioBehaviour
    {
        public ColliderMode mode;
        public string ColliderTag;
        public string EventName;
    }
}
