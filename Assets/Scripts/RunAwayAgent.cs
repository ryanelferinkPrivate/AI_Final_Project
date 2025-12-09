using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

// controls behavior for runner
public class RunAwayAgent : Agent
{
    // freezetag manager
    [SerializeField] FreezeTagManager manager;

    // fized move speed of 5
    [SerializeField] float moveSpeed = 5f;

    // frozen material when frozen
    [SerializeField] Material frozenMat;
    // base material for runner
    public Material runnerMat;

    Rigidbody rb;
    // deterimines if frozen
    public bool frozen { get; private set; } = false;
    bool immune;
    Renderer rend;

    //apply rigid body component when agent first created
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponentInChildren<Renderer>();
        Debug.Log($"{name} renderer is on object: {rend.gameObject.name}");

    }

    // called at the start of every episode and instantiates parameters for runner
    public override void OnEpisodeBegin()
    {
        // start coroutine that makes tagger temporarily immune 
        StartCoroutine(EnableTaggingAfterDelay());
        Debug.Log("Runner begin");

        // at start runner is not frozen
        frozen = false;

        // set the runner's material to the base material
        if (rend && runnerMat)
        {
            rend.material = runnerMat;
        }

    }

    // coroutine that briefly makes the runner immune to being tagged
    // prevents a tagger from instantly tagging a runner
    IEnumerator EnableTaggingAfterDelay()
    {
        // makes it immune 
        immune = true;

        // wait 0.2 seconds 
        yield return new WaitForSeconds(0.2f);

        // then turns off immunity 
        immune = false;
    }

    // tells agent what observations it sees in the game
    public override void CollectObservations(VectorSensor sensor)
    {
        // observe its own position and tagger position 
        sensor.AddObservation(transform.position);
        sensor.AddObservation(manager.tagger.transform.position);

        // observe if this runner is frozen or not
        sensor.AddObservation(this.frozen ? 1f : 0f);

        // for each runner 
        foreach (var r in manager.runners)
        {
            // if at current runner go to next
            if (r == this)
            {
                continue;
            }

            // add observation of the other runners and if they are frozen or not 
            sensor.AddObservation(r.IsFrozen() ? 1f : 0f);
            sensor.AddObservation(r.transform.position);
        }
    }
    
    // behavior for when runner recieves actions
    public override void OnActionReceived(ActionBuffers actions)
    {
        // if it is not frozen
        if (!frozen)
        {
            // move in X or Z direciton 
            float moveX = actions.ContinuousActions[0];
            float moveZ = actions.ContinuousActions[1];
            
            // determines move agent can make 
            Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed;
            rb.AddForce(move, ForceMode.VelocityChange);

            // add 0.005 at each step for outlasting tagger 
            AddReward(0.005f);
        }
    }

    // reset runner's state
    public void ResetAgentState()
    {
        Debug.Log($"{name} RESET STATE");

        // set frozen to false
        frozen = false;

        // stop agent from moving
        rb.linearVelocity = Vector3.zero;

        // respawn runner at random position
        transform.position = new Vector3(Random.Range(-4f, 4f), 1.3f, Random.Range(-4f, 4f));

        // set material to base runner material
        if (rend && runnerMat)
        {
            rend.material = runnerMat;
        }

    }

    // allows user to control the Tagger on our own
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
        // if collides with wall 
        if (collision.collider.CompareTag("Wall"))
        {
            // decrease reward by 0.1 to discourage this behavior
            Debug.Log("Hit wall: -0.1");
            AddReward(-0.1f);
        }

        // if collides with tagger
        if (collision.collider.CompareTag("Tagger"))
        {
            // if it is already not frozen and not immune 
            if (!frozen && !immune) {

                // decrease reward by 1 for getting tagged 
                Debug.Log("Got tagged: -1");
                AddReward(-1f);
                manager.FreezeRunner(this);
                return;
                
            }
        }

        // if it collides with another runner 
        if (collision.collider.CompareTag("Runner"))
        {
            RunAwayAgent runner = collision.collider.GetComponent<RunAwayAgent>();

            // if runner it hits is frozen 
            if (runner != null && runner.IsFrozen() && !frozen)
            {
                // add reward of 0.5 for un-freezing a runner
                Debug.Log("Hit frozen runner");
                AddReward(+0.5f);

                // unfreeze the runner that was tagged by this runner
                manager.UnfreezeRunner(runner);
            }
        }
    }

    // determines if unner is frozen
    public bool IsFrozen()
    {
        return this.frozen;
    }

    // called by the manager when this runner should be frozen
    public void Freeze()
    {
        // if immune it ignores 
        if (immune)
        {
            return;
        }

        // otherwise it freezes 
        this.frozen = true;
        Debug.Log($"{name} frozen flag set to TRUE");

        // makes it so runner is truly frozen and cannot move
        rb.linearVelocity = Vector3.zero;

        // changes material to frozen runner material
        if (frozenMat != null && rend != null)
        {
            rend.material = frozenMat;
        }
    }

    // called by the manager when this runner should be unfroze
    public void Unfreeze()
    {
        // sets frozen to false
        this.frozen = false;

        // resets material to base runner material
        if (runnerMat != null && rend != null)
        {
            rend.material = runnerMat;
        }

    }
}
