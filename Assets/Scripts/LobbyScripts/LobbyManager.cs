using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public Button[] playerButtons;
    public GameObject settingsPanel;
    public TMP_InputField nameInput;
    public Image colorPreview;
    public TMP_Dropdown colorDropdown; 

    private int selectedPlayer = -1; 
    private Dictionary<int, PlayerData> playerData = new Dictionary<int, PlayerData>();

    void Start()
    {
        for (int i = 0; i < playerButtons.Length; i++)
        {
            int index = i;
            playerButtons[i].onClick.AddListener(() => SelectPlayer(index));
            playerData[index] = new PlayerData();
        }

        // Избор на цветове
        colorDropdown.ClearOptions();
        List<string> colorOptions = new List<string> { "Червен", "Зелен", "Син", "Жълт", "Бял" }; // Bulgarian color names
        colorDropdown.AddOptions(colorOptions);

        colorDropdown.onValueChanged.AddListener((index) => SelectColor(index));

        settingsPanel.SetActive(false);
    }
    void OnSceneLoaded()
    {
        if (RegionInteraction.Instance != null)
        {
            RegionInteraction.Instance.ResetAllPanels();
        }
        else
        {
            Debug.LogWarning("RegionInteraction instance is missing!");
        }
    }
    public void StartGame()
    {

        if (selectedPlayer == -1)
        {
            Debug.LogError("No player selected!");
            return;
        }

        if (!playerData.ContainsKey(selectedPlayer))
        {
            Debug.LogError($"Player data for index {selectedPlayer} not found!");
            return;
        }

        // Запазване на инфо за играч в PlayerDataManager
        SavePlayerData(selectedPlayer);
        PlayerDataManager.Instance.playerName = playerData[selectedPlayer].playerName;
        PlayerDataManager.Instance.playerColor = playerData[selectedPlayer].playerColor;

        SceneManager.LoadScene("Game"); 
    }

    // Избор на играч
    void SelectPlayer(int playerIndex)
    {
        if (selectedPlayer != -1)
        {
            SavePlayerData(selectedPlayer);
        }

        selectedPlayer = playerIndex;
        LoadPlayerData(playerIndex);
        settingsPanel.SetActive(true);
    }

    // Запазване на инфо за играч
    void SavePlayerData(int playerIndex)
    {
        if (nameInput == null)
        {
            Debug.LogError("nameInput is not assigned!");
            return;
        }

        if (colorPreview == null)
        {
            Debug.LogError("colorPreview is not assigned!");
            return;
        }

        // Запазване на име
        if (!string.IsNullOrEmpty(nameInput.text))
            playerData[playerIndex].playerName = nameInput.text;

        // Запазване на цвят
        playerData[playerIndex].playerColor = colorPreview.color;
    }

    // Зареждане на инфо на играч
    void LoadPlayerData(int playerIndex)
    {
        if (nameInput == null)
        {
            Debug.LogError("nameInput is not assigned!");
            return;
        }

        if (colorPreview == null)
        {
            Debug.LogError("colorPreview is not assigned!");
            return;
        }

        nameInput.text = playerData[playerIndex].playerName;

        colorPreview.color = playerData[playerIndex].playerColor;

        // Избор на цвят от списъка
        int colorIndex = GetColorIndex(playerData[playerIndex].playerColor);
        colorDropdown.value = colorIndex;
    }

    // Избран на цвят
    void SelectColor(int dropdownIndex)
    {
        Color selectedColor = GetColorByIndex(dropdownIndex);
        if (colorPreview != null)
        {
            colorPreview.color = selectedColor; // Ъпдейт на preview
        }
        else
        {
            Debug.LogError("colorPreview is not assigned!");
        }
    }

    // Цвят по индекс
    Color GetColorByIndex(int index)
    {
        switch (index)
        {
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
            case 4: return Color.white;
            default: return Color.white;
        }
    }

    // Индекс по цвят
    int GetColorIndex(Color color)
    {
        if (color == Color.red) return 0;
        if (color == Color.green) return 1;
        if (color == Color.blue) return 2;
        if (color == Color.yellow) return 3;
        return 4; // По подразбиране - бял
    }
}