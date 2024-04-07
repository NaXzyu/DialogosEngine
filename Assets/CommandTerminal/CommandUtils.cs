using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CommandTerminal
{
    public static class CommandUtils
    {
        public static Color GetLogColor(CommandSettings settings, LogType type)
        {
            return type switch
            {
                LogType.Message => settings.ForegroundColor,
                LogType.Warning => settings.WarningColor,
                LogType.Input => settings.InputColor,
                LogType.ShellMessage => settings.ShellColor,
                _ => settings.ErrorColor,
            };
        }

        public static Dictionary<string, MethodInfo> CacheCommandMethods()
        {
            var methodDictionary = new Dictionary<string, MethodInfo>();
            var methodsWithCommandAttribute = Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0);

            foreach (var method in methodsWithCommandAttribute)
            {
                var attribute = (CommandAttribute)method.GetCustomAttributes(typeof(CommandAttribute), false).First();
                string keyName = attribute.Name.ToUpper();
                if (!methodDictionary.ContainsKey(keyName))
                {
                    methodDictionary.Add(keyName, method);
                }
            }

            return methodDictionary;
        }

        public static CommandArg ParseCommand(ref string s)
        {
            var arg = new CommandArg();
            s = s.Trim();

            char[] quoteChars = { '\"', '\'' };

            foreach (var quoteChar in quoteChars)
            {
                if (s.StartsWith(quoteChar.ToString()))
                {
                    int quoteIndex = FindClosingQuote(s, quoteChar);
                    if (quoteIndex >= 0)
                    {
                        arg.String = UnescapedQuotes(s.Substring(1, quoteIndex - 1), quoteChar);
                        s = s.Substring(quoteIndex + 1).Trim();
                        return arg;
                    }
                }
            }

            int spaceIndex = s.IndexOf(' ');
            if (spaceIndex >= 0)
            {
                arg.String = s.Substring(0, spaceIndex);
                s = s.Substring(spaceIndex + 1).Trim();
            }
            else
            {
                arg.String = s;
                s = "";
            }

            return arg;
        }

        public static int FindClosingQuote(string s, char quoteChar)
        {
            int quoteIndex = s.IndexOf(quoteChar, 1);
            while (quoteIndex > 0 && s[quoteIndex - 1] == '\\')
            {
                quoteIndex = s.IndexOf(quoteChar, quoteIndex + 1);
            }
            return quoteIndex;
        }

        public static string UnescapedQuotes(string s, char quoteChar)
        {
            return s.Replace("\\" + quoteChar, quoteChar.ToString());
        }
    }
}
