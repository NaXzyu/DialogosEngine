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
        Terminal.Log("[BOOT] Attempting to load bootstrap file...");
        BootFile = Resources.Load<TextAsset>("bootstrap");

        if (BootFile == null)
        {
            Terminal.LogError("[BOOT] ERROR: Bootstrap file missing, unable to boot.");
            StartCoroutine(WaitAndQuit());
        }
        else
        {
            Terminal.Log("[BOOT] Bootstrap file loaded successfully.");
            StartCoroutine(BootSequence());
        }
    }

    IEnumerator WaitAndQuit()
    {
        Terminal.Log("[BOOT] Waiting 3 seconds before quitting...");
        yield return new WaitForSeconds(3);
        Terminal.Log("[BOOT] Quitting application");
        Utility.Quit();
    }

    IEnumerator BootSequence()
    {
        yield return new WaitForSeconds(3);
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
            Terminal.Buffer.Clear();
            foreach (string line in lines)
            {
                Terminal.Log("[BOOT] " + line);
            }
        }
        else
        {
            Terminal.LogError("[BOOT] Welcome file not found in Assets/Resources");
        }
    }

    void PrintBootMessage(TextAsset bootFileAsset)
    {
        string[] lines = bootFileAsset.text.Split(LineSeparators, StringSplitOptions.None);
        foreach (string line in lines)
        {
            Terminal.Log(line);
        }
        Terminal.Log("[BOOT] Sequence COMPLETE!");
    }

    void ClearTerminal()
    {
        Terminal.Buffer.Clear();
    }

    void PostBoot()
    {
        ScriptInterpreter scriptInterpreter = ScriptInterpreter.CreateInstance(k_Interpreter);
        if (scriptInterpreter != null)
        {
            scriptInterpreter.InitializeAndExecuteScript(k_PostBoot);
            Terminal.Log("[BOOT] ScriptInterpreter: Verification successful");
        }
        else
        {
            Terminal.LogError("[BOOT] ScriptInterpreter: Verification failed");
        }
    }
}
