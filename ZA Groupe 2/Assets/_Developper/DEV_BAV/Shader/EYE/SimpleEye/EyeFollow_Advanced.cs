using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class EyeFollow_Advanced : MonoBehaviour
{
    //public List<Material> materialZChange;
    [SerializeField] private float adjustFollow;
    [SerializeField] private Transform objectToFollow;

    //private List<GameObject> objectOnZWall;
    
    
    //X WALL
    [SerializeField]private List<GameObject> objectOnXWall;
    public List<Vector3> objectTransformX;
    public List<Material> matObjectX;

    
    //Z WALL
    [SerializeField]private List<GameObject> objectOnZWall;
    public List<Vector3> objectTransformZ;
    public List<Material> matObjectZ;

    
    
    public Vector3 roomSize;
    private Vector3 positionObjectToFollow;
    
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in objectOnXWall)
        {
            objectTransformX.Add(obj.transform.position);
            matObjectX.Add(obj.GetComponent<MeshRenderer>().material);
        }
        
        foreach (GameObject obj in objectOnZWall)
        {
            objectTransformZ.Add(obj.transform.position);
            matObjectZ.Add(obj.GetComponent<MeshRenderer>().material);
        }
    }

    private void Update()
    {
        positionObjectToFollow = transform.position;
        UpdateEyeFollowX();
    }

    public void SwitchFollowedPillar(Transform pillar)
    {
        objectToFollow = pillar;
    }

    void UpdateEyeFollowX()
    {
        float eyeMaxOffset = 0.5f;
        Vector3 position = objectToFollow.transform.position;
        for (int i = 0; i < matObjectX.Count; i++)
        {
            matObjectX[i].SetVector("_Pupil_Position", new Vector4(
                Mathf.Clamp((objectTransformX[i].x - position.x),
                -0.5f * adjustFollow, 
                0.5f * adjustFollow), 
                Mathf.Clamp((position.y - objectTransformX[i].y) * -1,
                    -0.25f, 
                    0.25f),
                0, 1));
        }
        
        for (int i = 0; i < matObjectZ.Count; i++)
        {
            matObjectZ[i].SetVector("_Pupil_Position", new Vector4(
                Mathf.Clamp((objectTransformZ[i].z - position.z),
                    -0.5f * adjustFollow, 
                    0.5f * adjustFollow), 
                Mathf.Clamp((position.y - objectTransformZ[i].y) * -1,
                    -0.25f, 
                    0.25f),
                0, 1));
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