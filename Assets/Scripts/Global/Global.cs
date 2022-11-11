using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class Global
{
    public static GameObject SelectedTower;
    public static string PointerState;
    public static bool RoundInProgress = false;
    public static List<Vector2> SpawnLocations = new List<Vector2>() { new Vector2(-10,0.5f) };
    public static int MaxRounds = 0;

    public static void EndRound()
    {
        var towers = GameController.PlayerTowers;
        var sum = -(long)towers.Where(x => x.IsActive).Sum(x => x.TowerUpkeep + x.UpkeepPerWorker * x.WorkerCount);

        var moneyTowers = towers.Where(x => x.GetType() == typeof(MoneyTower) && x.IsActive).Select(x => x as MoneyTower).ToList();

        var populationIncrease = PlayerInfo.Civilians * PlayerInfo.PopulationMultiplier;

        if (!moneyTowers.IsNullOrEmpty())
        {
            sum += (long)moneyTowers.Sum(x => x.MoneyPerWorkerPerRound * x.WorkerCount);
            populationIncrease += moneyTowers.Sum(x => x.PopulationPerRound);
        }



        RoundInProgress = false;
        PlayerInfo.CurrentRound++;
       if (PlayerInfo.CurrentRound > MaxRounds)
        {
            Debug.Log("Game Over!");
            SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
            return;
        }
        PlayerInfo.Money += sum;

        PlayerInfo.Population += populationIncrease;
    }

    public static void ResetValues()
    {
        PlayerInfo.CurrentRound = 1;
        PlayerInfo.Population = PlayerInfo.StartPopulationCount;
        PlayerInfo.Money = PlayerInfo.StartMoneyCount;
        RoundInProgress = false;
    }
}

public static class PlayerInfo
{
#if DEBUG
    public static long StartMoneyCount = 100000;
    public static long StartPopulationCount = 10000;
#else
    public static long StartMoneyCount = 500;
    public static long StartPopulationCount = 10;
#endif

    public static string Name = "Player Name";
    public static int CurrentRound = 1;
    public static float Population
    {
        get
        {
            return _population;
        }
        set
        {
            _population = value;


            if((long)_population <= 0)
            {
                Debug.Log("Game Over!");
                Global.ResetValues();
                SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
                return;
            }

            while (GameController.ActiveWorkers > _population)
            {
                GameController.PlayerTowers.OrderByDescending(x => x.WorkerCount).First().RemoveWorkers(1);
            }
        }
    }
    public static long Money = StartMoneyCount;

    private static float _population = StartPopulationCount;

    public const float PopulationMultiplier = 0.1f; // The amount the population is increesed by every round

    //public static ulong WorkerCount { get => (ulong)GameObject.FindGameObjectsWithTag("Tower").Sum(x => x.GetComponent<TowerObject>().WorkerCount); }
    public static float Civilians { get => _population - (long)GameController.ActiveWorkers; }
}