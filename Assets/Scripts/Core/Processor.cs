using UnityEngine;
using System.Diagnostics;
using System.Threading;

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
                    Process _process = new Process();
                    string _path = Application.dataPath + batchFile;
                    _process.StartInfo.FileName = _path;
                    _process.StartInfo.UseShellExecute = false;
                    _process.StartInfo.RedirectStandardOutput = true;
                    _process.StartInfo.RedirectStandardError = true;
                    _process.Start();

                    while (!_process.StandardOutput.EndOfStream)
                    {
                        string _line = _process.StandardOutput.ReadLine();
                        UnityEngine.Debug.Log(_line);
                    }
                    if (!_process.StandardError.EndOfStream)
                    {
                        string _error = _process.StandardError.ReadToEnd();
                        UnityEngine.Debug.LogError(_error);
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
