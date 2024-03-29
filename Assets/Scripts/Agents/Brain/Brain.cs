using System.Collections.Generic;
using Unity.MLAgents;

namespace DialogosEngine
{
    public class Brain : IBrain
    {
        public Agent Agent { get; set; }
        public string Name { get; set; }
        private Dictionary<string, BrainComponent> _Components;

        public Brain(Agent agent)
        {
            Agent = agent;
            _Components = new Dictionary<string, BrainComponent>
            {
                { "VTA", new BrainVentralTegmentalArea() },
                { "Hippocampus", new BrainHippocampus() }
            };
        }

        public T GetComponent<T>(string key) where T : BrainComponent
        {
            if (_Components.TryGetValue(key, out BrainComponent component))
            {
                return component as T;
            }
            return null;
        }

        public void UpdateComponents()
        {
            foreach (var component in _Components.Values)
            {
                component.UpdateComponent();
            }
        }
    }
}
