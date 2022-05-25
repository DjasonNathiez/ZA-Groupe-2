using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[ExecuteAlways]
public class EyeFollow_Basic : MonoBehaviour
{
    private MeshRenderer eyeRenderer;
    private Vector3 eyeOffset;
    public float eyeMaxOffset = 0.5f;
    public float smallValueOffset = 0.1f;


    public List<Material> materialXChange;
    public List<Material> materialZChange;

    public bool invertEye;

    // Start is called before the first frame update
    void Start()
    {
        eyeRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.localPosition;
        eyeOffset = new Vector3(position.x *smallValueOffset, -position.y *smallValueOffset, position.z * smallValueOffset);

        if (eyeOffset.x < -eyeMaxOffset || eyeOffset.x > eyeMaxOffset)
        {
            eyeOffset.x = Mathf.Clamp(eyeOffset.x, -eyeMaxOffset, eyeMaxOffset);
        }


        if (eyeOffset.y < -eyeMaxOffset || eyeOffset.y > eyeMaxOffset)
        {
            eyeOffset.y = Mathf.Clamp(eyeOffset.y, -eyeMaxOffset, eyeMaxOffset);
        }
        
        if (eyeOffset.z < -eyeMaxOffset || eyeOffset.z > eyeMaxOffset)
        {
            eyeOffset.z = Mathf.Clamp(eyeOffset.z, -eyeMaxOffset, eyeMaxOffset);
        }
        
        if (invertEye)
        {
            foreach (Material matZEye in materialZChange)
            {
                matZEye.SetVector("_EyePosition", new Vector4(eyeOffset.z, eyeOffset.y, 0, 1));
            }
        }

        foreach (Material matXEye in materialXChange)
        {
            matXEye.SetVector("_EyePosition", new Vector4(-eyeOffset.x, eyeOffset.y, 0, 1));
        }
    }
}