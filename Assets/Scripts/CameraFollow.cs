using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Target to follow
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);
    
    // Camera damping
    [SerializeField] private float damping = 0.5f;
    
    // Aiming settings
    [SerializeField] private float aimOffsetStrength = 0.5f;
    [SerializeField] private float maxAimDistance = 3f;
    
    // Reference to player for checking aim state
    private SlimeKnightController playerController;
    
    // Track initial orthographic size 
    private float defaultOrthographicSize;
    
    private void Start()
    {
        // Find player if no target assigned
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                playerController = playerObj.GetComponent<SlimeKnightController>();
            }
            
            if (target == null)
                Debug.LogWarning("CameraFollow: No target assigned and no 'Player' found.");
        }
        else
        {
            playerController = target.GetComponent<SlimeKnightController>();
        }
        
        // Set initial position instantly
        if (target != null)
        {
            transform.position = target.position + offset;
        }
        
        // Store the default orthographic size
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            defaultOrthographicSize = cam.orthographicSize;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        // Calculate base target position
        Vector3 targetPos = target.position + offset;
        
        // Apply aim offset if player is aiming
        if (playerController != null && playerController.IsAiming())
        {
            // Get mouse position in world space
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = targetPos.z; // Keep the same z
            
            // Calculate direction to mouse
            Vector3 aimDirection = mousePos - targetPos;
            
            // Limit the distance
            if (aimDirection.magnitude > maxAimDistance)
            {
                aimDirection = aimDirection.normalized * maxAimDistance;
            }
            
            // Apply aim offset based on strength
            targetPos += aimDirection * aimOffsetStrength;
        }
        
        // Apply exponential damping for smooth movement
        transform.position = Vector3.Lerp(
            transform.position, 
            targetPos, 
            1 - Mathf.Exp(-damping * Time.fixedDeltaTime * 30)
        );
    }
    
    // Method to restore default camera size (can be called when exiting boss battles)
    public void RestoreDefaultSize()
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographicSize = defaultOrthographicSize;
        }
    }
}