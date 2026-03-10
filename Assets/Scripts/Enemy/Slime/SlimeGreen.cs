using UnityEngine;
using System.Collections;

public class SlimeGreen : Enemy
{
    private Vector2 wanderDirection;
    private float wanderTimer;
    private bool isWaiting = false;

    [SerializeField] private float dashSpeed = 6f;
    [SerializeField] private float dashDistance = 2f;

    private Vector2 dashTarget;
    private bool isDashing;

    protected override void HandleMovement()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;

            case EnemyState.Wander:
                Wander();
                break;

            case EnemyState.Chase:
                Chase();
                break;

            case EnemyState.Attack:
                Attack();
                break;

            case EnemyState.ReturnToSpawn:
                ReturnToSpawn();
                break;
        }
    }

    protected override void Attack()
    {
        if (isDashing)
        {
            Dash();
            return;
        }

        StartDash();
    }

    private void StartDash()
    {
        if (!TryAttack())
            return;

        Vector2 direction = (player.position - transform.position).normalized;

        float dashLength = Vector2.Distance(transform.position, player.position) + dashDistance;

        dashTarget = (Vector2)transform.position + direction * dashLength;
        isDashing = true;
    }

    private void Dash()
    {
        Vector2 direction = (dashTarget - (Vector2)transform.position).normalized;

        rb.linearVelocity = direction * dashSpeed;

        float dist = Vector2.Distance(transform.position, dashTarget);

        if (dist < 0.1f)
        {
            isDashing = false;
            rb.linearVelocity = Vector2.zero;

            OnAttackFinished();
        }
    }

    private void Idle()
    {
        rb.linearVelocity = Vector2.zero;

        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0)
        {
            currentState = EnemyState.Wander;
            wanderTimer = 2f;
        }
    }

    private void Wander()
    {
        if (isWaiting)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0)
        {
            Vector2 randomPoint = spawnPosition + Random.insideUnitCircle * 3f;

            Vector2 newDirection = (randomPoint - (Vector2)transform.position).normalized;

            wanderDirection = Vector2.Lerp(wanderDirection, newDirection, 0.6f).normalized;

            wanderTimer = 2f;
        }

        rb.linearVelocity = wanderDirection * (moveSpeed * 0.5f);
    }

    private void Chase()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    private void ReturnToSpawn()
    {
        Vector2 direction = (spawnPosition - (Vector2)transform.position).normalized;

        rb.linearVelocity = direction * (moveSpeed * 1.5f);

        float dist = Vector2.Distance(transform.position, spawnPosition);

        if (dist < 0.2f)
        {
            currentState = EnemyState.Idle;
            rb.linearVelocity = Vector2.zero;
        }
    }
}