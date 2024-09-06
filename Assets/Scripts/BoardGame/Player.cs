using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public bool isToggle = false;
    public Dictionary<Vector3Int, int> reachableGrid;

    public override void Initialize(int id)
    {
        base.Initialize(id);
        IsBot = false;

    }

    private void LateUpdate()
    {
        Vector3 worldInitPos = HexGrid.Instance.GridToWorldPosition(CurrentPosition);
        transform.position = new Vector3(worldInitPos.x, worldInitPos.y + 0.5f, worldInitPos.z);
    }

    void OnMouseDown()
    {
        // Do something with the clicked object
        if (TurnManager.Instance.IsMyTurn(this))
        {
            if (!isToggle)
            {
                reachableGrid = HexGrid.Instance.GetReachableCells(CurrentPosition, Stamina);
                isToggle = !isToggle;
            }
            else
            {
                reachableGrid = HexGrid.Instance.ResetHightLightCells();
                isToggle = !isToggle;
            }
        }
        else
        {
            // It's not my turn, but I was clicked, check if they can attack me or not?
            if (HexGrid.Instance.GetCellAtGridPosition(CurrentPosition).gameObject.layer == LayerMask.NameToLayer("AttackGround"))
            {
                Actor attacker = TurnManager.Instance.GetCurrentActor();
                GameLogic.Instance.HandleAttackCommand(attacker, this);
            }
        }
    }

    public override void StartTurn(int ap)
    {
        Stamina = ap;
        IsMyTurn = true;
        isToggle = false;
        Debug.Log($"{gameObject.name}'s turn started with {Stamina} AP");
    }

    public override void EndTurn()
    {
        isToggle = false;
        IsMyTurn = false;
        // Debug.Log($"{gameObject.name}'s turn started has ended");
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
            HexGrid.Instance.ResetHightLightCells();
            HexGrid.Instance.GetReachableCells(CurrentPosition, Stamina);
            if (Stamina <= 0)
            {
                TurnManager.Instance.EndTurn();
            }
        }
        return true;
    }

    public override bool Attack(Actor defender, int damage, int usedStamina)
    {
        HexGrid.Instance.ResetHightLightCells();
        Stamina -= usedStamina;
        defender.BeingHitBy(this, damage);
        Debug.Log($"{gameObject.name} Attack to {defender.name} with {damage} damage, use {usedStamina} stamina and have {Stamina} left");
        // highlight attacker possible walk-able tiles
        HexGrid.Instance.GetReachableCells(CurrentPosition, Stamina);

        if (Stamina <= 0)
        {
            TurnManager.Instance.EndTurn();
        }
        return true;
    }
}
