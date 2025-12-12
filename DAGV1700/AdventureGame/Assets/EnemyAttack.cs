using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f; // How fast the enemy moves toward the player
    public bool lookAtPlayer = true; // Whether enemy should face the player while moving
    public bool stopOnCollision = true; // Stop moving when colliding with player
    
    [Header("Rotation Settings")]
    public bool addRandomRotation = true; // Add random spinning to the enemy
    public float rotationSpeed = 90f; // Speed of random rotation (degrees per second)
    public bool randomRotationOnStart = true; // Apply random rotation when spawned
    
    [Header("Movement Type")]
    public bool useRigidbody = false; // Use Rigidbody movement instead of Transform
    public bool flyingEnemy = false; // If true, moves in 3D space, if false stays on ground
    
    private Transform player; // Reference to the player's transform
    private Rigidbody enemyRigidbody; // Reference to enemy's Rigidbody (if using physics movement)
    private bool hasCollided = false; // Track if enemy has collided with player
    private Vector3 randomRotationAxis; // Random axis for continuous rotation
    
    void Start()
    {
        // Find the player GameObject by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            player = playerObject.transform;
            Debug.Log("EnemyAttack: Found player at " + player.name);
        }
        else
        {
            Debug.LogError("EnemyAttack: No GameObject with 'Player' tag found! Make sure your player has the 'Player' tag.");
        }
        
        // Get Rigidbody component if using physics movement
        if (useRigidbody)
        {
            enemyRigidbody = GetComponent<Rigidbody>();
            if (enemyRigidbody == null)
            {
                Debug.LogWarning("EnemyAttack: No Rigidbody found but useRigidbody is enabled. Adding Rigidbody component.");
                enemyRigidbody = gameObject.AddComponent<Rigidbody>();
            }
            
            // Freeze rotation if not flying (prevents tipping over)
            if (!flyingEnemy)
            {
                enemyRigidbody.freezeRotation = true;
            }
        }
        
        // Make sure enemy has a collider for collision detection
        Collider enemyCollider = GetComponent<Collider>();
        if (enemyCollider == null)
        {
            Debug.LogWarning("EnemyAttack: No Collider found. Adding BoxCollider for collision detection.");
            gameObject.AddComponent<BoxCollider>();
        }
        
        // Setup random rotation
        if (addRandomRotation)
        {
            // Generate random rotation axis (normalized vector)
            randomRotationAxis = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f), 
                Random.Range(-1f, 1f)
            ).normalized;
            
            Debug.Log("EnemyAttack: Random rotation axis set to " + randomRotationAxis);
        }
        
        // Apply random starting rotation if enabled
        if (randomRotationOnStart)
        {
            transform.rotation = Random.rotation;
            Debug.Log("EnemyAttack: Applied random starting rotation");
        }
    }

    void Update()
    {
        // Don't move if we don't have a player reference or if we've collided and should stop
        if (player == null || (stopOnCollision && hasCollided)) return;
        
        // Always move toward player (no stopping distance check)
        MoveTowardPlayer();
        
        // Apply random rotation if enabled and not looking at player
        if (addRandomRotation && !lookAtPlayer)
        {
            ApplyRandomRotation();
        }
        // Make enemy look at player if enabled (overrides random rotation)
        else if (lookAtPlayer)
        {
            LookAtPlayer();
        }
        // Just apply random rotation if not looking at player
        else if (addRandomRotation)
        {
            ApplyRandomRotation();
        }
    }
    
    void MoveTowardPlayer()
    {
        if (useRigidbody && enemyRigidbody != null)
        {
            // Physics-based movement
            MoveWithRigidbody();
        }
        else
        {
            // Transform-based movement
            MoveWithTransform();
        }
    }
    
    void MoveWithTransform()
    {
        Vector3 direction;
        
        if (flyingEnemy)
        {
            // 3D movement (flying enemy)
            direction = (player.position - transform.position).normalized;
        }
        else
        {
            // 2D movement (ground enemy) - ignore Y axis
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            direction = (targetPosition - transform.position).normalized;
        }
        
        // Move toward player
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
    
    void MoveWithRigidbody()
    {
        Vector3 direction;
        
        if (flyingEnemy)
        {
            // 3D movement (flying enemy)
            direction = (player.position - transform.position).normalized;
        }
        else
        {
            // 2D movement (ground enemy) - ignore Y axis
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            direction = (targetPosition - transform.position).normalized;
        }
        
        // Apply force toward player
        Vector3 targetVelocity = direction * moveSpeed;
        
        if (!flyingEnemy)
        {
            // Keep current Y velocity (gravity) for ground enemies
            targetVelocity.y = enemyRigidbody.linearVelocity.y;
        }
        
        enemyRigidbody.linearVelocity = targetVelocity;
    }
    
    void LookAtPlayer()
    {
        Vector3 lookDirection;
        
        if (flyingEnemy)
        {
            // Look directly at player in 3D space
            lookDirection = player.position - transform.position;
        }
        else
        {
            // Look at player but keep upright (ignore Y axis)
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            lookDirection = targetPosition - transform.position;
        }
        
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
    }
    
    void ApplyRandomRotation()
    {
        // Apply continuous rotation around the random axis
        transform.Rotate(randomRotationAxis * rotationSpeed * Time.deltaTime, Space.World);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if we collided with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("EnemyAttack: Collided with player!");
            hasCollided = true;
            
            // Stop movement immediately if using Rigidbody
            if (useRigidbody && enemyRigidbody != null)
            {
                enemyRigidbody.linearVelocity = Vector3.zero;
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Alternative collision detection for trigger colliders
        if (other.CompareTag("Player"))
        {
            Debug.Log("EnemyAttack: Triggered collision with player!");
            hasCollided = true;
            
            // Stop movement immediately if using Rigidbody
            if (useRigidbody && enemyRigidbody != null)
            {
                enemyRigidbody.linearVelocity = Vector3.zero;
            }
        }
    }
    
    // Public method to reset collision state (useful for respawning enemies)
    public void ResetCollision()
    {
        hasCollided = false;
        Debug.Log("EnemyAttack: Collision state reset - enemy will resume chasing player");
    }
    
    // Public method to change target (useful if you want enemy to chase something else)
    public void SetTarget(Transform newTarget)
    {
        player = newTarget;
        Debug.Log("EnemyAttack: Target changed to " + newTarget.name);
    }
    
    // Public method to get current distance to player (useful for other scripts)
    public float GetDistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Vector3.Distance(transform.position, player.position);
    }
    
    // Public method to check if enemy is close to player
    public bool IsNearPlayer()
    {
        return GetDistanceToPlayer() <= 2.0f; // Default close distance
    }
    
    // Public method to check if enemy has collided with player
    public bool HasCollidedWithPlayer()
    {
        return hasCollided;
    }
}
