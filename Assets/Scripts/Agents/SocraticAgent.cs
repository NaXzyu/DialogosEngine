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
        SocraticBrain _Brain;
        ParallelStateMachine _PSM;
        Dictionary<IState, float> _Rewards = new Dictionary<IState, float>();
        int _BufferOffset = 0;
        int _ObservationSize = 1000;

        public override void Initialize()
        {
            _Brain = new(this);
            _PSM = new(this);
            _PSM.AddState("Observe", new ObserveState(), 1.0f, 0.1f);
            _PSM.AddState("Orient", new OrientState(), 1.0f, 0.1f);
            _PSM.AddState("Decide", new DecideState(), 1.0f, 0.1f);
            _PSM.AddState("Act", new ActState(), 1.0f, 0.1f);
            _PSM.AddState("Learn", new LearnState(), 1.0f, 0.1f);
        }

        public override void OnEpisodeBegin()
        {
            Terminal.Instance.Buffer.Reset();
            _PSM.Reset();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            string[] _text = Terminal.Instance.Buffer.ToArray(ref _BufferOffset, ref _ObservationSize);
            int _size = 0;
            CollectTextObservations(_text, sensor, ref _size, ref _ObservationSize);
            CollectedContextObservations(sensor, _text, ref _size);
        }

        private bool ShouldAddObservation(ref int totalAdded, ref int maxObservations, ref int textIndex, int textLength)
        {
            return totalAdded < maxObservations && textIndex < textLength;
        }

        private void CollectTextObservations(string[] text, VectorSensor sensor, ref int size, ref int maxObservations)
        {
            int _totalAdded = 0;
            int _textIndex = 0;
            int _vectorIndex = 0;
            float[] _vector = null;

            while (ShouldAddObservation(ref _totalAdded, ref maxObservations, ref _textIndex, text.Length))
            {
                if (_vector == null || _vectorIndex >= _vector.Length)
                {
                    _vector = Lexer.VectorizeUTF8(text[_textIndex]);
                    size += _vector.Length;
                    _vectorIndex = 0;
                    _textIndex++;
                }

                // truncate for max vectors
                while (_vectorIndex < _vector.Length && _totalAdded < maxObservations)
                {
                    sensor.AddObservation(_vector[_vectorIndex]);
                    _vectorIndex++;
                    _totalAdded++;
                }
            }
        }

        private void CollectedContextObservations(VectorSensor sensor, string[] text, ref int size)
        {
            sensor.AddObservation(Lexer.CreateQuaternion(text, size));
            sensor.AddObservation(GetTimestamp());
        }

        private float GetTimestamp()
        {
            long _timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return (float)(_timestamp % 1e6) / 1e6f; // Normalize
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
            float _combinedReward = CalculateCombinedReward();
            SetReward(_combinedReward);
        }

        public float CalculateCombinedReward()
        {
            float _combinedReward = 0f;
            foreach (var _reward in _Rewards.Values)
            {
                float _sigmoidReward = 2f / (1f + Mathf.Exp(-_reward)) - 1f;
                _combinedReward += _sigmoidReward;
            }
            _combinedReward = Mathf.Clamp(_combinedReward / _Rewards.Count, -1f, 1f);
            return _combinedReward;
        }

        public void SetBufferOffset(int offsetAmount)
        {
            _BufferOffset = offsetAmount;
        }
    }
}
