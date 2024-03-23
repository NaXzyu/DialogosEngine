using CommandTerminal;
using System;
using System.Collections.Generic;

public class Kernel
{
    private static Kernel _instance;
    public static Kernel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Kernel();
            }
            return _instance;
        }
    }

    private Dictionary<string, Action> commandDelegates = new Dictionary<string, Action>();

    private Kernel() { }

    public void AddCommand(Terminal terminal, string commandName, Action commandDelegate)
    {
        if (!commandDelegates.ContainsKey(commandName))
        {
            commandDelegates.Add(commandName, commandDelegate);
            SessionManager.Instance.RegisterCommand(commandName);
        }
        else
        {
            terminal.LogWarning($"[KERNEL] The command '{commandName}' is already registered.");
        }
    }

    public void ExecuteCommand(Terminal terminal, string commandName)
    {
        if (commandDelegates.TryGetValue(commandName, out Action commandDelegate))
        {
            commandDelegate();
            SessionManager.Instance.LogCommandExecution(commandName);
        }
        else
        {
            terminal.LogError($"[KERNEL] The command '{commandName}' does not exist.");
        }
    }

    public void RemoveCommand(Terminal terminal, string commandName)
    {
        if (commandDelegates.ContainsKey(commandName))
        {
            commandDelegates.Remove(commandName);
            SessionManager.Instance.UnregisterCommand(commandName);
        }
        else
        {
            terminal.LogError($"[KERNEL] The command '{commandName}' cannot be removed because it does not exist.");
        }
    }

    public void ClearCommands()
    {
        commandDelegates.Clear();
        SessionManager.Instance.ClearAllCommands();
    }
}
