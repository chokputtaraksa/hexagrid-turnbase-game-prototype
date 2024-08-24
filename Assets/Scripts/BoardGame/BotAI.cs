using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotAI : MonoBehaviour
{

    public static BotAI Instance { get; private set; }
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

    // public void PerformBotTurn(Bot bot)
    // {
    //     switch (bot.Difficulty)
    //     {
    //         case BotDifficulty.Easy:
    //             PerformEasyBotTurn(bot);
    //             break;
    //             // case BotDifficulty.Normal:
    //             //     PerformNormalBotTurn(bot);
    //             //     break;
    //             // case BotDifficulty.Hard:
    //             //     PerformHardBotTurn(bot);
    //             //     break;
    //     }
    // }

    // private void PerformEasyBotTurn(Bot bot)
    // {
    //     while (bot.ActionPoints > 0)
    //     {
    //         if (Random.value < 0.5f && TryAttackNearestEnemy(bot))
    //         {
    //             continue;
    //         }

    //         if (TryMoveRandomly(bot))
    //         {
    //             continue;
    //         }

    //         break; // If no action was possible, end the turn
    //     }
    // }

    // private void PerformNormalBotTurn(Bot bot)
    // {
    //     while (bot.ActionPoints > 0)
    //     {
    //         if (TryAttackNearestEnemy(bot))
    //         {
    //             continue;
    //         }

    //         if (TryMoveTowardsNearestEnemy(bot))
    //         {
    //             continue;
    //         }

    //         break; // If no action was possible, end the turn
    //     }
    // }

    // private void PerformHardBotTurn(Bot bot)
    // {
    //     while (bot.ActionPoints > 0)
    //     {
    //         if (TryAttackMostVulnerableEnemy(bot))
    //         {
    //             continue;
    //         }

    //         if (TryMoveToOptimalPosition(bot))
    //         {
    //             continue;
    //         }

    //         break; // If no action was possible, end the turn
    //     }
    // }

    // private bool TryAttackNearestEnemy(Bot bot)
    // {
    //     Actor nearestEnemy = FindNearestEnemy(bot);
    //     if (nearestEnemy != null && IsWithinAttackRange(bot, nearestEnemy))
    //     {
    //         PerformAttack(bot, nearestEnemy);
    //         return true;
    //     }
    //     return false;
    // }

    // private bool TryMoveRandomly(Bot bot)
    // {
    //     List<Vector3Int> neighbors = HexGrid.Instance.GetNeighbors(bot.CurrentGridPosition);
    //     neighbors = neighbors.Where(n => HexGrid.Instance.IsCellWalkable(n)).ToList();

    //     if (neighbors.Count > 0)
    //     {
    //         Vector3Int randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
    //         return TryMove(bot, randomNeighbor);
    //     }

    //     return false;
    // }

    // private bool TryMoveTowardsNearestEnemy(Bot bot)
    // {
    //     Actor nearestEnemy = FindNearestEnemy(bot);
    //     if (nearestEnemy != null)
    //     {
    //         List<Vector3Int> path = HexGrid.Instance.FindPath(bot.CurrentGridPosition, nearestEnemy.CurrentGridPosition);
    //         if (path.Count > 1)
    //         {
    //             return TryMove(bot, path[1]); // Move to the next cell in the path
    //         }
    //     }
    //     return false;
    // }

    // private bool TryAttackMostVulnerableEnemy(Bot bot)
    // {
    //     Actor mostVulnerableEnemy = FindMostVulnerableEnemy(bot);
    //     if (mostVulnerableEnemy != null && IsWithinAttackRange(bot, mostVulnerableEnemy))
    //     {
    //         PerformAttack(bot, mostVulnerableEnemy);
    //         return true;
    //     }
    //     return false;
    // }

    // private bool TryMoveToOptimalPosition(Bot bot)
    // {
    //     // This would involve more complex logic to determine the best position
    //     // For now, we'll just move towards the nearest enemy as a placeholder
    //     return TryMoveTowardsNearestEnemy(bot);
    // }

    private bool TryMove(Actor bot, Vector3Int destination)
    {
        return GameLogic.Instance.RequestMove(bot, destination);
    }

    private void PerformAttack(Actor bot, Actor target)
    {
        GameLogic.Instance.HandleAttackCommand(bot, target);
    }

    public Actor FindNearestEnemy(Actor bot)
    {
        // Implementation depends on how you're tracking actors
        // This is a placeholder
        Actor shortestEnemy = null;
        int shortestValue = 9999;
        foreach (Actor actor in TurnManager.Instance.GetActors())
        {
            if (actor == bot) continue;
            (int totalCost, List<Vector3Int> _) = HexGrid.Instance.GetShortestPathCost(bot.CurrentPosition, actor.CurrentPosition);
            if (totalCost < shortestValue)
            {
                shortestValue = totalCost;
                shortestEnemy = actor;
            }
        }
        return shortestEnemy;
    }

    // find the lowestHP enemy list (low -> high)
    public List<Actor> FindMostVulnerableEnemy(Actor bot)
    {
        List<Actor> enemies = TurnManager.Instance.GetActors().Where((actor) => actor != bot).ToList();
        enemies.Sort((a, b) => a.CurrentHP.CompareTo(b.CurrentHP));
        return enemies;
    }

    // check if this bot's stamina allow him to attack the target
    public bool IsWithinAttackRange(Actor bot, Actor target, int stamina)
    {
        // @TODO change when there is AP for each weapons
        int pointToAttack = 2;
        if (target == bot) return false;
        (int totalCost, List<Vector3Int> _) = HexGrid.Instance.GetShortestPathCost(bot.CurrentPosition, target.CurrentPosition);
        if (totalCost + pointToAttack > stamina)
        {
            Debug.Log($"Not enough stamina for this bot");
            return false;
        }

        return true;
    }

    // How many times this bot can attack
    private int CalculateAttackingTimes(Actor bot)
    {

        // Implementation depends on your game's rules
        // This is a placeholder
        return 10;
    }
}
