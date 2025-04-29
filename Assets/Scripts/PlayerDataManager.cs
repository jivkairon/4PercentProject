using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public string playerName = "Player"; // Име по подразбиране
    public Color playerColor = Color.white; // Цвят по подразбиране
    public Color botColor; // Цвят на бота (ще се генерира автоматично)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GenerateBotColor(); // Генерира цвят за бота при стартиране
            Debug.Log($"Player: {playerColor}, Bot: {botColor}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Генерира контрастен/произволен цвят за бота
    private void GenerateBotColor()
    {
        // Вариант 1: Произволен цвят (изключва твърде тъмните/светли)
        botColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);

        // Вариант 2: Контрастен цвят (спрямо играча)
        //botColor = Color.HSVToRGB((playerColor.ToHSV().h + 0.5f) % 1f, 1f, 1f);
    }

    // Опционално: Промяна на цвета на бота при промяна на цвета на играча
    public void SetPlayerColor(Color newColor)
    {
        playerColor = newColor;
        GenerateBotColor(); // Регенерира цвят за бота
    }
}