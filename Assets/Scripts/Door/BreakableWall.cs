using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [Header("Wall Properties")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    private Collider2D wallCollider;

    void Awake()
    {
        currentHealth = maxHealth;
        wallCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if hit by a weapon projectile
        if (other.CompareTag("Weapon") || other.CompareTag("Projectile"))
        {
            Weapon weapon = other.GetComponent<Weapon>();
            if (weapon != null)
            {
                TakeDamage(weapon.damage);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return; // Already broken

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            BreakWall();
        }
    }

    private void BreakWall()
    {
        // Disable the wall
        if (wallCollider != null) wallCollider.enabled = false;

        // You could add any breaking effects here
        Debug.Log("Wall broken!");

        // Optional: Destroy the game object completely after a delay
        Destroy(gameObject, 0.1f);
    }

    // For debugging purposes
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (wallCollider != null)
        {
            if (wallCollider is BoxCollider2D boxCollider)
            {
                Gizmos.DrawWireCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
            }
            else if (wallCollider is CircleCollider2D circleCollider)
            {
                Gizmos.DrawWireSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
            }
        }
    }
}