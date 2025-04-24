using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    /*   public static void TriggerRandomEvent()
       {
           int eventId = Random.Range(0, 3);
           switch (eventId)
           {
               case 0: Debug.Log("Скандал! Губите 10% влияние в София."); break;
               case 1: Debug.Log("Доброволци! +500 монети."); break;
           }
       }
    */
    public static EventSystem Instance;


    public Player player;
    public BotController bot;

    public void TriggerScandalEvent()
    {
        player.overallInfluence -= 10f;
        player.UpdateOverallInfluenceDisplay();
    }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Запазва между сцени
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerRandomEvent()
    {
        string[] events = {
            "Скандал! Губите 10% влияние в София.",
            "Доброволци! +500 монети.",
            "Медиите ви атакуват! Влиянието ви намалява с 5%."
        };
        string randomEvent = events[Random.Range(0, events.Length)];
        Debug.Log(randomEvent);
        // Тук добавете логика за ефекти върху играта
    }
}