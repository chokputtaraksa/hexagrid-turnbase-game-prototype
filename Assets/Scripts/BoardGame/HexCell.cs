using UnityEngine;
using System.Collections.Generic;

public class HexCell : MonoBehaviour
{
    public Vector3Int gridPosition;
    public List<HexCell> adjacentCells = new List<HexCell>();
    private HexGrid hexGrid;

    public int usedStamina = 1;

    public void Initialize(Vector3Int gridPos, HexGrid grid)
    {
        gridPosition = gridPos;
        hexGrid = grid;
        gameObject.AddComponent<BoxCollider>();
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
