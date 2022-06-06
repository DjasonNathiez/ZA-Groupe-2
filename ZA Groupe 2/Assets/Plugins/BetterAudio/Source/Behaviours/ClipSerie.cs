using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    [System.Serializable]
    public partial class ClipSerie : BetterAudioBehaviour, IPlayBehaviour, IEndBehaviour
    {
        [SerializeField]
        [SerializeReference]
        BetterArrayAudioClip Clips;

        [SerializeField]
        int index = 0;

        public override void GUI(Rect rect)
        {
#if UNITY_EDITOR
            Clips.OnGUI(Main);
#endif
        }



        public override void Init(BetterAudioSource m)
        {
            base.Init(m);
            Name = "Clip serie";
            Priority = 10;
            Clips = new BetterArrayAudioClip(false);


        }

        public enum Properties
        {
            String_Name
        }

        public void SetProperty(Properties property, object value)
        {
            switch (property)
            {
                case Properties.String_Name:
                    if (value.GetType() == typeof(string))
                    {
                        Name = (string)value;
                    }
                    else
                    {
                        if (Main)
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !", Main);
                        else
                            Debug.LogError("[BetterAudio] Trying to set a properties with the wrong type !");
                    }
                    break;
                default:
                    break;
            }
        }

        public void OnEnd(BetterAudioSource caller)
        {
            Action(caller);
        }

        public void OnPlay(BetterAudioSource caller)
        {
            Action(caller);
        }

        public void Action(BetterAudioSource caller)
        {
            if (Clips.items.Count < index)
                index = 0;

            if (Clips.items.Count > 0)
            {
                caller.source.clip = (AudioClip) Clips.items[index];
                caller.source.Play();
            }

            index++;
        }
    }
}
