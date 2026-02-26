using System;
using UnityEngine;

public class DevCommandBase
{
    private string _commandId;
    string _commandDescription;
    string _commandFormat;

    public string commandId { get { return _commandId; } }
    public string commandDescription { get { return _commandDescription; }}
    public string commandFormat { get { return _commandFormat; }}

    public DevCommandBase(string id, string description, string format)
    {
        _commandId = id;
        _commandDescription = description;
        _commandFormat = format;
    }
}

public class DevCommand : DevCommandBase
{
    Action command;
    public DevCommand(string id, string description, string format, Action command) : base (id, description, format)
    {
        this.command = command;
    }

    public void Invoke()
    {
        command.Invoke();
    }
}

public class DevCommand<T1> : DevCommandBase
{
    Action<T1> command;
    public DevCommand(string id, string description, string format, Action<T1> command) : base (id, description, format)
    {
        this.command = command;
    }

    public void Invoke(T1 value)
    {
        command.Invoke(value);
    }
}