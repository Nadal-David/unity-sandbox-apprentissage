using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected int contactDamage = 10;
    [SerializeField] protected float aggroRange = 5f;

    [Header("Damage Feedback")]
    [SerializeField] private float damageFlashDuration = 0.15f;
    [SerializeField] private Color damageColor = new Color(1f, 0.4f, 0.4f);

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    private Coroutine flashCoroutine;
    private int health;
    protected Transform player;

    private SpriteRenderer spriteRenderer;
    private Color[] originalColors;
    protected Rigidbody2D rb;
    private bool isKnockedBack = false;
    public bool IsKnockedBack => isKnockedBack;
    private bool isDead = false;


    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColors = new Color[] { spriteRenderer.color };
    }


    protected virtual void Start()
    {
        player = FindFirstObjectByType<Player>()?.transform;
        health = maxHealth;
    }

    protected virtual void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > aggroRange) return;

        if (isKnockedBack) return;

        HandleMovement();
    }
    protected abstract void HandleMovement();

    private void OnCollisionStay2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable == null) return;

        Vector2 hitDir = collision.transform.position - transform.position; // ennemi -> player
        damageable.TakeDamage(contactDamage, hitDir);
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (health <= 0) return;

        health -= damage;

        StartCoroutine(ApplyKnockback(hitDirection));

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(DamageFlash());

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        Destroy(gameObject);

    }

    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = damageColor;

        yield return new WaitForSeconds(damageFlashDuration);

        spriteRenderer.color = originalColors[0];
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        isKnockedBack = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero; // IMPORTANT
        isKnockedBack = false;
    }


    // --------- DEBUG
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
