using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TaggerAgent : Agent
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 4;
    [SerializeField] bool manualControl = false;

    [Header("Platform")]
    [SerializeField] BoxCollider playArea;
    [SerializeField] float spawnHeight = 1.3f;

    [Header("Runners")]
    [SerializeField] List<Transform> runnerTransforms = new List<Transform>();

    [Header("Tagging")]
    [SerializeField] float tagDistance = 0.8f;

    [Header("Rewards")]
    [SerializeField] float tagReward = 1;
    [SerializeField] float stepPenalty = 0.5f;
    [SerializeField] float wallCollisionPenalty = 0.05f;

    bool[] runnerFrozen;
    
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

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 myPos = transform.position;
        sensor.AddObservation(myPos.x);
        sensor.AddObservation(myPos.z);

        for (int i = 0; i < runnerTransforms.Count; i++)
        {
            if (runnerTransforms[i] != null)
            {
                Vector3 rPos = runnerTransforms[i].position;
                sensor.AddObservation(rPos.x);
                sensor.AddObservation(rPos.z);
                sensor.AddObservation(runnerFrozen[i] ? 1f : 0f);
            }


            else
            {
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var cont = actions.ContinuousActions;
        float moveX = cont[0];
        float moveZ = cont[1];

        Vector3 direction = new Vector3(moveX, 0, moveZ);

        if (direction.sqrMagnitude > 0.0001f)
        {
            direction = direction.normalized;
            Vector3 displacement = direction * moveSpeed * Time.deltaTime;
            transform.position += displacement;
        }

        CheckForTag();

        AddReward(-stepPenalty);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var cont = actionsOut.ContinuousActions;
        cont[0] = 0;
        cont[1] = 0;

        // go after closest unfrozen runner if there are multiple (un tested no clue if this even works)
        int bestIndex = -1;
        float bestDistSq = float.MaxValue;
        Vector3 myPos = transform.position;

        for (int i = 0; i < runnerTransforms.Count; i++)
        {
            if (runnerTransforms[i] == null || runnerFrozen[i])
            {
                continue;
            }

            Vector3 rPos = runnerTransforms[i].position;
            float dSq = (rPos - myPos).sqrMagnitude;
            if (dSq < bestDistSq)
            {
                bestDistSq = dSq;
                bestIndex = i;
            }
        }

        if (bestIndex != -1)
        {
            Vector3 targetDir = runnerTransforms[bestIndex].position - myPos;
            targetDir.y = 0;
            if (targetDir.sqrMagnitude > 0.0001f)
            {
                targetDir = targetDir.normalized;
                cont[0] = targetDir.x;
                cont[1] = targetDir.z;
            }
        }
    }

    void CheckForTag()
    {
        Vector3 myPos = transform.position;

        for (int i = 0; i < runnerTransforms.Count; i++)
        {
            if (runnerTransforms[i] == null || runnerFrozen[i])
            {
                continue;
            }

            float dist = Vector3.Distance(myPos, runnerTransforms[i].position);
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

    void ResetTaggerPosition()
    {
        transform.position = GetRandomPositionInPlayArea();
        transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }

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

    Vector3 GetRandomPositionInPlayArea()
    {
        Bounds b = GetPlayBounds();
        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);
        return new Vector3(x, spawnHeight, z);
    }

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
