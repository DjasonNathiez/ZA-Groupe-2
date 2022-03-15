using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManagerMenu : MonoBehaviour
{
    [MenuItem("GameObject/GameManager", priority = 0)]
    public static void CreateGameManager()
    {
        var gameManager = Resources.Load("GameManager");
        PrefabUtility.InstantiatePrefab(gameManager);
        Debug.Log(gameManager.name);
        Debug.Log("Add a Game Manager To The Scene");
    }
}
