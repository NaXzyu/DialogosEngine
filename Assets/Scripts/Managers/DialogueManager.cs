using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager _instance;
    public static DialogueManager Instance { get { return _instance; } }

    private Dictionary<int, List<string>> conversations;
    private int conversationIDCounter = 0;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            conversations = new Dictionary<int, List<string>>();
        }
    }

    public int StartNewConversation()
    {
        int newID = conversationIDCounter++;
        conversations[newID] = new List<string>();
        return newID;
    }

    public void AddDialogueTurn(int conversationID, string userInput, string agentOutput)
    {
        if (conversations.ContainsKey(conversationID))
        {
            conversations[conversationID].Add($"User: {userInput}");
            conversations[conversationID].Add($"Agent: {agentOutput}");
        }
        else
        {
            Debug.LogError("Conversation ID not found.");
        }
    }

    public void SaveConversation(int conversationID)
    {
        if (conversations.ContainsKey(conversationID))
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, $"conversation_output_{conversationID}.txt");
            File.WriteAllLines(filePath, conversations[conversationID]);
            conversations.Remove(conversationID);
        }
        else
        {
            Debug.LogError("Conversation ID not found.");
        }
    }
}
