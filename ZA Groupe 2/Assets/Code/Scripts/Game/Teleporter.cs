using System.Collections;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform teleportPoint;
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            Debug.Log("attention");
            StartCoroutine(ChangeScene(player));
        }
    }
    
    public void StartTP()
    {
        StartCoroutine(ChangeScene(PlayerManager.instance));
    }
    
    public IEnumerator ChangeScene(PlayerManager player)
    {
        player.Rewind();
        GameManager.instance.transitionOn = false;
        yield return new WaitForSeconds(.4f);
        player.transform.position = teleportPoint.position;
        player.cameraController.transform.position = teleportPoint.position;
        PlayerManager.instance.ExitDialogue();
        GameManager.instance.transitionOn = true;
    }
}
