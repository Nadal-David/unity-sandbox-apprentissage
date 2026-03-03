using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private bool canHit = false;

    public void EnableHitbox()
    {
        canHit = true;
        gameObject.SetActive(true);
    }

    public void DisableHitbox()
    {
        canHit = false;
        gameObject.SetActive(false);
    }

private void OnTriggerEnter2D(Collider2D other)
{
    if (!canHit) return;

    IDamageable damageable = other.GetComponent<IDamageable>();
    if (damageable == null) return;

    Vector2 hitDirection = other.transform.position - transform.position;

    damageable.TakeDamage(damage, hitDirection);
}
}