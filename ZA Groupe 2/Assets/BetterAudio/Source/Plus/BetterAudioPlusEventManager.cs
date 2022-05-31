using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    public static partial class BetterAudioEvent
    {
        /// <summary>
        /// Will trigger all behaviours in "Event" categorie with matching name (case sensible)
        /// </summary>
        /// <param name="eventName"></param>
        public static void SendEvent(string eventName, object argument = null)
        {
            BetterAudioSource[] sources = GameObject.FindObjectsOfType<BetterAudioSource>();

            foreach (BetterAudioSource source in sources)
            {
                foreach (BetterAudioBehaviour behaviour in source.EventBehaviours.items)
                {
                    if (behaviour.EventName == eventName)
                    {
                        ((IEventBehaviour)behaviour).OnEvent(source, argument);
                    }
                }
            }
        }

        /// <summary>
        /// Will trigger all behaviours in "Event" categorie with matching name (case sensible). Will output the last gameobject wich have been targeted.
        /// </summary>
        /// <param name="eventName"></param>
        public static void SendEvent(string eventName, out GameObject target, object argument = null)
        {
            BetterAudioSource[] sources = GameObject.FindObjectsOfType<BetterAudioSource>();

            target = null;
            foreach (BetterAudioSource source in sources)
            {
                foreach (BetterAudioBehaviour behaviour in source.EventBehaviours.items)
                {
                    if (behaviour.EventName == eventName)
                    {
                        ((IEventBehaviour)behaviour).OnEvent(source, argument);
                        
                    }
                }
                target = source.gameObject;
            }

            if (target == null)
            {
                Debug.LogError("[BetterAudio] Send Event do not have find any target");
            }
        }


        /// <summary>
        /// Will trigger all behaviours in "Event" categorie with matching name (case sensible). Will output all the targeted gameobjects.
        /// </summary>
        /// <param name="eventName"></param>
        public static void SendEvent(string eventName, out GameObject[] targets, object argument = null)
        {
            BetterAudioSource[] sources = GameObject.FindObjectsOfType<BetterAudioSource>();

            List<GameObject> objs = new List<GameObject>();
            foreach (BetterAudioSource source in sources)
            {
                foreach (BetterAudioBehaviour behaviour in source.EventBehaviours.items)
                {
                    if (behaviour.EventName == eventName)
                    {
                        ((IEventBehaviour)behaviour).OnEvent(source, argument);

                    }
                }
                objs.Add(source.gameObject);
            }

            if (objs.Count == 0)
            {
                Debug.LogError("[BetterAudio] Send Event do not have find any target");
            }

            targets = objs.ToArray();
        }
    }

}
