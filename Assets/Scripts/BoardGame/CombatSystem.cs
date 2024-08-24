using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool PerformAttack(Actor attacker, Actor defender)
    {
        // @TODO damage should calculate from Weapon and Equipment
        int damage = 40;
        // @TODO use stamina depend on weapon
        int usedStamina = 2;
        bool attackSuccess = attacker.Attack(defender, damage, usedStamina);
        if (!attackSuccess)
        {
            return false;
        }
        if (defender.CurrentHP <= 0)
        {
            Debug.Log($"{defender.ActorId} has been defeated by {attacker.ActorId}!");
        }
        else
        {
            Debug.Log($"{defender.ActorId} took {damage} damage from {attacker.ActorId}. Remaining HP: {defender.CurrentHP}");
        }
        return true;
    }
}
