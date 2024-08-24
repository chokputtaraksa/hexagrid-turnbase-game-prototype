using UnityEngine;

[CreateAssetMenu(fileName = "New Player Skin", menuName = "Game/Player Skin")]
public class PlayerSkin : ScriptableObject
{
    public string skinName;
    public GameObject playerPrefab;
    public GameObject botPrefab;
}
