using System;
using UnityEngine;

namespace DialogosEngine
{
    public static class AgentUtils
    {
        public const string k_EndOfSequence = "<eos>";
        public const string k_AgentLogFile = "EchoAgentLog.txt";

        public static float CalculateEchoReward(string expectedString, string guessedString)
        {
            // Validate input strings
            if (string.IsNullOrEmpty(expectedString) || string.IsNullOrEmpty(guessedString))
            {
                throw new ArgumentException("Expected and guessed strings must not be null or empty.");
            }
            if (!expectedString.EndsWith(k_EndOfSequence))
            {
                throw new ArgumentException("Expected string must end with '<eos>'.");
            }
            if (expectedString == guessedString)
            {
                return 1f;
            }

            // Calculate Levenshtein distance
            int levenshteinDistance = Lexer.LevenshteinDistance(expectedString, guessedString);
            float maxStringLength = Mathf.Max(expectedString.Length, guessedString.Length);
            float similarityScore = 1f - (float)levenshteinDistance / maxStringLength;
            similarityScore = (similarityScore * 2f) - 1f; // Normalize to range [-1, 1]

            // Calculate length match score
            float lengthDifference = Mathf.Abs(expectedString.Length - guessedString.Length);
            float lengthMatchScore = 1f - Mathf.Min(2f * lengthDifference / maxStringLength, 1f);
            lengthMatchScore = (lengthMatchScore * 2f) - 1f; // Normalize to range [-1, 1]

            // Combine similarity and length match scores
            float combinedScore = (0.5f * similarityScore) + (0.5f * lengthMatchScore);

            // Ensure the final score is within the range [-1, 1]
            return Mathf.Clamp(combinedScore, -1f, 1f);
        }

        public static void ProcessActionArray(ref float[] actionArray, int outputLength)
        {
            if (actionArray == null)
            {
                throw new ArgumentNullException(nameof(actionArray), "The action array cannot be null.");
            }

            // Shift the elements in the array to the left by one position to exclude the length control
            for (int i = 1; i < actionArray.Length; i++)
            {
                actionArray[i - 1] = actionArray[i];
            }

            Array.Resize(ref actionArray, outputLength);
        }
    }
}
