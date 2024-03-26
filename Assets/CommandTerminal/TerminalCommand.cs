using UnityEngine;
using CommandTerminal;
using System;
using System.Reflection;
using System.Collections.Generic;

public class TerminalCommand
{
    public static Terminal Terminal;
    public Dictionary<string, MethodInfo> CommandMethods;
    public const string k_RegisterCommand = "Register";

    public TerminalCommand()
    {
        CommandMethods = TerminalUtils.CacheCommandMethods();
    }

    public void Initialize(Terminal terminal, TextAsset bootstrapFile)
    {
        Terminal = terminal;
        RegisterCommandsFromAsset(bootstrapFile);
    }

    private void RegisterCommandsFromAsset(TextAsset asset)
    {
        string[] lines = asset.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("#") && !string.IsNullOrWhiteSpace(line))
            {
                CommandData? commandData = CommandUtils.ParseCommandData(line);

                if (commandData == null)
                {
                    Terminal.LogError("Critical Bootstrap Error. Quit.");
                    Utility.Quit();
                }

                if (commandData.Value.CommandName == k_RegisterCommand &&
                    commandData.Value.ProcedureName == k_RegisterCommand)
                {
                    CommandInfo? commandInfo = CommandUtils.InferCommandInfo(commandData, CommandMethods);
                    RegisterCommand(commandInfo.Value.procedure.Method.Name, commandInfo.Value.min_arg_count, commandInfo.Value.max_arg_count, commandInfo.Value.help);
                }
                else
                {
                    Terminal.Shell.RunCommand(line);
                }
            }
        }
    }
    public void RegisterCommand(string procedureName, int minArgs, int maxArgs, string helpText)
    {
        Debug.Log(procedureName);
        if (CommandMethods.TryGetValue(procedureName, out MethodInfo methodInfo))
        {
            CommandInfo commandInfo = new CommandInfo
            {
                procedure = (CommandArg[] args) => methodInfo.Invoke(null, new object[] { args }),
                min_arg_count = minArgs,
                max_arg_count = maxArgs,
                help = helpText
            };

            Terminal.Shell.AddCommand(procedureName.ToUpper(), commandInfo);
            Terminal.Log($"Registered Command: {procedureName.ToUpper()}, MinArgs: {minArgs}, MaxArgs: {maxArgs}, Help: {helpText}");
        }
        else
        {
            Terminal.LogError($"Command method not found: {procedureName}");
        }
    }

    public void UnregisterCommand(string procedureName)
    {
        string commandKey = procedureName.ToUpper();

        if (CommandMethods.TryGetValue(commandKey, out MethodInfo methodInfo))
        {
            Terminal.Shell.Unregister(commandKey);
            CommandMethods.Remove(commandKey);

            Terminal.Log($"Unregistered Command: {commandKey}");
        }
        else
        {
            Terminal.LogError($"Command method not found: {procedureName}");
        }
    }
}
