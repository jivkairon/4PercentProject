using UnityEngine;

public class RegionInteraction : MonoBehaviour
{
    public GameObject actionPanel; 
    private RegionData regionData; 

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

    void OnMouseDown()
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