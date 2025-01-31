using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public Button[] playerButtons;
    public GameObject settingsPanel;
    public TMP_InputField nameInput;
    public Image colorPreview;
    public TMP_Dropdown colorDropdown; // Dropdown for selecting color

    private int selectedPlayer = -1;
    private Dictionary<int, PlayerData> playerData = new Dictionary<int, PlayerData>();

    void Start()
    {
        // Initialize player buttons
        for (int i = 0; i < playerButtons.Length; i++)
        {
            int index = i;
            playerButtons[i].onClick.AddListener(() => SelectPlayer(index));
            playerData[index] = new PlayerData();
        }

        // Add dropdown options for colors
        colorDropdown.ClearOptions();
        List<string> colorOptions = new List<string> { "Red", "Green", "Blue", "Yellow", "White" };
        colorDropdown.AddOptions(colorOptions);

        // Listen to color selection
        colorDropdown.onValueChanged.AddListener((index) => SelectColor(index));

        settingsPanel.SetActive(false);
    }

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

    void SavePlayerData(int playerIndex)
    {
        if (!string.IsNullOrEmpty(nameInput.text))
            playerData[playerIndex].playerName = nameInput.text;

        // Save selected color
        playerData[playerIndex].playerColor = colorPreview.color;
    }

    void LoadPlayerData(int playerIndex)
    {
        nameInput.text = playerData[playerIndex].playerName;

        // Load player's saved color
        colorPreview.color = playerData[playerIndex].playerColor;

        // Set dropdown value based on saved color
        int colorIndex = GetColorIndex(playerData[playerIndex].playerColor);
        colorDropdown.value = colorIndex;
    }

    void SelectColor(int dropdownIndex)
    {
        Color selectedColor = GetColorByIndex(dropdownIndex);
        colorPreview.color = selectedColor; // Update color preview
    }

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

    int GetColorIndex(Color color)
    {
        if (color == Color.red) return 0;
        if (color == Color.green) return 1;
        if (color == Color.blue) return 2;
        if (color == Color.yellow) return 3;
        return 4; // Default to white
    }
}
