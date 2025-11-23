using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RunAwayAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] float moveSpeed = 5f;

    Rigidbody rb;

    public override void Initialize()
{
    rb = GetComponent<Rigidbody>();
    Debug.Log("RB assigned? " + (rb != null));
}
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed;

        rb.AddForce(move, ForceMode.VelocityChange);

        AddReward(0.005f);

        Debug.Log("Actions: " + moveX + ", " + moveZ);
    }

    //Allows us to control the Tagger on our own
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal2");
        continuousActions[1] = Input.GetAxisRaw("Vertical2");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            Debug.Log("Hit wall: -0.1");
            SetReward(-0.1f);
        }
        
        if (collision.collider.CompareTag("Tagger"))
        {
            Debug.Log("Got tagged: -1");
            SetReward(-1f);
            EndEpisode();
        }
    }
}
