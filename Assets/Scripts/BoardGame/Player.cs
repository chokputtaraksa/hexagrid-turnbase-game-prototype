using UnityEngine;

public class Player : Actor
{
    public GameObject PlayerPrefab;
    public LayerMask selectableLayer;
    public LayerMask movableTiles;
    public bool isToggle = false;

    public override void Initialize(int id, Vector3Int position)
    {

        TurnManager.Instance.RegisterActor(this);
        ActorId = id;
        IsBot = false;
        CurrentPosition = position;
        hexGrid = GetComponent<HexGrid>();
    }

    public override void StartTurn()
    {
        Debug.Log($"{gameObject.name}'s turn started"); ;
    }

    public override void EndTurn() { }


    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            MouseClickPlayer();
            MouseClickTile();
        }
    }

    private void LateUpdate()
    {
        HexCell currentCell = hexGrid.GetCellAtGridPosition(CurrentPosition);
        Vector3 worldInitPos = hexGrid.GridToWorldPosition(CurrentPosition);
        transform.position = new Vector3(worldInitPos.x, worldInitPos.y + 0.5f, worldInitPos.z);
    }

    void MouseClickTile()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, movableTiles))
        {
            // Do something with the clicked object
            GameObject clickedObject = hit.collider.gameObject;
            // if (isToggle)
            // {
            //     MoveToCell(clickedObject.GetComponent<HexCell>());
            //     DeHighlightAdjacentCells();
            //     isToggle = !isToggle;
            // }
        }
    }

    void MouseClickPlayer()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
        {
            // Do something with the clicked object
            if (!isToggle)
            {
                HighlightAdjacentCells();
                isToggle = !isToggle;
            }
            // else
            // {
            //     DeHighlightAdjacentCells();
            //     isToggle = !isToggle;
            // }
        }

    }

    void MoveToCell(HexCell targetCell)
    {
        SetCurrentPosition(targetCell.gridPosition);
        Stamina--;
        if (Stamina <= 0)
        {
            FinishTurn();
        }
    }

}
