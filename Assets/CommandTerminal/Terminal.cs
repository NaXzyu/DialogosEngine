using UnityEngine;
using DialogosEngine;

namespace CommandTerminal
{
    public class Terminal : MonoBehaviour
    {
        public static Terminal Instance;

        public CommandState State;
        public bool InputFix;
        public bool MoveCursor;

        bool _initialOpen;
        GUIStyle _labelStyle;
        GUIStyle _inputStyle;
        bool _allowInput = true;
        bool _scrollToBottom = false;
        int _autoScrollFrameCount = 0;
        int _maxAutoScrollFrames = 5;

        const string k_CommandTextInput = "command_text_field";
        const string k_FontPath = "fonts/F25_Bank_Printer";

        public Commands Commands { get; private set; }
        public CommandSettings Settings { get; private set; }
        public CommandBuffer Buffer { get; private set; }
        public CommandShell Shell { get; private set; }
        public CommandHistory History { get; private set; }
        public CommandAutocomplete Autocomplete { get; private set; }
        public CommandStates States { get; private set; }
        public CommandEvents Events { get; private set; }
        public CommandGUI GUI { get; private set; }
        public CommandSystem System { get; private set; }

        void Awake()
        {
            Instance = this;
            Initialize();
        }

        void Initialize()
        {
            Commands = new Commands();
            Settings = new CommandSettings();
            Buffer = new CommandBuffer(Settings.BufferSize);
            Shell = new CommandShell();
            History = new CommandHistory();
            Autocomplete = new CommandAutocomplete();
            States = new CommandStates();
            Events = new CommandEvents();
            GUI = new CommandGUI();
            System = new CommandSystem();
        }

        void Start()
        {
            if (Settings.ConsoleFont == null)
            {
                Settings.ConsoleFont = Resources.Load(k_FontPath, typeof(Font)) as Font;
                if (Settings.ConsoleFont == null)
                {
                    Log(LogType.Error, "[ERRO] The font {0} could not be loaded.", k_FontPath);
                    Utility.Quit(3);
                }
            }

            States.CommandText = "";
            States.CachedCommandText = States.CommandText;

            GUI.SetupWindow(this);
            SetupInput();
            SetupLabels();

            if (IssuedError)
            {
                Log(LogType.Error, "[ERRO] {0}", Shell.IssuedErrorMessage);
            }

            foreach (var _command in Shell.Commands)
            {
                Autocomplete.Register(_command.Key);
            }

            States.SetState(this, CommandState.OpenFull);
            _initialOpen = true;
        }

        private void Update()
        {
            System?.Update();
        }

        void OnGUI()
        {
            if (IsClosed)
            {
                return;
            }
            GUI.HandleOpenness(this);
            GUI.Window = GUILayout.Window(88, GUI.Window, DrawConsole, "", GUI.WindowStyle);
        }

        void SetupLabels()
        {
            _labelStyle = new GUIStyle();
            _labelStyle.font = Settings.ConsoleFont;
            _labelStyle.normal.textColor = Settings.ForegroundColor;
            _labelStyle.wordWrap = true;
        }

        void SetupInput()
        {
            _inputStyle = new GUIStyle();
            _inputStyle.padding = new RectOffset(4, 4, 4, 4);
            _inputStyle.font = Settings.ConsoleFont;
            _inputStyle.fixedHeight = Settings.ConsoleFont.fontSize * 1.6f;
            _inputStyle.normal.textColor = Settings.InputColor;

            var _darkBackground = new Color();
            _darkBackground.r = Settings.BackgroundColor.r - Settings.InputContrast;
            _darkBackground.g = Settings.BackgroundColor.g - Settings.InputContrast;
            _darkBackground.b = Settings.BackgroundColor.b - Settings.InputContrast;
            _darkBackground.a = 0.5f;

            Texture2D _inputBackground = new Texture2D(1, 1);
            _inputBackground.SetPixel(0, 0, _darkBackground);
            _inputBackground.Apply();
            _inputStyle.normal.background = _inputBackground;
        }

