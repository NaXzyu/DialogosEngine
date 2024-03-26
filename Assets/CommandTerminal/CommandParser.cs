using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommandTerminal
{
    public class CommandParser
    {
        public static string ConvertCommandDataToCommandLine(CommandData commandData)
        {
            string commandLine = commandData.Name;
            foreach (var arg in commandData.Args)
            {
                commandLine += " " + arg.String;
            }
            return commandLine;
        }

        public static string GetCommandNameFromLine(string line)
        {
            var trimmedLine = line.Trim();
            var firstSpaceIndex = trimmedLine.IndexOf(' ');
            var commandName = trimmedLine.Substring(0, firstSpaceIndex);
            return commandName;
        }

        public static string ParsedCommandLine(string line)
        {
            string[] parts = line.Replace("{", "").Replace("}", "").Split(',');
            string parsedLine = string.Join(" ", parts.Take(parts.Length - 1).Select(p => p.Trim()));
            string lastPart = parts.Last().Trim();

            if (!Regex.IsMatch(lastPart, "^\".*\"$"))
            {
                parsedLine += " \"" + lastPart + "\"";
            }
            else
            {
                parsedLine += " " + lastPart;
            }

            return parsedLine;
        }

        public static CommandData? ParseCommandData(string line)
        {
            try
            {
                var matches = Regex.Matches(line, @"[\""].+?[\""]|[^ ]+")
                                   .Cast<Match>()
                                   .Select(m => m.Value)
                                   .ToList();

                string commandName = matches[0].Trim().ToUpper();
                matches.RemoveAt(0);

                List<CommandArg> args = new List<CommandArg>();

                foreach (var match in matches)
                {
                    string argValue = match.Trim().Trim('\"');
                    args.Add(new CommandArg { Value = argValue });
                }

                return new CommandData
                {
                    Name = commandName,
                    Args = args.ToArray(),
                };
            }
            catch (Exception ex)
            {
                Terminal.Instance.LogError($"Error parsing command data: {line}\nException: {ex.Message}");
                return null;
            }
        }
    }
}
