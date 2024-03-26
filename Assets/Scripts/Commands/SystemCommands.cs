using DialogosEngine;

namespace CommandTerminal
{
    public static class SystemCommands
    {
        [Command("register")]
        public static void RegisterCommand(CommandArg[] args)
        {
            if (args.Length >= 3)
            {
                var _name = args[0].String;
                var _minArgs = args[1].Int;
                var _maxArgs = args[2].Int;
                var _helpText = args[3].String;

                if (Terminal.Instance != null)
                {
                    CommandData _data = new CommandData
                    {
                        Name = _name,
                        MinArgs = _minArgs,
                        MaxArgs = _maxArgs,
                        HelpText = _helpText
                    };

                    Terminal.Instance.TerminalCommands.Register(_data);
                }
                else
                {
                    Terminal.Instance.LogError("The Terminal is null.");
                    Utility.Quit(3);
                }
            }
            else
            {
                Terminal.Instance.LogError("Insufficient arguments provided for 'register' command.");
            }
        }


        [Command("unregister")]
        public static void UnregisterCommand(CommandArg[] args)
        {
            if (Terminal.Instance != null)
            {
                Terminal.Instance.TerminalCommands.Unregister(args[0].String);
            }
            else
            {
                Terminal.Instance.LogError("Unable to locate the terminal in TerminalCommand.");
            }
        }
    }
}
