using System.Collections.Generic;

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

    public class CommandBuffer
    {
        List<LogItem> _Logs = new();
        List<LogItem> _Archive = new();
        int _MaxItems;
        int _ArchiveOffset = 0;

        public List<LogItem> Logs
        {
            get { return _Logs; }
        }

        public List<LogItem> Archive
        {
            get { return _Archive; }
        }

        public CommandBuffer(int maxItems)
        {
            _MaxItems = maxItems;
        }

        public void Append(string message, LogType type)
        {
            Append(message, "", type);
        }

        public void Append(string message, string stackTrace, LogType type)
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

        public void ArchiveLogs()
        {
            _Archive.AddRange(_Logs);
            _Logs.Clear();
        }

        public string[] ToArray(ref int startOffset, ref int maxChars)
        {
            List<string> _lines = new();
            int _count = 0;

            ProcessLogs(ref _Logs, ref startOffset, ref maxChars, ref _lines, ref _count);
            if (_count < maxChars)
            {
                ProcessLogs(ref _Archive, ref _ArchiveOffset, ref maxChars, ref _lines, ref _count);
            }
            return _lines.ToArray();
        }

        private void ProcessLogs(ref List<LogItem> logs, ref int startOffset, ref int maxChars, ref List<string> lines, ref int count)
        {
            int _index = logs.Count - 1 - startOffset;
            while (_index >= 0 && count < maxChars)
            {
                ProcessLogLine(logs[_index], maxChars, ref lines, ref count, ref _index);
            }
        }

        private void ProcessLogLine(LogItem item, int maxChars, ref List<string> lines, ref int count, ref int index)
        {
            index--;
            string _line = $"{(int)item.Type}{item.Message}";
            int _length = _line.Length;
            if (count + _length <= maxChars)
            {
                lines.Add(_line);
                count += _length;
            }
            else
            {
                int _remaining = maxChars - count;
                lines.Add(_line.Substring(0, _remaining));
            }
        }

        public void Reset()
        {
            _Logs.Clear();
            _Archive.Clear();
            _MaxItems = 0;
            _ArchiveOffset = 0;
        }
    }
}
