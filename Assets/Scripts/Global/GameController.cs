using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static List<TowerObject> PlayerTowers 
    {
        get 
        {
            return FindObjectsOfType<TowerObject>().ToList();
        }
    }
    public static long TotalWorkerCount;
    public static long ActiveWorkers
    { 
        get
        {
            long count = 0;
            foreach(var tower in PlayerTowers)
            {
                count += tower.WorkerCount;
            }
            return count;
        } 
    }
    public static long AvaliableWorkers { get { return TotalWorkerCount - ActiveWorkers; } }

    // Start is called before the first frame update
    void Awake()
    {
        TotalWorkerCount = 5;
    }

    void Update()
    {
        
    }
}
