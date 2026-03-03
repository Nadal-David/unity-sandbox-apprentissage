using UnityEngine;

public class WeaponAnimationRelay : MonoBehaviour
{
    private WeaponController weaponController;
    private WeaponHitbox hitbox;

    private void Awake()
    {
        // Récupère le controller sur le parent (Weapon)
        weaponController = GetComponentInParent<WeaponController>();
        hitbox = GetComponentInChildren<WeaponHitbox>();
    }

    public void EnableHitbox()
    {
        hitbox.EnableHitbox();
    }

    // Animation Event (fin du slash)
    // public void DisableHitbox()
    // {
    //     hitbox.DisableHitbox();
    // }

    // Appelé par l'Animation Event
    public void OnAttackFinished()
    {
        weaponController.OnAttackFinished();
        hitbox.DisableHitbox();
    }
}