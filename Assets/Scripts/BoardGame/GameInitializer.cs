using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject hexGridPrefab;
    // public PlayerSkin defaultSkin; // for skins
    private HexGrid hexGrid;

    // public GameObject botPrefab;
    public GameObject defaultPlayerPrefab;
    public GameObject defaultBotPrefab;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        this.AddComponent<CombatSystem>();
        this.AddComponent<GameLogic>();
        this.AddComponent<BotAI>();
        CreateHexGrid();
        CreateActors();
        TurnManager.Instance.StartGame();
    }

    private void CreateHexGrid()
    {
        hexGrid = hexGridPrefab.GetComponent<HexGrid>();
        hexGrid.Initialize(GameManager.Instance.GridSizeX, GameManager.Instance.GridSizeY);
    }

    private void CreateActors()
    {

        int totalActors = GameManager.Instance.PlayerNumber + GameManager.Instance.BotNumber;
        for (int i = 0; i < totalActors; i++)
        {

            if (i < GameManager.Instance.PlayerNumber)
            {
                GameObject actorObject = Instantiate(defaultPlayerPrefab);
                actorObject.name = "Player-" + i;
                Actor actor = actorObject.GetComponent<Actor>();
                actor.Initialize(i);
                TurnManager.Instance.RegisterActor(actor);
            }
            else
            {
                GameObject actorObject = Instantiate(defaultBotPrefab);
                actorObject.name = "Bot-" + i;
                Bot actor = actorObject.GetComponent<Bot>();
                actor.Initialize(i, GameManager.Instance.botDifficulty);
                TurnManager.Instance.RegisterActor(actor);
            }
        }
    }
}
