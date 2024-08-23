using System;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject hexGridPrefab;
    public GameObject playerPrefab;
    public GameObject botPrefab;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        CreateHexGrid();
        CreateActors();
    }

    private void CreateHexGrid()
    {
        HexGrid hexGrid = hexGridPrefab.GetComponent<HexGrid>();
        hexGrid.Initialize(GameManager.Instance.GridSizeX, GameManager.Instance.GridSizeY);
    }

    private void CreateActors()
    {
        int totalActors = GameManager.Instance.PlayerNumber + GameManager.Instance.BotNumber;
        for (int i = 0; i < totalActors; i++)
        {
            GameObject actorObject;
            if (i < GameManager.Instance.PlayerNumber)
            {
                actorObject = Instantiate(playerPrefab);
            }
            else
            {
                actorObject = Instantiate(botPrefab);
            }

            Actor actor = actorObject.GetComponent<Actor>();
            actor.Initialize(i, new Vector3Int(0, 0, 0));
            TurnManager.Instance.RegisterActor(actor);

            // Position the actor on the grid
            // You'll need to implement this based on your grid system
            // PositionActorOnGrid(actor);
        }
    }
}
