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
    public GameObject toDestroyOn2;
    [SerializeField] public Collectable[] collectables;

    private void Awake()
    {
        if (PlayerManager.instance.storyState >= 1)
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
            if(GameManager.instance) GameManager.instance.Initialize();
            foreach (GameObject obj in activeManoir)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in unactiveManoir)
            {
                obj.SetActive(false);
            }
        }

        if (PlayerManager.instance.storyState == 2 && toDestroyOn2)
        {
            Destroy(toDestroyOn2);
        }

        if(changestoryState) PlayerManager.instance.storyState = state;

        foreach (Collectable col in collectables)
        {
            foreach (PlayerManager.Hat hat in PlayerManager.instance.hats)
            {
                if (hat.hatName == col.name && hat.collected)
                {
                    col.loreObj.SetActive(false);
                }
            }
        }
    }

    private void Start()
    {
        if (PlayerManager.instance.storyState == -1)
        {
            PlayerManager.instance.storyState = 0;
        }
        
        GameManager.instance.CheckScene();
    }
}

[Serializable]
public class Collectable
{
    public GameObject loreObj;
    public string name;
}
