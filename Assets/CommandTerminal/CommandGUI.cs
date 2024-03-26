using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace CommandTerminal
{
    public class CommandGUI
    {
        public float RealWindowSize;
        public Rect Window;
        public float CurrentOpenT;
        public GUIStyle WindowStyle;
        public float OpenTarget;
        public Vector2 ScrollPosition;
        public TextEditor EditorState;

        public void SetupWindow(Terminal terminal)
        {
            RealWindowSize = Screen.height * terminal.Settings.MaxHeight / 3;
            Window = new Rect(0, CurrentOpenT - RealWindowSize, Screen.width, RealWindowSize);

            Texture2D _background = new Texture2D(1, 1);
            _background.SetPixel(0, 0, terminal.Settings.BackgroundColor);
            _background.Apply();

            WindowStyle = new GUIStyle();
            WindowStyle.normal.background = _background;
            WindowStyle.padding = new RectOffset(4, 4, 4, 4);
            WindowStyle.normal.textColor = terminal.Settings.ForegroundColor;
            WindowStyle.font = terminal.Settings.ConsoleFont;
        }

        public void CloseTerminal(Terminal terminal)
        {
            OpenTarget = 0;
        }

        public void OpenTerminalSmall(Terminal terminal)
        {
            OpenTarget = Screen.height * terminal.Settings.MaxHeight * terminal.Settings.SmallTerminalRatio;
            if (CurrentOpenT > OpenTarget)
            {
                CloseTerminal(terminal);
                return;
            }
            RealWindowSize = OpenTarget;
            ScrollPosition.y = int.MaxValue;
        }

        public void OpenTerminalFull(Terminal terminal)
        {
            RealWindowSize = Screen.height * terminal.Settings.MaxHeight;
            OpenTarget = RealWindowSize;
        }

        public void CursorToEnd(Terminal terminal)
        {
            if (EditorState == null)
            {
                EditorState = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            }

            EditorState.MoveCursorToPosition(new Vector2(999, 999));
        }

        public void HandleOpenness(Terminal terminal)
        {
            float _dt = terminal.Settings.ToggleSpeed * Time.deltaTime;

            if (CurrentOpenT < OpenTarget)
            {
                CurrentOpenT += _dt;
                if (CurrentOpenT > OpenTarget) CurrentOpenT = OpenTarget;
            }
            else if (CurrentOpenT > OpenTarget)
            {
                CurrentOpenT -= _dt;
                if (CurrentOpenT < OpenTarget) CurrentOpenT = OpenTarget;
            }
            else
            {
                return;
            }

            Window = new Rect(0, CurrentOpenT - RealWindowSize, Screen.width, RealWindowSize);
        }
    }
}
