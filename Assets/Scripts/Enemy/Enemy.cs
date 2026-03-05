using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected int contactDamage = 10;
    [SerializeField] protected float aggroRange = 5f;
    [SerializeField] float maxChaseDistance = 15f;

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
    private EnemyHealthBarUI healthBar;
    protected EnemyState currentState;
    protected Vector2 spawnPosition;
    private bool isAggro = false;

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

        healthBar = EnemyHealthBarManager.Instance.CreateBar(transform);
        currentState = EnemyState.Idle;
        spawnPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (player == null) return;
        if (isKnockedBack) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        float distToSpawn = Vector2.Distance(transform.position, spawnPosition);

        if (!isAggro && distToPlayer <= aggroRange && !EnemyState.ReturnToSpawn.Equals(currentState))
        {
            isAggro = true;
            currentState = EnemyState.Chase;
        }

        // Si trop loin du spawn -> stop aggro
        if (isAggro && distToSpawn > maxChaseDistance)
        {
            isAggro = false;
            currentState = EnemyState.ReturnToSpawn;
        }

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

    public void TakeDamage(int amount, Vector2 hitDirection)
    {
        if (health <= 0) return;

        health -= amount;

        FindFirstObjectByType<DamageNumberPool>().Spawn(amount, transform.position);

        healthBar.SetHealth(health, maxHealth);
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

        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }

        StopAllCoroutines();

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        currentState = EnemyState.Dead;

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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnPosition, maxChaseDistance);
    }
}
