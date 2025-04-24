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
               case 0: Debug.Log("�������! ������ 10% ������� � �����."); break;
               case 1: Debug.Log("����������! +500 ������."); break;
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
            DontDestroyOnLoad(gameObject); // ������� ����� �����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerRandomEvent()
    {
        string[] events = {
            "�������! ������ 10% ������� � �����.",
            "����������! +500 ������.",
            "������� �� ��������! ��������� �� �������� � 5%."
        };
        string randomEvent = events[Random.Range(0, events.Length)];
        Debug.Log(randomEvent);
        // ��� �������� ������ �� ������ ����� ������
    }
}