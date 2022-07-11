using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAnimationEnd : MonoBehaviour
{
    public void DestroyParentObj()
    {
        GameObject p_obj = transform.parent.gameObject;
        Destroy(p_obj);
    }
}
