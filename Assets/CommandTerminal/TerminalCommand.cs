using UnityEngine;
using CommandTerminal;
using System;
using System.Reflection;
using System.Collections.Generic;

public class TerminalCommand
{
    public Dictionary<string, MethodInfo> CommandMethods;
    public const string k_RegisterCommand = "Register";
    public const string k_RegisterProcedure = "RegisterProcedure";

    public TerminalCommand()
    {
        CommandMethods = TerminalUtils.CacheCommandMethods();
    }

    public void Initialize(TextAsset bootstrapFile)
    {
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
                    Terminal.Instance.LogError("Critical Bootstrap Error. Quit.");
                    Utility.Quit();
                }

                if (commandData.Value.CommandName == k_RegisterCommand &&
                    commandData.Value.ProcedureName == k_RegisterProcedure)
                {
                    CommandInfo? commandInfo = CommandUtils.InferCommandInfo(commandData, CommandMethods);
                    RegisterCommand(commandInfo.Value.command.Method.Name, commandInfo.Value.min_arg_count, commandInfo.Value.max_arg_count, commandInfo.Value.help);
                }
                else
                {
                    Terminal.Instance.Shell.RunCommand(line);
                }
            }
        }
    }
    public void RegisterCommand(string commandName, int minArgs, int maxArgs, string helpText)
    {
        if (CommandMethods.TryGetValue(commandName.ToUpper(), out MethodInfo methodInfo))
        {
            CommandInfo commandInfo = new CommandInfo
            {
                command = (CommandArg[] args) => methodInfo.Invoke(null, new object[] { args }),
                min_arg_count = minArgs,
                max_arg_count = maxArgs,
                help = helpText
            };

            Terminal.Instance.Shell.AddCommand(commandName.ToUpper(), commandInfo);
            Terminal.Instance.Log($"Registered Command: {commandName.ToUpper()}, MinArgs: {minArgs}, MaxArgs: {maxArgs}, Help: {helpText}");
        }
        else
        {
            Terminal.Instance.LogError($"Command method not found: {commandName}");
        }
    }

    public void UnregisterCommand(string commandName)
    {
        string commandKey = commandName.ToUpper();

        if (CommandMethods.TryGetValue(commandKey, out MethodInfo methodInfo))
        {
            Terminal.Instance.Shell.Unregister(commandKey);
            CommandMethods.Remove(commandKey);

            Terminal.Instance.Log($"Unregistered Command: {commandKey}");
        }
        else
        {
            Terminal.Instance.LogError($"Command method not found: {commandName}");
        }
    }
}
