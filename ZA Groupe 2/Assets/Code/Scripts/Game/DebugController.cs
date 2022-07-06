using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{
    private bool showConsole;
    private string input;
    
    public static DebugCommand SPEED;
    public static DebugCommand GOD_MOD;

    public List<object> commandList;

    private void Awake()
    {
        SPEED = new DebugCommand("speed", "Change speed of player to 20", "speed", () =>
        {
            PlayerManager.instance.ChangeSpeedPlayer();
        });

        GOD_MOD = new DebugCommand("god_mod", "", "god_mod", () =>
        {
            PlayerManager.instance.SetGodMode();
        });
        
            commandList = new List<object>
        {
            SPEED,
            GOD_MOD,
        };
    }

    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame) showConsole = !showConsole;
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
