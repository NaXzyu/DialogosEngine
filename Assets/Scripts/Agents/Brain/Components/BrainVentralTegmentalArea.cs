using UnityEngine;

namespace DialogosEngine
{
    public class BrainVentralTegmentalArea : BrainComponent
    {
        private float _DopamineLevel;
        private float _DecayRate = 0.1f;

        public BrainVentralTegmentalArea() : base()
        {
            Name = "VTA";
        }

        public float DopamineLevel
        {
            get { return _DopamineLevel; }
            set { _DopamineLevel = Mathf.Clamp(value, 0, 1); }
        }

        public override void Initialize()
        {
            // ...
        }

        public void ReleaseDopamine(float rewardSignal)
        {
            // Simulate dopamine release based on the reward signal
            DopamineLevel += rewardSignal;
        }

        public override void UpdateComponent()
        {
            // Simulate the natural decay of dopamine over time
            DopamineLevel -= _DecayRate * Time.deltaTime;
            DopamineLevel = Mathf.Clamp(DopamineLevel, 0, 1); 
        }
    }
}