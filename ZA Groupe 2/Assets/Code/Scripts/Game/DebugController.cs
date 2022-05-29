using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugController : MonoBehaviour
{
    private bool showConsole;
    private string input;
    
    public static DebugCommand PLAYER_TP;
    public static DebugCommand RELOAD_SCENE;
    public static DebugCommand SPEED;

    public List<object> commandList;

    private void Awake()
    {
        PLAYER_TP = new DebugCommand("player_tp", "Reset player Position on 0,0,0.", "player_tp", () =>
        {
            PlayerManager.instance.gameObject.transform.position = Vector3.zero;
        });
        RELOAD_SCENE = new DebugCommand("reload_scene", "Reload current scene", "reload_scene", () =>
        {
            GameManager.instance.LoadScene("MAP_Parc");
        });
        SPEED = new DebugCommand("speed", "Change speed of player to 20", "speed", () =>
        {
            PlayerManager.instance.ChangeSpeedPlayer();
        });

            commandList = new List<object>
        {
            PLAYER_TP,
            RELOAD_SCENE,
            SPEED,
        };
    }

    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame) showConsole = !showConsole;
        if (!Keyboard.current.backslashKey.wasPressedThisFrame) return;
        if (!showConsole) return;
        HandleInput();
        input = "";
    }

    private void OnGUI()
    {
        if (!showConsole) return;
        
        float y  = 0f;
        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
    }

    private void HandleInput()
    {
        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;
            if (input.Contains(commandBase.commandId))
            {
                if (commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                }
            }
        }
    }
}
