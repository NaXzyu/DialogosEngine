using Unity.MLAgents.Actuators;

namespace DialogosEngine
{
    public interface IState
    {
        void FixedUpdate(SocraticAgent agent);
        void OnActionReceived(SocraticAgent agent, ActionBuffers actionBuffer);
        float CalculateReward(SocraticAgent agent);
    }
}
