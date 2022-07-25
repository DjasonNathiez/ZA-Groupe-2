using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    /*
    private void Awake()
    {
        if (PlayerManager.instance.storyState >= 1)
        {
            GameManager.instance.Initialize();
            foreach (GameObject obj in activePost)
            {
                if(obj != null) continue;
                obj.SetActive(true);
            }
            foreach (GameObject obj in unactivePost)
            {
                if(obj != null) continue;
                obj.SetActive(false);
            }
        }
        else if (PlayerManager.instance.storyState == 0)
        {
            if(GameManager.instance != null) GameManager.instance.Initialize();
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
            GameManager.instance.enemyList.Remove(toDestroyOn2.GetComponent<BearBehaviour>());
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
    */

    private void Start()
    {
       
        Debug.Log("Start");
        
        if (PlayerManager.instance.storyState == -1)
        {
            PlayerManager.instance.storyState = 0;
        }
        
        if (PlayerManager.instance.storyState >= 1)
        {
            GameManager.instance.Initialize();
            foreach (GameObject obj in activePost)
            {
                if(obj == null) continue;
                obj.SetActive(true);
            }
            foreach (GameObject obj in unactivePost)
            {
                if(obj == null) continue;
                obj.SetActive(false);
            }
        }
        else if (PlayerManager.instance.storyState == 0)
        {
            if(GameManager.instance != null) GameManager.instance.Initialize();
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
            GameManager.instance.enemyList.Remove(toDestroyOn2.GetComponent<BearBehaviour>());
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
        
        
        
        
        GameManager.instance.CheckScene();

        StartCoroutine(LateStart());

    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(1);
        
        if (SceneManager.GetActiveScene().name.Contains("Parc"))
        {    
            if (DATA_LOAD.instance)
            {
                if (state != 0)
                {
                    DATA_LOAD.instance.LoadData();
                }
                
                if(state == 0 && !DATA_LOAD.instance.resume)
                {
                    DATA_LOAD.instance.SaveData();
                }

                if (DATA_LOAD.instance.resume)
                {
                    DATA_LOAD.instance.SetGameData();
                }
            }
        }
    }
}

[Serializable]
public class Collectable
{
    public GameObject loreObj;
    public string name;
}
