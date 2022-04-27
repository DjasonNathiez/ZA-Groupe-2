using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class EyeFollow_Advanced : MonoBehaviour
{
    //public List<Material> materialZChange;

    [SerializeField]private List<GameObject> objectOnXWall;
    //private List<GameObject> objectOnZWall;
    
    public List<Material> matObjectX;
    public Vector3 roomSize;
    private Vector3 positionObjectToFollow;
    
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject objectX in objectOnXWall)
        {
            matObjectX.Add(objectX.GetComponent<MeshRenderer>().sharedMaterial);
        }
    }

    private void Update()
    {
        positionObjectToFollow = transform.localPosition;
        UpdateEyeFollowX();
    }

    void UpdateEyeFollowX()
    {
        float eyeMaxOffset = 0.5f;
        
        for (int m = 0; m < objectOnXWall.Count; m++)
        {
            for (int i = 0; i < matObjectX.Count; i++)
            {
                positionObjectToFollow.x *= objectOnXWall[i].transform.position.x;
                positionObjectToFollow.x = Mathf.Clamp(positionObjectToFollow.x,-eyeMaxOffset, eyeMaxOffset);
                matObjectX[m].SetVector("_EyePosition", new Vector4(-positionObjectToFollow.x , positionObjectToFollow.y , 0, 1));
            }
            Debug.Log(positionObjectToFollow.x);

        }
    }

    private void OnDisable()
    {
        matObjectX.Clear();
    }

    /*
    // Update is called once per frame
    void Update()
    {

        eyeOffset = new Vector3(position.x *smallValueOffset, -position.y *smallValueOffset, position.z * smallValueOffset);


        
        if (invertEye)
        {
            foreach (Material matZEye in materialZChange)
            {
                matZEye.SetVector("_EyePosition", new Vector4(eyeOffset.z, eyeOffset.y, 0, 1));
            }
        }

        foreach (Material matXEye in materialXChange)
        {
            
        }
    }
    */
}