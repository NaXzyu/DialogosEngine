using UnityEngine;
using System.IO;
using CommandTerminal;
using System.Collections;
using System;
using System.Reflection;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] TextAsset BootFile;
    public const string k_PostBoot = "post.dil";
    public const string k_Bootstrap = "bootstrap.unityboot";
    public const string k_WelcomeMsg = "welcome.message";
    public const string k_Interpreter = "ScriptInterpreter";
    public static readonly string[] LineSeparators = new[] { "\r\n", "\r", "\n" };

    private void Start()
    {
        BootFile = Resources.Load<TextAsset>(k_Bootstrap);
        if (BootFile == null)
        {
            Terminal.Log("[BOOT] Bootstrap missing, creating a new one...");
            CreateBootstrap();
            Reboot();
        }
        else
        {
            Terminal.Log("[BOOT] Bootstrap OKAY!");
            StartCoroutine(BootSequence());
        }
    }

    public void Reboot()
    {
        Terminal.Log("[BOOT] Rebooting with BootManager...");
        BootManager.Instance.RebootGame();
    }

    IEnumerator BootSequence()
    {
        yield return new WaitForSeconds(1);
        PrintWelcomeMessage();
        yield return new WaitForSeconds(1);
        PrintBootMessage(BootFile);
        yield return new WaitForSeconds(3);
        PostBoot();
        yield return new WaitForSeconds(1);
        ClearTerminal();
    }

    private void CreateBootstrap()
    {
        string[] _defaultCommands = {
            "InitializeSystem",
            "LoadEntityData",
            "StartAsyncJobs"
        };
        File.WriteAllLines(Path.Combine(Application.dataPath, "Resources", k_Bootstrap), _defaultCommands);
        string _filePath = Path.Combine("Assets/Resources", k_Bootstrap);
        BootFile = Resources.Load<TextAsset>(k_Bootstrap);
        if (BootFile != null)
        {
            Terminal.Log($"[BOOT] Verification successful, bootstrap file loaded: {_filePath}");
        }
        else
        {
            Terminal.LogError("[BOOT] Verification failed, bootstrap file not loaded.");
        }
        Terminal.Log($"[BOOT] Created new bootstrap file at: {_filePath}");
    }



    void PrintWelcomeMessage()
    {
        TextAsset welcomeMessageAsset = Resources.Load<TextAsset>(k_WelcomeMsg);
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
            Terminal.LogError("[BOOT] Welcome message file not found in Assets/Resources.");
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
        Terminal.Log("[BOOT] POST...");

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
