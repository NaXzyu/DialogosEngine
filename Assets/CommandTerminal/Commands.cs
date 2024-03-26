using UnityEngine;
using CommandTerminal;
using System;
using System.Reflection;
using System.Collections.Generic;
using DialogosEngine;

public class Commands
{
    public Dictionary<string, MethodInfo> Methods;
    public const string k_RegisterCommand = "REGISTER";
    public CommandParser CommandParser { get; private set; }

    public Commands()
    {
        Methods = TerminalUtils.CacheCommandMethods();
        CommandParser = new CommandParser();
    }

    public void Initialize(TextAsset bootfile)
    {
        string[] _lines = bootfile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string _line in _lines)
        {
            if (!_line.Trim().StartsWith("#") && !string.IsNullOrWhiteSpace(_line))
            {
                string _commandName = CommandParser.GetCommandNameFromLine(_line);
                string _commandLine = CommandParser.ParsedCommandLine(_line);
                CommandData? _commandData = CommandParser.ParseCommandData(_commandLine);
                string _command = CommandParser.ConvertCommandDataToCommandLine(_commandData.Value);

                if (_commandData == null)
                {
                    Terminal.Instance.LogError("Critical Bootstrap Error. Quit.");
                    Utility.Quit(3);
                }

                if (_commandName.ToUpper() == k_RegisterCommand)
                {
                    Register(_commandData);
                }
                else
                {
                    Terminal.Instance.Shell.Run(_command);
                }
            }
        }
    }

    public void Register(CommandData? commandData)
    {
        if (commandData.HasValue)
        {
            CommandData _data = commandData.Value;

            if (_data.Args.Length == 4)
            {
                string _commandName = _data.Args[0].String;
                if (Methods.TryGetValue(_commandName.ToUpper(), out MethodInfo _methodInfo))
                {
                    int _minArgs = _data.Args[1].Int;
                    int _maxArgs = _data.Args[2].Int;

                    _data.Name = _commandName;
                    _data.MinArgs = _minArgs;
                    _data.MaxArgs = _maxArgs;
                    _data.Action = (CommandArg[] args) => _methodInfo.Invoke(null, new object[] { args });
                    _data.HelpText = _data.Args[3].String;

                    Terminal.Instance.Shell.AddCommand(_commandName.ToUpper(), _data);
                    Terminal.Instance.Log("Registered Command: \"" + _commandName.ToUpper() + "\"");
                }
                else
                {
                    Terminal.Instance.LogError($"Command method not found: {_commandName}");
                }
            }
            else
            {
                Terminal.Instance.LogError($"Command data arguments expected 4, got {_data.Args.Length}.");
            }
        }
        else
        {
            Terminal.Instance.LogError($"Command data is null.");
        }
    }


    public void Unregister(string commandName)
    {
        string commandKey = commandName.ToUpper();

        if (Methods.TryGetValue(commandKey, out MethodInfo methodInfo))
        {
            Terminal.Instance.Shell.Unregister(commandKey);
            Methods.Remove(commandKey);

            Terminal.Instance.Log($"Unregistered Command: {commandKey}");
        }
        else
        {
            Terminal.Instance.LogError($"Command method not found: {commandName}");
        }
    }
}
