using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Target to follow
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);
    
    // Camera damping
    [SerializeField] private float damping = 0.5f;
    
    private void Start()
    {
        // Find player if no target assigned
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (target == null)
                Debug.LogWarning("CameraFollow: No target assigned and no 'Player' found.");
        }
        
        // Set initial position instantly
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        // Calculate target position
        Vector3 targetPos = target.position + offset;
        
        // Apply exponential damping for smooth movement
        transform.position = Vector3.Lerp(
            transform.position, 
            targetPos, 
            1 - Mathf.Exp(-damping * Time.fixedDeltaTime * 30)
        );
    }
}