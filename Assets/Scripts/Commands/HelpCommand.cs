using CommandTerminal;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CommandTerminal
{
    public static class HelpCommand
    {
        [Command("help")]
        public static void HelpProcedure(CommandArg[] args)
        {
            var terminal = Terminal.Instance;
            if (args.Length > 0)
            {
                var name = args[0].String;
                // TODO: Implement help display for specific command
                terminal.Log($"TODO display help for {0}", name);
            }
            else
            {
                // TODO: Implement system-wide help display
                terminal.Log("TODO display general help");
            }
        }
    }
}
