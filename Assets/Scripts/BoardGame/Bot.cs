using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : Actor
{
    public BotDifficulty Difficulty { get; private set; }
    public int ActionPoints { get; private set; }

    public override void Initialize(int id)
    {
        base.Initialize(id);
        IsBot = true;
    }

    public void Initialize(int id, BotDifficulty difficulty)
    {
        base.Initialize(id);
        IsBot = true;
        Difficulty = difficulty;
    }

    public override void StartTurn(int ap)
    {
        Debug.Log($"Bot {ActorId}'s turn started with {ap} action point");

        StartCoroutine(SimulateBotTurn());
    }


    // Bot attack don't need highlight
    public override bool Attack(Actor defender, int damage, int usedStamina)
    {
        if (!defender.CanBeAttacked(CurrentPosition))
        {
            Debug.Log("too far away from the enemy, move close to them before Attack");
            return false;
        }

        if (Stamina >= usedStamina)
        {
            // Tell the system that this player will attack
            Stamina -= usedStamina;
            defender.BeingHitBy(this, damage);
            Debug.Log($"{gameObject.name} Attack to {defender.name} with {damage} damage, use {usedStamina} stamina and have {Stamina} left");

            if (Stamina <= 0)
            {
                // @TODO Handle end turn more smarter
                TurnManager.Instance.EndTurn();
            }
        }
        Debug.Log("No stamina to attack");
        return false;
    }

    private IEnumerator SimulateBotTurn()
    {
        yield return new WaitForSeconds(2f); // Simulate thinking time
        // Perform bot actions here
        EndTurn();
    }

    public override void EndTurn()
    {
        TurnManager.Instance.StartNextTurn();
    }

    public override bool Move(Vector3Int newGridPosition)
    {
        int pathCost = HexGrid.Instance.TryMovePlayerAndGetPathCost(CurrentPosition, newGridPosition, Stamina);
        if (pathCost > 0)
        {
            Stamina -= pathCost;
            HexGrid.Instance.VacateCell(CurrentPosition);
            CurrentPosition = newGridPosition;
            HexGrid.Instance.OccupyCell(newGridPosition, this);
            if (Stamina <= 0)
            {
                TurnManager.Instance.EndTurn();
            }
        }
        return true;
    }
}
