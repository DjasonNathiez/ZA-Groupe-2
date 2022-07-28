using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializerScript : MonoBehaviour
{
    public bool changestoryState;
    public int state;
    public GameObject toDestroyOn2;
    [SerializeField] public Collectable[] collectables;

    public enum StoryState
    {
        BeginParty, AfterArcade, AfterMansion
    }

    public GameObject[] beginPartyObjects;
    public GameObject[] afterArcadeObjects;
    public GameObject[] afterMansionObjects;

    private void Start()
    {
        InitializeScene();
        
        GameManager.instance.CheckScene();

        StartCoroutine(LateStart());
    }

    public void InitializeScene()
    {
        Debug.Log(PlayerManager.instance.currentStoryState);
        
        // Check triggers
        
        foreach (var obj in beginPartyObjects) obj.SetActive(false);
        foreach (var obj in afterArcadeObjects) obj.SetActive(false);
        foreach (var obj in afterMansionObjects) obj.SetActive(false);
        
        switch (PlayerManager.instance.currentStoryState)
        {
            case StoryState.BeginParty:
                foreach (var obj in beginPartyObjects) obj.SetActive(true);
                break;
            case StoryState.AfterArcade:
                foreach (var obj in afterArcadeObjects) obj.SetActive(true);
                break;
            case StoryState.AfterMansion:
                foreach (var obj in afterMansionObjects) obj.SetActive(true);
                break;
            
            default:
                Debug.Log("State invalide");
                break;
        }
        
        // Check items
        
        foreach (var col in collectables)
        {
            foreach (var hat in PlayerManager.instance.hats)
            {
                if (hat.hatName == col.name && hat.collected)
                {
                    col.loreObj.SetActive(false);
                }
            }
        }
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
