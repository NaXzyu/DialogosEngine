using UnityEngine;
using System.Diagnostics;
using System.Threading;
using CommandTerminal;

namespace DialogosEngine
{
    public class Processor
    {
        public static void ExecuteBatchFile(string batchFile)
        {
            Thread _batchThread = new Thread(() =>
            {
                Process _process = new Process();
                string _filePath = Application.dataPath + batchFile;
                _process.StartInfo.FileName = _filePath;
                _process.StartInfo.UseShellExecute = false;
                _process.StartInfo.RedirectStandardOutput = true;
                _process.StartInfo.RedirectStandardError = true;
                _process.Start();
                while (!_process.StandardOutput.EndOfStream)
                {
                    string _line = _process.StandardOutput.ReadLine();
                    Terminal.Instance.Log("[PROC] " + _line);
                }
                if (!_process.StandardError.EndOfStream)
                {
                    string _error = _process.StandardError.ReadToEnd();
                    Terminal.Instance.LogError("[PROC] " + _error);
                }
                _process.WaitForExit();
            });
            _batchThread.Start();
        }
    }
}