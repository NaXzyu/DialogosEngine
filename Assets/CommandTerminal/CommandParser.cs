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
            string _commandLine = commandData.Name;
            foreach (var arg in commandData.Args)
            {
                _commandLine += " " + arg.String;
            }
            return _commandLine;
        }

        public static string GetCommandNameFromLine(string line)
        {
            var _trimmedLine = line.Trim();
            var _firstSpaceIndex = _trimmedLine.IndexOf(' ');
            var _commandName = _trimmedLine.Substring(0, _firstSpaceIndex);
            return _commandName;
        }

        public static string ParsedCommandLine(string line)
        {
            string[] _parts = line.Replace("{", "").Replace("}", "").Split(',');
            string _parsedLine = string.Join(" ", _parts.Take(_parts.Length - 1).Select(p => p.Trim()));
            string _lastPart = _parts.Last().Trim();

            if (!Regex.IsMatch(_lastPart, "^\".*\"$"))
            {
                _parsedLine += " \"" + _lastPart + "\"";
            }
            else
            {
                _parsedLine += " " + _lastPart;
            }

            return _parsedLine;
        }

        public static CommandData? ParseCommandData(string line)
        {
            try
            {
                var _matches = Regex.Matches(line, @"[\""].+?[\""]|[^ ]+")
                                   .Cast<Match>()
                                   .Select(m => m.Value)
                                   .ToList();

                string _commandName = _matches[0].Trim().ToUpper();
                _matches.RemoveAt(0);

                List<CommandArg> _args = new List<CommandArg>();

                foreach (var _match in _matches)
                {
                    string _argValue = _match.Trim().Trim('\"');
                    _args.Add(new CommandArg { Value = _argValue });
                }

                return new CommandData
                {
                    Name = _commandName,
                    Args = _args.ToArray(),
                };
            }
            catch (Exception ex)
            {
                Terminal.Instance.LogError($"[PRSR] Error parsing command data: {line}\nException: {ex.Message}");
                return null;
            }
        }
    }
}
