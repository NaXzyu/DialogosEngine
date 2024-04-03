using CommandTerminal;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace DialogosEngine
{
    public class EchoAgent : Agent
    {
        char _GuessedChar;

        public override void OnEpisodeBegin()
        {
            ClearConsole();

            //
        }

        public void FixedUpdate()
        {
            char expectedChar = GetExpectedChar();
            float reward = CalculateReward(expectedChar, _GuessedChar);
            SetReward(reward);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            string buffer = GetConsoleBuffer();
            float[] vectorizedBuffer = Lexer.VectorizeUTF8(buffer);
            foreach (var obs in vectorizedBuffer)
            {
                sensor.AddObservation(obs);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float[] actionArray = new float[1] { actions.ContinuousActions[0] };
            _GuessedChar = Lexer.QuantizeUTF8(actionArray)[0];
            HandleGuessedCharacter(_GuessedChar);
        }

        private void ClearConsole()
        {
            Terminal.Instance.Buffer.Reset();
        }

        private float CalculateReward(char expectedChar, char guessedChar)
        {
            // Implementation to calculate the reward based on the guessed character
            return 0;
        }

        private string GetConsoleBuffer()
        {
            return Terminal.Instance.Buffer.GetLastLog();
        }

        private void HandleGuessedCharacter(char guessedChar)
        {
            // Implementation to handle the guessed character
        }

        private char GetExpectedChar()
        {
            // Implementation to get the expected character for the current step
            return new char();
        }
    }
}
