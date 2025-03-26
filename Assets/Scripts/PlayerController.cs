using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement parameters
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f;

    // Ground detection
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // Component references
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    // State variables
    private float moveInput;
    private bool isGrounded;
    private bool isJumping;

    private void Awake()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Input handling
        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        // Jump when grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
        }
        
        // Flip sprite direction
        if (moveInput > 0)
            sprite.flipX = false;
        else if (moveInput < 0)
            sprite.flipX = true;
            
        UpdateAnimations();
    }
    
    private void FixedUpdate()
    {
        // Handle movement
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        
        // Apply jump force
        if (isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = false;
        }
        
        // Apply higher gravity when falling for better jump feel
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    
    private void UpdateAnimations()
    {
        if (anim != null)
        {
            // Set animation parameters - adapt these to your actual animator parameters
            anim.SetFloat("Speed", Mathf.Abs(moveInput));
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetFloat("VerticalSpeed", rb.velocity.y);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualize ground check area
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}