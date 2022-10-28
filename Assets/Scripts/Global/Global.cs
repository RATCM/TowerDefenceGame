using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Global
{
    public static GameObject SelectedTower;
    public static string PointerState;
    public static bool RoundInProgress = false;
    public static List<Vector2> SpawnLocations = new List<Vector2>() { new Vector2(-10,0) };
    public static int MaxRounds = 1;

    public static void EndRound()
    {
        RoundInProgress = false;
        PlayerInfo.CurrentRound++;
        if(PlayerInfo.CurrentRound % 5 == 0)
            PlayerInfo.Population = PlayerInfo.Population * PlayerInfo.PopulationMultiplier;
        Debug.Log(PlayerInfo.Population);
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

            if(_population <= 0)
            {
                Debug.Log("Game Over!");
            }
        }
    }
    public static ulong Money = 500;
    public static float PopulationMultiplier = 1.1f; // The amount the population is increesed by every round

    private static float _population = 10;

    //public static ulong WorkerCount { get => (ulong)GameObject.FindGameObjectsWithTag("Tower").Sum(x => x.GetComponent<TowerObject>().WorkerCount); }

    public static ulong Civilians { get => (ulong)_population - (ulong)GameController.ActiveWorkers; }
}