        void DrawConsole(int Window2D)
        {
            GUILayout.BeginVertical();
            DrawScrollView();
            HandleCommandInput();
            GUILayout.EndVertical();
        }

        void DrawScrollView()
        {
            GUI.ScrollPosition = GUILayout.BeginScrollView(GUI.ScrollPosition, false, false, GUIStyle.none, GUIStyle.none);
            GUILayout.FlexibleSpace();
            DrawLogs();
            GUILayout.EndScrollView();
            CheckScrollToBottom();
        }

        void HandleCommandInput()
        {
            if (_allowInput)
            {
                Events.HandleKeyboardEvents(this);
                DrawCommandTextField();
            }
        }

        void DrawCommandTextField()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Settings.InputCaret, _inputStyle, GUILayout.Width(Settings.ConsoleFont.fontSize));
            UnityEngine.GUI.SetNextControlName(k_CommandTextInput);
            States.CommandText = GUILayout.TextField(States.CommandText, _inputStyle);
            CheckInputFix();
            FocusOnInitialOpen();
            GUILayout.EndHorizontal();
        }

        void CheckInputFix()
        {
            if (InputFix && States.CommandText.Length > 0)
            {
                States.CommandText = States.CachedCommandText;
                InputFix = false;
            }
        }

        void FocusOnInitialOpen()
        {
            if (_initialOpen)
            {
                UnityEngine.GUI.FocusControl(k_CommandTextInput);
                _initialOpen = false;
            }
        }

        void CheckScrollToBottom()
        {
            if (_scrollToBottom)
            {
                _autoScrollFrameCount++;
                GUI.ScrollPosition.y = Mathf.Infinity;
                if (_autoScrollFrameCount < _maxAutoScrollFrames)
                {
                    _scrollToBottom = false;
                    _autoScrollFrameCount = 0;
                }
            }
        }

        void DrawLogs()
        {
            foreach (var log in Buffer.Logs)
            {
                _labelStyle.normal.textColor = CommandUtils.GetLogColor(Settings, log.Type);
                GUILayout.Label(log.Message, _labelStyle);
            }
        }

        public void EnterCommand()
        {
            Log(LogType.Input, "[TERM] {0}", States.CommandText);
            Shell.Run(States.CommandText);
            History.Push(States.CommandText);

            if (IssuedError)
            {
                Log(LogType.Error, "[TERM] {0}", Shell.IssuedErrorMessage);
            }

            States.CommandText = "";
            GUI.ScrollPosition.y = int.MaxValue;
        }

        public void CompleteCommand()
        {
            string head_text = States.CommandText;
            string[] completion_buffer = Autocomplete.Complete(ref head_text);
            int completion_length = completion_buffer.Length;

            if (completion_length == 1)
            {
                States.CommandText = head_text + completion_buffer[0];
            }
            else if (completion_length > 1)
            {
                Log(string.Join("    ", completion_buffer));
                GUI.ScrollPosition.y = int.MaxValue;
            }
        }

        public void ToggleCommandInput(bool enable)
        {
            _allowInput = enable;
        }

        public bool IssuedError
        {
            get { return Shell.IssuedErrorMessage != null; }
        }

        public bool IsClosed
        {
            get { return State == CommandState.Close && Mathf.Approximately(GUI.CurrentOpenT, GUI.OpenTarget); }
        }

        public void Log(string format, params object[] message)
        {
            Log(LogType.ShellMessage, format, message);
        }

        public void Log(LogType type, string message, params object[] args)
        {
            string formattedMessage = args.Length > 0 ? string.Format(message, args) : message;
            Buffer.Append(formattedMessage, type);
            _scrollToBottom = true;
        }
        public void LogWarning(string message)
        {
            Log($"[WARN] {message}");
        }

        public void LogError(string message)
        {
            Shell.IssueErrorMessage($"{message}");
        }
    }
}
