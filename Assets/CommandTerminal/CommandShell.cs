using System;
using System.Reflection;
using System.Collections.Generic;

namespace CommandTerminal
{
    public struct CommandInfo
    {
        public Action<CommandArg[]> proc;
        public int max_arg_count;
        public int min_arg_count;
        public string help;
    }

    public struct CommandArg
    {
        public string String { get; set; }

        public int Int
        {
            get
            {
                int int_value;

                if (int.TryParse(String, out int_value))
                {
                    return int_value;
                }

                return 0;
            }
        }

        public float Float
        {
            get
            {
                float float_value;

                if (float.TryParse(String, out float_value))
                {
                    return float_value;
                }

                return 0;
            }
        }

        public bool Bool
        {
            get
            {
                if (string.Compare(String, "TRUE", ignoreCase: true) == 0)
                {
                    return true;
                }

                if (string.Compare(String, "FALSE", ignoreCase: true) == 0)
                {
                    return false;
                }

                return false;
            }
        }

        public override string ToString()
        {
            return String;
        }
    }

    public class CommandShell
    {
        Dictionary<string, CommandInfo> commands = new Dictionary<string, CommandInfo>();
        List<CommandArg> arguments = new List<CommandArg>(); // Cache for performance

        public string IssuedErrorMessage { get; private set; }

        public Dictionary<string, CommandInfo> Commands
        {
            get { return commands; }
        }

        public void RunCommand(string line)
        {
            string remaining = line;
            IssuedErrorMessage = null;
            arguments.Clear();

            while (remaining != "")
            {
                var argument = EatArgument(ref remaining);

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

            RunCommand(command_name, arguments.ToArray());
        }

        public void RunCommand(string command_name, CommandArg[] arguments)
        {
            var command = commands[command_name];
            int arg_count = arguments.Length;
            string error_message = null;
            int required_arg = 0;

            if (arg_count < command.min_arg_count)
            {
                if (command.min_arg_count == command.max_arg_count)
                {
                    error_message = "exactly";
                }
                else
                {
                    error_message = "at least";
                }
                required_arg = command.min_arg_count;
            }
            else if (command.max_arg_count > -1 && arg_count > command.max_arg_count)
            {
                if (command.min_arg_count == command.max_arg_count)
                {
                    error_message = "exactly";
                }
                else
                {
                    error_message = "at most";
                }
                required_arg = command.max_arg_count;
            }

            if (error_message != null)
            {
                string plural_fix = required_arg == 1 ? "" : "s";
                IssueErrorMessage(
                    "{0} requires {1} {2} argument{3}",
                    command_name,
                    error_message,
                    required_arg,
                    plural_fix
                );
                return;
            }

            command.proc(arguments);
        }

        public void AddCommand(string name, CommandInfo info)
        {
            name = name.ToUpper();

            if (commands.ContainsKey(name))
            {
                IssueErrorMessage("Command {0} is already defined.", name);
                return;
            }

            commands.Add(name, info);
        }

        public void AddCommand(string name,
                               Action<CommandArg[]> proc,
                               int min_arg_count = 0,
                               int max_arg_count = -1,
                               string help = "")
        {
            var info = new CommandInfo()
            {
                proc = proc,
                min_arg_count = min_arg_count,
                max_arg_count = max_arg_count,
                help = help
            };

            AddCommand(name, info);
        }

        public void IssueErrorMessage(string format, params object[] message)
        {
            IssuedErrorMessage = string.Format(format, message);
        }

        CommandArg EatArgument(ref string s)
        {
            var arg = new CommandArg();
            int space_index = s.IndexOf(' ');

            if (space_index >= 0)
            {
                arg.String = s.Substring(0, space_index);
                s = s.Substring(space_index + 1); // Remaining
            }
            else
            {
                arg.String = s;
                s = "";
            }

            return arg;
        }
    }
}
