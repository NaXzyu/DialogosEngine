using Unity.MLAgents.Actuators;

namespace DialogosEngine
{
    public class ActState : IState
    {
        public void FixedUpdate(SocraticAgent agent)
        {
            // Implementation of FixedUpdate
        }

        float IState.CalculateReward(SocraticAgent agent)
        {
            // Implementation of CalculateReward
            return 0f;
        }

        void IState.FixedUpdate(SocraticAgent agent)
        {
            // Implementation of FixedUpdate
        }

        void IState.OnActionReceived(SocraticAgent agent, ActionBuffers actionBuffer)
        {
            // Implementation of OnActionReceived
        }
    }
}
