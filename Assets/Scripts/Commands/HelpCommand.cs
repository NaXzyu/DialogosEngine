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
            var terminal = TerminalCommand.Terminal;
            var name = args[0].String;
            
            // TODO Display the help system wide for Dialogos -- maybe iterate through all register commands in the terminal 
        }
    }
}
