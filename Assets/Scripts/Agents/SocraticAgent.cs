using CommandTerminal;
using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace DialogosEngine
{
    public class SocraticAgent : Agent
    {
        ParallelStateMachine _PSM;
        Lexer _Lexer;
        Dictionary<IState, float> _Rewards = new Dictionary<IState, float>();
        int _BufferOffset = 0;

        public override void Initialize()
        {
            _PSM = new(this);
            _PSM.AddState("Observe", new ObserveState(), 1.0f, 0.1f);
            _PSM.AddState("Orient", new OrientState(), 1.0f, 0.1f);
            _PSM.AddState("Decide", new DecideState(), 1.0f, 0.1f);
            _PSM.AddState("Act", new ActState(), 1.0f, 0.1f);
            _PSM.AddState("Learn", new LearnState(), 1.0f, 0.1f);
            _Lexer = new();
        }

        public override void OnEpisodeBegin()
        {
            ResetEnvironment();
            _PSM.Reset();
        }

        private void ResetEnvironment()
        {
            Terminal.Instance.Buffer.Reset();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            string[] _buffer = Terminal.Instance.Buffer.ToArray(_BufferOffset, 1000);
            ObserveBuffer(_buffer, sensor);
        }

        private void ObserveBuffer(string[] buffer, VectorSensor sensor)
        {
            foreach (string line in buffer)
            {
                float[] vector = _Lexer.Transform(line);
                foreach (var f in vector)
                {
                    sensor.AddObservation(f);
                }
            }
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            _PSM.OnActionReceived(this, actionBuffers);
        }

        void FixedUpdate()
        {
            _PSM.FixedUpdate();
            ApplyCombinedReward();
        }

        public void StoreReward(IState state, float reward)
        {
            _Rewards[state] = reward;
        }

        public void ApplyCombinedReward()
        {
            float combinedReward = CalculateCombinedReward();
            SetReward(combinedReward);
        }

        public float CalculateCombinedReward()
        {
            float combinedReward = 0f;
            foreach (var reward in _Rewards.Values)
            {
                float sigmoidReward = 2f / (1f + Mathf.Exp(-reward)) - 1f;
                combinedReward += sigmoidReward;
            }
            combinedReward = Mathf.Clamp(combinedReward / _Rewards.Count, -1f, 1f);
            return combinedReward;
        }

        public void SetBufferOffset(int offsetAmount)
        {
            _BufferOffset = offsetAmount;
        }
    }
}
