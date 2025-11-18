using UnityEngine;

public class DefaultRunner : MonoBehaviour
{
    [SerializeField] private Transform TaggerAgent;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float noiseStrength = 0.3f;
    [SerializeField] private float detectionRange = 10f;
    private Vector3 noiseDirection;
    private float noiseTimer;
    private float noiseChangeInterval = 0.5f;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-4f, -4f); // x, z
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(4f, 4f);   // x, z
    [SerializeField] private float spawnHeight = 1.3f; // y

    void Start()
    {
        SpawnAtRandomPosition();
        UpdateNoiseDirection();
    }

    void Update()
    {
        if (TaggerAgent == null)
        {
            return;
        }

        noiseTimer += Time.deltaTime;
        if (noiseTimer >= noiseChangeInterval)
        {
            UpdateNoiseDirection();
            noiseTimer = 0f;
        }
        MoveAwayFromTagger();
    }
    
    private void SpawnAtRandomPosition()
    {
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomZ = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        transform.position = new Vector3(randomX, spawnHeight, randomZ);
    }    

    private void MoveAwayFromTagger()
    {
        Vector3 directionAwayFromTagger = (transform.position - TaggerAgent.position).normalized;
        float distanceToTagger = Vector3.Distance(transform.position, TaggerAgent.position);
        
        if (distanceToTagger < detectionRange)
        {
            Vector3 movementDirection = (directionAwayFromTagger + noiseDirection * noiseStrength).normalized;
            movementDirection.y = 0;
            Vector3 newPosition = transform.position + movementDirection * moveSpeed * Time.deltaTime;
            
            newPosition.x = Mathf.Clamp(newPosition.x, spawnAreaMin.x, spawnAreaMax.x);
            newPosition.z = Mathf.Clamp(newPosition.z, spawnAreaMin.y, spawnAreaMax.y);
            
            transform.position = newPosition;
        }
        else
        {
            Vector3 lollyGaggingDirection = noiseDirection.normalized;
            lollyGaggingDirection.y = 0;
            
            Vector3 newPosition = transform.position + lollyGaggingDirection * moveSpeed * 0.5f * Time.deltaTime;
            
            newPosition.x = Mathf.Clamp(newPosition.x, spawnAreaMin.x, spawnAreaMax.x);
            newPosition.z = Mathf.Clamp(newPosition.z, spawnAreaMin.y, spawnAreaMax.y);
            
            transform.position = newPosition;
        }
    }

    private void UpdateNoiseDirection()
    {
        noiseDirection = new Vector3(
            Random.Range(-1f, 1f),
            0f, 
            Random.Range(-1f, 1f)
        );
    }
    
    private void OnDrawGizmosSelected()
    {
        // I want to visualise the detection range, for my own knowledge 
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw spawn area bounds
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(
            (spawnAreaMin.x + spawnAreaMax.x) / 2f,
            spawnHeight,
            (spawnAreaMin.y + spawnAreaMax.y) / 2f
        );
        Vector3 size = new Vector3(
            spawnAreaMax.x - spawnAreaMin.x,
            0.1f,
            spawnAreaMax.y - spawnAreaMin.y
        );
        Gizmos.DrawWireCube(center, size);
    }
}