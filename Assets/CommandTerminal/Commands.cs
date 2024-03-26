using UnityEngine;
using CommandTerminal;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace DialogosEngine
{

    public class Commands
    {
        public Dictionary<string, MethodInfo> Methods;
        public const string k_RegisterCommand = "REGISTER";
        public CommandParser Parser { get; private set; }

        public Commands()
        {
            Methods = CommandUtils.CacheCommandMethods();
            Parser = new CommandParser();
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
                        Terminal.Instance.LogError("[CMDS] Critical Bootstrap Error. Quit.");
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
                    string _name = _data.Args[0].String;
                    if (Methods.TryGetValue(_name.ToUpper(), out MethodInfo _method))
                    {
                        int _minArgs = _data.Args[1].Int;
                        int _maxArgs = _data.Args[2].Int;

                        _data.Name = _name;
                        _data.MinArgs = _minArgs;
                        _data.MaxArgs = _maxArgs;
                        _data.Action = (CommandArg[] args) => _method.Invoke(null, new object[] { args });
                        _data.HelpText = _data.Args[3].String;

                        Terminal.Instance.Log("[CMDS] Register: \"" + _name.ToUpper() + "\"");
                        Terminal.Instance.Shell.AddCommand(_name.ToUpper(), _data);
                    }
                    else
                    {
                        Terminal.Instance.LogError($"[CMDS] Command method not found: {_name}");
                    }
                }
                else
                {
                    Terminal.Instance.LogError($"[CMDS] Expected 4 arguments, got {_data.Args.Length}.");
                }
            }
            else
            {
                Terminal.Instance.LogError($"[CMDS] Command data is null.");
            }
        }


        public void Unregister(string commandName)
        {
            string _key = commandName.ToUpper();
            if (Methods.TryGetValue(_key, out MethodInfo _method))
            {
                Terminal.Instance.Log($"[CMDS] Unregister: \"{_key.ToUpper()}\"");
                Terminal.Instance.Shell.Unregister(_key);
                Methods.Remove(_key);
            }
            else
            {
                Terminal.Instance.LogError($"Command method not found: {commandName}");
            }
        }
    }
}