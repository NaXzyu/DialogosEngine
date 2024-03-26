using UnityEngine;

namespace CommandTerminal
{
    public class CommandEvents
    {
        public const string k_KeyEventReturn = "return";
        public const string k_KeyEventUp = "up";
        public const string k_KeyEventDown = "down";
        public const string k_KeyEventTab = "tab";

        public void HandleKeyboardEvents(Terminal terminal)
        {
            if (terminal.MoveCursor)
            {
                terminal.GUI.CursorToEnd(terminal);
                terminal.MoveCursor = false;
            }
            if (Event.current.Equals(Event.KeyboardEvent(k_KeyEventReturn)))
            {
                terminal.EnterCommand();
            }
            else if (Event.current.Equals(Event.KeyboardEvent(k_KeyEventUp)))
            {
                terminal.States.CommandText = terminal.History.Previous();
                terminal.MoveCursor = true;
            }
            else if (Event.current.Equals(Event.KeyboardEvent(k_KeyEventDown)))
            {
                terminal.States.CommandText = terminal.History.Next();
            }
            else if (Event.current.Equals(Event.KeyboardEvent(k_KeyEventTab)))
            {
                terminal.CompleteCommand();
                terminal.MoveCursor = true; // Wait for draw
            }
        }
    }
}