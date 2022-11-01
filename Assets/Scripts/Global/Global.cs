using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Global
{
    public static GameObject SelectedTower;
    public static string PointerState;
    public static bool RoundInProgress = false;
    public static List<Vector2> SpawnLocations = new List<Vector2>() { new Vector2(-10,0.5f) };
    public static int MaxRounds = 1;

    public static void EndRound()
    {
        var towers = GameController.PlayerTowers;
        var sum = (long)towers.Where(x => x.IsActive).Sum(x => x.TowerUpkeep + x.UpkeepPerWorker * x.WorkerCount);
        PlayerInfo.Money -= sum;

        RoundInProgress = false;
        PlayerInfo.CurrentRound++;
        if(PlayerInfo.CurrentRound % 1 == 0)
            PlayerInfo.Population = PlayerInfo.Population * PlayerInfo.PopulationMultiplier;
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
                SceneLoader.LoadScene("MainMenu");
                Debug.Log("Game Over!");
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
    public static float PopulationMultiplier = 1.1f; // The amount the population is increesed by every round

    private static float _population = 10;

    //public static ulong WorkerCount { get => (ulong)GameObject.FindGameObjectsWithTag("Tower").Sum(x => x.GetComponent<TowerObject>().WorkerCount); }

    public static float Civilians { get => _population - (long)GameController.ActiveWorkers; }
}