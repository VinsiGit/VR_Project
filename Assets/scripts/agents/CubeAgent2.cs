using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CubeAgent2 : Agent
{
    public float speedMultiplier = 0.5f;
    public float jumpForce = 2f;
    public float groundCheckDistance = 0.1f;
    private bool isGrounded = true;
    private bool hasJumped = false; // New flag to track jump action
    public Vector3 jump;
    Rigidbody rb;
    public override void OnEpisodeBegin()
    {
        jump = new Vector3(0.0f, 2.0f, 0.0f);
        rb = GetComponent<Rigidbody>();
        if (!isGrounded)
        {
            transform.localPosition = new Vector3(-0.15f, 0.5f, 3.33f);
            transform.localRotation = Quaternion.identity;
        }
        isGrounded = true;
    }

    private void OnCollisionStay()
    {
        isGrounded = true;
    }

    private void OnCollisionExit()
    {
        isGrounded = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.y = actions.ContinuousActions[0];

        if (controlSignal.y > 0 && isGrounded)
        {

            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;

            transform.Translate(controlSignal * speedMultiplier * Time.deltaTime);
            AddReward(-0.01f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
    }

    /* private void OnTriggerEnter(Collider other)
     {
         if (other.CompareTag("Enemy"))
         {
             touchingBar = true;
         }
     }
    */
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Agent raakt de enemy!!!!");
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    // Reset the hasJumped flag when the GameObject touches the ground
    
}