using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

// controls default tagger that simply moves towards the runners and tags them
public class TaggerAgent : Agent
{
    // give a fixed speed of 4
    [Header("Movement")]
    [SerializeField] float moveSpeed = 4;
    [SerializeField] bool manualControl = false;

    // assign y position to prevent agent from floating
    [Header("Platform")]
    [SerializeField] BoxCollider playArea;
    [SerializeField] float spawnHeight = 1.3f;

    // list of runners in the game 
    [Header("Runners")]
    [SerializeField] List<Transform> runnerTransforms = new List<Transform>();

    // distance at which runner can get tagged by the tagger
    [Header("Tagging")]
    [SerializeField] float tagDistance = 0.8f;

    // reward for when tagging runner 
    [Header("Rewards")]
    [SerializeField] float tagReward = 1;

    // penalty for each step to encourage getting runner more effieciently 
    [SerializeField] float stepPenalty = 0.5f;

    // penalty for hitting a wall
    [SerializeField] float wallCollisionPenalty = 0.05f;

    // array to track which runners are frozen
    bool[] runnerFrozen;
    
    // when agent is first created it instantiates runnerTransforms and runnerFrozen
    public override void Initialize()
    {

        // find runner by tag: "Runner"
        if (runnerTransforms.Count == 0)
        {
            GameObject[] runners = GameObject.FindGameObjectsWithTag("Runner");
            for (int i = 0; i < runners.Length; i++)
            {
                runnerTransforms.Add(runners[i].transform);
            }
        }

        // add frozen runner to list for every runner in game and mark as false
        runnerFrozen = new bool[runnerTransforms.Count];
    }

    // called at the start of every episode and it resets everything to base states
    public override void OnEpisodeBegin()
    {
        // unfreeze all runners at start just in case
        for (int i = 0; i < runnerFrozen.Length; i++)
        {
            runnerFrozen[i] = false;
        }

        // reset positions for tagger and runners
        ResetTaggerPosition();
        ResetRunnerPositions();
    }

    // tells agent what observations it sees in the game
    public override void CollectObservations(VectorSensor sensor)
    {
        // observe its own position 
        Vector3 myPos = transform.position;
        sensor.AddObservation(myPos.x);
        sensor.AddObservation(myPos.z);

        // for each runner add its position and frozen state 
        for (int i = 0; i < runnerTransforms.Count; i++)
        {
            if (runnerTransforms[i] != null)
            {
                Vector3 rPos = runnerTransforms[i].position;
                sensor.AddObservation(rPos.x);
                sensor.AddObservation(rPos.z);
                sensor.AddObservation(runnerFrozen[i] ? 1f : 0f);
            }


            // otherwise add placeholders for missing runner
            else
            {
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
            }
        }
    }

    // behavior for when runner recieves actions
    public override void OnActionReceived(ActionBuffers actions)
    {
        // move in X or Z direciton 
        var cont = actions.ContinuousActions;
        float moveX = cont[0];
        float moveZ = cont[1];

        Vector3 direction = new Vector3(moveX, 0, moveZ);

        // if direction vector magnitude is not 0
        if (direction.sqrMagnitude > 0.0001f)
        {
            // move agent with updated position in this direction
            direction = direction.normalized;
            Vector3 displacement = direction * moveSpeed * Time.deltaTime;
            transform.position += displacement;
        }

        //after moving check if tagger is close enough to tag any runner
        CheckForTag();

        // apply smalle step penalty to encourage efficiency
        AddReward(-stepPenalty);
    }

    // allows user to control tagger
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var cont = actionsOut.ContinuousActions;
        cont[0] = 0;
        cont[1] = 0;

        // go after closest unfrozen runner if there are multiple 
        int bestIndex = -1;
        float bestDistSq = float.MaxValue;
        Vector3 myPos = transform.position;

        // for each runner 
        for (int i = 0; i < runnerTransforms.Count; i++)
        {
            // if runner does not exist skip them
            if (runnerTransforms[i] == null || runnerFrozen[i])
            {
                continue;
            }

            // otherwise compute squared distance to this runner
            Vector3 rPos = runnerTransforms[i].position;
            float dSq = (rPos - myPos).sqrMagnitude;

            // if runner is the closest to tagger 
            if (dSq < bestDistSq)
            {
                // keep track of runner position
                bestDistSq = dSq;
                bestIndex = i;
            }
        }

        // if there is a valid runner that can be tagged
        if (bestIndex != -1)
        {
            // move towards it 
            Vector3 targetDir = runnerTransforms[bestIndex].position - myPos;
            targetDir.y = 0;
            if (targetDir.sqrMagnitude > 0.0001f)
            {
                // set tagger in direction of runner 
                targetDir = targetDir.normalized;
                cont[0] = targetDir.x;
                cont[1] = targetDir.z;
            }
        }
    }

    // checks if tagger is close to runner to tag it 
    void CheckForTag()
    {
        Vector3 myPos = transform.position;

        // for each runner 
        for (int i = 0; i < runnerTransforms.Count; i++)
        {
            if (runnerTransforms[i] == null || runnerFrozen[i])
            {
                continue;
            }

            // get distance to this runner 
            float dist = Vector3.Distance(myPos, runnerTransforms[i].position);

            // if its less than or equal to distance to tag a runner 
            if (dist <= tagDistance)
            {
                // mark as tagged
                runnerFrozen[i] = true;

                // reward + bonus
                AddReward(tagReward);

                // end episode immediately after runners tagged
                EndEpisode();
                return;
            }
        }
    }

    // respawn tagger at random starting position in game
    void ResetTaggerPosition()
    {
        transform.position = GetRandomPositionInPlayArea();
        transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }

    // respawn all runners at random starting position in game
    void ResetRunnerPositions()
    {
        for (int i = 0; i < runnerTransforms.Count; i++)
        {
            if (runnerTransforms[i] == null)
            {
                continue;
            }

            Vector3 pos = GetRandomPositionInPlayArea();
            runnerTransforms[i].position = pos;
        }
    }

    // returns a random position within the bounds of game 
    Vector3 GetRandomPositionInPlayArea()
    {
        Bounds b = GetPlayBounds();
        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);
        return new Vector3(x, spawnHeight, z);
    }

    // returns the bounds of the game 
    Bounds GetPlayBounds()
    {
        if (playArea != null)
        {
            return playArea.bounds;
        }

        Vector3 center = Vector3.zero;
        Vector3 size = new Vector3(10, 5, 10);
        return new Bounds(center, size);
    }
}
