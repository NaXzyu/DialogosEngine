using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            _streamWriter = new StreamWriter(new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
            _logBuffer = new List<string>();
            _bufferSize = bufferSize;
        }

        public void Log(string message)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("CommandLogger", "Cannot log to a disposed CommandLogger.");
            }
                
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
                FlushBuffer(); // Ensure buffer is flushed before closing
                _streamWriter?.Close(); // Close the stream before disposing
                _streamWriter?.Dispose();
                _disposed = true;
            }
        }
    }
}
