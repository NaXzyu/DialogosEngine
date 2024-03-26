namespace CommandTerminal
{
    public enum CommandState
    {
        Close,
        OpenSmall,
        OpenFull
    }

    public class CommandStates
    {
        public string CachedCommandText;
        public string CommandText;

        public void HandleStateChange(Terminal terminal, CommandState new_state)
        {
            switch (new_state)
            {
                case CommandState.Close:
                    terminal.GUI.CloseTerminal(terminal);
                    break;
                case CommandState.OpenSmall:
                    terminal.GUI.OpenTerminalSmall(terminal);
                    break;
                case CommandState.OpenFull:
                default:
                    terminal.GUI.OpenTerminalFull(terminal);
                    break;
            }
        }

        public void SetState(Terminal terminal, CommandState new_state)
        {
            PrepareForStateChange(terminal);
            HandleStateChange(terminal, new_state);
            terminal.State = new_state;
        }

        private void PrepareForStateChange(Terminal terminal)
        {
            terminal.InputFix = true;
            CachedCommandText = CommandText;
            CommandText = "";
        }

        public void ToggleState(Terminal terminal, CommandState new_state)
        {
            if (terminal.State == new_state)
            {
                terminal.States.SetState(terminal, CommandState.Close);
            }
            else
            {
                terminal.States.SetState(terminal, new_state);
            }
        }
    }
}