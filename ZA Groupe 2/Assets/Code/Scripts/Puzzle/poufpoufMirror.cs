using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class poufpoufMirror : MonoBehaviour
{
    public Vector3 detectSize;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position,detectSize);
    }

    private void Update()
    {
        Rect rect = new Rect(new Vector2(transform.position.x-detectSize.x/2, transform.position.z-detectSize.z/2), new Vector2(detectSize.x, detectSize.z));
        Debug.DrawLine(new Vector3(rect.max.x,0,rect.max.y),new Vector3(rect.min.x,0,rect.min.y),Color.blue);
        if (rect.Contains(new Vector2(PlayerManager.instance.transform.position.x, PlayerManager.instance.transform.position.z)) && PlayerManager.instance.poufpoufInstantiated)
        {
            PlayerManager.instance.poufpoufInstantiated = false;
            //GameObject go = Instantiate(PlayerManager.instance.VFXPoufpouf, PlayerManager.instance.transform.position + PlayerManager.instance.transform.TransformVector( PlayerManager.instance.poufpoufOffset), Quaternion.identity).gameObject;
            //go.transform.parent = transform;
            //go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, go.transform.localPosition.z * -1);
        }
    }
}
