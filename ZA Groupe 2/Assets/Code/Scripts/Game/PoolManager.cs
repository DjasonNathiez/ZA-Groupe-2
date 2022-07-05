using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    #region INSTANCE

    public static PoolManager Instance;

    private void Awake()
    {
       if(Instance == null) Instance = this;
    }

    #endregion
    
    public enum Object
    {
        // Bear Vfx
        VFX_BearHurt = 0,
        VFX_BearDeathExplosion, 
        VFX_BearAttack,
        VFX_BearHitable,

        // Lion Vfx
        VFX_LionDeathExplosion, 
        VFX_Lion_Fall, 
        VFX_Lion_Death, 
        VFX_Lion_StandUp, 
        VFX_Lion_Hurt, 
        VFX_Lion_Attack,
        
        // Rabbit
        VFX_Rabbit_Chase_Surprise,
        VFX_Rabbit_Attack, 
        VFX_Rabbit_Damage, 
        VFX_Rabbit_Death, 
        VFX_Rabbit_Chase_Begin, 
        VFX_Rabbit_Chase_Continue, 
        VFX_Rabbit_Chase_End, 
        VFX_Rabbit_Manga_Angry, 
        
        // Kent
        VFX_KentHurt, 
        VFX_KentAttack, 
        VFX_KentRoll, 
        VFX_Rope_Thow, 
        VFX_PuzzleReussite
        
    }
    
    [System.Serializable]
    struct Pool
    {
       public GameObject ObjectPrefabs;
       public Object typeOfObject;
    }
    

    [SerializeField] private Pool[] poolArray = new Pool[22];
    private Dictionary<Object, Queue<GameObject>> queueDictionary = new Dictionary<Object, Queue<GameObject>>(); // Store Queue of Object
    private Dictionary<Object, GameObject> prefabDictionary = new Dictionary<Object, GameObject>(); // Store prefabs independently of Queue


    void Start()
    {
        InitPool();
    }

    void InitPool()
    {
        foreach (Pool pol in poolArray)
        {
            queueDictionary.Add(pol.typeOfObject, new Queue<GameObject>());
            prefabDictionary.Add(pol.typeOfObject, pol.ObjectPrefabs);
//            Debug.Log(pol.typeOfObject);
        }
    }

    public GameObject PoolInstantiate(Object typeOfObject)
    {
        GameObject GM;
        Queue<GameObject> queue = queueDictionary[typeOfObject];
        if (queue.Count == 0)
        {
            GM = Instantiate(prefabDictionary[typeOfObject]);
        }
        else
        {
            GM = queue.Dequeue();
            GM.SetActive(true);
        }

        return GM;
    }

}
