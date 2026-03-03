using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject handWeapon;
    [SerializeField] private GameObject backWeapon;
    [SerializeField] private Animator handAnimator;
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Transform playerTransform; // IMPORTANT
    [SerializeField] private WeaponHitbox weaponHitbox;

    [Header("Settings")]
    [SerializeField] private float attackCooldown = 0.3f;

    private float lastAttackTime;
    private bool isAttacking;
    private Camera mainCamera;

    // Direction verrouillée au moment du clic
    private Vector2 lockedAttackDirection;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        handWeapon.SetActive(false);
        backWeapon.SetActive(true);
        weaponHitbox.DisableHitbox();
    }

    private void Update()
    {
        // On tourne vers la souris SEULEMENT si on n'attaque pas
        if (!isAttacking)
        {
            RotateTowardsMouseLive();
        }
    }

    public void TryAttack()
    {
        if (isAttacking) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        // 🔒 On lock la direction EXACTE au moment du clic
        lockedAttackDirection = GetMouseDirection();

        // On applique immédiatement la rotation figée
        ApplyRotation(lockedAttackDirection);

        StartAttack();
    }

    private void StartAttack()
    {
        isAttacking = true;

        backWeapon.SetActive(false);
        handWeapon.SetActive(true);

        handAnimator.SetTrigger("Attack");
    }

    public void OnAttackFinished()
    {
        isAttacking = false;

        handWeapon.SetActive(false);
        backWeapon.SetActive(true);
    }

    // Rotation dynamique (hors attaque)
    private void RotateTowardsMouseLive()
    {
        Vector2 dir = GetMouseDirection();
        ApplyRotation(dir);
    }

    private Vector2 GetMouseDirection()
    {
        if (mainCamera == null || Mouse.current == null)
            return Vector2.right;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        return (mouseWorld - playerTransform.position).normalized;
    }

    private void ApplyRotation(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (direction.x < 0)
        {
            angle += 180f;
        }

        weaponPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}