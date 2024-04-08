using CommandTerminal;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace DialogosEngine
{
    public class EchoAgent : Agent
    {
        string _CachedString;

        public override void OnEpisodeBegin()
        {
            Terminal.Instance.Buffer.Reset();
        }

        public void FixedUpdate()
        {
            string expectedString = GetExpectedString();
            if (_CachedString != null)
            {
                float reward = AgentUtils.CalculateEchoReward(expectedString, _CachedString);

                if (_CachedString.EndsWith(AgentUtils.k_EndOfSequence))
                {
                    _CachedString = _CachedString.Replace(AgentUtils.k_EndOfSequence, "");
                    
                    Terminal.Instance.Shell.Run(_CachedString);
                }

                SetReward(reward);
                _CachedString = null;
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            string _buffer = Terminal.Instance.Buffer.GetLastLog();
            float[] _vectorizedBuffer = Lexer.VectorizeUTF8(_buffer);
            foreach (var obs in _vectorizedBuffer)
            {
                sensor.AddObservation(obs);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float[] _actionArray = actions.ContinuousActions.Array;
            _CachedString = Lexer.QuantizeUTF8(_actionArray);
        }

        private string GetExpectedString()
        {
            return "echo hello <eos>"; // Testing
        }
    }
}
