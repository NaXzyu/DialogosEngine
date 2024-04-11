using CommandTerminal;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace DialogosEngine
{
    public class WormSimpleAgent : Agent
    {
        CommandLogger Logger;
        bool _IsInitialized = false;
        string _ExpectedString = "echo hello <eos>";
        string _CachedString;

        public override void Initialize()
        {
            Logger = new CommandLogger("WormSimpleAgent_log.txt", 1000);
            Logger.Log($"[{StepCount}] Initialize");
        }

        public override void OnEpisodeBegin()
        {
            Logger.Log($"[{StepCount}] OnEpisodeBegin");
            _IsInitialized = true;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            string simulatedTerminalOutput = "echo hello <eos>";
            float[] vectorizedOutput = Lexer.VectorizeUTF8(simulatedTerminalOutput);

            System.Array.Resize(ref vectorizedOutput, Lexer.k_MaxBufferLength);

            // Debug
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < Lexer.k_MaxBufferLength; i++)
            {
                float observationValue = i < vectorizedOutput.Length ? vectorizedOutput[i] : 0f;
                sensor.AddObservation(observationValue);

                // Debug
                //sb.Append(observationValue);
                //sb.Append(i < Lexer.k_MaxBufferLength - 1 ? ", " : "");
            }

            // Debug
            //Logger.Log($"[{StepCount}] CollectObservations: " + sb.ToString());
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float[] _actionArray = actions.ContinuousActions.Array;
            float _lengthControlValue = _actionArray[0];

            Logger.Log($"[{StepCount}] Length Control Value: {_lengthControlValue}");

            int outputLength = Transformer.RoundMax(ref _lengthControlValue);

            Logger.Log($"[{StepCount}] Rounded Output Length: {outputLength}");
            
            for (int i = 1; i < _actionArray.Length; i++)
            {
                _actionArray[i] = Transformer.Transform(ref _actionArray[i]);
            }

            AgentUtils.ProcessActionArray(ref _actionArray, outputLength);

            //Logger.Log($"[{StepCount}] Processed Action Array: {string.Join(", ", _actionArray)}");

            _CachedString = Lexer.QuantizeUTF8(_actionArray);

            Logger.Log($"[{StepCount}] Quantized String: {_CachedString}");
        }

        void FixedUpdate()
        {
            if (!_IsInitialized)
            {
                return;
            }

            var reward = Random.value;
            AddReward(reward);
            Logger.Log($"[{StepCount}] FixedUpdate.reward: {reward}");
        }
    }
}