using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Helper : MonoBehaviour
{
    public BoxCollider epeeColldier;

    public void DisableCollider() // Faut laisser cette fonction svp
    {
        epeeColldier.enabled = false;
    }
}
