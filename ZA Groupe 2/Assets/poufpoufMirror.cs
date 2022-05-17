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
        Rect rect = new Rect(new Vector2(transform.position.x, transform.position.z), new Vector2(detectSize.x, detectSize.z));
        if (rect.Contains(new Vector2(PlayerManager.instance.transform.position.x, PlayerManager.instance.transform.position.z)) && PlayerManager.instance.poufpoufInstantiated)
        {
            GameObject go = Instantiate(PlayerManager.instance.VFXPoufpouf, PlayerManager.instance.transform.position + PlayerManager.instance.transform.TransformVector( PlayerManager.instance.poufpoufOffset), Quaternion.identity);
            go.transform.parent = transform;
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z * -1);
        }
    }
}
