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
        _terminal = FindFirstObjectByType<Terminal>();
        if (_terminal == null)
        {
            Debug.LogError("Unable to find the terminal");
            Utility.Quit();
        }
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
        yield return new WaitForSeconds(3);
        PrintBootMessage(BootFile);
        yield return new WaitForSeconds(3);
        PostBoot();
        yield return new WaitForSeconds(3);
        if (_clearPostBoot) ClearTerminal();
    }

    void PrintWelcomeMessage()
    {
        TextAsset welcomeMessageAsset = Resources.Load<TextAsset>(k_Welcome);
        if (welcomeMessageAsset != null)
        {
            string[] lines = welcomeMessageAsset.text.Split(LineSeparators, StringSplitOptions.None);
            _terminal.Buffer.Clear();
            foreach (string line in lines)
            {
                _terminal.Log("[BOOT] " + line);
            }
        }
        else
        {
            _terminal.LogError("[BOOT] Welcome file not found in Assets/Resources");
        }
    }

    void PrintBootMessage(TextAsset bootFileAsset)
    {
        string[] lines = bootFileAsset.text.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("#") && !string.IsNullOrWhiteSpace(line))
            {
                _terminal.Log(line);
            }
        }
        _terminal.Log("[BOOT] Sequence COMPLETE!");
    }



    void ClearTerminal()
    {
        _terminal.Buffer.Clear();
    }

    void PostBoot()
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
