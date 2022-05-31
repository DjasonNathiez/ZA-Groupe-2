using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BetterAudio
{
    [System.Serializable]
    public partial class DistanceFade : BetterAudioBehaviour, IUpdateBehaviour
    {
        [SerializeField]
        //public SceneTransform target;
        public BetterValue target;

        [SerializeField]
        public float minDistance = 0;

        [SerializeField]
        public float maxDistance = 10;

        public enum AxisMask
        {
            None = 0, // Custom name for "Nothing" option
            x = 1 << 0,
            y = 1 << 1,
            z = 1 << 2,
            All = ~0, // Custom name for "Everything" option
        }

        [SerializeField]
        public AxisMask axisMask;

        public float distance;

        public override void GUI(Rect rect)
        {
#if UNITY_EDITOR
            target.DrawGUI();

            minDistance = EditorGUILayout.FloatField("Min Distance", minDistance);
            maxDistance = EditorGUILayout.FloatField("Max Distance", maxDistance );

            axisMask = (AxisMask)EditorGUILayout.EnumFlagsField("Axis Mask", axisMask );

            EditorGUILayout.LabelField("Distance: " + distance, EditorStyles.miniLabel);
#endif
        }

        public override void Init(BetterAudioSource m)
        {
            base.Init(m);
            Name = "Distance Fade";
            Priority = 100;
            // target = new SceneTransform("Target Transform");
            target = new BetterValue("Target transform", m, m.transform);
        }

        public void Action(BetterAudioSource caller)
        {
            Vector3 mask = Vector3.zero;
            if (axisMask.HasFlag(AxisMask.x))
                mask.x = 1;

            if (axisMask.HasFlag(AxisMask.y))
                mask.y = 1;

            if (axisMask.HasFlag(AxisMask.z))
                mask.z = 1;


            Vector3 callerPos = caller.transform.position;
            //Vector3 targetPos = target.target.position;
            Vector3 targetPos = target.GetValue<Transform>().position;
            distance = Vector3.Distance(new Vector3(callerPos.x * mask.x, callerPos.y * mask.y, callerPos.z * mask.z), new Vector3(targetPos.x * mask.x, targetPos.y * mask.y, targetPos.z * mask.z));

            if (distance<= minDistance)
            {
                caller.source.volume = 1;
            }else if (distance >= maxDistance)
            {
                caller.source.volume = 0;
            }
            else
            {
                caller.source.volume = Mathf.Lerp(1, 0, (distance) / (maxDistance-minDistance));
            }

        }

        public void OnUpdate(BetterAudioSource caller, bool playing, bool clip)
        {
            Action(caller);
        }

    }
}
