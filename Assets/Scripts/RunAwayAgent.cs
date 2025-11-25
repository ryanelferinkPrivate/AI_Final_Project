using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RunAwayAgent : Agent
{
    [SerializeField] FreezeTagManager manager;

    [SerializeField] float moveSpeed = 5f;

    [SerializeField] Material frozenMat;
    [SerializeField] Material runnerMat;

    Rigidbody rb;
    bool frozen = false;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("RB assigned? " + (rb != null));
    }


    public override void OnEpisodeBegin()
    {
        frozen = false;
        var rend = GetComponent<Renderer>();
        if (rend != null && runnerMat != null)
        {
            rend.material = runnerMat;
        }

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(manager.tagger.transform.position);

        foreach (var r in manager.runners)
        {
            sensor.AddObservation(r.IsFrozen() ? 1f : 0f);
            sensor.AddObservation(r.transform.position);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!frozen)
        {
            float moveX = actions.ContinuousActions[0];
            float moveZ = actions.ContinuousActions[1];

            Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed;
            rb.AddForce(move, ForceMode.VelocityChange);

            AddReward(0.005f);

            Debug.Log("Actions: " + moveX + ", " + moveZ);
        }
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
            AddReward(-0.1f);
        }

        if (collision.collider.CompareTag("Tagger"))
        {
            if (!frozen) {
                Debug.Log("Got tagged: -1");
                AddReward(-1f);
                this.manager.FreezeRunner(this);
                return;
                
            }
        }

        if (collision.collider.CompareTag("Runner"))
        {
            RunAwayAgent runner = collision.collider.GetComponent<RunAwayAgent>();
            if (runner != null && runner.IsFrozen() && !frozen)
            {
                Debug.Log("Hit frozen runner");
                AddReward(+0.5f);
                manager.UnfreezeRunner(runner);
            }
        }
    }

    public bool IsFrozen()
    {
        return this.frozen;
    }

    public void Freeze()
    {
        this.frozen = true;
        Debug.Log($"{name} frozen flag set to TRUE");
        rb.linearVelocity = Vector3.zero;
        var rend = GetComponent<Renderer>();
        if (frozenMat != null && rend != null)
        {
            rend.material = frozenMat;
        }
    }

    public void Unfreeze()
    {
        this.frozen = false;
        var rend = GetComponent<Renderer>();
        if (runnerMat != null && rend != null)
        {
            rend.material = runnerMat;
        }
    }
}
