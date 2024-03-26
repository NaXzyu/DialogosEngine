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

        public static CommandArg EatArgument(ref string s)
        {
            var arg = new CommandArg();
            int space_index = s.IndexOf(' ');
            if (space_index >= 0)
            {
                arg.String = s.Substring(0, space_index);
                s = s.Substring(space_index + 1);
            }
            else
            {
                arg.String = s;
                s = "";
            }
            return arg;
        }
    }
}
