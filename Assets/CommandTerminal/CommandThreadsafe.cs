using CommandTerminal;
using System.Collections.Concurrent;

public class CommandThreadsafe
{
    private static ConcurrentQueue<string> logQueue = new();
    private static ConcurrentQueue<CommandArg[]> commandQueue = new();

    public static void Log(string message)
    {
        logQueue.Enqueue(message);
    }

    public static void QueueCommand(CommandArg[] commandArgs)
    {
        commandQueue.Enqueue(commandArgs);
    }

    public void Update()
    {
        while (logQueue.TryDequeue(out string message))
        {
            Terminal.Instance.Log(message);
        }

        while (commandQueue.TryDequeue(out CommandArg[] commandArgs))
        {
            Terminal.Instance.Shell.Run(commandArgs[0].String);
        }
    }
}
