using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private Dictionary<string, GameObject> TowerPrefabs = new Dictionary<string, GameObject>();
    private GameObject TowerPlaceSelector;
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
        var towers = UnityManager.GetAllPrefabsOfTag("Tower");
        foreach (var tower in towers)
            TowerPrefabs.Add(tower.name, tower);

        TowerPlaceSelector = Instantiate(UnityManager.GetPrefab("Selector"));
    }

    public void PlaceTower(string towerName)
    {
        if(!Physics2D.Raycast(TowerPlaceSelector.transform.position, Vector2.zero,10,LayerMask.GetMask("Tower")))
        {
            var tower = Instantiate(TowerPrefabs[towerName]);

            tower.transform.position = TowerPlaceSelector.transform.position;
        }
    }

    void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        TowerPlaceSelector.transform.position = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y) ,0);

        if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            PlaceTower("ShootTower");
        }
    }
}
