using System;
using System.Reflection;
using System.Collections.Generic;

namespace CommandTerminal
{
    public class CommandShell
    {
        Dictionary<string, CommandData> _Commands = new Dictionary<string, CommandData>();
        List<CommandArg> _Args = new List<CommandArg>();

        public string IssuedErrorMessage { get; private set; }

        public Dictionary<string, CommandData> Commands
        {
            get { return _Commands; }
        }

        public void Run(string line)
        {
            string _remaining = line;
            IssuedErrorMessage = null;
            _Args.Clear();

            while (_remaining != "")
            {
                var _arg = CommandUtils.EatArgument(ref _remaining);

                if (_arg.String != "")
                {
                    _Args.Add(_arg);
                }
            }

            if (_Args.Count == 0)
            {
                return;
            }

            string _commandName = _Args[0].String.ToUpper();
            _Args.RemoveAt(0);

            if (!_Commands.ContainsKey(_commandName))
            {
                IssueErrorMessage("[SHLL] Command {0} could not be found", _commandName);
                return;
            }

            Run(_commandName, _Args.ToArray());
        }

        public void Run(string commandName, CommandArg[] arguments)
        {
            if (_Commands.TryGetValue(commandName, out CommandData _command))
            {
                int _argCount = arguments.Length;
                string _errorMsg = null;
                int _requiredArg = 0;

                if (_argCount < _command.MinArgs)
                {
                    _errorMsg = _command.MinArgs == _command.MaxArgs ? "exactly" : "at least";
                    _requiredArg = _command.MinArgs;
                }
                else if (_command.MaxArgs > -1 && _argCount > _command.MaxArgs)
                {
                    _errorMsg = _command.MinArgs == _command.MaxArgs ? "exactly" : "at most";
                    _requiredArg = _command.MaxArgs;
                }

                if (_errorMsg != null)
                {
                    string _pluralFix = _requiredArg == 1 ? "" : "s";
                    IssueErrorMessage(
                        "[SHLL] {0} requires {1} {2} argument{3}",
                        commandName,
                        _errorMsg,
                        _requiredArg,
                        _pluralFix
                    );
                    return;
                }

                _command.Action(arguments);
            }
            else
            {
                Terminal.Instance.LogError($"[SHLL] Command not found: {commandName}");
            }
        }

        public void AddCommand(string name, CommandData data)
        {
            name = name.ToUpper();

            if (_Commands.ContainsKey(name))
            {
                IssueErrorMessage("[SHLL] Command {0} is already defined.", name);
                return;
            }

            _Commands.Add(name, data);
        }

        public void IssueErrorMessage(string format, params object[] message)
        {
            IssuedErrorMessage = string.Format(format, message);
        }

        public void Unregister(string commandKey)
        {
            commandKey = commandKey.ToUpper();

            if (_Commands.ContainsKey(commandKey))
            {
                _Commands.Remove(commandKey);
            }
            else
            {
                IssueErrorMessage("[SHLL] Unregistered Command {0} could not be found.", commandKey);
            }
        }
    }
}
