using System.Collections.Generic;
using Newtonsoft.Json;

public class TerminalSession
{
    public int SessionID { get; private set; }
    public Dictionary<string, string> EnvironmentVariables { get; private set; }
    public List<string> CommandHistory { get; private set; }
    public bool IsPaused { get; set; }

    public TerminalSession(int id)
    {
        SessionID = id;
        EnvironmentVariables = new Dictionary<string, string>();
        CommandHistory = new List<string>();
        IsPaused = false;
    }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static TerminalSession Deserialize(string sessionData)
    {
        return JsonConvert.DeserializeObject<TerminalSession>(sessionData);
    }
}
