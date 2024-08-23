using UnityEngine;
using System.Collections.Generic;


public class HexGrid : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public GameObject hexCellPrefab;
    // public GameObject player;
    public bool isReady = false;

    private Dictionary<Vector3Int, HexCell> cells = new Dictionary<Vector3Int, HexCell>();
    private Dictionary<Vector3Int, GameObject> cellObjects = new Dictionary<Vector3Int, GameObject>();

    public static readonly Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 1),
        new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, -1)
    };

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
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
        isReady = true;
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

    public void HightLightWalkableCells(Vector3Int currentPos)
    {
        GameObject adjacentCell = cellObjects[currentPos];

        MeshRenderer gameObjectRenderer = adjacentCell.GetComponent<MeshRenderer>();

        Material newMaterial = new Material(Shader.Find("Transparent/Diffuse"))
        {
            color = Color.green
        };
        gameObjectRenderer.material = newMaterial;
        int layerIndex = LayerMask.NameToLayer("Tile");
        adjacentCell.layer = layerIndex;
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
}
