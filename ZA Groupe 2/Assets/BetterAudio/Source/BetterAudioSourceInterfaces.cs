using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    public interface IPlayBehaviour : IBehaviour
    {
        void OnPlay(BetterAudioSource caller);
    }
    
    public interface IStartBehaviour : IBehaviour
    {
        void OnStart(BetterAudioSource caller);
    }

    public interface IUpdateBehaviour : IBehaviour {
        void OnUpdate(BetterAudioSource caller, bool playing, bool clip);
    }

    public interface IEndBehaviour : IBehaviour
    {
        void OnEnd(BetterAudioSource caller);
    }

    public interface IBehaviour
    {
        string GetName();
        int GetPriority();
    }

}