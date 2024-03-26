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
            var name = args[0].String;
            var minArgs = args[1].Int;
            var maxArgs = args[2].Int;
            var helpText = args[3].String;

            if (Terminal.Instance != null)
            {
                if (Terminal.Instance.TerminalCommands.CommandMethods.ContainsKey(name))
                {
                    Terminal.Instance.TerminalCommands.RegisterCommand(name, minArgs, maxArgs, helpText);
                    return;
                }
            }
            Terminal.Instance.LogError("The Terminal is null.");
            Utility.Quit();
        }



        [Command("unregister")]
        public static void UnregisterProcedure(CommandArg[] args)
        {
            if (Terminal.Instance != null)
            {
                Terminal.Instance.TerminalCommands.UnregisterCommand(args[0].String);
            }
            else
            {
                Terminal.Instance.LogError("Unable to local the terminal in TerminalCommand.");
            }
        }

    }
}
