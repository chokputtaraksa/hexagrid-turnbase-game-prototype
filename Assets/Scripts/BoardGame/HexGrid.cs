using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class HexGrid : MonoBehaviour
{
    public float cellSize = 1f;
    public GameObject hexCellPrefab;
    // public GameObject player;

    private Dictionary<Vector3Int, HexCell> cells = new Dictionary<Vector3Int, HexCell>();
    private Dictionary<Vector3Int, GameObject> cellObjects = new Dictionary<Vector3Int, GameObject>();

    public static readonly Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 1),
        new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, -1)
    };

    public void Initialize(int width, int height)
    {
        for (int q = 0; q < width; q++)
        {
            for (int r = 0; r < height; r++)
            {
                CreateCell(new Vector3Int(q, r, -q - r));
            }
        }

        // Find adjacent cells after all cells are created
        foreach (HexCell cell in cells.Values)
        {
            cell.FindAdjacentCells();
        }
    }

    void CreateCell(Vector3Int gridPos)
    {
        Vector3 worldPos = GridToWorldPosition(gridPos);

        GameObject cellObject = Instantiate(hexCellPrefab, worldPos, Quaternion.identity, transform);
        cellObject.name = "cell-" + gridPos.x + "-" + gridPos.z;
        HexCell cell = cellObject.GetComponent<HexCell>();
        if (cell == null) cell = cellObject.AddComponent<HexCell>();

        cell.Initialize(gridPos, this);
        cells[gridPos] = cell;
        cellObjects[gridPos] = cellObject;
    }

    public Vector3 GridToWorldPosition(Vector3Int gridPos)
    {
        float x = (gridPos.x + gridPos.z * 0.5f) * (cellSize * 1.732f);
        float y = 0f;
        float z = gridPos.z * (cellSize * 1.5f);
        return new Vector3(x, y, z);
    }

    public HexCell GetCellAtGridPosition(Vector3Int gridPos)
    {
        if (cells.TryGetValue(gridPos, out HexCell cell))
        {
            return cell;
        }
        return null;
    }

    public void HighlightWalkableCells(Vector3Int startPos, int stamina)
    {
        Queue<Vector3Int> cellsToCheck = new Queue<Vector3Int>();
        HashSet<Vector3Int> visitedCells = new HashSet<Vector3Int>();

        cellsToCheck.Enqueue(startPos);
        visitedCells.Add(startPos);

        while (cellsToCheck.Count > 0 && stamina > 0)
        {
            int cellsInCurrentLevel = cellsToCheck.Count;
            for (int i = 0; i < cellsInCurrentLevel; i++)
            {
                Vector3Int currentPos = cellsToCheck.Dequeue();
                HighlightCell(currentPos);

                foreach (Vector3Int neighbor in GetNeighbors(currentPos))
                {
                    if (!visitedCells.Contains(neighbor) && IsCellWalkable(neighbor))
                    {
                        cellsToCheck.Enqueue(neighbor);
                        visitedCells.Add(neighbor);
                    }
                }
            }
            stamina--;
        }
    }

    public void ResetHightLightCells(Vector3Int currentPos)
    {
        GameObject adjacentCell = cellObjects[currentPos];

        MeshRenderer gameObjectRenderer = adjacentCell.GetComponent<MeshRenderer>();

        Material newMaterial = new Material(Shader.Find("Transparent/Diffuse"))
        {
            color = Color.white
        };
        gameObjectRenderer.material = newMaterial;
        int layerIndex = LayerMask.NameToLayer("Default");
        adjacentCell.layer = layerIndex;
    }

    private void HighlightCell(Vector3Int pos)
    {
        if (cellObjects.TryGetValue(pos, out GameObject cell))
        {
            MeshRenderer renderer = cell.GetComponent<MeshRenderer>();
            Material newMaterial = new Material(Shader.Find("Transparent/Diffuse"))
            {
                color = new Color(0, 1, 0, 0.5f) // Semi-transparent green
            };
            renderer.material = newMaterial;
            cell.layer = LayerMask.NameToLayer("Tile");
        }
    }

    private List<Vector3Int> GetNeighbors(Vector3Int pos)
    {
        return cells[pos].GetAdjacentCells();
    }

    private bool IsCellWalkable(Vector3Int pos)
    {
        // Implement this method to check if a cell is walkable
        // (e.g., not occupied by an obstacle)
        return true; // Placeholder
    }
}
