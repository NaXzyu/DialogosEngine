using System;
using UnityEngine;

namespace CommandTerminal
{
    public class Terminal : MonoBehaviour
    {
        TerminalState state;
        TextEditor editor_state;
        bool input_fix;
        bool move_cursor;
        bool initial_open;
        Rect window;
        float current_open_t;
        float open_target;
        float real_window_size;
        string command_text;
        string cached_command_text;
        Vector2 scroll_position;
        GUIStyle window_style;
        GUIStyle label_style;
        GUIStyle input_style;
        bool _allowCommandInput = true;
        bool _shouldScrollToBottom = false;
        int _autoScrollFrameCount = 0;
        int _maxAutoScrollFrames = 5;
        const string k_CommandTextInput = "command_text_field";
        const string k_FontPath = "fonts/F25_Bank_Printer";

        public TerminalSettings TerminalSettings { get; private set; }
        public CommandLog Buffer { get; private set; }
        public CommandShell Shell { get; private set; }
        public CommandHistory History { get; private set; }
        public CommandAutocomplete Autocomplete { get; private set; }

        public bool IssuedError
        {
            get { return Shell.IssuedErrorMessage != null; }
        }

        public bool IsClosed
        {
            get { return state == TerminalState.Close && Mathf.Approximately(current_open_t, open_target); }
        }

        public void Log(string format, params object[] message)
        {
            Log(TerminalLogType.ShellMessage, format, message);
        }

        public void Log(TerminalLogType type, string format, params object[] message)
        {
            Buffer.HandleLog(string.Format(format, message), type);
            _shouldScrollToBottom = true;
        }

        public void SetState(TerminalState new_state)
        {
            PrepareForStateChange();
            HandleStateChange(new_state);
            state = new_state;
        }

        private void PrepareForStateChange()
        {
            input_fix = true;
            cached_command_text = command_text;
            command_text = "";
        }

        private void HandleStateChange(TerminalState new_state)
        {
            switch (new_state)
            {
                case TerminalState.Close:
                    CloseTerminal();
                    break;
                case TerminalState.OpenSmall:
                    OpenTerminalSmall();
                    break;
                case TerminalState.OpenFull:
                default:
                    OpenTerminalFull();
                    break;
            }
        }

        private void CloseTerminal()
        {
            open_target = 0;
        }

        private void OpenTerminalSmall()
        {
            open_target = Screen.height * TerminalSettings.MaxHeight * TerminalSettings.SmallTerminalRatio;
            if (current_open_t > open_target)
            {
                CloseTerminal();
                return;
            }
            real_window_size = open_target;
            scroll_position.y = int.MaxValue;
        }

        private void OpenTerminalFull()
        {
            real_window_size = Screen.height * TerminalSettings.MaxHeight;
            open_target = real_window_size;
        }


        public void ToggleState(TerminalState new_state)
        {
            if (state == new_state)
            {
                SetState(TerminalState.Close);
            }
            else
            {
                SetState(new_state);
            }
        }

        void Awake()
        {
            Initialize();
        }

        void Initialize()
        {
            TerminalSettings = new TerminalSettings();
            Buffer = new CommandLog(TerminalSettings.BufferSize);
            Shell = new CommandShell();
            History = new CommandHistory();
            Autocomplete = new CommandAutocomplete();
        }

        void Start()
        {
            if (TerminalSettings.ConsoleFont == null)
            {
                TerminalSettings.ConsoleFont = Resources.Load(k_FontPath, typeof(Font)) as Font;
                if (TerminalSettings.ConsoleFont == null)
                {
                    Debug.LogError($"The font '{k_FontPath}' could not be loaded.");
                    Utility.Quit();
                }
            }


            command_text = "";
            cached_command_text = command_text;

            SetupWindow();
            SetupInput();
            SetupLabels();

            if (IssuedError)
            {
                Log(TerminalLogType.Error, "Error: {0}", Shell.IssuedErrorMessage);
            }

            foreach (var command in Shell.Commands)
            {
                Autocomplete.Register(command.Key);
            }

            SetState(TerminalState.OpenFull);
            initial_open = true;
        }

        void OnGUI()
        {

            if (IsClosed)
            {
                return;
            }

            HandleOpenness();
            window = GUILayout.Window(88, window, DrawConsole, "", window_style);
        }

        void SetupWindow()
        {
            real_window_size = Screen.height * TerminalSettings.MaxHeight / 3;
            window = new Rect(0, current_open_t - real_window_size, Screen.width, real_window_size);

            Texture2D background_texture = new Texture2D(1, 1);
            background_texture.SetPixel(0, 0, TerminalSettings.BackgroundColor);
            background_texture.Apply();

            window_style = new GUIStyle();
            window_style.normal.background = background_texture;
            window_style.padding = new RectOffset(4, 4, 4, 4);
            window_style.normal.textColor = TerminalSettings.ForegroundColor;
            window_style.font = TerminalSettings.ConsoleFont;
        }

        void SetupLabels()
        {
            label_style = new GUIStyle();
            label_style.font = TerminalSettings.ConsoleFont;
            label_style.normal.textColor = TerminalSettings.ForegroundColor;
            label_style.wordWrap = true;
        }

