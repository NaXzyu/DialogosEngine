using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace DialogosEngine
{
    public class ParallelStateMachine
    {
        private Dictionary<string, StateInfo> _States = new Dictionary<string, StateInfo>();
        private SocraticAgent _Agent;

        public ParallelStateMachine(SocraticAgent agent)
        {
            _Agent = agent;
        }

        public void FixedUpdate()
        {
            foreach (var _state in _States)
            {
                StateInfo _info = _state.Value;
                UpdateCooldown(_info);
                UpdateActionPotential(_info);
                ActivateState(_info);
            }
        }

        public void OnActionReceived(SocraticAgent agent, ActionBuffers actionBuffers)
        {
            foreach (var state in _States.Values)
            {
                if (state.IsActive)
                {
                    state.State.OnActionReceived(agent, actionBuffers);
                }
            }
        }

        private void UpdateCooldown(StateInfo info)
        {
            if (info.IsActive && info.CooldownTimer > 0)
            {
                info.CooldownTimer -= Time.deltaTime;
                if (info.CooldownTimer <= 0)
                {
                    info.IsActive = false;
                }
            }
        }

        private void UpdateActionPotential(StateInfo info)
        {
            if (info.ActionPotential > 0)
            {
                info.ActionPotential -= info.DiminishingReturns * Time.deltaTime;
                if (info.ActionPotential < info.ActivationThreshold)
                {
                    info.ActionPotential = 0;
                }
            }
        }

        private void ActivateState(StateInfo info)
        {
            if (info.IsActive)
            {
                info.State.FixedUpdate(_Agent);
            }
        }

        public void AddState(string name, IState state, float threshold, float diminishingReturn)
        {
            _States[name] = new StateInfo
            {
                State = state,
                ActivationThreshold = threshold,
                DiminishingReturns = diminishingReturn
            };
        }

        public void ReceiveActionPotential(string name, float potential)
        {
            if (_States.TryGetValue(name, out StateInfo _info))
            {
                _info.ActionPotential += potential;
                if (_info.ActionPotential >= _info.ActivationThreshold)
                {
                    _info.IsActive = true;
                    _info.CooldownTimer = _info.CooldownDuration;
                }
            }
        }

        public void Reset()
        {
            foreach (var state in _States.Values)
            {
                state.Reset();
            }
        }
    }
}
