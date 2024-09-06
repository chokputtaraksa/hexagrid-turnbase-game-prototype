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

    public void PerformBotTurn(Bot bot)
    {
        Debug.Log($"Start bot turn with {bot.Difficulty}");
        switch (bot.Difficulty)
        {
            case BotDifficulty.Easy:
                PerformEasyBotTurn(bot);
                break;
            case BotDifficulty.Normal:
                PerformNormalBotTurn(bot);
                break;
            case BotDifficulty.Hard:
                PerformHardBotTurn(bot);
                break;
        }
    }

    private void PerformEasyBotTurn(Actor bot)
    {
        while (bot.Stamina > 0)
        {
            new WaitForSeconds(1f);
            if (Random.value < 0.5f && TryAttackNearestEnemy(bot))
            {
                continue;
            }

            if (TryMoveRandomly(bot))
            {
                continue;
            }

            break; // If no action was possible, end the turn
        }
        TurnManager.Instance.EndTurn();
    }

    private void PerformNormalBotTurn(Actor bot)
    {
        while (bot.Stamina > 0)
        {
            new WaitForSeconds(1f);
            if (TryAttackNearestEnemy(bot))
            {
                continue;
            }

            if (TryMoveTowardsNearestEnemy(bot))
            {
                continue;
            }

            break; // If no action was possible, end the turn
        }
        TurnManager.Instance.EndTurn();
    }

    private void PerformHardBotTurn(Actor bot)
    {
        while (bot.Stamina > 0)
        {
            new WaitForSeconds(1f);
            if (TryAttackMostVulnerableEnemy(bot))
            {
                continue;
            }

            if (TryMoveToOptimalPosition(bot))
            {
                continue;
            }

            break; // If no action was possible, end the turn
        }
        TurnManager.Instance.EndTurn();
    }

    private bool TryAttackNearestEnemy(Actor bot)
    {
        (Actor nearestEnemy, List<Vector3Int> _) = FindNearestEnemy(bot);
        if (nearestEnemy != null && IsWithinAttackRange(bot, nearestEnemy))
        {
            PerformAttack(bot, nearestEnemy);
            return true;
        }
        return false;
    }

    private bool TryMoveRandomly(Actor bot)
    {
        List<Vector3Int> neighbors = HexGrid.Instance.GetNeighbors(bot.CurrentPosition);
        neighbors = neighbors.Where(n => HexGrid.Instance.IsCellWalkable(n, true)).ToList();

        if (neighbors.Count > 0)
        {
            Vector3Int randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
            return TryMove(bot, randomNeighbor);
        }

        return false;
    }

    private bool TryMoveTowardsNearestEnemy(Actor bot)
    {
        (Actor nearestEnemy, List<Vector3Int> path) = FindNearestEnemy(bot);
        if (nearestEnemy != null)
        {
            if (path.Count > 1)
            {
                return TryMove(bot, path[1]); // Move to the next cell in the path
            }
        }
        return false;
    }

    public bool TryAttackMostVulnerableEnemy(Actor bot)
    {
        List<Actor> vulnerableEnemies = FindVulnerableEnemies(bot);
        if (vulnerableEnemies.Count < 0)
        {
            return false;
        }
        // default is the nearest
        Actor mostVulnerableEnemy = null;
        // but find the most vulnerable enemies within attack range
        foreach (Actor actor in vulnerableEnemies)
        {
            if (!IsWithinAttackRange(bot, actor)) continue;
            mostVulnerableEnemy = actor;
            break;
        }
        if (mostVulnerableEnemy == null) return false;

        PerformAttack(bot, mostVulnerableEnemy);
        return true;
    }

    private bool TryMoveToOptimalPosition(Actor bot)
    {
        // This would involve more complex logic to determine the best position
        // For now, we'll just move towards the nearest enemy as a placeholder
        // @TODO take advantage of terrain, weapon and etc.
        return TryMoveTowardsNearestEnemy(bot);
    }

    private bool TryMove(Actor bot, Vector3Int destination)
    {
        return GameLogic.Instance.RequestMove(bot, destination);
    }

    private void PerformAttack(Actor bot, Actor target)
    {
        GameLogic.Instance.HandleAttackCommand(bot, target);
    }

    public (Actor, List<Vector3Int>) FindNearestEnemy(Actor bot)
    {
        // Implementation depends on how you're tracking actors
        // This is a placeholder
        Actor shortestEnemy = null;
        // @TODO Implement of FOG, the player or bot vision
        int shortestValue = 9999;
        List<Vector3Int> path = null;
        foreach (Actor actor in TurnManager.Instance.GetEnemyActors())
        {
            if (actor == bot) continue;
            (int totalCost, List<Vector3Int> p) = HexGrid.Instance.GetShortestPathCost(bot.CurrentPosition, actor.CurrentPosition);
            if (totalCost > -1 && totalCost < shortestValue)
            {
                shortestValue = totalCost;
                shortestEnemy = actor;
                path = p;
            }
        }
        return (shortestEnemy, path);
    }

    // find the lowestHP enemy list
    public List<Actor> FindVulnerableEnemies(Actor bot)
    {
        Dictionary<int, List<Actor>> distantActors = new();
        List<Actor> enemies = TurnManager.Instance.GetEnemyActors().Where((actor) => actor != bot).ToList();
        // The lowest HP will always be in front of the list
        enemies.Sort((a, b) => a.CurrentHP.CompareTo(b.CurrentHP));
        return enemies;
    }

    // check if this bot's stamina allow him to attack the target
    public bool IsWithinAttackRange(Actor bot, Actor target)
    {
        return target.CanBeAttacked(bot.CurrentPosition);
    }

    // How many times this bot can attack
    private int CalculateAttackingTimes(Actor bot)
    {

        // Implementation depends on your game's rules
        // This is a placeholder
        return 10;
    }
}
