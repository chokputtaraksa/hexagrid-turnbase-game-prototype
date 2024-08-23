using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public TurnManager turnManager;
    public Camera mainCamera;
    // current position in vector3 to get the cell from HexGrid
    private Vector3Int currentPosition;
    // current position of the cell
    public HexCell currentCell;
    // world map
    public HexGrid hexGrid;

    public int stamina = 0;
    // public int hp


    public virtual void StartTurn()
    {
        Debug.Log($"{gameObject.name}'s turn started");
        // Implement turn start logic here
        for (int i = 0; i < stamina; i++)
        {

        }
    }

    public virtual void EndTurn()
    {
        Debug.Log($"{gameObject.name}'s turn ended");
        // Implement turn end logic here
        HighlightAdjacentCells(stamina);
    }

    public void FinishTurn()
    {
        turnManager.EndTurn();
    }

    public void SetCurrentPosition(Vector3Int position)
    {
        currentPosition = position;
    }

    public Vector3Int GetCurrentPosition()
    {
        return currentPosition;
    }

    public void SetStamina(int amount)
    {
        stamina = amount;
    }

    public void HighlightAdjacentCells(int deep)
    {

        foreach (HexCell adjacentCell in currentCell.adjacentCells)
        {
            // Highlight the adjacent cell
            hexGrid.HightLightWalkableCells(adjacentCell.gridPosition);
        }
    }

    public void DeHighlightAdjacentCells()
    {
        foreach (HexCell adjacentCell in currentCell.adjacentCells)
        {
            // Highlight the adjacent cell
            hexGrid.ResetHightLightCells(adjacentCell.gridPosition);
        }
    }

}
