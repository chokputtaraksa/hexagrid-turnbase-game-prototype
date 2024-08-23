using UnityEngine;

public class Player : Character
{
    public LayerMask selectableLayer;
    public LayerMask movableTiles;
    public bool isToggle = false;

    // private int nextTurn

    void Start()
    {
        turnManager.AddCharacter(this);
        // Assuming the player starts at grid position (0, 0, 0)
        SetCurrentPosition(new Vector3Int(0, 0, 0));
    }

    void Update()
    {

        if (hexGrid.isReady)
        {
            currentCell = hexGrid.GetCellAtGridPosition(GetCurrentPosition());
            Vector3 worldInitPos = hexGrid.GridToWorldPosition(GetCurrentPosition());
            transform.position = new Vector3(worldInitPos.x, worldInitPos.y + 0.5f, worldInitPos.z);
        }

        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // MouseClickPlayer();
            MouseClickTile();
        }
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
            if (isToggle)
            {
                MoveToCell(clickedObject.GetComponent<HexCell>());
                DeHighlightAdjacentCells();
                isToggle = !isToggle;
            }
        }
    }

    // void MouseClickPlayer()
    // {
    //     Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit hit;

    //     // Perform the raycast
    //     if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
    //     {
    //         // Do something with the clicked object
    //         if (!isToggle)
    //         {
    //             HighlightAdjacentCells();
    //             isToggle = !isToggle;
    //         }
    //         else
    //         {
    //             DeHighlightAdjacentCells();
    //             isToggle = !isToggle;
    //         }
    //     }

    // }

    void MoveToCell(HexCell targetCell)
    {
        SetCurrentPosition(targetCell.gridPosition);
        stamina--;
        if (stamina <= 0)
        {
            FinishTurn();
        }
    }

}
