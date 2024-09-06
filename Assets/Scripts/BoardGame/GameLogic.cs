using UnityEngine;
public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance { get; private set; }

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

    public bool RequestMove(Actor actor, Vector3Int newGridPosition)
    {
        if (!HexGrid.Instance.IsCellWalkable(newGridPosition, true))
        {
            Debug.LogWarning("Cannot move to occupied cell");
            return false;
        }
        return actor.Move(newGridPosition);
    }

    public void HandleAttackCommand(Actor attacker, Actor defender)
    {
        if (defender == null || defender == attacker)
        {
            Debug.Log("Invalid attack target!");
            return;
        }

        bool attackSuccessful = CombatSystem.Instance.PerformAttack(attacker, defender);
        if (attackSuccessful)
        {
            // @TODO log something or make some action
        }
    }
}
