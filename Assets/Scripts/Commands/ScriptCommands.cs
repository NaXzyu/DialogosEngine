using CommandTerminal;
using System.IO;
using UnityEngine;

public static class ScriptCommands
{
    [RegisterCommand(Help = "List the current .dil programs available", MaxArgCount = 0)]
    static void CommandList(CommandArg[] args)
    {
        Terminal.Log("[SCRIPT] Listing .dil files in Resources and StreamingAssets directories...");

        TextAsset[] dilFilesInResources = Resources.LoadAll<TextAsset>("");
        foreach (var file in dilFilesInResources)
        {
            if (file.name.EndsWith(ScriptInterpreter.k_ScriptExtension))
            {
                Terminal.Log($"[SCRIPT] Resources: {file.name}");
            }
        }

        string[] dilFilesInStreamingAssets = Directory.GetFiles(Application.streamingAssetsPath, "*" + ScriptInterpreter.k_ScriptExtension, SearchOption.AllDirectories);
        foreach (var filePath in dilFilesInStreamingAssets)
        {
            Terminal.Log($"[SCRIPT] StreamingAssets: {Path.GetFileName(filePath)}");
        }
    }
}
