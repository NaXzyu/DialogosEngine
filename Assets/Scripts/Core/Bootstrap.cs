using UnityEngine;
using CommandTerminal;
using System.Collections;
using System;
using DialogosEngine;

namespace DialogosEngine
{
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
            Initialize();
            LoadBootstrap();
        }

        private void Initialize()
        {
            Terminal.Instance = FindFirstObjectByType<Terminal>();
            if (Terminal.Instance == null)
            {
                Debug.LogError("Unable to find the terminal");
                Utility.Quit(3);
            }
            Terminal.Instance.ToggleCommandInput(false);
        }

        private void LoadBootstrap()
        {
            Terminal.Instance.Log("[BOOT] Attempting to load bootstrap file...");
            BootFile = Resources.Load<TextAsset>(k_Bootstrap);

            if (BootFile == null)
            {
                Terminal.Instance.LogError("[BOOT] ERROR: Bootstrap file missing, unable to boot. Quit.");
                Utility.Quit(3);
            }
            else
            {
                Terminal.Instance.Log("[BOOT] Bootstrap file loaded successfully.");
                StartCoroutine(BootSequence());
            }
        }

        IEnumerator BootSequence()
        {
            yield return new WaitForSeconds(1);
            PrintWelcomeMessage();
            yield return new WaitForSeconds(1);
            RunBoot(BootFile);
            yield return new WaitForSeconds(1);
            PostBoot();
            yield return new WaitForSeconds(1);
            FinalizeBoot();
        }

        private void PrintWelcomeMessage()
        {
            TextAsset _welcome = Resources.Load<TextAsset>(k_Welcome);
            if (_welcome != null)
            {
                PrintLinesFromTextAsset(_welcome, "[BOOT] ");
            }
            else
            {
                Terminal.Instance.LogError("[BOOT] Welcome file not found in Assets/Resources");
            }
        }

        private void RunBoot(TextAsset bootstrap)
        {
            Terminal.Instance.Commands.Initialize(bootstrap);
            Terminal.Instance.Log("[BOOT] Sequence COMPLETE!");
        }

        private void PrintLinesFromTextAsset(TextAsset asset, string prefix)
        {
            string[] _lines = asset.text.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
            Terminal.Instance.Buffer.ArchiveLogs();
            foreach (string _line in _lines)
            {
                if (!_line.Trim().StartsWith("#") && !string.IsNullOrWhiteSpace(_line))
                {
                    Terminal.Instance.Log(prefix + _line);
                }
            }
        }

        private void FinalizeBoot()
        {
            if (_clearPostBoot) Terminal.Instance.Buffer.ArchiveLogs();
            Terminal.Instance.ToggleCommandInput(true);
        }

        private void PostBoot()
        {
            ScriptInterpreter _interpreter = ScriptInterpreter.CreateInstance(k_Interpreter);
            if (_interpreter != null)
            {
                _interpreter.InitializeAndExecuteScript(k_PostBoot);
                Terminal.Instance.Log("[BOOT] ScriptInterpreter: Verification successful");
            }
            else
            {
                Terminal.Instance.LogError("[BOOT] ScriptInterpreter: Verification failed");
            }
        }
    }
}