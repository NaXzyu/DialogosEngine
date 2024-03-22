using UnityEngine;
using System.IO;
using CommandTerminal;
using System.Collections;
using System;
using System.Reflection;

public class BootstrapManager : MonoBehaviour
{
    [SerializeField] string BootFile;
    public const string k_BootProgram = "boot";
    public const string k_Bootstrap = "bootstrap.unityboot";
    public const string k_WelcomeMessage = "welcome.message";
    public const string k_ScriptInterpreter = "ScriptInterpreter";
    public static readonly string[] LineSeparators = new[] { "\r\n", "\r", "\n" };

    private void Start()
    {
        TextAsset _bootFile = Resources.Load<TextAsset>(k_Bootstrap);
        if (_bootFile == null)
        {
            Terminal.Log("[BOOT] Bootstrap missing, creating a new one...");
            CreateBootstrap();
            Reboot();
        }
        else
        {
            Terminal.Log("[BOOT] Bootstrap OKAY!");
            StartCoroutine(BootSequence(_bootFile));
        }
    }

    public void Reboot()
    {
        Terminal.Log("[BOOT] Rebooting...");
        StartCoroutine(WaitAndQuit());
    }

    IEnumerator WaitAndQuit()
    {
        yield return new WaitForSeconds(1);
        BootstrapUtil.Quit();
    }

    IEnumerator BootSequence(TextAsset bootFile)
    {
        yield return new WaitForSeconds(1);
        PrintWelcomeMessage();
        yield return new WaitForSeconds(1);
        PrintBootMessage(bootFile);
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
        "StartAsyncJobs",
        "ExecuteStaticFunctionCalls"
    };

        File.WriteAllLines(BootFile, _defaultCommands);
        Terminal.Log($"[BOOT] Created new bootstrap file at: {BootFile}");

        foreach (var _commandName in _defaultCommands)
        {
            // Use reflection here to add commands to the terminal
            MethodInfo methodInfo = typeof(Kernel).GetMethod(_commandName);
            if (methodInfo != null)
            {
                Action commandDelegate = (Action)Delegate.CreateDelegate(typeof(Action), Kernel.Instance, methodInfo);
                Kernel.Instance.AddCommand(_commandName, commandDelegate);
            }
            else
            {
                Terminal.LogError($"[BOOT] Command method {_commandName} not found in Kernel.");
            }
        }
    }

    private void ExecuteCommandWithReflection(string commandName)
    {
        // Use reflection to invoke the method with the same name as the command
        MethodInfo methodToExecute = this.GetType().GetMethod(commandName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (methodToExecute != null)
        {
            methodToExecute.Invoke(this, null);
        }
        else
        {
            Terminal.LogError($"[BOOT] Unknown command: {commandName}");
        }
    }

    private void InitializeSystem()
    {
        Terminal.Log("[BOOT] initialize_system()");
    }

    private void LoadEntityData()
    {
        Terminal.Log("[BOOT] load_entity_data()");
    }

    private void StartAsyncJobs()
    {
        Terminal.Log("[BOOT] start_async_jobs");
    }

    void PrintWelcomeMessage()
    {
        TextAsset welcomeMessageAsset = Resources.Load<TextAsset>(k_WelcomeMessage);
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

        ScriptInterpreter scriptInterpreter = ScriptInterpreter.CreateInstance(k_ScriptInterpreter);
        if (scriptInterpreter != null)
        {
            scriptInterpreter.InitializeAndExecuteScript(k_BootProgram);
            Terminal.Log("[BOOT] ScriptInterpreter: Verification successful");
        }
        else
        {
            Terminal.LogError("[BOOT] ScriptInterpreter: Verification failed");
        }
    }


}
