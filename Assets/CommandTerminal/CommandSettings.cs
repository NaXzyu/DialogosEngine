using UnityEngine;

namespace CommandTerminal
{
    public class CommandSettings
    {
        public float MaxHeight { get; set; } = 1f;
        public float SmallTerminalRatio { get; set; } = 0.33f;
        public float ToggleSpeed { get; set; } = 1000;
        public int BufferSize { get; set; } = 1000000;
        public Font ConsoleFont { get; set; }
        public string InputCaret { get; set; } = ">";
        public float InputContrast { get; set; }
        public Color BackgroundColor { get; set; } = Color.black;
        public Color ForegroundColor { get; set; } = Color.white;
        public Color ShellColor { get; set; } = Color.white;
        public Color InputColor { get; set; } = Color.white;
        public Color WarningColor { get; set; } = Color.yellow;
        public Color ErrorColor { get; set; } = Color.red;
    }
}