using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ThrowingWeapon : MonoBehaviour
{
    [FormerlySerializedAs("m_PlayerManager")] [SerializeField] private PlayerManager playerManager;
    public Transform grip;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TurretBehaviour>())
        {
            other.GetComponent<TurretBehaviour>().isPin = true;
        }
        
        if (playerManager.state == "Throw")
        {
            if (other.CompareTag("GrippableObject"))
            {
                playerManager.state = "Rope";
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
                playerManager.rope.CheckElectrocution();
                
                if (other.GetComponent<PuzzleBoutton>())
                {
                    other.GetComponent<PuzzleBoutton>().isGripped = true;
                }
                
                
            }   
            else if (other.CompareTag("UngrippableObject"))
            {
                Debug.Log("HEEEEY YA");
                playerManager.state = "Rope";
                playerManager.Rewind();
            }
            else if (other.CompareTag("TractableObject"))
            {
                playerManager.state = "Rope";
                playerManager.rope.pinnedToObject = true;
                playerManager.rope.pinnedRb = other.attachedRigidbody;
                playerManager.rope.pinnedObjectDistance = playerManager.rope.lenght;
                grip.position = transform.position;
                grip.parent = other.transform;
            }
        }
    }
    
    private void Update()
    {
        if (playerManager.state == "Rope" && !playerManager.rope.rewinding)
        {
            transform.position = grip.position;
        }
        
        if (playerManager.state == "Throw" && playerManager.rope.lenght >= playerManager.rope.maximumLenght)
        {
            playerManager.rope.rewinding = true;
        }

        if (playerManager.rope.rewinding)
        {
            grip.parent = transform;
        }
    }
}
