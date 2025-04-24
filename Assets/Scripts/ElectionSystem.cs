using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ElectionSystem : MonoBehaviour
{
    public static ElectionSystem Instance;  // За глобален достъп

    [Header("References")]
    public RegionData[] allRegions;
    public Player player;
    public BotController bot;

    [Header("Settings")]
    public int totalMandates = 240;  // Общ брой мандати

    void Awake()
    {
        // Автоматично намиране на региони (ако не са зададени ръчно)
        if (allRegions == null || allRegions.Length == 0)
        {
            allRegions = FindObjectsOfType<RegionData>();
        }
        // Сингълтън логика
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Ако е нужно между сцени
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeRegions();
    }

    void InitializeRegions()
    {
        foreach (var region in allRegions)
        {
            region.mandates = totalMandates / allRegions.Length;  // Равно разпределение
        }
    }

    public void SimulateElections()
    {
        foreach (var region in allRegions)
        {
            // Примерна логика: играчът печели, ако влиянието му е >50%
            if (region.GetPlayerInfluencePercentage() > 50)
            {
                Debug.Log($"{region.regionName} спечелен от играча!");
            }
        }
    }
}
/*   public class ElectionSystem : MonoBehaviour
   {
     //  public List<Region> regions;
    //   public List<Party> parties;
    /*
       public void SimulateElections()
       {
           foreach (Region region in regions)
           {
               int totalVotes = 0;
               Dictionary<Party, int> regionResults = new Dictionary<Party, int>();

               // Разпределяне на гласове
               foreach (Party party in parties)
               {
                   int votes = CalculateVotes(party, region);
                   regionResults.Add(party, votes);
                   totalVotes += votes;
               }

               // Изчисляване на проценти
               foreach (var result in regionResults)
               {
                   float percentage = (result.Value / (float)totalVotes) * 100f;
                   Debug.Log($"{result.Key.name}: {percentage}% в {region.name}");
               }
           }
       }

       private int CalculateVotes(Party party, Region region)
       {
           // Логика за изчисляване на гласове (напр. популярност, ресурси, случайни фактори)
           int baseVotes = Mathf.RoundToInt(party.popularity * region.population * 0.01f);
           return Random.Range(baseVotes - 1000, baseVotes + 1000); // Добавяне на случайност
       }
   }

   [System.Serializable]
   public class Region
   {
       public string name;
       public int population;
   }

   [System.Serializable]
   public class Party
   {
       public string name;
       public float popularity; // 0-100
   }
    */