using UnityEngine;

public class RegionInteraction : MonoBehaviour
{
    private RegionData regionData;
    private ActionPanel actionPanelScript;

    void Awake()
    {
        // Намираме RegionData компонента
        regionData = GetComponent<RegionData>();
        if (regionData == null)
        {
            Debug.LogError("RegionData component not found on this object.", this);
        }
    }

    void Start()
    {
        // Намираме ActionPanel в сцената само веднъж
        actionPanelScript = FindObjectOfType<ActionPanel>();
        if (actionPanelScript == null)
        {
            Debug.LogError("ActionPanel not found in the scene. Make sure it exists!", this);
        }
        else
        {
            Debug.Log($"Found ActionPanel: {actionPanelScript.gameObject.name}");
        }
    }

    void OnMouseEnter()
    {
        Debug.Log($"Мишката е върху {name}");
    }

    void OnMouseExit()
    {
        Debug.Log($"Мишката напусна {name}");
    }

    private void OnMouseDown()
    {
        // Проверка дали е ред на играча
        if (GameManager.Instance != null && !GameManager.Instance.isPlayerTurn)
        {
            Debug.Log("Not player's turn - skipping interaction");
            return;
        }

        Debug.Log($"Clicked on region: {name}");

        // Проверка дали имаме нужните компоненти
        if (regionData == null)
        {
            Debug.LogError("RegionData is missing, cannot proceed with interaction", this);
            return;
        }

        // Оцветяване на региона с цвета на играча
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && PlayerDataManager.Instance != null)
        {
            spriteRenderer.color = PlayerDataManager.Instance.playerColor;
        }

        // Проверка дали actionPanelScript е намерен
        if (actionPanelScript == null)
        {
            // Опитваме се да го намерим отново, в случай че е бил унищожен или деактивиран
            actionPanelScript = FindObjectOfType<ActionPanel>(true);

            if (actionPanelScript == null)
            {
                Debug.LogError("ActionPanel not found in the scene!", this);
                return;
            }
        }

        // Показване на панела и задаване на избрания регион
        Debug.Log($"Showing action panel for region: {name}");
        actionPanelScript.ShowForRegion(regionData);
    }
}


/*using UnityEngine;

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
          GetComponent<SpriteRenderer>().color = PlayerDataManager.Instance.playerColor;
      //  ShowActionPanel();

       // Debug.Log("Регион избран: " + name);
        // GetComponent<SpriteRenderer>().color = Color.magenta; // Ярък тестов цвят

        // 2. Показване на панела
         ActionPanel panel = FindObjectOfType<ActionPanel>(true); // Намира дори скрити панели
        
          panel.ShowForRegion(GetComponent<RegionData>());

        // Debug.Log($"Избран регион: {name}"); // За тестване
    }
  /*  private void ShowActionPanel()
    {
        // Намери и покажи панела
        Transform actionPanel = transform.Find("ActionPanel");
        if (actionPanel != null)
        {
            actionPanel.gameObject.SetActive(true);
            Debug.Log("Action Panel активиран");
        }
        else
        {
            Debug.LogError("Action Panel не е намерен!");
        }
    }
  */
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

}
*/