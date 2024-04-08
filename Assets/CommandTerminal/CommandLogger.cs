using System;
using System.Collections.Generic;
using System.IO;

namespace CommandTerminal
{
    public class CommandLogger : IDisposable
    {
        private List<string> _logBuffer;
        private readonly string _logFilePath;
        private readonly int _bufferSize;
        private StreamWriter _streamWriter;
        private bool _disposed = false;

        public CommandLogger(string fileName = "command_log.txt", int bufferSize = 100)
        {
            string _desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _logFilePath = Path.Combine(_desktopPath, fileName);
            _streamWriter = new StreamWriter(_logFilePath, true);
            _logBuffer = new List<string>();
            _bufferSize = bufferSize;
        }

        public void Log(string message)
        {
            lock (_logBuffer)
            {
                long _epochTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
                _logBuffer.Add($"[{_epochTime}] {message}");

                if (_logBuffer.Count >= _bufferSize)
                {
                    FlushBuffer();
                }
            }
        }

        public void FlushBuffer()
        {
            lock (_logBuffer)
            {
                try
                {
                    foreach (var _logEntry in _logBuffer)
                    {
                        _streamWriter.WriteLine(_logEntry);
                    }
                    _streamWriter.Flush();
                    _logBuffer.Clear();
                }
                catch (Exception ex)
                {
                    // Handle the exception, e.g., log it to a separate file or rethrow
                    Console.WriteLine($"An error occurred while flushing the buffer: {ex.Message}");
                }
            }
        }

        public void Close()
        {
            FlushBuffer();
            Dispose();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _streamWriter?.Dispose();
                _disposed = true;
            }
        }
    }
}
