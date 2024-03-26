using System.Collections.Generic;
using UnityEngine;

namespace CommandTerminal
{
    public enum LogType
    {
        Error = UnityEngine.LogType.Error,
        Assert = UnityEngine.LogType.Assert,
        Warning = UnityEngine.LogType.Warning,
        Message = UnityEngine.LogType.Log,
        Exception = UnityEngine.LogType.Exception,
        Input,
        ShellMessage
    }

    public struct LogItem
    {
        public LogType Type;
        public string Message;
        public string StackTrace;
    }

    public class CommandLog
    {
        List<LogItem> _Logs = new();
        int _MaxItems;

        public List<LogItem> Logs
        {
            get { return _Logs; }
        }

        public CommandLog(int maxItems)
        {
            _MaxItems = maxItems;
        }

        public void Handle(string message, LogType type)
        {
            Handle(message, "", type);
        }

        public void Handle(string message, string stackTrace, LogType type)
        {
            LogItem _log = new()
            {
                Message = message,
                StackTrace = stackTrace,
                Type = type
            };

            _Logs.Add(_log);

            if (_Logs.Count > _MaxItems)
            {
                _Logs.RemoveAt(0);
            }
        }

        public void Clear()
        {
            _Logs.Clear();
        }
    }
}
