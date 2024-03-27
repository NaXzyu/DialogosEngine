using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace DialogosEngine
{
    public class Processor
    {
        public static int k_MaxProcessors = 16;
        private static SemaphoreSlim _Semaphore = new SemaphoreSlim(k_MaxProcessors, k_MaxProcessors);

        public static void ExecuteBatchFile(string batchFile)
        {
            Thread _thread = new Thread(() =>
            {
                _Semaphore.Wait();
                try
                {
                    Process _process = new();
                    string _path = Application.streamingAssetsPath + batchFile;
                    _process.StartInfo.FileName = _path;
                    _process.StartInfo.UseShellExecute = false;
                    _process.StartInfo.RedirectStandardOutput = true;
                    _process.StartInfo.RedirectStandardError = true;
                    _process.Start();

                    while (!_process.StandardOutput.EndOfStream)
                    {
                        string _line = _process.StandardOutput.ReadLine();
                        CommandSystem.Log(_line);
                    }
                    if (!_process.StandardError.EndOfStream)
                    {
                        string _error = _process.StandardError.ReadToEnd();
                        CommandSystem.LogError(_error);
                    }
                    _process.WaitForExit();
                }
                finally
                {
                    _Semaphore.Release();
                }
            });
            _thread.Start();
        }
    }
}
