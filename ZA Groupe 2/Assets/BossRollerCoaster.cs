using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Timers;
using UnityEngine;

public class BossRollerCoaster : MonoBehaviour
{
    public rotatingProp rotatingProp;
    public GameObject crank;
    public FollowCurve wagon;
    public QuadraticCurve curve;
    public GameObject entry;
    public Material materialLight;
    public bool returning;
    public BossBehaviour boss;
    
    void Update()
    {
        if (rotatingProp.gameObject.activeInHierarchy)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(-10.78f, 6.14f,  Mathf.Abs(rotatingProp.myrotation) / 720), transform.position.z);
            wagon.transform.position = curve.points[0].point;

            if (Mathf.Abs(rotatingProp.myrotation) >= 720)
            {
                crank.SetActive(false);
                if (PlayerManager.instance.rope.pinnedTo == rotatingProp.gameObject) PlayerManager.instance.Rewind();
                wagon.points = curve.points;
                entry.SetActive(true);
                rotatingProp.myrotation = 0;
                materialLight.SetFloat("_flick",1);
            }
        }
        else if (returning)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, -10.78f,  Time.deltaTime*5), transform.position.z);
            wagon.transform.position = curve.points[0].point;
        }
    }

    public void EndRollerCoaster()
    {
        entry.SetActive(false);
        StartCoroutine(boss.BreakRoller(1));
        boss.spawnedRabbit = false;
    }
}
