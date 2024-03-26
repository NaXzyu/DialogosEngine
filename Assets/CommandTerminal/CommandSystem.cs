using CommandTerminal;
using System.Collections.Concurrent;

public class CommandSystem
{
    private static ConcurrentQueue<string> logQueue = new();
    private static ConcurrentQueue<string> errorQueue = new();
    private static ConcurrentQueue<CommandArg[]> commandQueue = new();

    public static void Log(string message)
    {
        logQueue.Enqueue(message);
    }

    public static void LogError(string message)
    {
        errorQueue.Enqueue(message);
    }

    public static void QueueCommand(CommandArg[] commandArgs)
    {
        commandQueue.Enqueue(commandArgs);
    }

    public void Update()
    {
        while (logQueue.TryDequeue(out string message))
        {
            Terminal.Instance.Log("[PROC] " + message);
        }
        while (errorQueue.TryDequeue(out string message))
        {
            Terminal.Instance.LogError("[PROC] " + message);
        }
        while (commandQueue.TryDequeue(out CommandArg[] commandArgs))
        {
            Terminal.Instance.Shell.Run(commandArgs[0].String);
        }
    }
}
