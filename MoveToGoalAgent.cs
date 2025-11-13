using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToTarget : Agent
{
    [SerializeField] Transform target;
    [SerializeField] float moveSpeed = 0f;
    public override void OnActionReceived(ActionBuffers actions) {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    // How the agent observes the environment. The inputs
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.position);
    }

    public override void OnEpisodeBegin() {
        transform.position = new Vector3(Random.Range(1f, 4f), 1.3f, Random.Range(-4f, 4f));
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Runner")) {
            SetReward(+1f);
            EndEpisode();
        }

        if (other.CompareTag("Wall")) {
            SetReward(-.01f);
        }
    }
}
