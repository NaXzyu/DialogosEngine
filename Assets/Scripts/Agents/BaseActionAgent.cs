using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;
using UnityEngine;

public class BaseActionAgent : Agent
{
    // Dictionary to hold known words and their meanings
    private Dictionary<string, string> knowledgeBase;

    // Initialize the agent
    public override void Initialize()
    {
        base.Initialize();
        knowledgeBase = new Dictionary<string, string>();
        // Prepopulate the knowledge base with known words and their meanings
        knowledgeBase.Add("hello `{person}`", "A greeting used when meeting someone.");
        knowledgeBase.Add("What is `{word}`", "A description of something");
    }

    // Method to process user input
    public void ProcessInput(string userInput)
    {
        string[] words = userInput.Split(' ');
        foreach (string word in words)
        {
            if (!knowledgeBase.ContainsKey(word.ToLower()))
            {
                // The agent does not know this word, ask for its meaning
                AskForMeaning(word);
            }
        }
    }

    // Method to ask for the meaning of an unknown word
    private void AskForMeaning(string word)
    {
        Debug.Log($"What is '{word}'?");
    }

    // Method to update the knowledge base with new information
    public void UpdateKnowledgeBase(string word, string meaning)
    {
        if (!knowledgeBase.ContainsKey(word))
        {
            knowledgeBase.Add(word, meaning);
        }
    }

    // Implement other necessary methods for the ML-Agents framework
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Implement decision-making logic here
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Implement heuristic logic here for testing without ML model
    }
}
