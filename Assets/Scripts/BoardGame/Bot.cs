using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : Actor
{
    public override void Initialize(int id, Vector3Int position)
    {
        ActorId = id;
        IsBot = true;
    }

    public override void StartTurn()
    {
        Debug.Log($"Bot {ActorId}'s turn started");
        // Implement bot AI logic here
        // For example:
        StartCoroutine(SimulateBotTurn());
    }

    private IEnumerator SimulateBotTurn()
    {
        yield return new WaitForSeconds(2f); // Simulate thinking time
        // Perform bot actions here
        EndTurn();
    }

    public override void EndTurn()
    {
        TurnManager.Instance.StartNextTurn();
    }
}
