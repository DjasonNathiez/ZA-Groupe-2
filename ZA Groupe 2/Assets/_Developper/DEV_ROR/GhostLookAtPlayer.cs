using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostLookAtPlayer : MonoBehaviour
{
  [SerializeField]
   private Transform Player;
   public int speed = 3;

    public bool PlayerDetect;

    void Start()
    {
        Player = PlayerManager.instance.transform;
        PlayerDetect = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerDetect)
        {
            Vector3 relativePos = Player.position - transform.position;

            Quaternion rotation = Quaternion.LookRotation(relativePos);

            Quaternion current = transform.localRotation;

            transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == Player)
        {
            PlayerDetect = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == Player)
        {
            PlayerDetect = false;
        }
    }
}
