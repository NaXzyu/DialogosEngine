using Unity.MLAgents.Actuators;
using UnityEngine;

namespace DialogosEngine
{
    public class ObserveState : IState
    {
        private float _rewardValue;
        private int _FloatToIntRange = 1000;

        public void FixedUpdate(SocraticAgent agent)
        {
            _rewardValue = CalculateReward(agent);
            agent.StoreReward(this, _rewardValue);
        }

        public void OnActionReceived(SocraticAgent agent, ActionBuffers actionBuffer)
        {
            float startOffset = actionBuffer.ContinuousActions[0] * _FloatToIntRange;
            int bufferOffset = (int)(startOffset);
            agent.SetBufferOffset(bufferOffset);
        }

        public float CalculateReward(SocraticAgent agent)
        {
            float reward = 0f;

            // Pseudocode for reward calculation:
            // 1. Reward the agent for correctly identifying an object of interest
            // if (agent.IdentifiesTarget()) { reward += positiveValue; }

            // 2. Punish the agent for incorrectly identifying an object or missing it
            // if (agent.MissesTarget() || agent.IncorrectlyIdentifiesTarget()) { reward -= negativeValue; }

            // 3. Reward the agent for efficiency (e.g., time taken to observe)
            // reward += CalculateTimeBonus(agent.TimeToObserve());

            // 4. Punish the agent for taking actions that lead to negative outcomes
            // if (agent.TakesNegativeAction()) { reward -= negativeActionPenalty; }

            // 5. Adjust the reward based on the agent's performance metrics
            // reward += PerformanceMetrics(agent);

            // Ensure the reward is within the range of -1 to 1
            return Mathf.Clamp(reward, -1f, 1f);
        }

    }
}
