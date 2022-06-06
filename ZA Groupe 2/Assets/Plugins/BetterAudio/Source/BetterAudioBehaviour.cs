using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    [AddComponentMenu("BetterAudio/")]
    [System.Serializable]
    public abstract partial class BetterAudioBehaviour : IBehaviour
    {
        [SerializeField]
        public int Priority = 100;
        [SerializeField]
        public string Name;

        [SerializeField]
        public bool IsShowed;

        public virtual bool canMultipleInstances()
        {
            return false;
        }


        [SerializeField]
        [SerializeReference]
        public BetterAudioSource Main;



        public virtual void Init(BetterAudioSource m)
        {
            Main = m;
        }

        public abstract void GUI(Rect rect);

        public string GetName()
        {
            return Name;
        }

        public int GetPriority()
        {
            return Priority;
        }
    }
}
