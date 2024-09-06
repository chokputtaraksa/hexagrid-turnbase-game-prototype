using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Actor : MonoBehaviour
{
    public int ActorId { get; protected set; }
    public bool IsBot { get; protected set; }
    public bool IsMyTurn { get; protected set; }
    public Vector3Int CurrentPosition { get; set; }
    public int Stamina { get; protected set; }
    [SerializeField] protected int maxHP = 10;
    public int CurrentHP { get; protected set; }

    public event Action<Actor> OnActorDeath;
    public int AttackRange = 1;

    public virtual void Initialize(int id)
    {
        ActorId = id;
        CurrentPosition = HexGrid.Instance.startingPositions[id];
        CurrentHP = maxHP;
        OnActorDeath = TurnManager.Instance.HandleActorDeath;
        HexGrid.Instance.OccupyCell(CurrentPosition, this);
    }
    public abstract void StartTurn(int actionPoint);
    public abstract void EndTurn();

    public abstract bool Move(Vector3Int newPos);

    public abstract bool Attack(Actor enemy, int damage, int usedStamina);
    public virtual void BeingHitBy(Actor attacker, int damage)
    {
        TakeDamage(damage);
        // remove old position
        HexGrid.Instance.VacateCell(CurrentPosition);
        // move the player back
        Vector3Int moveBackVector = attacker.CurrentPosition - CurrentPosition;
        if (HexGrid.Instance.GetCellAtGridPosition(CurrentPosition - moveBackVector) == null)
        {
            Die();
            CurrentHP = 0;
        }
        CurrentPosition -= moveBackVector;
        // reserve the position
        if (CurrentHP > 0)
        {
            HexGrid.Instance.OccupyCell(CurrentPosition, this);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        OnActorDeath?.Invoke(this);
        Destroy(gameObject);
        IsMyTurn = false;
    }

    protected virtual void RemoveFromGrid()
    {
        HexGrid.Instance.VacateCell(CurrentPosition);
    }

    public virtual bool CanBeAttacked(Vector3Int attackerPos)
    {
        int distant = HexGrid.Instance.GetDistant(attackerPos, CurrentPosition);
        // only if the attacker is in 1 space away from this actor can attack
        return distant <= AttackRange;
    }
}
