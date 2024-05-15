using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;
using Random = UnityEngine.Random;
using System;

public class CubeAgentRays : Agent
{
    public Transform Target;
    public Collider Target2;

    public override void OnEpisodeBegin()
    {


        hasTouchedTarget1 = false;
        hasTouchedTarget2 = false;
        this.transform.localPosition = new Vector3(0, 0.5f, 0); 
        this.transform.localRotation = Quaternion.identity;
        Target.localPosition = new Vector3(Random.value * 6 - 4, 0.5f, Random.value * 6 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);

    }

    public float speedMultiplier = 0.2f;
    public float rotationMultiplier = 10;
    public bool hasTouchedTarget1 = false;
    public bool hasTouchedTarget2 = false;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;


        controlSignal.z = actionBuffers.ContinuousActions[0];
        transform.Translate(controlSignal * speedMultiplier);

        transform.Rotate(0.0f, rotationMultiplier * actionBuffers.ContinuousActions[1], 0.0f);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        if (distanceToTarget < 1.42f && !hasTouchedTarget1)
        {
            AddReward(0.15f);
            hasTouchedTarget1 = true;
            Target.localPosition = new Vector3(-99, -99, -99);
        }
        if (Target2.bounds.Contains(transform.position))
        {
            AddReward(0.1f);
            if (hasTouchedTarget1) { AddReward(0.85f); };
            hasTouchedTarget1 = false;
            EndEpisode();
        }

        if (this.transform.localPosition.y < 0)
        {
            AddReward(-0.1f);
            EndEpisode();
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
    }

}