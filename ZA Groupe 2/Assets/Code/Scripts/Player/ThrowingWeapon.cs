using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ThrowingWeapon : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    private Vector3 collisionPoint;
    public Transform grip;

    private void OnTriggerEnter(Collider other)
    {
        collisionPoint = other.ClosestPoint(transform.position);
        var normalPoint = transform.position - collisionPoint;

        if (other.GetComponent<TurretBehaviour>())
        {
            other.GetComponent<TurretBehaviour>().isPin = true;
        }

        if (playerManager.state == ActionType.Throwing)
        {
            if (other.CompareTag("GrippableObject"))
            {
                playerManager.PlaySFX("P_RopeGrip");
                playerManager.state = ActionType.RopeAttached;
                if (other.ClosestPoint(transform.position) == transform.position)
                {
                    grip.position = other.ClosestPoint(playerManager.transform.position) - transform.forward * 0.3f;
                }
                else
                {
                    grip.position = other.ClosestPoint(transform.position) - transform.forward * 0.3f;
                }

                grip.parent = other.transform;
                playerManager.rope.pinnedTo = other.gameObject;
                if (other.GetComponent<ValueTrack>())
                    playerManager.rope.pinnedValueTrack = other.GetComponent<ValueTrack>();
                playerManager.rope.CheckElectrocution();

                if (other.GetComponent<PuzzleBoutton>())
                {
                    other.GetComponent<PuzzleBoutton>().isGripped = true;
                }

                if (other.GetComponent<ElectrocutedProp>())
                {
                    if (!playerManager.gloves)
                    {
                        playerManager.state = ActionType.RopeAttached;
                        playerManager.Rewind();
                    }
                }
                
                GameManager.instance.RumbleConstant(.15f, .2f, .2f);
                Destroy(Instantiate(playerManager.throwHit, collisionPoint, Quaternion.LookRotation(normalPoint)), 3f);
            }
            else if (other.CompareTag("UngrippableObject"))
            {
                playerManager.state = ActionType.RopeAttached;
                playerManager.Rewind();

                if (other.GetComponent<LionBehaviour>())
                {
                    other.GetComponent<LionBehaviour>().StopCounterState();
                    Destroy(Instantiate(playerManager.throwHitEnemy, collisionPoint,
                        Quaternion.LookRotation(normalPoint)));
                }

                if (other.GetComponent<Taupe>())
                {
                    Destroy(Instantiate(playerManager.throwHitEnemy, collisionPoint,
                        Quaternion.LookRotation(normalPoint)));
                    other.GetComponent<Taupe>().TaupeHit(true);
                }
            }
            else if (other.CompareTag("TractableObject"))
            {
                playerManager.PlaySFX("P_RopeGrip");
                playerManager.state = ActionType.RopeAttached;
                playerManager.rope.pinnedTo = other.gameObject;
                playerManager.rope.pinnedToObject = true;
                if (other.GetComponent<ValueTrack>())
                    playerManager.rope.pinnedValueTrack = other.GetComponent<ValueTrack>();
                playerManager.rope.pinnedRb = other.attachedRigidbody;
                if (playerManager.rope.pinnedValueTrack.trailVFX != null)
                {
                    playerManager.rope.pinnedValueTrack.trailVFX.gameObject.SetActive(true);
                    playerManager.rope.pinnedValueTrack.trailVFX.Play();
                }

                grip.position = transform.position;
                grip.parent = other.transform;

                if (other.GetComponent<ElectrocutedProp>())
                {
                    if (!playerManager.gloves)
                    {
                        playerManager.state = ActionType.RopeAttached;
                        playerManager.Rewind();
                    }
                }
                
                GameManager.instance.RumbleConstant(.15f, .2f, .2f);
                Destroy(Instantiate(playerManager.throwHit, collisionPoint, Quaternion.LookRotation(normalPoint)), 3f);
            }
        }
    }

    private void Update()
    {
        if (playerManager.state == ActionType.RopeAttached && !playerManager.rope.rewinding)
        {
            transform.position = grip.position;
            transform.rotation = grip.rotation;
        }

        if (playerManager.state == ActionType.Throwing &&
            playerManager.rope.lenght >= playerManager.rope.maximumLenght)
        {
            playerManager.Rewind();
        }

        if (playerManager.rope.rewinding)
        {
            grip.parent = transform;
        }
    }
}