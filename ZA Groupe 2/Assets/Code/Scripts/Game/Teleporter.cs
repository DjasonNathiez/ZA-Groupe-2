using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform teleportPoint;
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            player.transform.position = teleportPoint.position;
        }
    }
}
