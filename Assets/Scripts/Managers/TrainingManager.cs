using System.Diagnostics;
using System.IO;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    public void StartTraining()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "bin\\train.bat",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = new Process { StartInfo = startInfo };
        process.Start();

        // Asynchronously read the standard output of the spawned process.
        StreamReader reader = process.StandardOutput;
        string output = reader.ReadToEnd();

        // Write the redirected output to the console.
        UnityEngine.Debug.Log(output);

        process.WaitForExit();
    }
}
