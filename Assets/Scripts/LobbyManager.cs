using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public TMP_InputField nameInput;
    public Button[] playerButtons;
    public TMP_Dropdown colorDropdown;
    public Image colorPreview;
    public Image playerMarker;
    public RawImage politicalCompass;
    public TMP_Text playerIDText;
    public TMP_Text playerPerksText;
    public TMP_Text politicalPositionText;
    public Slider liberalsBar, conservativesBar, libertariansBar, authoritariansBar;

    private int selectedPlayer = -1;
    private Dictionary<int, PlayerData> playerData = new Dictionary<int, PlayerData>();

    private string apiUrl = "http://localhost:5000/api/players/";

    void Start()
    {
        playerData = new Dictionary<int, PlayerData>(); // Initialize Dictionary
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
            SavePlayerData();
        }

        selectedPlayer = playerIndex;
        LoadPlayerData(playerIndex);
        settingsPanel.SetActive(true);
    }

    void SavePlayerData()
    {
        PlayerData player = new PlayerData()
        {
            PlayerName = nameInput.text,  // Capitalized PlayerName
            PlayerColor = ColorUtility.ToHtmlStringRGB(colorPreview.color),  // Capitalized PlayerColor
            PoliticalX = Random.Range(-1f, 1f),  // Example Data
            PoliticalY = Random.Range(-1f, 1f),
            LikeabilityLiberals = 80,
            LikeabilityConservatives = 20,
            LikeabilityLibertarians = 50,
            LikeabilityAuthoritarians = 40
        };
        StartCoroutine(SendPlayerData(player));
    }

    IEnumerator SendPlayerData(PlayerData player)
    {
        string json = JsonUtility.ToJson(player);
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "save", json))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("Player data saved!");
        }
    }

    public void EndGame()
    {
        StartCoroutine(DeleteAllPlayers());
    }

    IEnumerator DeleteAllPlayers()
    {
        using (UnityWebRequest www = UnityWebRequest.Delete(apiUrl + "delete"))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("All players deleted!");
        }
    }

    void LoadPlayerData(int playerIndex)
    {
        nameInput.text = playerData[playerIndex].PlayerName;

        // Use the GetColor method to convert the hex string to a Color object
        colorPreview.color = playerData[playerIndex].GetColor();

        // Set dropdown value based on saved color
        int colorIndex = GetColorIndex(playerData[playerIndex].GetColor());
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
