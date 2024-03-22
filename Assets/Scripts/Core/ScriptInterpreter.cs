using UnityEngine;
using System.IO;
using CommandTerminal;
using System.Collections;
using System.Collections.Generic;

public class ScriptInterpreter : MonoBehaviour
{
    private Queue<string> commandQueue;
    public const string k_ScriptExtension = ".dil";

    public static ScriptInterpreter CreateInstance(string prefabName)
    {
        GameObject scriptInterpreterPrefab = Resources.Load<GameObject>(prefabName);
        if (scriptInterpreterPrefab != null)
        {
            ScriptInterpreter instance = Instantiate(scriptInterpreterPrefab).GetComponent<ScriptInterpreter>();
            if (instance != null)
            {
                Terminal.Log("[SCRIPT] ScriptInterpreter loaded successfully");
                return instance;
            }
            else
            {
                Terminal.LogError("[SCRIPT] ScriptInterpreter component not found on prefab.");
                return null;
            }
        }
        else
        {
            Terminal.LogError("[SCRIPT] Failed to load ScriptInterpreter prefab. Please ensure it is located in the Assets/Resources folder.");
            return null;
        }
    }

    public void LoadAndExecuteScript(string scriptName, Queue<string> queue)
    {
        commandQueue = queue;
        string _scriptPath = Path.Combine(Application.streamingAssetsPath, $"{scriptName}", k_ScriptExtension);
        if (File.Exists(_scriptPath))
        {
            string[] _commands = File.ReadAllLines(_scriptPath);
            foreach (string _command in _commands)
            {
                commandQueue.Enqueue(_command);
            }
            StartCoroutine(ExecuteCommands());
        }
        else
        {
            Terminal.LogError($"[SCRIPT] Script not found at path: {_scriptPath}");
        }
    }

    public void InitializeAndExecuteScript(string scriptName)
    {
        LoadAndExecuteScript(scriptName, new Queue<string>());
    }

    private IEnumerator ExecuteCommands()
    {
        while (commandQueue.Count > 0)
        {
            string _command = commandQueue.Dequeue();
            yield return StartCoroutine(ExecuteCommandAsync(_command));
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator ExecuteCommandAsync(string command)
    {
        bool _commandCompleted = false;

        Terminal.Shell.RunCommand(command);
        yield return new WaitForSeconds(5); // Simulate command execution time
        _commandCompleted = true;

        if (!_commandCompleted)
        {
            Terminal.LogError($"[SCRIPT] Command execution timed out: {command}");
        }
        else
        {
            Terminal.Log($"[SCRIPT] Command execution completed: {command}");
        }
    }
}
