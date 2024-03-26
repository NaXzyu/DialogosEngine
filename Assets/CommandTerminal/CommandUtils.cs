using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CommandTerminal
{
    public static class CommandUtils
    {
        public static CommandData? ParseCommandData(string line)
        {
            try
            {
                string[] parts = line.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                string commandName = parts[0].Trim();
                string[] details = parts[1].Trim().Split(',');
                string procedureName = details[1].Trim();
                int minArgs = int.Parse(details[2].Trim());
                int maxArgs = int.Parse(details[3].Trim());
                string helpText = details[4].Trim();

                return new CommandData
                {
                    CommandName = commandName,
                    ProcedureName = procedureName,
                    MinArgs = minArgs,
                    MaxArgs = maxArgs,
                    HelpText = helpText
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing command data: {line}\nException: {ex.Message}");
                return null;
            }
        }

        public static CommandInfo? InferCommandInfo(CommandData? commandData, Dictionary<string, MethodInfo> commandMethods)
        {
            if (commandData.HasValue)
            {
                if (commandMethods.TryGetValue(commandData.Value.ProcedureName, out MethodInfo methodInfo))
                {
                    return new CommandInfo
                    {
                        procedure = (CommandArg[] args) => methodInfo.Invoke(null, new object[] { args }),
                        min_arg_count = commandData.Value.MinArgs,
                        max_arg_count = commandData.Value.MaxArgs,
                        help = commandData.Value.HelpText
                    };
                }
                else
                {
                    Debug.LogError($"Command method not found: {commandData.Value.ProcedureName}");
                    return null;
                }
            }
            else
            {
                Debug.LogError("CommandData does not have a value.");
                return null;
            }
        }
    }
}
