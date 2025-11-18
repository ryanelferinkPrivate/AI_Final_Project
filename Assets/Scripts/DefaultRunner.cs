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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateNoiseDirection(); // Init noise direction
    }

    // Update is called once per frame
    void Update()
    {
        if (TaggerAgent == null)
        {
            Debug.LogWarning("DefaultRunner: Target (TaggerAgent) is not assigned!");
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

    private void MoveAwayFromTagger()
    {
        Vector3 directionAwayFromTagger = (transform.position - TaggerAgent.position).normalized; // Distance away from the target
        float distanceToTagger = Vector3.Distance(transform.position, TaggerAgent.position);
        if (distanceToTagger < detectionRange)
        {
            Vector3 movementDirection = (directionAwayFromTagger + noiseDirection * noiseStrength).normalized;
            // We don't have jumping right :D 
            movementDirection.y = 0;
            transform.position += movementDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            // If tagger is outside of our view, we should just wander around. We can change this always run away but I fear that if our Runner automatically runs away
            // Then during training they'll have too great of a distance on the Tagger 
            Vector3 lollyGaggingDirection = noiseDirection.normalized;
            lollyGaggingDirection.y = 0;
            transform.position += lollyGaggingDirection * moveSpeed * 0.5f * Time.deltaTime;
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
