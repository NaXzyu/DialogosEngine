using CommandTerminal;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    private static SessionManager _instance;
    public static SessionManager Instance { get { return _instance; } }

    private Dictionary<int, TerminalSession> sessions;
    private int sessionIDCounter = 0;
    private int currentSessionID;
    private Terminal _terminal;

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
            sessions = new Dictionary<int, TerminalSession>();
            currentSessionID = -1;
        }
    }

    public Terminal Bind(Terminal terminal)
    {
        return _terminal = terminal;
    }

    public int CreateNewSession()
    {
        int newID = sessionIDCounter++;
        sessions[newID] = new TerminalSession(newID);
        return newID;
    }

    public TerminalSession GetSession(int sessionID)
    {
        if (sessions.ContainsKey(sessionID))
        {
            return sessions[sessionID];
        }
        else
        {
            _terminal.LogError("[SESSION] Session ID not found.");
            return null;
        }
    }

    public void CloseSession(int sessionID)
    {
        if (sessions.ContainsKey(sessionID))
        {
            sessions.Remove(sessionID);
        }
        else
        {
            _terminal.LogError("[SESSION] Session ID not found.");
        }
    }

    public void LogCommandExecution(string commandName)
    {
        var currentSession = GetCurrentSession();
        if (currentSession != null)
        {
            currentSession.CommandHistory.Add(commandName);
            _terminal.Log($"[SESSION] Command '{commandName}' executed.");
        }
    }

    public TerminalSession GetCurrentSession()
    {
        return sessions.TryGetValue(currentSessionID, out TerminalSession currentSession) ? currentSession : null;
    }

    public void SaveSession(int sessionID)
    {
        if (sessions.TryGetValue(sessionID, out TerminalSession session))
        {
            string path = Path.Combine(Application.streamingAssetsPath, $"session_{sessionID}.txt");
            File.WriteAllText(path, session.Serialize());
        }
    }

    public void LoadSession(int sessionID)
    {
        string path = Path.Combine(Application.streamingAssetsPath, $"session_{sessionID}.txt");
        if (File.Exists(path))
        {
            string sessionData = File.ReadAllText(path);
            sessions[sessionID] = TerminalSession.Deserialize(sessionData);
        }
    }

    public void SwitchSession(int sessionID)
    {
        if (sessions.ContainsKey(sessionID))
        {
            currentSessionID = sessionID;
        }
    }

    public void ToggleSessionPause(int sessionID)
    {
        if (sessions.TryGetValue(sessionID, out TerminalSession session))
        {
            session.IsPaused = !session.IsPaused;
        }
    }
}
