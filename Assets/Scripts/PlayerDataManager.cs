using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public string playerName = "Player"; // Име по подразбиране
    public Color playerColor = Color.white; // Цвят по подразбиране

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Задържане на инстанция през сцените
            Debug.Log("PlayerDataManager initialized.");
        }
        else
        {
            Destroy(gameObject); // Само една инстанция може да съществува
        }
    }
}