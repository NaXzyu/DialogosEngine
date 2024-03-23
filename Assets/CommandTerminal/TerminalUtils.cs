using CommandTerminal;
using UnityEngine;

public static class TerminalUtils
{
    public static Color GetLogColor(TerminalSettings settings, TerminalLogType type)
    {
        switch (type)
        {
            case TerminalLogType.Message: return settings.ForegroundColor;
            case TerminalLogType.Warning: return settings.WarningColor;
            case TerminalLogType.Input: return settings.InputColor;
            case TerminalLogType.ShellMessage: return settings.ShellColor;
            default: return settings.ErrorColor;
        }
    }
}
