using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class WormSimpleAgent : Agent
{
    public override void Initialize()
    {
        //
    }

    public override void OnEpisodeBegin()
    {
        //
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Random.value);
    }

    public void TouchedTarget()
    {
        AddReward(1f);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        var i = -1;
        var continuousActions = actionBuffers.ContinuousActions;
        var output = continuousActions[++i];
        Debug.Log(output);
    }

    void FixedUpdate()
    {
        AddReward(Random.value);
    }
}
