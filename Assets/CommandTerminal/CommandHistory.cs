using System.Collections.Generic;

namespace CommandTerminal
{
    public class CommandHistory
    {
        List<string> _History = new List<string>();
        int _Position;

        public void Push(string commandString)
        {
            if (commandString == "")
            {
                return;
            }

            _History.Add(commandString);
            _Position = _History.Count;
        }

        public string Next()
        {
            _Position++;

            if (_Position >= _History.Count)
            {
                _Position = _History.Count;
                return "";
            }

            return _History[_Position];
        }

        public string Previous()
        {
            if (_History.Count == 0)
            {
                return "";
            }

            _Position--;

            if (_Position < 0)
            {
                _Position = 0;
            }

            return _History[_Position];
        }

        public void Clear()
        {
            _History.Clear();
            _Position = 0;
        }
    }
}
