using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToTarget : Agent
{
    [SerializeField] Transform target;
    [SerializeField] float moveSpeed = 5f;

    Rigidbody rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed;

        // Use physics instead of teleportation
        rb.AddForce(move, ForceMode.VelocityChange);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.position);
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;

        transform.position = new Vector3(0, 1.3f, 0);
        target.position = new Vector3(Random.Range(-4f, 4f), 1.3f, Random.Range(-4f, 4f));
    }

        public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            Debug.Log("Hit wall: -0.1");
            SetReward(-0.1f);
        }
        
        if (collision.collider.CompareTag("Runner"))
        {
            Debug.Log("Reached Runner: +1");
            SetReward(+1f);
            EndEpisode();
        }
    }
}
