using Unity.MLAgents;

namespace DialogosEngine
{
    public interface IBrain
    {
        Agent Agent { get; set; }
        string Name { get; set; }
    }
}