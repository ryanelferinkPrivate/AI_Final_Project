using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

// controls tagger behavior in game
public class MoveToTarget : Agent
{
    // freezetag manager
    [SerializeField] FreezeTagManager manager;

    // fixed movement speed of 5
    [SerializeField] float moveSpeed = 5f;

    Rigidbody rb;

    // apply rigid body component when agent first created
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    // behavior for when tagger recieves actions
    public override void OnActionReceived(ActionBuffers actions)
    {
        // move in X or Z direciton 
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // determines move agent can make 
        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed;

        // Use physics instead of teleportation
        rb.AddForce(move, ForceMode.VelocityChange);
    }

    // tells agent what observations it sees in the game. 
    public override void CollectObservations(VectorSensor sensor)
    {
        // observe its own position and each runners position 
        sensor.AddObservation(transform.position);
        foreach (var r in manager.runners)
        {
            sensor.AddObservation(r.transform.position);

            // can also tell when runner is frozen or not 
            sensor.AddObservation(r.IsFrozen() ? 1f : 0f);
        }
    }

    // called at the start of each episode 
    public override void OnEpisodeBegin()
    {
        Debug.Log("Tagger begin");

        // tells manager to reset all agent psition and states
        manager.Reset();        
    }

    // reset tagger's state
    public void ResetAgentState()
    {
        Debug.Log($"{name} RESET STATE");

        //stop the agent from moving
        rb.linearVelocity = Vector3.zero;

        // place tagger at a random position on the board
        transform.position = new Vector3(Random.Range(-4f, 4f), 1.3f, Random.Range(-4f, 4f));
    }

    // behavior for user to control tagger
        public override void Heuristic(in ActionBuffers actionsOut)
    {
        // allows unity to take in user input to perform position changes on x and z axis
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    // handles when an agent collides with environment or other agents
    private void OnCollisionEnter(Collision collision)
    {
        Collider collided = collision.collider;

        // if agent hits the wall
        if (collided.CompareTag("Wall"))
        {
            // decrease reward by 0.1 to discourage this behavior
            Debug.Log("Hit wall: -0.1");
            AddReward(-0.1f);
        }
        
        // if it collides with either runner 
        if (collided.CompareTag("Runner") || collided.CompareTag("Runner2"))
        {
            // if runner is not frozen
            RunAwayAgent runner = collision.collider.GetComponent<RunAwayAgent>();
            if (runner != null && !runner.IsFrozen())
            {
                // give reward of 1 for tagging runner 
                Debug.Log("Tagged Runner: +1");
                AddReward(+1f);

                // Freeze runners through the game manager. This will also keep track of how many runners are frozen
                manager.FreezeRunner(runner);
                return;
            }
        }
    }
}
