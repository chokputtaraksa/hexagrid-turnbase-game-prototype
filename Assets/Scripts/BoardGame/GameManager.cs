using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    public int PlayerNumber { get; private set; }
    public int BotNumber { get; private set; }
    public int GridSizeX { get; private set; }
    public int GridSizeY { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void LoadSettings()
    {
        PlayerNumber = int.Parse(PlayerPrefs.GetString("PlayerNumber", "1"));
        BotNumber = int.Parse(PlayerPrefs.GetString("BotNumber", "1"));
        GridSizeX = int.Parse(PlayerPrefs.GetString("xGrid", "20"));
        GridSizeY = int.Parse(PlayerPrefs.GetString("yGrid", "20"));
    }
}
