using System;

namespace CommandTerminal
{
    public struct CommandData
    {
        public string Name;
        public CommandArg[] Args;
        public Action<CommandArg[]> Action;
        public int MinArgs;
        public int MaxArgs;
        public string HelpText;
    }
}
