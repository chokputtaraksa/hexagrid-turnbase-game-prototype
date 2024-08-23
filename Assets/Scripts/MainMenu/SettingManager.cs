using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public TMP_InputField playerNumberInput;
    public TMP_InputField botNumberInput;
    public TMP_InputField xGridNumberInput;
    public TMP_InputField yGridNumberInput;

    public void OnStartButtonClicked()
    {
        PlayerPrefs.SetString("PlayerNumber", playerNumberInput.text);
        PlayerPrefs.SetString("BotNumber", botNumberInput.text);
        PlayerPrefs.SetString("xGrid", xGridNumberInput.text);
        PlayerPrefs.SetString("yGrid", yGridNumberInput.text);
        Debug.Log("ClickHERE");
        // Load the game scene
        SceneManager.LoadScene("BoardGame");
    }
}
