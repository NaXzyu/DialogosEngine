using CommandTerminal;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace DialogosEngine
{
    public class EchoAgent : Agent
    {
        string _CachedString;
        string _ExpectedString = "echo hello <eos>";

        public override void OnEpisodeBegin()
        {
            Terminal.Instance.Buffer.Reset();
        }

        public void FixedUpdate()
        {
            if (_CachedString != null)
            {
                float reward = AgentUtils.CalculateEchoReward(_ExpectedString, _CachedString);

                // Debug
                Terminal.Instance.LogToFile($"[{StepCount}]{reward} | {_CachedString} | {_ExpectedString}");

                if (_CachedString.EndsWith(AgentUtils.k_EndOfSequence))
                {
                    _CachedString = _CachedString.Replace(AgentUtils.k_EndOfSequence, "");
                    Terminal.Instance.Shell.Run(_CachedString);
                }

                if (_CachedString == _ExpectedString)
                {
                    _CachedString = null;
                    SetReward(1f);
                    EndEpisode();
                }
                else
                {
                    _CachedString = null;
                    SetReward(reward);
                }
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            string _buffer = Terminal.Instance.Buffer.GetLastLog();
            float[] _vectorizedBuffer = Lexer.VectorizeUTF8(_buffer);
            int maxObservations = Mathf.Min(_vectorizedBuffer.Length, Lexer.k_MaxBufferLength);

            for (int i = 0; i < maxObservations; i++)
            {
                sensor.AddObservation(_vectorizedBuffer[i]);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float[] _actionArray = actions.ContinuousActions.Array;
            float _lengthControlValue = _actionArray[0];

            int outputLength = Transformer.RoundMax(ref _lengthControlValue);
            AgentUtils.ProcessActionArray(ref _actionArray, outputLength);

            _CachedString = Lexer.QuantizeUTF8(_actionArray);
        }
    }
}
