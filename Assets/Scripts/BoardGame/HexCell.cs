using UnityEngine;
using System.Collections.Generic;

public class HexCell : MonoBehaviour
{
    public Vector3Int gridPosition;
    public List<HexCell> adjacentCells = new List<HexCell>();
    private HexGrid hexGrid;

    public int requireAP = 1;

    public void Initialize(Vector3Int gridPos, HexGrid grid)
    {
        gameObject.AddComponent<BoxCollider>();
        gridPosition = gridPos;
        hexGrid = grid;
        gameObject.AddComponent<BoxCollider>();
    }

    void OnMouseDown()
    {
        if (gameObject.layer == LayerMask.NameToLayer("WalkableGround"))
        {
            GameLogic.Instance.RequestMove(TurnManager.Instance.GetCurrentActor(), gridPosition);
        }
    }

    public void FindAdjacentCells()
    {
        adjacentCells.Clear();
        Vector3Int[] directions = HexGrid.directions;

        foreach (Vector3Int dir in directions)
        {
            Vector3Int neighborPos = gridPosition + dir;
            HexCell neighbor = hexGrid.GetCellAtGridPosition(neighborPos);
            if (neighbor != null)
            {
                adjacentCells.Add(neighbor);
            }
        }
    }

    public List<Vector3Int> GetAdjacentCells()
    {
        List<Vector3Int> returnList = new List<Vector3Int>();
        foreach (HexCell cell in adjacentCells)
        {
            returnList.Add(cell.gridPosition);
        }
        return returnList;
    }
}
