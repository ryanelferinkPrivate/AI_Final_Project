using UnityEngine;

// class for a "default" runner that simply moves away from the tagger and incorporates random movements
// used as a base runner to train the tagger
public class DefaultRunner : MonoBehaviour
{
    // tagger's position in the unity environment
    [SerializeField] private Transform TaggerAgent;

    // runner speed
    [SerializeField] private float moveSpeed = 3f;

    // noise/random movement probability 
    [SerializeField] private float noiseStrength = 0.3f;

    // range of runner detecting the tagger 
    [SerializeField] private float detectionRange = 10f;

    // direction of noise in the movement
    private Vector3 noiseDirection;

    //keeps track of how much time has passed since lastest change to the noise direction
    private float noiseTimer;

    // how often noise changes direction
    private float noiseChangeInterval = 0.5f;

    // determine spawn area in environment between the spawnAreaMin and spawnAreaMax
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-4f, -4f); // x, z
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(4f, 4f);   // x, z

    // determine y position of spawn
    [SerializeField] private float spawnHeight = 1.3f; // y

    // when scene begins start behavior of runner 
    void Start()
    {
        // spawn runner at a random spot in environment 
        SpawnAtRandomPosition();

        // update noise direcion of movement
        UpdateNoiseDirection();
    }

    // update runner behavior in unity
    void Update()
    {
        // if there is no tagger do nothing 
        if (TaggerAgent == null)
        {
            return;
        }

        // increase noise timer since last instance 
        noiseTimer += Time.deltaTime;

        // if noise timer reaches or passes change interval
        if (noiseTimer >= noiseChangeInterval)
        {
            // update noise direction 
            UpdateNoiseDirection();

            // reset to 0
            noiseTimer = 0f;
        }

        // default behvaior to always run away from tagger 
        MoveAwayFromTagger();
    }
    
    // picks random position to spawn runner 
    private void SpawnAtRandomPosition()
    {
        // picks a random x and z position to do so and updates runners position in unity
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomZ = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        transform.position = new Vector3(randomX, spawnHeight, randomZ);
    }    

    // behavior for runner 
    private void MoveAwayFromTagger()
    {
        // makes runner point away from tagger
        Vector3 directionAwayFromTagger = (transform.position - TaggerAgent.position).normalized;

        // distance between tagger and runner 
        float distanceToTagger = Vector3.Distance(transform.position, TaggerAgent.position);
        
        // if the tagger is in detection range
        if (distanceToTagger < detectionRange)
        {
            // runs with the noise in movement and direction 
            Vector3 movementDirection = (directionAwayFromTagger + noiseDirection * noiseStrength).normalized;

            // prevents runner from changing y position to prevent it from floating in the air 
            movementDirection.y = 0;

            //calculates new position for runner to run to
            Vector3 newPosition = transform.position + movementDirection * moveSpeed * Time.deltaTime;
            
            // keeps x and z positions within the environment
            newPosition.x = Mathf.Clamp(newPosition.x, spawnAreaMin.x, spawnAreaMax.x);
            newPosition.z = Mathf.Clamp(newPosition.z, spawnAreaMin.y, spawnAreaMax.y);
            
            // update position of runner in unity 
            transform.position = newPosition;
        }

        // otherwise if tagger is not in detection range 
        else
        {
            // allow runner to use noise to randomly roam
            Vector3 lollyGaggingDirection = noiseDirection.normalized;

            // confine any movement in the y position
            lollyGaggingDirection.y = 0;
            
            // new positon for runner to run to
            Vector3 newPosition = transform.position + lollyGaggingDirection * moveSpeed * 0.5f * Time.deltaTime;
            
            // keep it confined in the environment
            newPosition.x = Mathf.Clamp(newPosition.x, spawnAreaMin.x, spawnAreaMax.x);
            newPosition.z = Mathf.Clamp(newPosition.z, spawnAreaMin.y, spawnAreaMax.y);
            
            //update runner's position in unity 
            transform.position = newPosition;
        }
    }

    // chooses new direction for noise movement 
    private void UpdateNoiseDirection()
    {
        // randomly chooses an x and z position to move to 
        noiseDirection = new Vector3(
            Random.Range(-1f, 1f),
            0f, 
            Random.Range(-1f, 1f)
        );
    }
    
    // visually draw gizmos to see how far detectionRange is
    private void OnDrawGizmosSelected()
    {
        // visualise the detection range, for my own knowledge 
        Gizmos.color = Color.yellow;

        // shows how far the detectionRange is
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}