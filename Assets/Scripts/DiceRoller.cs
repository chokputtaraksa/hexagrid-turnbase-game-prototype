using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    public int numberOfSides = 6;
    public float rollSpeed = 5;
    // Start is called before the first frame update

    public int RollDice()
    {
        return Random.Range(1, numberOfSides + 1);
    }
}
