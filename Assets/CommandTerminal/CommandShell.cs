using System;
using System.Reflection;
using System.Collections.Generic;

namespace CommandTerminal
{
    public class CommandShell
    {
        Dictionary<string, CommandData> commands = new Dictionary<string, CommandData>();
        List<CommandArg> arguments = new List<CommandArg>();

        public string IssuedErrorMessage { get; private set; }

        public Dictionary<string, CommandData> Commands
        {
            get { return commands; }
        }

        public void Run(string line)
        {
            string remaining = line;
            IssuedErrorMessage = null;
            arguments.Clear();

            while (remaining != "")
            {
                var argument = TerminalUtils.EatArgument(ref remaining);

                if (argument.String != "")
                {
                    arguments.Add(argument);
                }
            }

            if (arguments.Count == 0)
            {
                return;
            }

            string command_name = arguments[0].String.ToUpper();
            arguments.RemoveAt(0);

            if (!commands.ContainsKey(command_name))
            {
                IssueErrorMessage("Command {0} could not be found", command_name);
                return;
            }

            Run(command_name, arguments.ToArray());
        }

        public void Run(string commandName, CommandArg[] arguments)
        {
            if (commands.TryGetValue(commandName, out CommandData _command))
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
                        "{0} requires {1} {2} argument{3}",
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
                Terminal.Instance.LogError($"Command not found: {commandName}");
            }
        }

        public void AddCommand(string name, CommandData data)
        {
            name = name.ToUpper();

            if (commands.ContainsKey(name))
            {
                IssueErrorMessage("Command {0} is already defined.", name);
                return;
            }

            commands.Add(name, data);
        }

        public void IssueErrorMessage(string format, params object[] message)
        {
            IssuedErrorMessage = string.Format(format, message);
        }

        public void Unregister(string commandKey)
        {
            commandKey = commandKey.ToUpper();

            if (commands.ContainsKey(commandKey))
            {
                commands.Remove(commandKey);
            }
            else
            {
                IssueErrorMessage("Unregistered Command {0} could not be found.", commandKey);
            }
        }
    }
}
