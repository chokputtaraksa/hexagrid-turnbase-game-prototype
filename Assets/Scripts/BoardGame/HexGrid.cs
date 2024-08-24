using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class HexGrid : MonoBehaviour
{

    public static HexGrid Instance { get; private set; }
    public float cellSize = 1f;
    private int width;
    private int height;
    public GameObject hexCellPrefab;
    public List<Vector3Int> startingPositions;

    private Dictionary<Vector3Int, HexCell> cells = new Dictionary<Vector3Int, HexCell>();
    private Dictionary<Vector3Int, GameObject> cellObjects = new Dictionary<Vector3Int, GameObject>();
    private Dictionary<Vector3Int, Actor> occupiedCells = new Dictionary<Vector3Int, Actor>();
    private List<Vector3Int> highlightedPos = new();

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

    public static readonly Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(-1, 0, 0), new Vector3Int(-1, 0, -1), new Vector3Int(0, 0, -1),
        new Vector3Int(1, 0, 0), new Vector3Int(1, 0, 1), new Vector3Int(0, 0, 1)
    };

    public void Initialize(int w, int h)
    {
        width = w;
        height = h;
        for (int q = 0; q < width; q++)
        {
            for (int r = 0; r < height; r++)
            {
                GenerateGrid(new Vector3Int(q, 0, r));
            }
        }

        // Find adjacent cells after all cells are created
        foreach (HexCell cell in cells.Values)
        {
            cell.FindAdjacentCells();
        }
        int totalActors = GameManager.Instance.PlayerNumber + GameManager.Instance.BotNumber;
        startingPositions = GetStartingPositions(totalActors);
    }

    // Generate map with grid position from Initialize
    void GenerateGrid(Vector3Int gridPos)
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

    // To get the actual world position from vector3int
    public Vector3 GridToWorldPosition(Vector3Int gridPos)
    {
        float x = (gridPos.x - gridPos.z * 0.5f) * (cellSize * 1.732f);
        float y = 0f;
        float z = gridPos.z * (cellSize * 1.5f);
        return new Vector3(x, y, z);
    }

    // Check are there any enemy, impassable terrain in this cell
    public bool IsCellOccupied(Vector3Int gridPosition)
    {
        return occupiedCells.ContainsKey(gridPosition);
    }


    // Occupied the cell with the actor
    public void OccupyCell(Vector3Int gridPosition, Actor actor)
    {
        occupiedCells[gridPosition] = actor;
    }

    public void VacateCell(Vector3Int gridPosition)
    {
        if (occupiedCells.ContainsKey(gridPosition))
        {
            occupiedCells.Remove(gridPosition);
        }
    }

    public Actor GetActorAtCell(Vector3Int gridPosition)
    {
        return occupiedCells.TryGetValue(gridPosition, out Actor actor) ? actor : null;
    }

    public HexCell GetCellAtGridPosition(Vector3Int gridPos)
    {
        if (cells.TryGetValue(gridPos, out HexCell cell))
        {
            return cell;
        }
        return null;
    }

    // generate start position for players
    public List<Vector3Int> GetStartingPositions(int numberOfPositions)
    {
        List<Vector3Int> startingPositions = new List<Vector3Int>();
        List<Vector3Int> corners = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(width - 1, 0, height - 1),
            new Vector3Int(0, 0, height - 1),
            new Vector3Int(width - 1, 0, 0)
        };

        // Add corner positions first
        startingPositions.AddRange(corners);

        // If we need more positions, add some near the corners
        while (startingPositions.Count < numberOfPositions)
        {
            foreach (Vector3Int corner in corners)
            {
                if (startingPositions.Count >= numberOfPositions) break;

                Vector3Int nearCorner = new Vector3Int(
                    corner.x + (corner.x == 0 ? 1 : -1),
                    corner.y + (corner.y == 0 ? 1 : -1),
                    0
                );

                if (!startingPositions.Contains(nearCorner))
                {
                    startingPositions.Add(nearCorner);
                }
            }

            // If we still need more positions, add random positions
            if (startingPositions.Count < numberOfPositions)
            {
                Vector3Int randomPos = new Vector3Int(
                    Random.Range(0, width),
                    Random.Range(0, height),
                    0
                );

                if (!startingPositions.Contains(randomPos))
                {
                    startingPositions.Add(randomPos);
                }
            }
        }

        return startingPositions;
    }

    public Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        if (cellObjects.TryGetValue(gridPosition, out GameObject cellObject))
        {
            return cellObject.transform.position;
        }
        return Vector3.zero; // Return a default position if the cell doesn't exist
    }

    public Dictionary<Vector3Int, int> GetReachableCells(Vector3Int startPos, int initialActionPoints)
    {
        Queue<(Vector3Int, int)> cellsToCheck = new Queue<(Vector3Int, int)>();
        Dictionary<Vector3Int, int> reachableCells = new Dictionary<Vector3Int, int>();

        cellsToCheck.Enqueue((startPos, initialActionPoints));
        reachableCells[startPos] = 0;

        while (cellsToCheck.Count > 0)
        {
            (Vector3Int currentPos, int remainingAP) = cellsToCheck.Dequeue();
            if (currentPos != startPos)
            {
                HighlightCell(currentPos);
            }
            foreach (Vector3Int neighbor in GetNeighbors(currentPos))
            {
                int costToEnter = GetActionPoint(neighbor);
                int totalCost = reachableCells[currentPos] + costToEnter;
                int newRemainingAP = remainingAP - costToEnter;

                if (newRemainingAP >= 0 && IsCellWalkable(neighbor))
                {
                    if (!reachableCells.ContainsKey(neighbor) || totalCost < reachableCells[neighbor])
                    {
                        cellsToCheck.Enqueue((neighbor, newRemainingAP));
                        reachableCells[neighbor] = totalCost;
                    }
                }
            }
        }

        return reachableCells;
    }

    public Dictionary<Vector3Int, int> ResetHightLightCells()
    {
        foreach (Vector3Int pos in highlightedPos)
        {
            GameObject adjacentCell = cellObjects[pos];

            MeshRenderer gameObjectRenderer = adjacentCell.GetComponent<MeshRenderer>();

            Material newMaterial = new Material(Shader.Find("Transparent/Diffuse"))
            {
                color = Color.white
            };
            gameObjectRenderer.material = newMaterial;
            int layerIndex = LayerMask.NameToLayer("Default");
            adjacentCell.layer = layerIndex;
        }
        return new Dictionary<Vector3Int, int>();
    }

    private void HighlightCell(Vector3Int pos)
    {
        if (cellObjects.TryGetValue(pos, out GameObject cell))
        {
            // just check if the cell is occupied in something
            // @TODO Add terrain and item/ separate them and Actor
            if (IsCellOccupied(pos))
            {
                MeshRenderer renderer = cell.GetComponent<MeshRenderer>();
                Material newMaterial = new Material(Shader.Find("Transparent/Diffuse"))
                {
                    color = new Color(255, 0, 0, 0.5f) // Semi-transparent green
                };
                renderer.material = newMaterial;
                cell.layer = LayerMask.NameToLayer("AttackGround");
                highlightedPos.Add(pos);
            }
            else
            {
                MeshRenderer renderer = cell.GetComponent<MeshRenderer>();
                Material newMaterial = new Material(Shader.Find("Transparent/Diffuse"))
                {
                    color = new Color(0, 1, 0, 0.5f) // Semi-transparent green
                };
                renderer.material = newMaterial;
                cell.layer = LayerMask.NameToLayer("WalkableGround");
                highlightedPos.Add(pos);
            }
        }
    }

    private List<Vector3Int> GetNeighbors(Vector3Int pos)
    {
        return cells[pos].GetAdjacentCells();
    }

    public int GetActionPoint(Vector3Int pos)
    {
        return cells[pos].requireAP;
    }

    private bool IsCellWalkable(Vector3Int pos)
    {
        // @TODO in case of unpassable terrain
        // Implement this method to check if a cell is walkable
        // (e.g., not occupied by an obstacle)
        return true; // Placeholder
    }

    public List<Vector3Int> FindShortestPath(Vector3Int startPos, Vector3Int endPos)
    {
        var openSet = new List<Vector3Int> { startPos };
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var gScore = new Dictionary<Vector3Int, int> { [startPos] = 0 };
        var fScore = new Dictionary<Vector3Int, int> { [startPos] = HeuristicCostEstimate(startPos, endPos) };

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(pos => fScore.GetValueOrDefault(pos, int.MaxValue)).First();

            if (current == endPos)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (!IsCellWalkable(neighbor)) continue;

                var tentativeGScore = gScore[current] + GetActionPoint(neighbor);
                if (tentativeGScore < gScore.GetValueOrDefault(neighbor, int.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, endPos);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null; // Path not found
    }

    public int HeuristicCostEstimate(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }

    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        var path = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    public (int, List<Vector3Int>) GetShortestPathCost(Vector3Int startPos, Vector3Int endPos)
    {
        var path = FindShortestPath(startPos, endPos);

        if (path == null)
        {
            Debug.Log("No valid path found.");
            return (-1, null);
        }

        int totalCost = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            totalCost += GetActionPoint(path[i + 1]);
        }
        return (totalCost, path);
    }

    public int TryMovePlayerAndGetPathCost(Vector3Int startPos, Vector3Int endPos, int stamina)
    {
        int playerStamina = stamina;
        var reachableCells = GetReachableCells(startPos, playerStamina);

        if (!reachableCells.ContainsKey(endPos))
        {
            Debug.Log($"Clicked cell is not reachable with current stamina. {stamina}");
            return -1;
        }

        (int totalCost, List<Vector3Int> path) = GetShortestPathCost(startPos, endPos);

        if (totalCost > playerStamina)
        {
            Debug.Log("Not enough stamina to reach the clicked cell.");
            return -1;
        }

        // Move the player along the path
        foreach (var pos in path)
        {
            // Implement your logic to move the player to each position
            // This could involve updating the player's position, animating movement, etc.
            // @TODO illusion of player to the new position
        }

        // Update player's stamina
        playerStamina -= totalCost;

        Debug.Log($"Player moved to {endPos}. Remaining stamina: {playerStamina}");
        return totalCost;
    }
}
