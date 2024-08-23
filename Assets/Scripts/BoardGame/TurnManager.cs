using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    private CameraController cameraController;
    // to tell the system whether player disable the auto focus camera
    public bool focusPlayer = true;
    private int currentActorIndex = 0;
    private List<Actor> actors = new List<Actor>();
    public int turnCount = 1;

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

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController == null)
        {
            Debug.LogError("CameraController not found on main camera!");
        }
    }

    public void RegisterActor(Actor actor)
    {
        actors.Add(actor);
    }

    public void StartGame()
    {
        if (actors.Count > 0)
        {
            SetCurrentPlayer(0);
        }
        else
        {
            Debug.LogError("No actors registered!");
        }
    }

    public void StartNextTurn()
    {
        if (actors.Count == 0) return;
        Actor currentActor = actors[(currentActorIndex + 1) % actors.Count];
        currentActor.StartTurn();
        if (cameraController != null && focusPlayer)
        {
            cameraController.SetTarget(currentActor.transform);
        }
    }

    public void EndTurn()
    {
        Actor currentActor = actors[currentActorIndex];
        currentActor.EndTurn();

        currentActorIndex = (currentActorIndex + 1) % actors.Count;
        if (currentActorIndex == 0)
        {
            turnCount++;
        }
        StartNextTurn();
    }

    private void SetCurrentPlayer(int index)
    {
        Actor currentPlayer = actors[index];
        // Set the camera to follow the current player
        if (cameraController != null)
        {
            cameraController.SetTarget(currentPlayer.transform);
        }
    }
}
