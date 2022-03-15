using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    private Rigidbody m_rb;
    public float objectMass;
    public float fallSpeed;
    public float floatRange;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float distToGround = Vector3.Distance(transform.position, hit.point);

            if (distToGround > floatRange)
            {
                m_rb.AddForce(0, -fallSpeed * objectMass, 0, ForceMode.Acceleration);
            }
        }
    }
}
