using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Global
{
    public static GameObject SelectedTower;
    public static string PointerState;
    public static bool RoundInProgress = false;
    public static List<Vector2> SpawnLocations = new List<Vector2>() { new Vector2(-10,0) };
    public static int MaxRounds = 1;
}

public static class PlayerInfo
{
    // PlayerInfo:
    public static string Name = "Player Name";
    public static int CurrentRound = 1;
    public static ulong Population = 10;
    public static ulong Money = 500;
}