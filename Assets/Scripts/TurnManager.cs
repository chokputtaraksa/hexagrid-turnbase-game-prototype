using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public List<Character> characters = new List<Character>();
    private int currentCharacterIndex = 0;
    public int turnCount = 1;
    private DiceRoller diceRoller;

    void Start()
    {
        StartNextTurn();
    }

    void StartNextTurn()
    {
        if (characters.Count == 0) return;
        int stamina = diceRoller.RollDice();
        Character currentCharacter = characters[currentCharacterIndex];
        currentCharacter.stamina = stamina;
        currentCharacter.StartTurn();
    }

    public void EndTurn()
    {
        Character currentCharacter = characters[currentCharacterIndex];
        currentCharacter.EndTurn();

        currentCharacterIndex = (currentCharacterIndex + 1) % characters.Count;
        if (currentCharacterIndex == 0)
        {
            turnCount++;
        }
        StartNextTurn();
    }

    public void AddCharacter(Character character)
    {
        characters.Add(character);
    }

    public void RemoveCharacter(Character character)
    {
        characters.Remove(character);
        if (currentCharacterIndex >= characters.Count)
        {
            currentCharacterIndex = 0;
        }
    }

}
