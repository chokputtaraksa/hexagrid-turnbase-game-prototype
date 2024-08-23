using UnityEngine;
public abstract class Actor : MonoBehaviour
{
    public int ActorId { get; protected set; }
    public bool IsBot { get; protected set; }
    public int Stamina { get; protected set; }
    public Vector3Int CurrentPosition { get; protected set; }

    public HexGrid hexGrid { get; set; }

    public abstract void Initialize(int id, Vector3Int position);
    public abstract void StartTurn();
    public abstract void EndTurn();
}
