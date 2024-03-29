using Unity.MLAgents;

namespace DialogosEngine
{
    public class SocraticBrain : Brain
    {
        public SocraticBrain(Agent agent) : base(agent)
        {
            Name = "Alpha";
        }
    }
}