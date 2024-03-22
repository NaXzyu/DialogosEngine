using System.Text;
using System.Diagnostics;
using UnityEngine;

namespace CommandTerminal
{
    public static class BuiltinCommands
    {
        [RegisterCommand(Help = "Clears the Command Console", MaxArgCount = 0)]
        public static void CommandClear(CommandArg[] args)
        {
            Terminal.Buffer.Clear();
        }

        [RegisterCommand(Help = "Lists all Commands or displays Command help", MaxArgCount = 1)]
        public static void CommandHelp(CommandArg[] args)
        {
            if (args.Length == 0)
            {
                foreach (var command in Terminal.Shell.Commands)
                {
                    Terminal.Log("{0}: {1}", command.Key.PadRight(16), command.Value.help);
                }
                return;
            }

            string command_name = args[0].String.ToUpper();

            if (!Terminal.Shell.Commands.ContainsKey(command_name))
            {
                Terminal.Shell.IssueErrorMessage("Command {0} could not be found.", command_name);
                return;
            }

            string help = Terminal.Shell.Commands[command_name].help;

            if (help == null)
            {
                Terminal.Log("{0} does not provide any help documentation.", command_name);
            }
            else
            {
                Terminal.Log(help);
            }
        }

        [RegisterCommand(Help = "Outputs message")]
        public static void CommandPrint(CommandArg[] args)
        {
            Terminal.Log(JoinArguments(args));
        }

        [RegisterCommand(Help = "Quits running Application", MaxArgCount = 0)]
        public static void CommandQuit(CommandArg[] args)
        {
            BootstrapUtil.Quit();
        }

        public static string JoinArguments(CommandArg[] args)
        {
            var sb = new StringBuilder();
            int arg_length = args.Length;

            for (int i = 0; i < arg_length; i++)
            {
                sb.Append(args[i].String);

                if (i < arg_length - 1)
                {
                    sb.Append(" ");
                }
            }

            return sb.ToString();
        }
    }
}
