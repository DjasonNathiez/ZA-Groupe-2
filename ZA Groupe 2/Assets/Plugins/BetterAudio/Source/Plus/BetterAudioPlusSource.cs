using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    public partial class BetterAudioSource
    {
        public bool showOnCollider;
        public bool showOnEvent;



        #region 2D Handlers
        private void OnCollisionEnter2D(Collision2D collision)
        {
            foreach (BetterAudioBehaviour b in ColliderBehaviours.items)
            {
                if(b.ColliderTag == null || b.ColliderTag == "" || collision.gameObject.CompareTag(b.ColliderTag))
                if (b.mode.HasFlag(ColliderMode.CollisionEnter2D))
                {
                    ((IColliderBehaviour)b).OnTrigger(this, ColliderMode.CollisionEnter2D, collision.gameObject, collision);
                }
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            foreach (BetterAudioBehaviour b in ColliderBehaviours.items)
            {
                if (b.ColliderTag == null || b.ColliderTag == "" || collision.gameObject.CompareTag(b.ColliderTag))
                    if (b.mode.HasFlag(ColliderMode.CollisionStay2D))
                {
                    ((IColliderBehaviour)b).OnTrigger(this, ColliderMode.CollisionStay2D, collision.gameObject, collision);
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            foreach (BetterAudioBehaviour b in ColliderBehaviours.items)
            {
                if (b.ColliderTag == null || b.ColliderTag == "" || collision.gameObject.CompareTag(b.ColliderTag))
                    if (b.mode.HasFlag(ColliderMode.CollisionExit2D))
                {
                    ((IColliderBehaviour)b).OnTrigger(this, ColliderMode.CollisionExit2D, collision.gameObject, collision);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            foreach (BetterAudioBehaviour b in ColliderBehaviours.items)
            {
                if (b.ColliderTag == null || b.ColliderTag == "" || collision.gameObject.CompareTag(b.ColliderTag))
                    if (b.mode.HasFlag(ColliderMode.TriggerEnter2D))
                {
                    ((IColliderBehaviour)b).OnTrigger(this, ColliderMode.TriggerEnter2D, collision.gameObject, collision);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            foreach (BetterAudioBehaviour b in ColliderBehaviours.items)
            {
                if (b.ColliderTag == null || b.ColliderTag == "" || collision.gameObject.CompareTag(b.ColliderTag))
                    if (b.mode.HasFlag(ColliderMode.TriggerStay2D))
                {
                    ((IColliderBehaviour)b).OnTrigger(this, ColliderMode.TriggerStay2D, collision.gameObject, collision);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            foreach (BetterAudioBehaviour b in ColliderBehaviours.items)
            {
                if (b.ColliderTag == null || b.ColliderTag == "" || collision.gameObject.CompareTag(b.ColliderTag))
                    if (b.mode.HasFlag(ColliderMode.TriggerExit2D))
                {
                    ((IColliderBehaviour)b).OnTrigger(this, ColliderMode.TriggerExit2D, collision.gameObject, collision);
                }
            }
        }
        #endregion

        #region 3D handlers
        private void OnCollisionEnter(Collision collision)
        {
            foreach (BetterAudioBehaviour b in ColliderBehaviours.items)
            {
                if (b.ColliderTag == null || b.ColliderTag == "" || collision.gameObject.CompareTag(b.ColliderTag))
                    if (b.mode.HasFlag(ColliderMode.CollisionEnter3D))
                {
                    ((IColliderBehaviour)b).OnTrigger(this, ColliderMode.CollisionEnter3D, collision.gameObject, collision);
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            foreach (BetterAudioBehaviour b in ColliderBehaviours.items)
            {
                if (b.ColliderTag == null || b.ColliderTag == "" || collision.gameObject.CompareTag(b.ColliderTag))
                    if (b.mode.HasFlag(ColliderMode.CollisionStay3D))
                {
                    ((IColliderBehaviour)b).OnTrigger(this, ColliderMode.CollisionStay3D, collision.gameObject, collision);
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            foreach (BetterAudioBehaviour b in ColliderBehaviours.items)
            {
                if (b.ColliderTag == null || b.ColliderTag == "" || collision.gameObject.CompareTag(b.ColliderTag))
                    if (b.mode.HasFlag(ColliderMode.CollisionExit3D))
                {
                    ((IColliderBehaviour)b).OnTrigger(this, ColliderMode.CollisionExit3D, collision.gameObject, collision);
                }
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            foreach (BetterAudioBehaviour b in ColliderBehaviours.items)
            {
                if (b.ColliderTag == null || b.ColliderTag == "" || collision.gameObject.CompareTag(b.ColliderTag))
                    if (b.mode.HasFlag(ColliderMode.TriggerEnter3D))
                {
                    ((IColliderBehaviour)b).OnTrigger(this, ColliderMode.TriggerEnter3D, collision.gameObject, collision);
                }
            }
        }

        private void OnTriggerStay(Collider collision)
        {
            foreach (BetterAudioBehaviour b in ColliderBehaviours.items)
            {
                if (b.ColliderTag == null || b.ColliderTag == "" || collision.gameObject.CompareTag(b.ColliderTag))
                    if (b.mode.HasFlag(ColliderMode.TriggerStay3D))
                {
                    ((IColliderBehaviour)b).OnTrigger(this, ColliderMode.TriggerStay3D, collision.gameObject, collision);
                }
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            foreach (BetterAudioBehaviour b in ColliderBehaviours.items)
            {
                if (b.ColliderTag == null || b.ColliderTag == "" || collision.gameObject.CompareTag(b.ColliderTag))
                    if (b.mode.HasFlag(ColliderMode.TriggerExit3D))
                {
                    ((IColliderBehaviour)b).OnTrigger(this, ColliderMode.TriggerExit3D, collision.gameObject, collision);
                }
            }
        }
        #endregion

    }
}
