using CommandTerminal;

public static class TrainCommands
{
    [RegisterCommand(Help = "Trains the Agent using conversation turn data", MaxArgCount = 0)]
    static void CommandTrain(CommandArg[] args)
    {
        // TODO implement argument for agent name, ex: 'socratic'. This should be in a switch case statement.

        // TODO add error handling for the arguments

        if (TrainingManager.Instance.IsTrainingDataAvailable())
        {
            TrainingManager.Instance.StartTraining();
        }
        else
        {
            Terminal.Log("[TRAIN] Not enough data to start training. Please collect more dialogue turns.");
        }
    }
}