        void SetupInput()
        {
            input_style = new GUIStyle();
            input_style.padding = new RectOffset(4, 4, 4, 4);
            input_style.font = TerminalSettings.ConsoleFont;
            input_style.fixedHeight = TerminalSettings.ConsoleFont.fontSize * 1.6f;
            input_style.normal.textColor = TerminalSettings.InputColor;

            var dark_background = new Color();
            dark_background.r = TerminalSettings.BackgroundColor.r - TerminalSettings.InputContrast;
            dark_background.g = TerminalSettings.BackgroundColor.g - TerminalSettings.InputContrast;
            dark_background.b = TerminalSettings.BackgroundColor.b - TerminalSettings.InputContrast;
            dark_background.a = 0.5f;

            Texture2D input_background_texture = new Texture2D(1, 1);
            input_background_texture.SetPixel(0, 0, dark_background);
            input_background_texture.Apply();
            input_style.normal.background = input_background_texture;
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
            scroll_position = GUILayout.BeginScrollView(scroll_position, false, false, GUIStyle.none, GUIStyle.none);
            GUILayout.FlexibleSpace();
            DrawLogs();
            GUILayout.EndScrollView();
            CheckScrollToBottom();
        }

        void HandleCommandInput()
        {
            if (_allowCommandInput)
            {
                HandleKeyboardEvents();
                DrawCommandTextField();
            }
        }

        void HandleKeyboardEvents()
        {
            if (move_cursor)
            {
                CursorToEnd();
                move_cursor = false;
            }
            if (Event.current.Equals(Event.KeyboardEvent("return")))
            {
                EnterCommand();
            }
            else if (Event.current.Equals(Event.KeyboardEvent("up")))
            {
                command_text = History.Previous();
                move_cursor = true;
            }
            else if (Event.current.Equals(Event.KeyboardEvent("down")))
            {
                command_text = History.Next();
            }
            else if (Event.current.Equals(Event.KeyboardEvent("tab")))
            {
                CompleteCommand();
                move_cursor = true; // Wait till next draw call
            }
        }

        void DrawCommandTextField()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(TerminalSettings.InputCaret, input_style, GUILayout.Width(TerminalSettings.ConsoleFont.fontSize));
            GUI.SetNextControlName(k_CommandTextInput);
            command_text = GUILayout.TextField(command_text, input_style);
            CheckInputFix();
            FocusOnInitialOpen();
            GUILayout.EndHorizontal();
        }

        void CheckInputFix()
        {
            if (input_fix && command_text.Length > 0)
            {
                command_text = cached_command_text;
                input_fix = false;
            }
        }

        void FocusOnInitialOpen()
        {
            if (initial_open)
            {
                GUI.FocusControl(k_CommandTextInput);
                initial_open = false;
            }
        }

        void CheckScrollToBottom()
        {
            if (_shouldScrollToBottom)
            {
                _autoScrollFrameCount++;
                scroll_position.y = Mathf.Infinity;
                if (_autoScrollFrameCount < _maxAutoScrollFrames)
                {
                    _shouldScrollToBottom = false;
                    _autoScrollFrameCount = 0;
                }
            }
        }

        void DrawLogs()
        {
            foreach (var log in Buffer.Logs)
            {
                label_style.normal.textColor = TerminalUtils.GetLogColor(TerminalSettings, log.type);
                GUILayout.Label(log.message, label_style);
            }
        }

        void HandleOpenness()
        {
            float dt = TerminalSettings.ToggleSpeed * Time.deltaTime;

            if (current_open_t < open_target)
            {
                current_open_t += dt;
                if (current_open_t > open_target) current_open_t = open_target;
            }
            else if (current_open_t > open_target)
            {
                current_open_t -= dt;
                if (current_open_t < open_target) current_open_t = open_target;
            }
            else
            {
                return;
            }

            window = new Rect(0, current_open_t - real_window_size, Screen.width, real_window_size);
        }

        void EnterCommand()
        {
            Log(TerminalLogType.Input, "{0}", command_text);
            Shell.RunCommand(command_text);
            History.Push(command_text);

            if (IssuedError)
            {
                Log(TerminalLogType.Error, "{0}", Shell.IssuedErrorMessage);
            }

            command_text = "";
            scroll_position.y = int.MaxValue;
        }

        void CompleteCommand()
        {
            string head_text = command_text;
            string[] completion_buffer = Autocomplete.Complete(ref head_text);
            int completion_length = completion_buffer.Length;

            if (completion_length == 1)
            {
                command_text = head_text + completion_buffer[0];
            }
            else if (completion_length > 1)
            {
                Log(string.Join("    ", completion_buffer));
                scroll_position.y = int.MaxValue;
            }
        }

        void CursorToEnd()
        {
            if (editor_state == null)
            {
                editor_state = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            }

            editor_state.MoveCursorToPosition(new Vector2(999, 999));
        }

        public void LogError(string message)
        {
            Shell.IssueErrorMessage($"{message}");
        }

        public void LogWarning(string message)
        {
            Log($"[WARN] {message}");
        }

        public void ToggleCommandInput(bool enable)
        {
            _allowCommandInput = enable;
        }
    }
}
