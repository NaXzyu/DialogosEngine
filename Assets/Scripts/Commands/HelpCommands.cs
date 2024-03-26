using CommandTerminal;
using System.Linq;
using UnityEngine;

namespace CommandTerminal
{
    public static class HelpCommands
    {
        [Command("help")]
        public static void HelpCommand(CommandArg[] args)
        {
            var terminal = Terminal.Instance;
            if (args.Length > 0)
            {
                var name = args[0].String.ToUpper();
                if (terminal.Shell.Commands.TryGetValue(name, out CommandData commandData))
                {
                    terminal.Log($"\nHelp for {name}:\n\n    {commandData.HelpText}\n");
                }
                else
                {
                    terminal.LogError($"No help found for command: {name}");
                }
            }
            else
            {
                string helpText = "\nAvailable commands:\n\n";
                foreach (var command in terminal.Shell.Commands)
                {
                    helpText += $"    {command.Key.ToLower()} - {command.Value.HelpText}\n";
                }
                terminal.Log(helpText);
            }
        }
    }
}
