using UnityEngine;

public class RegionInteraction : MonoBehaviour
{
    public GameObject actionPanel; 
    private RegionData regionData;
    void OnMouseEnter() => Debug.Log($"Мишката е върху {name}");
    void OnMouseExit() => Debug.Log($"Мишката напусна {name}");
    void Start()
    {
        regionData = GetComponent<RegionData>();
        if (regionData == null)
        {
            Debug.LogError("RegionData component not found on this object.", this);
        }

        // Скрит панел
        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("ActionPanel reference is not assigned.", this);
        }
    }
    private void OnMouseDown()
    {
        if (!GameManager.Instance.isPlayerTurn) return;

        // 1. Оцветяване на региона
        //   GetComponent<SpriteRenderer>().color = PlayerDataManager.Instance.playerColor;

        GetComponent<SpriteRenderer>().color = Color.magenta; // Ярък тестов цвят

        // 2. Показване на панела
        ActionPanel panel = FindObjectOfType<ActionPanel>(true); // Намира дори скрити панели
        panel.ShowForRegion(GetComponent<RegionData>());

        Debug.Log($"Избран регион: {name}"); // За тестване
    }
 /*   void OnMouseDown()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

        // Проверка дали играта е свършила
        if (gameManager != null && gameManager.IsGameOver())
        {
            Debug.Log("Game is over. Interactions are disabled.");
            return;
        }

        if (actionPanel != null && regionData != null)
        {
            ActionPanel panelScript = actionPanel.GetComponent<ActionPanel>();
            if (panelScript != null)
            {
                // Подаване на инфо за регион в панела и показване на панела
                panelScript.SetRegion(regionData);
                actionPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("ActionPanel script not found on the actionPanel object.", this);
            }
        }
        else
        {
            Debug.LogError("ActionPanel or RegionData is not assigned.", this);
        }
    }
 */
}