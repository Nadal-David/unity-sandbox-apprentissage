using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private WeaponController weaponController;

    private PlayerControls controls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Camera mainCamera;
    private Animator animator;
    private Player player;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void Update()
    {
        PlayerInput();
        RotatePlayerSprite();
        HandleAttack();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void PlayerInput()
    {
        movement = controls.Movement.Move.ReadValue<Vector2>();
    }

    private void Move()
    {
        if (player != null && player.IsKnockedBack)
            return;

        float speed = movement.magnitude;
        animator.SetFloat("Speed", speed);

        Vector2 targetVelocity = movement * moveSpeed;
        rb.linearVelocity = targetVelocity;
    }

    private void RotatePlayerSprite()
    {
        if (mainCamera == null || Mouse.current == null)
            return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();

        // Ignore les positions invalides au lancement
        if (mouseScreen == Vector2.zero)
            return;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        visualRoot.localScale = new Vector3(
            mouseWorld.x < transform.position.x ? -1f : 1f,
            1f,
            1f
        );
    }

    private void HandleAttack()
    {
        if (controls.Combat.Attack.triggered)
        {
            weaponController.TryAttack();
        }
    }
}
