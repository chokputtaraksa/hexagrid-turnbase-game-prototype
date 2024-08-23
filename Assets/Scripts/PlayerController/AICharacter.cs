using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacter : Character
{
    public override void StartTurn()
    {
        base.StartTurn();
        // Start AI decision-making process
        PerformAIActions();
    }

    void PerformAIActions()
    {
        // Implement AI logic here
        Debug.Log($"AI {gameObject.name} performed its action");
        FinishTurn();
    }
}
