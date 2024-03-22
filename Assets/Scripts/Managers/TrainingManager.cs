using UnityEngine;
using System.Diagnostics;
using System.IO;
using CommandTerminal;
using System.Collections.Generic;

public class TrainingManager : MonoBehaviour
{
    public static TrainingManager Instance { get; private set; }

    [SerializeField] private string _trainingScriptPath = "bin\\train.bat";
    public delegate void TrainingStatusHandler(string status);
    public event TrainingStatusHandler OnTrainingStatusChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartTraining()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Application.streamingAssetsPath, _trainingScriptPath),
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            Process process = new Process { StartInfo = startInfo };
            process.Start();
            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    Terminal.Log($"[TRAIN] {args.Data}");
                }
            };
            process.BeginOutputReadLine();
            process.WaitForExit();

            OnTrainingStatusChanged?.Invoke("Training completed successfully.");
        }
        catch (System.Exception e)
        {
            Terminal.LogError($"[TRAIN] Failed to start training process: {e.Message}");
            OnTrainingStatusChanged?.Invoke("Training failed to start.");
        }
    }

    public void SaveTrainingData(Dictionary<string, string> dialogueTurns)
    {
        string trainingDataPath = Path.Combine(Application.streamingAssetsPath, "training_data.txt");
        using (StreamWriter writer = new StreamWriter(trainingDataPath, true))
        {
            foreach (var turn in dialogueTurns)
            {
                writer.WriteLine($"{turn.Key}\t{turn.Value}");
            }
        }
    }

    public bool IsTrainingDataAvailable()
    {
        string trainingDataPath = Path.Combine(Application.streamingAssetsPath, "training_data.txt");
        FileInfo fileInfo = new FileInfo(trainingDataPath);
        return fileInfo.Exists && fileInfo.Length > 1024;
    }

    public void PrepareTrainingData()
    {
        // TODO Logic to prepare training data before starting the training process

        // TODO Logic for formatting dialogue turns, balancing datasets, etc.

        OnTrainingStatusChanged?.Invoke("Preparing training data...");
    }

    public void MonitorTraining()
    {
        // TODO Logic to monitor the training process

        // TODO logic for checking for completion, tracking metrics, etc.

        OnTrainingStatusChanged?.Invoke("Monitoring training process...");
    }

    public void PostTraining()
    {
        // TODO Logic to handle tasks after training is complete

        // TODO logic for evaluating the model, saving the model, etc.

        OnTrainingStatusChanged?.Invoke("Post-training tasks underway...");
    }
}
