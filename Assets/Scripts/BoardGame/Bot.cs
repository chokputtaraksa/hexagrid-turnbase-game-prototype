using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : Actor
{
    public BotDifficulty Difficulty { get; private set; }

    private void LateUpdate()
    {
        Vector3 worldInitPos = HexGrid.Instance.GridToWorldPosition(CurrentPosition);
        transform.position = new Vector3(worldInitPos.x, worldInitPos.y + 0.5f, worldInitPos.z);
    }

    void OnMouseDown()
    {
        // Do something with the clicked object
        if (!TurnManager.Instance.IsMyTurn(this))
        {
            // It's not my turn, but I was clicked, check if they can attack me or not?
            if (HexGrid.Instance.GetCellAtGridPosition(CurrentPosition).gameObject.layer == LayerMask.NameToLayer("AttackGround"))
            {
                Actor attacker = TurnManager.Instance.GetCurrentActor();
                GameLogic.Instance.HandleAttackCommand(attacker, this);
            }
        }
    }


    public override void Initialize(int id)
    {
        base.Initialize(id);
        IsBot = true;
    }

    public void Initialize(int id, BotDifficulty difficulty)
    {
        Initialize(id);
        Difficulty = difficulty;
    }

    public override void StartTurn(int ap)
    {
        Stamina = ap;
        Debug.Log($"{gameObject.name}'s turn started with {Stamina} AP");
        PerformActions();
    }

    private void PerformActions()
    {
        BotAI.Instance.PerformBotTurn(this);
    }

    // Bot attack don't need highlight
    public override bool Attack(Actor defender, int damage, int usedStamina)
    {
        Stamina -= usedStamina;
        defender.BeingHitBy(this, damage);
        Debug.Log($"{gameObject.name} Attack to {defender.name} with {damage} damage, use {usedStamina} stamina and have {Stamina} left");
        return true;
    }

    public override void EndTurn()
    {
        Debug.Log($"EndTurn of {gameObject.name}");

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
        }
        return true;
    }
}
