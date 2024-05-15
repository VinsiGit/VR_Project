using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;

public class HockeyAgent : Agent
{
    public Transform Puck;
    public Transform OptimalPosition;
    public Rigidbody agentRb;
    public Rigidbody puckRb; // Reference to the Rigidbody component of the puck
    public float rotationSpeed = 100f;
    public float moveSpeed = 5f;

    private Vector3 lastAgentPosition; // Store the agent's last position for collision detection

    // Called when the Agent starts
    public override void Initialize()
    {
        
        agentRb = GetComponent<Rigidbody>();
        puckRb = Puck.GetComponent<Rigidbody>(); // Get the Rigidbody component from the puck
        lastAgentPosition = agentRb.position;
    }
    
    public override void OnEpisodeBegin()
    {
        // Reset puck's position
        Puck.localPosition = new Vector3(Random.value * 2, 0.7f, Random.value * 7 - 3.5f);

        // Random rotation between 50 and 60 degrees
        float randomRotationAngle = Random.Range(50f, 60f);
        Quaternion randomRotation = Quaternion.Euler(0f, randomRotationAngle, 0f);

        // Apply random rotation to the puck
        Puck.localRotation = randomRotation;

        // Define the magnitude factor for the force
        float forceMagnitude = 5f; // You can adjust this value as needed

        // Generate a random direction vector
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        // Apply the magnitude factor to the direction vector
        Vector3 randomForce = randomDirection * forceMagnitude;

        // Apply the force to the puck
        puckRb.AddForce(randomForce, ForceMode.Impulse);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(agentRb.position);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var continuousActions = actionBuffers.ContinuousActions;

        // Move sideways
        float moveActionSW = continuousActions[1]; // Assuming sideways movement is at index 0
        Vector3 moveDirectionSW = transform.right * moveActionSW * moveSpeed * Time.deltaTime;
        agentRb.MovePosition(agentRb.position + moveDirectionSW);

        // Move forward or backward
        float moveActionFW = continuousActions[0]; // Assuming forward/backward movement is at index 1
        Vector3 moveDirectionFW = transform.forward * -moveActionFW * moveSpeed * Time.deltaTime;
        agentRb.MovePosition(agentRb.position + moveDirectionFW);

        // Add any reward logic here based on the actions
        float distanceToTarget = Vector3.Distance(agentRb.position, OptimalPosition.position);

        //rewards


    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering collider has the tag "goal"
        if (other.CompareTag("Goal_Agent"))
        {
            AddReward(-1f);
            EndEpisode();
        }
        if (other.CompareTag("Goal_Player"))
        {
            AddReward(1f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Puck"))
        {
            AddReward(0.1f);
            // Calculate the movement vector of the parent object
            Vector3 agentMovement = agentRb.position - lastAgentPosition;
            lastAgentPosition = agentRb.position;

            // Add the movement vector of the parent object to the movement vector of the puck
            Rigidbody puckRb = collision.gameObject.GetComponent<Rigidbody>();
            if (puckRb != null)
            {
                puckRb.velocity += agentMovement;
            }
        }

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        // Control rotation and movement using keyboard inputs for testing
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    
}
