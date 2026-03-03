using UnityEngine;

public class SlimeGreen : Enemy
{
    protected override void HandleMovement()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        // transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }
}
