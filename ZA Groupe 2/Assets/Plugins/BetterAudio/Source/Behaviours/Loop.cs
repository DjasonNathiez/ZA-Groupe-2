using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BetterAudio
{
    [System.Serializable]
    public partial class Loop : BetterAudioBehaviour, IEndBehaviour
    {
        [SerializeField]
        [SerializeReference]
        public RandomInt loopCount;

        [SerializeField]
        private int _loop = -1;

        [SerializeField]
        public bool autoDestroy = false;
        [SerializeField]
        public RandomFloat autoDestroyFloat;

        [SerializeField] public bool callOnPlayOnLoop = true;

        public override void GUI(Rect rect)
        {
#if UNITY_EDITOR
            loopCount.DrawGUI();

            EditorGUILayout.Space();

            autoDestroy = EditorGUILayout.Toggle("Auto Destroy", autoDestroy);
            if (autoDestroy)
            {
                autoDestroyFloat.DrawGUI();
            }
            
            EditorGUILayout.Space();
            callOnPlayOnLoop = EditorGUILayout.Toggle(new GUIContent("Call On Play on loop", "Set to true if you want On Play behaviours to triggers on each loop."), callOnPlayOnLoop);
#endif
        }

        public override void Init(BetterAudioSource m)
        {
            base.Init(m);
            Name = "Loop x times";
            Priority = 100;

            loopCount = new RandomInt("Loop Count", sMin: 0, sMax: 999);
            autoDestroyFloat = new RandomFloat("Auto Destroy Delay", sMin: 0, sMax: 99999, value: 0);
        }



        public void OnEnd(BetterAudioSource caller)
        {
            Action(caller);
        }

        public void Action(BetterAudioSource caller)
        {
            if (_loop == -1)
                _loop = loopCount.Value;



            if ((_loop -= 1) >= 0)
            {
                caller.source.time = 0;
                caller.source.Play();
            }
            else
            {
                if (autoDestroy)
                {
                    caller.DoDestroy(autoDestroyFloat.Value);
                }
            }
        }
    }


    
}
