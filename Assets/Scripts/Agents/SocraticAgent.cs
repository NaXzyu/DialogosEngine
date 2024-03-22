using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;
using UnityEngine;

public class SocraticAgent : Agent
{
    [SerializeField] private Dictionary<string, string> _dialogueTurns;

    public override void Initialize()
    {
        base.Initialize();
        _dialogueTurns = new Dictionary<string, string>();
    }

    public void ProcessInput(string userInput)
    {
        string generatedQuestion = GenerateSocraticQuestion(userInput);
        CollectDialogueTurns(userInput, generatedQuestion);
    }

    private string GenerateSocraticQuestion(string input)
    {
        // Analyze the input to identify key concepts or assumptions
        // For example, use regular expressions or keyword matching

        // Based on the analysis, formulate a Socratic question
        // This is a placeholder for the logic you would use to generate a question

        return "What evidence supports your claim?";
    }

    private void CollectDialogueTurns(string userInput, string agentOutput)
    {
        _dialogueTurns.Add(userInput, agentOutput);
    }

    public void SaveDialogueTurns()
    {
        TrainingManager.Instance.SaveTrainingData(_dialogueTurns);
        _dialogueTurns.Clear();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // TODO handling actions received from the neural network

        // TODO interpret the actions and updating the agent's state
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // TODO providing heuristic (hand-coded) actions in the absence of a trained model
    }
}
