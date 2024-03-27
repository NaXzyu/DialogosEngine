using DialogosEngine;
using UnityEngine;

namespace CommandTerminal
{
    public static class BuildCommands
    {
        public const int k_ExpectedArgs = 0;
        public const string k_BinBuildBatchFile = "/Bin/Build.bat";

        [Command("build")]
        public static void BuildCommand(CommandArg[] args)
        {
            if (args.Length == k_ExpectedArgs)
            {
                Terminal.Instance.Log("[BULD] Starting build process...");
                bool hasCompileErrors = false;

                Processor.ExecuteBatchFile(k_BinBuildBatchFile);

                if (hasCompileErrors)
                {
                    Terminal.Instance.LogError("[BULD] Build failed due to compile errors.");
                }
            }
            else
            {
                Terminal.Instance.LogError($"[BULD] Incorrect number of arguments. Expected: {k_ExpectedArgs}");
            }
        }
    }
}
