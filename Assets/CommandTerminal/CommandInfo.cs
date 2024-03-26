using System;

namespace CommandTerminal
{
    public struct CommandInfo
    {
        public Action<CommandArg[]> procedure;
        public int max_arg_count;
        public int min_arg_count;
        public string help;
    }
}