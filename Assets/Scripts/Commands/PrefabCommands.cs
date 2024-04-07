using UnityEngine;

namespace CommandTerminal
{
    public static class PrefabCommands
    {
        [Command("prefab")]
        public static void PrefabCommand(CommandArg[] args)
        {
            if (args.Length < 2)
            {
                Terminal.Instance.LogError("[PREFAB] Usage: prefab <mode> <prefab_name>");
                return;
            }

            string mode = args[0].String.ToLower();
            string prefabName = args[1].String;

            switch (mode)
            {
                case "load":
                    LoadPrefab(prefabName);
                    break;
                case "destroy":
                    DestroyPrefab(prefabName);
                    break;
                default:
                    Terminal.Instance.LogError($"[PREFAB] Unknown mode: {mode}");
                    break;
            }
        }

        private static void LoadPrefab(string prefabName)
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
            if (prefab != null)
            {
                Object.Instantiate(prefab);
                Terminal.Instance.Log($"[PREFAB] {prefabName} loaded successfully.");
            }
            else
            {
                Terminal.Instance.LogError($"[PREFAB] Could not find prefab: {prefabName}");
            }
        }

        private static void DestroyPrefab(string prefabName)
        {
            GameObject prefabObject = GameObject.Find($"{prefabName}(Clone)");
            if (prefabObject != null)
            {
                Object.Destroy(prefabObject);
                Terminal.Instance.Log($"[PREFAB] {prefabName} destroyed successfully.");
            }
            else
            {
                Terminal.Instance.LogError($"[PREFAB] Could not find object: {prefabName}");
            }
        }
    }
}
