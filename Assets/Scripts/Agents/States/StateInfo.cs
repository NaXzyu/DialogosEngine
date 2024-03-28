using System;

namespace DialogosEngine
{
    public struct StateInfo
    {
        public IState State { get; set; }
        public float ActionPotential { get; set; }
        public float ActivationThreshold { get; set; }
        public float DiminishingReturns { get; set; }
        public bool IsActive { get; set; }
        public float CooldownTimer { get; set; }
        public float CooldownDuration { get; set; }

        public void Reset()
        {
            State = null;
            ActionPotential = 0f;
            ActivationThreshold = 1f;
            DiminishingReturns = 0.1f;
            IsActive = false;
            CooldownTimer = 0f;
            CooldownDuration = 0f;
        }
    }
}