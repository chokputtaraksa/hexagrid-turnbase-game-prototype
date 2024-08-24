using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    public DiceRoller dice;
    private CameraController cameraController;
    // to tell the system whether player disable the auto focus camera
    public bool focusPlayer = true;
    public int currentActorIndex = 0;
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
        int index = currentActorIndex % actors.Count;
        SetCurrentPlayer(index);
    }

    public void EndTurn()
    {
        Actor currentActor = actors[currentActorIndex];
        currentActor.EndTurn();

        currentActorIndex = (currentActorIndex + 1) % actors.Count;
        if (currentActorIndex == 0)
        {
            EndMainTurn();
        }
        StartNextTurn();
    }

    // End main turn when all players have played in this Mainturn
    public void EndMainTurn()
    {
        turnCount++;
    }


    public void RegisterActor(Actor actor)
    {
        actors.Add(actor);
    }

    private void SetCurrentPlayer(int index)
    {
        Actor currentActor = actors[index];
        int actionPoint = dice.RollDice();
        currentActor.StartTurn(actionPoint);
        if (cameraController != null && focusPlayer)
        {
            cameraController.SetTarget(currentActor.transform);
        }
    }

    public Actor GetCurrentActor()
    {
        return actors[currentActorIndex];
    }

    public List<Actor> GetActors()
    {
        return actors;
    }

    public bool IsMyTurn(Actor actor)
    {
        int index = currentActorIndex % actors.Count;
        return actors[index] == actor;
    }

    public void HandleActorDeath(Actor deadActor)
    {
        int deadActorIndex = actors.IndexOf(deadActor);
        actors.Remove(deadActor);
        if (deadActorIndex < currentActorIndex || currentActorIndex >= actors.Count)
        {
            currentActorIndex--;
        }

        if (actors.Count <= 1)
        {
            EndGame();
        }
        else if (deadActor == actors[currentActorIndex])
        {
            currentActorIndex = (currentActorIndex + 1) % actors.Count;
            StartNextTurn();
        }
    }

    private void EndGame()
    {
        Debug.Log("Game Over!");
        // Implement your game over logic here
    }
}
