using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ThrowingWeapon : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    public Transform grip;
    

    private void OnTriggerEnter(Collider other)
    {
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

                playerManager.LoadVFX(playerManager.throwHit, other.transform);
            }
            else if (other.CompareTag("UngrippableObject"))
            {
                playerManager.state = ActionType.RopeAttached;
                playerManager.Rewind();

                if (other.GetComponent<LionBehaviour>())
                {
                    other.GetComponent<LionBehaviour>().StopCounterState();
                    playerManager.LoadVFX(playerManager.throwHitEnemy, other.transform);
                }

                if (other.GetComponent<Taupe>())
                {
                    other.GetComponent<Taupe>().TapeTaupeArcade.TaupeIsTaped(other.GetComponent<Taupe>().number);
                    other.GetComponent<Taupe>().TaupeHit();
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

        if (playerManager.state == ActionType.RopeAttached &&
            playerManager.rope.lenght >= playerManager.rope.maximumLenght)
        {
            playerManager.rope.rewinding = true;
        }

        if (playerManager.rope.rewinding)
        {
            grip.parent = transform;
        }
    }
}