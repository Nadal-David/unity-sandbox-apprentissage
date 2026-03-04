using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float damageCooldown = 1f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("Damage Feedback")]
    [SerializeField] private float damageFlashDuration = 0.15f;
    [SerializeField] private Color damageColor = new Color(1f, 0.4f, 0.4f);

    private int health;
    private float lastDamageTime;
    private Rigidbody2D rb;
    private bool isKnockedBack = false;
    public bool IsKnockedBack => isKnockedBack;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private Coroutine flashCoroutine;


    private PlayerHealthUI healthUI;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }
    }

    private void Start()
    {
        health = maxHealth;
        healthUI = FindFirstObjectByType<PlayerHealthUI>();
        healthUI?.UpdateHealthBar();
    }

    public void TakeDamage(int amount, Vector2 hitDirection)
    {
        if (Time.time < lastDamageTime + damageCooldown)
            return;

        lastDamageTime = Time.time;

        health -= amount;

        FindFirstObjectByType<DamageNumberPool>().Spawn(amount, transform.position);

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(DamageFlash());

        StartCoroutine(ApplyKnockback(hitDirection));

        healthUI?.UpdateHealthBar();


        Debug.Log($"Player prend {amount} dégâts. HP restants : {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        isKnockedBack = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
    }

    private IEnumerator DamageFlash()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = damageColor;
        }

        yield return new WaitForSeconds(damageFlashDuration);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = originalColors[i];
        }
    }

    private void Die()
    {
        Debug.Log("Player mort !");
    }

    // UI

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealthPercent()
    {
        return (float)health / maxHealth;
    }
}
