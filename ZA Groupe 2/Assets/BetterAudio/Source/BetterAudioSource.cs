using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    [AddComponentMenu("BetterAudio/BetterSource")]
    [RequireComponent(typeof(AudioSource))]
    [ExecuteAlways]
    public partial class BetterAudioSource : MonoBehaviour
    {
        AudioSource _source;
        public AudioSource source
        {
            get
            {
                if (!_source)
                {
                    if (!TryGetComponent<AudioSource>(out _source))
                    {
                        Error = "No source found";
                        Debug.LogError("[BetterAudio] No audio source component found, please add one");
                    }
                }
                   

                return _source;
            }
        }

        public string Error;

        [SerializeField]
        public int selectedMenu;

        public Dictionary<string, LocalVariable> localVariables;


        [SerializeField]
        [SerializeReference]
        BehavioursGroup _startBehaviours;
        public BehavioursGroup StartBehaviours
        {
            get
            {
                if (_startBehaviours == null)
                {
                    _startBehaviours = new BehavioursGroup();
                }

                return _startBehaviours;
            }
        }

        [SerializeField]
        [SerializeReference]
        BehavioursGroup _playBehaviours;
        public BehavioursGroup PlayBehaviours
        {
            get
            {
                if (_playBehaviours == null)
                {
                    _playBehaviours = new BehavioursGroup();
                }

                return _playBehaviours;
            }
        }

        [SerializeField]
        [SerializeReference]
        BehavioursGroup _updateBehaviours;
        public BehavioursGroup UpdateBehaviours
        {
            get
            {
                if (_updateBehaviours == null)
                {
                    _updateBehaviours = new BehavioursGroup();
                }

                return _updateBehaviours;
            }
        }

        [SerializeField]
        [SerializeReference]
        BehavioursGroup _endBehaviours;
        public BehavioursGroup EndBehaviours
        {
            get
            {
                if (_endBehaviours == null)
                {
                    _endBehaviours = new BehavioursGroup();
                }

                return _endBehaviours;
            }
        }

        #region OnlyForPlusVersion
        [SerializeField]
        [SerializeReference]
        BehavioursGroup _colliderBehaviours;
        public BehavioursGroup ColliderBehaviours
        {
            get
            {
                if (_colliderBehaviours == null)
                {
                    _colliderBehaviours = new BehavioursGroup();
                }

                return _colliderBehaviours;
            }
        }

        [SerializeField]
        [SerializeReference]
        BehavioursGroup _eventBehaviours;
        public BehavioursGroup EventBehaviours
        {
            get
            {
                if (_eventBehaviours == null)
                {
                    _eventBehaviours = new BehavioursGroup();
                }

                return _eventBehaviours;
            }
        }
        #endregion


        bool _isPlaying = false;
        bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                if (_isPlaying == value)
                    return;

                _isPlaying = value;

                if (value)
                {
                    StartCoroutine(doOnPlay());
                }
                else
                {
                    StartCoroutine(doOnEnd());
                }
            }
        }

        public bool EditorPlay;


        void Awake()
        {
            localVariables = new Dictionary<string, LocalVariable>();

            for (int i = 0; i < StartBehaviours.items.Count; i++)
            {
                ((IStartBehaviour)StartBehaviours.items[i]).OnStart(this);

            }

#if UNITY_EDITOR
            if (Application.isPlaying || EditorPlay)
#endif
                if (source.playOnAwake)
                {
                    source.Play();
                }
        }


        private void FixedUpdate()
        {
            IsPlaying = source.isPlaying;
        }


        // Update is called once per frame
        void Update()
        {

                for (int i = 0; i < UpdateBehaviours.items.Count; i++)
                {
                    if (Application.isPlaying)
                       ((IUpdateBehaviour)UpdateBehaviours.items[i]).OnUpdate(this, IsPlaying, source.clip);

                }
            
        }

        public void DoDestroy(float delay)
        {
            Destroy(gameObject, delay);
        }

        IEnumerator doOnPlay()
        {
            for (int i = 0; i < PlayBehaviours.items.Count; i++)
            {
                
                ((IPlayBehaviour)(PlayBehaviours.items[i])).OnPlay(this);
                if (PlayBehaviours.items[i].GetPriority() < 50)
                    source.Play();

            }

            yield return null;
        }

        IEnumerator doOnEnd()
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < EndBehaviours.items.Count; i++)
            {
                ((IEndBehaviour)EndBehaviours.items[i]).OnEnd(this);

            }
            yield return null;
        }

        /// <summary>
        /// Get behaviour return the first behaviour of type T with the corresponding name in the selected categorie.
        /// </summary>
        /// <typeparam name="T">The type researched, example: Fade, AutoDestroy, Loop</typeparam>
        /// <param name="categorie">List of categories, Collider and Event categories are only available with the Plus version.</param>
        /// <param name="name">The name of the targeted behaviour (Shown next to the button remove in the inspector)</param>
        /// <param name="CanFindNothing">If toggled no error message will be sent if no behaviour are find</param>
        /// <returns>The first behaviour with matching parameters</returns>
        public T GetBehaviour<T>(Categories categorie, string name, bool CanFindNothing = false) where T : BetterAudioBehaviour
        {

            T result=null;

            if (categorie.HasFlag(Categories.Start))
                if (StartBehaviours.Find<T>(name, out result))
                        return result;
            if (categorie.HasFlag(Categories.Play))
                if (PlayBehaviours.Find<T>(name, out result))
                        return result;
            if (categorie.HasFlag(Categories.Update))
                if (UpdateBehaviours.Find<T>(name, out result))
                        return result;
            if (categorie.HasFlag(Categories.End))
                if (EndBehaviours.Find<T>(name, out result))
                        return result;
            if (categorie.HasFlag(Categories.Collider))
                if (ColliderBehaviours.Find<T>(name, out result))
                        return result;
            if (categorie.HasFlag(Categories.Event))
                if (EventBehaviours.Find<T>(name, out result))
                        return result;

            
            if (result==null&& !CanFindNothing)
            {
                Debug.LogError("[BetterAudio] Get behaviour returned nothing, please verify your arguments.", gameObject);
            }

            return result;
        }

        /// <summary>
        /// Get behaviour return all behaviours of type T with the corresponding name in the selected categorie.
        /// </summary>
        /// <typeparam name="T">The type researched, example: Fade, AutoDestroy, Loop</typeparam>
        /// <param name="categorie">List of categories, Collider and Event categories are only available with the Plus version.</param>
        /// <param name="name">The name of the targeted behaviour (Shown next to the button remove in the inspector)</param>
        /// <param name="CanFindNothing">If toggled no error message will be sent if no behaviour are find</param>
        /// <returns>All behaviours with matching parameters</returns>
        public T[] GetBehaviours<T>(Categories categorie, string name, bool CanFindNothing = false) where T : BetterAudioBehaviour
        {

            List<T> _result = new List<T>();
            T result = null;

            if (categorie.HasFlag(Categories.Start))
                if (StartBehaviours.Find<T>(name, out result))
                    _result.Add(result);
            if (categorie.HasFlag(Categories.Play))
                if (PlayBehaviours.Find<T>(name, out result))
                    _result.Add(result);
            if (categorie.HasFlag(Categories.Update))
                if (UpdateBehaviours.Find<T>(name, out result))
                    _result.Add(result);
            if (categorie.HasFlag(Categories.End))
                if (EndBehaviours.Find<T>(name, out result))
                    _result.Add(result);
            if (categorie.HasFlag(Categories.Collider))
                if (ColliderBehaviours.Find<T>(name, out result))
                    _result.Add(result);
            if (categorie.HasFlag(Categories.Event))
                if (EventBehaviours.Find<T>(name, out result))
                    _result.Add(result);


            if (_result.Count == 0 && !CanFindNothing)
            {
                Debug.LogError("[BetterAudio] Get behaviour returned nothing, please verify your arguments.", gameObject);
            }

            return _result.ToArray();
        }


    }
}
