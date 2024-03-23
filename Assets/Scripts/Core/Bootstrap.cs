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
    private Terminal _terminal;

    private void Start()
    {
        InitializeTerminal();
        LoadBootstrapFile();
    }

    private void InitializeTerminal()
    {
        _terminal = FindFirstObjectByType<Terminal>();
        if (_terminal == null)
        {
            Debug.LogError("Unable to find the terminal");
            Utility.Quit();
        }
        _terminal.ToggleCommandInput(false);
    }

    private void LoadBootstrapFile()
    {
        _terminal.Log("[BOOT] Attempting to load bootstrap file...");
        BootFile = Resources.Load<TextAsset>("bootstrap");

        if (BootFile == null)
        {
            _terminal.LogError("[BOOT] ERROR: Bootstrap file missing, unable to boot.");
            StartCoroutine(WaitAndQuit());
        }
        else
        {
            _terminal.Log("[BOOT] Bootstrap file loaded successfully.");
            StartCoroutine(BootSequence());
        }
    }

    IEnumerator WaitAndQuit()
    {
        _terminal.Log("[BOOT] Waiting 3 seconds before quitting...");
        yield return new WaitForSeconds(3);
        _terminal.Log("[BOOT] Quitting application");
        Utility.Quit();
    }

    IEnumerator BootSequence()
    {
        yield return new WaitForSeconds(1);
        PrintWelcomeMessage();
        yield return new WaitForSeconds(1);
        PrintBootMessage(BootFile);
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
            _terminal.LogError("[BOOT] Welcome file not found in Assets/Resources");
        }
    }

    private void PrintBootMessage(TextAsset bootFileAsset)
    {
        PrintLinesFromAsset(bootFileAsset, "");
        _terminal.Log("[BOOT] Sequence COMPLETE!");
    }

    private void PrintLinesFromAsset(TextAsset asset, string prefix)
    {
        string[] lines = asset.text.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
        _terminal.Buffer.Clear();
        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("#") && !string.IsNullOrWhiteSpace(line))
            {
                _terminal.Log(prefix + line);
            }
        }
    }

    private void FinalizeBoot()
    {
        if (_clearPostBoot) _terminal.Buffer.Clear();
        _terminal.ToggleCommandInput(true);
    }

    private void PostBoot()
    {
        ScriptInterpreter scriptInterpreter = ScriptInterpreter.CreateInstance(_terminal, k_Interpreter);
        scriptInterpreter.Bind(_terminal);
        if (scriptInterpreter != null)
        {
            scriptInterpreter.InitializeAndExecuteScript(k_PostBoot);
            _terminal.Log("[BOOT] ScriptInterpreter: Verification successful");
        }
        else
        {
            _terminal.LogError("[BOOT] ScriptInterpreter: Verification failed");
        }
    }
}
