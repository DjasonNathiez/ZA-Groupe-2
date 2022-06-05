using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallingOnNewZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.CheckScene();
    }

}
