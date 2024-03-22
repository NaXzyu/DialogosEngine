// WhitespaceTokenizer class that inherits from TokenAgent
using System.Collections.Generic;
using Unity.MLAgents.Actuators;

public class WhitespaceTokenizer : TokenAgent
{
    private List<(int start, int end)> whitespaceIndices;

    public override void Initialize()
    {
        base.Initialize();
        whitespaceIndices = new List<(int start, int end)>();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        whitespaceIndices.Clear();
    }

    private void DetectWhitespaces()
    {
        bool inWhitespace = false;
        int whitespaceStart = 0;

        for (int i = 0; i < textToTokenize.Length; i++)
        {
            if (char.IsWhiteSpace(textToTokenize[i]))
            {
                if (!inWhitespace)
                {
                    inWhitespace = true;
                    whitespaceStart = i;
                }
            }
            else if (inWhitespace)
            {
                inWhitespace = false;
                whitespaceIndices.Add((whitespaceStart, i - 1));
            }
        }

        if (inWhitespace)
        {
            whitespaceIndices.Add((whitespaceStart, textToTokenize.Length - 1));
        }
    }

    private void ExtractTokens()
    {
        int tokenStart = 0;

        foreach (var (start, end) in whitespaceIndices)
        {
            if (tokenStart < start)
            {
                tokens.Add(textToTokenize.Substring(tokenStart, start - tokenStart));
            }
            tokenStart = end + 1;
        }

        if (tokenStart < textToTokenize.Length)
        {
            tokens.Add(textToTokenize.Substring(tokenStart));
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        base.OnActionReceived(actionBuffers);

        DetectWhitespaces();
        ExtractTokens();

        // Reward the agent if the tokenization is done correctly
        AddReward(1.0f);

        EndEpisode();
    }
}
