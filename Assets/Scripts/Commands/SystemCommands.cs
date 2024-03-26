using CommandTerminal;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CommandTerminal
{
    public static class SystemCommands
    {
        [Command("register")]
        public static void RegisterProcedure(CommandArg[] args)
        {
            var terminal = TerminalCommand.Terminal;
            var name = args[0].String;
            var minArgs = args[1].Int;
            var maxArgs = args[2].Int;
            var helpText = args[3].String;

            if (terminal != null)
            {
                if (terminal.TerminalCommands.CommandMethods.ContainsKey(name))
                {
                    terminal.TerminalCommands.RegisterCommand(name, minArgs, maxArgs, helpText);
                }
                else
                {
                    Debug.LogError($"Command {name} is not found in CommandMethods and cannot be registered.");
                }
            }
            else
            {
                Debug.LogError("Unable to locate the terminal in TerminalCommand.");
            }
        }



        [Command("unregister")]
        public static void UnregisterProcedure(CommandArg[] args)
        {
            var terminal = TerminalCommand.Terminal;

            if (terminal != null)
            {
                terminal.TerminalCommands.UnregisterCommand(args[0].String);
            }
            else
            {
                Debug.LogError("Unable to local the terminal in TerminalCommand.");
            }
        }

    }
}
