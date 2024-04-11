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
            var obs = Random.value;
            sensor.AddObservation(obs);
            Logger.Log($"[{StepCount}] CollectObservations: {obs}");
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {

            var i = -1;
            var continuousActions = actionBuffers.ContinuousActions;
            var output = continuousActions[++i];
            Logger.Log($"[{StepCount}] OnActionReceived: {output}");
        }

        void FixedUpdate()
        {
            if(!_IsInitialized)
            {
                return;
            }

            var reward = Random.value;
            AddReward(reward);
            Logger.Log($"[{StepCount}] FixedUpdate.reward: {reward}");
        }
    }
}