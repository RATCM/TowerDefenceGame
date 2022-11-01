using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    public static List<TowerObject> PlayerTowers { get => FindObjectsOfType<TowerObject>().ToList(); }

    // Func is nessecary due to dumb C# compiler stuff https://stackoverflow.com/questions/41179792/why-is-linq-sum-with-decimals-ambiguous-between-int-and-int
    public static ulong ActiveWorkers => (ulong)PlayerTowers.Sum(new Func<TowerObject, float>(x => x.WorkerCount));

    //public static long TotalWorkerCount;
    public static ulong AvaliableWorkers { get => (ulong)PlayerInfo.Population - ActiveWorkers; }
}