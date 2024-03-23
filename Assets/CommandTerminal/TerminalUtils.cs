using CommandTerminal;
using UnityEngine;

public static class TerminalUtils
{
    public const string InputCaret = ">";

    public static readonly Color BackgroundColor = Color.black;
    public static readonly Color ForegroundColor = Color.white;
    public static readonly Color ShellColor = Color.white;
    public static readonly Color InputColor = Color.white;
    public static readonly Color WarningColor = Color.yellow;
    public static readonly Color ErrorColor = Color.red;

    public static Color GetLogColor(TerminalLogType type)
    {
        switch (type)
        {
            case TerminalLogType.Message: return ForegroundColor;
            case TerminalLogType.Warning: return WarningColor;
            case TerminalLogType.Input: return InputColor;
            case TerminalLogType.ShellMessage: return ShellColor;
            default: return ErrorColor;
        }
    }

    // Other utility methods...
}
