using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommandBase
{
    private string _commandID;
    private string _commandDescription;
    private string _commandFormat; 
    
    public string commandId
    {
        get { return _commandID; }
    }
    public string commandDescription
    {
        get { return _commandDescription; }
    }
    public string commandFormat
    {
        get { return _commandFormat; }
    }

    public DebugCommandBase(string id, string description, string format)
    {
        _commandID = id;
        _commandDescription = description;
        _commandFormat = format;
    }
}

public class DebugCommand : DebugCommandBase
{
    private Action command;
    
    public DebugCommand(string id, string description, string format, Action command) : base(id, description, format)
    {
        this.command = command; 
    }

    public void Invoke()
    {
        command.Invoke();
    }
}
