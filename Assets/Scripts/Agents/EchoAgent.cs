using CommandTerminal;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace DialogosEngine
{
    public class EchoAgent : Agent
    {
        string _CachedGuessedString;
        const string k_EndOfSequence = "<eos>";

        public override void OnEpisodeBegin()
        {
            ClearConsole();
        }

        public void FixedUpdate()
        {
            string expectedString = GetExpectedString();
            if (_CachedGuessedString != null)
            {
                float _reward;
                if (_CachedGuessedString.EndsWith(k_EndOfSequence))
                {
                    _reward = CalculateReward(expectedString, _CachedGuessedString);
                    _CachedGuessedString = _CachedGuessedString.Replace(k_EndOfSequence, "");
                    Terminal.Instance.Shell.Run(_CachedGuessedString);
                    if(Terminal.Instance.IssuedError)
                    {
                        _reward -= 0.5f; // Penalize for bad commands
                    }
                }
                else
                {
                    _reward = -1f;
                }
                SetReward(_reward);
                _CachedGuessedString = null;
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            string _buffer = GetConsoleBuffer();
            float[] _vectorizedBuffer = Lexer.VectorizeUTF8(_buffer);
            foreach (var obs in _vectorizedBuffer)
            {
                sensor.AddObservation(obs);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float[] _actionArray = actions.ContinuousActions.Array;
            _CachedGuessedString = Lexer.QuantizeUTF8(_actionArray);
        }

        private void ClearConsole()
        {
            Terminal.Instance.Buffer.Reset();
        }

        private float CalculateReward(string expectedString, string guessedString)
        {
            int levenshteinDistance = Lexer.LevenshteinDistance(expectedString, guessedString);
            float maxStringLength = Mathf.Max(expectedString.Length, guessedString.Length);
            float similarityScore = 1f - (float)levenshteinDistance / maxStringLength;
            similarityScore = (similarityScore * 2f) - 1f; // Normalize to range [-1, 1]

            float lengthDifference = Mathf.Abs(expectedString.Length - guessedString.Length);
            float lengthMatchScore = 1f - Mathf.Min(2f * lengthDifference / maxStringLength, 1f);
            lengthMatchScore = (lengthMatchScore * 2f) - 1f; // Normalize to range [-1, 1]

            float combinedScore = (0.5f * similarityScore) + (0.5f * lengthMatchScore);
            return Mathf.Clamp(combinedScore, -1f, 1f); // Ensure final score is within [-1, 1]
        }


        private string GetConsoleBuffer()
        {
            return Terminal.Instance.Buffer.GetLastLog();
        }

        private string GetExpectedString()
        {
            // Implementation to get the expected string for the current step
            return "";
        }
    }
}
