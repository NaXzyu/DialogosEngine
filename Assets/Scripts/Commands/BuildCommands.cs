using DialogosEngine;

namespace CommandTerminal
{
    public static class BuildCommands
    {
        public const int k_ExpectedArgs = 0;

        [Command("build")]
        public static void BuildCommand(CommandArg[] args)
        {
            if (args.Length == k_ExpectedArgs)
            {
                Terminal.Instance.Log("[BILD] Starting build process...");
                bool hasCompileErrors = false;

                Processor.ExecuteBatchFile("/Resources/Bin/Build.bat");

                if (hasCompileErrors)
                {
                    Terminal.Instance.LogError("[BILD] Build failed due to compile errors.");
                }
            }
            else
            {
                Terminal.Instance.LogError($"[BILD] Incorrect number of arguments. Expected: {k_ExpectedArgs}");
            }
        }
    }
}
