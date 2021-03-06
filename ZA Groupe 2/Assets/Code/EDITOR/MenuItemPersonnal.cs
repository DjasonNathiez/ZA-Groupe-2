using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuItemPersonnal : MonoBehaviour
{
       [MenuItem("GameObject/Game Manager", priority = 0)]
        public static void CreateGameManager()
        {
            Object gameManager = Resources.Load("GameManager");
            PrefabUtility.InstantiatePrefab(gameManager);
            Debug.Log(gameManager.name);
            Debug.Log("Add a Game Manager To The Scene");
        }
        
        [MenuItem("GameObject/Grappin Spot", priority = 0)]
                public static void GrappinSpot()
                {
                    Object grappinSpot = Resources.Load("GrappinSpot");
                    Instantiate(grappinSpot);
                }

        
        [MenuItem("GameObject/Enemy/New Empty AI", priority = 0)]
        public static void CreateEmptyAI()
        {
            Object newAI = Resources.Load("Empty_AI");
            Instantiate(newAI);
        }

        
        [MenuItem("GameObject/Enemy/Lion", priority = 1)]
        public static void CreateLion()
        {
            Object newLion = Resources.Load("E_Lion");
            Instantiate(newLion);
        }
        
        [MenuItem("GameObject/Enemy/Bear", priority = 1)]
        public static void CreateBear()
        {
            Object newBear = Resources.Load("E_Bear");
            Instantiate(newBear);
        }
        
        [MenuItem("GameObject/Enemy/Rabbit", priority = 1)]
        public static void CreateRabbit()
        {
            Object newRabbit = Resources.Load("E_Rabbit");
            Instantiate(newRabbit);
        }
        
        [MenuItem("GameObject/Enemy/Turret", priority = 1)]
        public static void CreateTurret()
        {
            Object newTurret = Resources.Load("E_Turret");
            Instantiate(newTurret);
        }
}
