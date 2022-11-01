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
    public static int MaxRounds = 5;

    public static void EndRound()
    {
        var towers = GameController.PlayerTowers;
        var sum = -(long)towers.Where(x => x.IsActive).Sum(x => x.TowerUpkeep + x.UpkeepPerWorker * x.WorkerCount);

        var moneyTowers = towers.Where(x => x.GetType() == typeof(MoneyTower)).Where(x => x.IsActive) as List<MoneyTower>;

        if(!moneyTowers.IsNullOrEmpty())
            sum += (long)moneyTowers.Sum(x => (x as MoneyTower).MoneyPerWorkerPerRound * x.WorkerCount);

        PlayerInfo.Money += sum;


        RoundInProgress = false;
        PlayerInfo.CurrentRound++;
       if (PlayerInfo.CurrentRound > MaxRounds)
        {
            Debug.Log("Game Over!");
            SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
            return;
        }
        var populationIncrease = PlayerInfo.Population + PlayerInfo.Civilians * PlayerInfo.PopulationMultiplier;

        if(!moneyTowers.IsNullOrEmpty())
            populationIncrease += moneyTowers.Sum(x => x.WorkerCount * x.PopulationPerRoundMultiplier);

        PlayerInfo.Population += populationIncrease;
    }

    public static void ResetValues()
    {
        PlayerInfo.CurrentRound = 1;
        PlayerInfo.Population = 10;
        PlayerInfo.Money = 500;
        RoundInProgress = false;
    }
}

public static class PlayerInfo
{
    // PlayerInfo:
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

            Debug.Log("Civilians: " + Civilians);

            while (GameController.ActiveWorkers > _population)
            {
                GameController.PlayerTowers.OrderByDescending(x => x.WorkerCount).First().RemoveWorkers(1);
            }
        }
    }
    public static long Money = 500;

    private static float _population = 10;

    public const float PopulationMultiplier = 1.1f; // The amount the population is increesed by every round

    //public static ulong WorkerCount { get => (ulong)GameObject.FindGameObjectsWithTag("Tower").Sum(x => x.GetComponent<TowerObject>().WorkerCount); }

    public static float Civilians { get => _population - (long)GameController.ActiveWorkers; }
}