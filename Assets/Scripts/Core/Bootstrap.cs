using UnityEngine;
using CommandTerminal;
using System.Collections;
using System;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] TextAsset BootFile;
    [SerializeField] bool _clearPostBoot = false;
    public const string k_PostBoot = "post.dil";
    public const string k_Bootstrap = "bootstrap";
    public const string k_Welcome = "welcome";
    public const string k_Interpreter = "ScriptInterpreter";
    public static readonly string[] LineSeparators = new[] { "\r\n", "\r", "\n" };

    private void Start()
    {
        InitializeTerminal();
        LoadBootstrapFile();
    }

    private void InitializeTerminal()
    {
        Terminal.Instance = FindFirstObjectByType<Terminal>();
        if (Terminal.Instance == null)
        {
            Debug.LogError("Unable to find the terminal");
            Utility.Quit();
        }
        Terminal.Instance.ToggleCommandInput(false);
    }

    private void LoadBootstrapFile()
    {
        Terminal.Instance.Log("[BOOT] Attempting to load bootstrap file...");
        BootFile = Resources.Load<TextAsset>(k_Bootstrap);

        if (BootFile == null)
        {
            Terminal.Instance.LogError("[BOOT] ERROR: Bootstrap file missing, unable to boot.");
            StartCoroutine(WaitAndQuit());
        }
        else
        {
            Terminal.Instance.Log("[BOOT] Bootstrap file loaded successfully.");
            StartCoroutine(BootSequence());
        }
    }

    IEnumerator WaitAndQuit()
    {
        Terminal.Instance.Log("[BOOT] Waiting 3 seconds before quitting...");
        yield return new WaitForSeconds(3);
        Terminal.Instance.Log("[BOOT] Quitting application");
        Utility.Quit();
    }

    IEnumerator BootSequence()
    {
        yield return new WaitForSeconds(1);
        PrintWelcomeMessage();
        yield return new WaitForSeconds(1);
        RunBoot(BootFile);
        yield return new WaitForSeconds(1);
        PostBoot();
        yield return new WaitForSeconds(1);
        FinalizeBoot();
    }

    private void PrintWelcomeMessage()
    {
        TextAsset welcomeMessageAsset = Resources.Load<TextAsset>(k_Welcome);
        if (welcomeMessageAsset != null)
        {
            PrintLinesFromAsset(welcomeMessageAsset, "[BOOT] ");
        }
        else
        {
            Terminal.Instance.LogError("[BOOT] Welcome file not found in Assets/Resources");
        }
    }

    private void RunBoot(TextAsset bootstrapFile)
    {
        Terminal.Instance.TerminalCommands.Initialize(bootstrapFile);
        Terminal.Instance.Log("[BOOT] Sequence COMPLETE!");
    }

    private void PrintLinesFromAsset(TextAsset asset, string prefix)
    {
        string[] lines = asset.text.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
        Terminal.Instance.Buffer.Clear();
        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("#") && !string.IsNullOrWhiteSpace(line))
            {
                Terminal.Instance.Log(prefix + line);
            }
        }
    }

    private void FinalizeBoot()
    {
        if (_clearPostBoot) Terminal.Instance.Buffer.Clear();
        Terminal.Instance.ToggleCommandInput(true);
    }

    private void PostBoot()
    {
        ScriptInterpreter scriptInterpreter = ScriptInterpreter.CreateInstance(k_Interpreter);
        if (scriptInterpreter != null)
        {
            scriptInterpreter.InitializeAndExecuteScript(k_PostBoot);
            Terminal.Instance.Log("[BOOT] ScriptInterpreter: Verification successful");
        }
        else
        {
            Terminal.Instance.LogError("[BOOT] ScriptInterpreter: Verification failed");
        }
    }
}
