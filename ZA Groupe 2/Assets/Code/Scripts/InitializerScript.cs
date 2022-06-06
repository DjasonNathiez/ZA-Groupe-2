using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InitializerScript : MonoBehaviour
{
    public bool changestoryState;
    public int state;
    public GameObject[] activePost;
    public GameObject[] unactivePost;
    public GameObject[] activeManoir;
    public GameObject[] unactiveManoir;
    public GameObject[] activeArcade;
    public GameObject[] unactiveArcade;
    
    private void Awake()
    {
        if (PlayerManager.instance.storyState == 1)
        {
            GameManager.instance.Initialize();
            foreach (GameObject obj in activePost)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in unactivePost)
            {
                obj.SetActive(false);
            }
        }
        else if (PlayerManager.instance.storyState == 0)
        {
            GameManager.instance.Initialize();
            foreach (GameObject obj in activeManoir)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in unactiveManoir)
            {
                obj.SetActive(false);
            }
        }
        else if (PlayerManager.instance.storyState == 2)
        {
            GameManager.instance.Initialize();
            foreach (GameObject obj in activeArcade)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in unactiveArcade)
            {
                obj.SetActive(false);
            }
        }
        
        if(changestoryState) PlayerManager.instance.storyState = state;
    }

    private void Start()
    {
        if (PlayerManager.instance.storyState == -1)
        {
            PlayerManager.instance.storyState = 0;
        }
    }
}
