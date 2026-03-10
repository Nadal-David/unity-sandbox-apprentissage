using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected int contactDamage = 10;
    [SerializeField] protected float aggroRange = 5f;
    [SerializeField] float maxChaseDistance = 15f;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float attackCooldown = 2f;

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
    private EnemySpawnPoint spawnPoint;
    protected float lastAttackTime;
    protected bool isAttacking = false;
    private Animator animator;

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
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (player == null) return;
        if (isKnockedBack) return;

        if (isAttacking)
        {
            HandleMovement();
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        float distToSpawn = Vector2.Distance(transform.position, spawnPosition);
        Debug.Log(currentState);
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

        if (isAggro && distToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        // else if (isAggro)
        // {
        //     currentState = EnemyState.Chase;
        // }

        HandleMovement();
    }

    protected abstract void HandleMovement();
    protected abstract void Attack();

    private void OnCollisionStay2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player == null) return;

        Vector2 hitDir = collision.transform.position - transform.position;
        player.TakeDamage(contactDamage, hitDir);
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

        if (spawnPoint != null)
        {
            spawnPoint.EnemyDied();
        }

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

    public void SetSpawnPoint(EnemySpawnPoint spawner)
    {
        spawnPoint = spawner;
    }

    protected bool TryAttack()
    {
        if (isAttacking) return false;
        if (Time.time < lastAttackTime + attackCooldown) return false;

        lastAttackTime = Time.time;
        isAttacking = true;

        animator.SetTrigger("Attack");

        TransformUtils.FaceTarget(transform, player);

        return true;
    }

    public void OnAttackFinished()
    {
        isAttacking = false;
    }

    // --------- DEBUG
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnPosition, maxChaseDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
