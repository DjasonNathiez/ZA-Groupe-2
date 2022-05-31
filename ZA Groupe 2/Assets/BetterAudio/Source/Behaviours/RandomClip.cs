using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

namespace BetterAudio
{
    [System.Serializable]
    public partial class RandomClip : BetterAudioBehaviour, IStartBehaviour, IPlayBehaviour, IEndBehaviour
    {
        [SerializeField]
        [SerializeReference]
        BetterArrayAudioClip Clips;

        [SerializeField] public bool avoidDouble = true;

        public override void GUI(Rect rect)
        {
#if UNITY_EDITOR

            Clips.OnGUI(Main);
            EditorGUILayout.Space();
            if (Clips.items.Count > 2)
            {
                avoidDouble =
                    EditorGUILayout.Toggle(
                        new GUIContent("Avoid Double", "If true avoid the same clip to be played twice in chain."),
                        avoidDouble);
            }
            else
            {
                
                EditorGUI.BeginDisabledGroup(true);
                avoidDouble =
                    EditorGUILayout.Toggle(
                        new GUIContent("Avoid Double", "If true avoid the same clip to be played twice in chain."),
                        avoidDouble);
                GUILayout.Label("With 2 clips or less you cannot have random without double.");
                EditorGUI.EndDisabledGroup();
            }

#endif
        }

        

        public override void Init(BetterAudioSource m)
        {
            base.Init(m);
            Name = "Random Clip";
            Priority = 10;
            Clips = new BetterArrayAudioClip(false);
            

        }

        public void Action(BetterAudioSource caller)
        {
            if (Clips.items.Count > 0)
            {
                if (avoidDouble && Clips.items.Count > 2)
                {
                    AudioClip currentClip = caller.source.clip;
                    List<AudioClip> clipsBuffer = new List<AudioClip>(Clips.items);
                    if (clipsBuffer.Contains(currentClip))
                        clipsBuffer.Remove(currentClip);
                    
                    caller.source.clip = (AudioClip) clipsBuffer[Random.Range(0, clipsBuffer.Count)];
                }
                else
                {
                    caller.source.clip = (AudioClip) Clips.items[Random.Range(0, Clips.items.Count)];
                }
            }
            else
            {
                Debug.LogError("[BetterAudio] RandomClip called without clips.", caller.gameObject);
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

        public void OnStart(BetterAudioSource caller)
        {
            Action(caller);
        }
    }
}
