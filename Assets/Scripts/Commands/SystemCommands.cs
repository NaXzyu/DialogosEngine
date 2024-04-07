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

                    Terminal.Instance.Commands.Register(_data);
                }
                else
                {
                    Terminal.Instance.LogError("[SYST] The Terminal is null.");
                    Utility.Quit(3);
                }
            }
            else
            {
                Terminal.Instance.LogError("[SYST] Insufficient arguments provided for 'register' command.");
            }
        }


        [Command("unregister")]
        public static void UnregisterCommand(CommandArg[] args)
        {
            if (Terminal.Instance != null)
            {
                Terminal.Instance.Commands.Unregister(args[0].String);
            }
            else
            {
                Terminal.Instance.LogError("[SYST] Unable to locate the terminal in TerminalCommand.");
            }
        }

        [Command("clear")]
        public static void ClearCommand(CommandArg[] args)
        {
            if (args.Length == 0)
            {
                Terminal.Instance.Buffer.ArchiveLogs();
            }
            else if (args.Length == 1 && args[0].String == "--reset")
            {
                Terminal.Instance.Buffer.Reset();
            }
            else
            {
                Terminal.Instance.LogError("[SYST] Insufficient arguments provided for 'clear' command.");
            }
        }

        [Command("print")]
        public static void PrintCommand(CommandArg[] args)
        {
            if (args.Length == 1)
            {
                Terminal.Instance.Log(args[0].String);
            }
            else
            {
                Terminal.Instance.LogError("[SYST] Insufficient arguments provided for 'print' command.");
            }
        }

        [Command("echo")]
        public static void EchoCommand(CommandArg[] args)
        {
            if (args.Length == 1)
            {
                PrintCommand(new CommandArg[] { args[0] });
            }
            else
            {
                Terminal.Instance.LogError("[SYST] Insufficient arguments provided for 'echo' command.");
            }
        }
    }
}
