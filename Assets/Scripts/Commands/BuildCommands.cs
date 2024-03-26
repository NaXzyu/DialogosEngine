using DialogosEngine;
using UnityEngine;

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

                // TODO implement logic for scriptable build pipeline

                if (hasCompileErrors)
                {
                    Terminal.Instance.LogError("[UNTY] Build failed due to compile errors.");
                }
                else
                {
                    // TODO implement scriptable build pipeline

                    Terminal.Instance.Log("[BILD] Compiling scripts...");
                    Terminal.Instance.Log("[BILD] Building asset bundles...");
                    Terminal.Instance.Log("[BILD] Build completed successfully.");
                }
            }
            else
            {
                Terminal.Instance.LogError($"[BILD] Incorrect number of arguments. Expected: {k_ExpectedArgs}");
            }
        }
    }
}
