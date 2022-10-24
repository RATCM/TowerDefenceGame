using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    [HideInInspector] private Dictionary<string, GameObject> TowerPrefabs = new Dictionary<string, GameObject>();
    [HideInInspector] private GameObject TowerPlaceSelector;
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
    private TowerPlaceSelectorScript SelectorScript;

    // Start is called before the first frame update
    void Awake()
    {
        TotalWorkerCount = 5;
        var towers = UnityManager.GetAllPrefabsOfTag("Tower");
        foreach (var tower in towers)
            TowerPrefabs.Add(tower.name, tower);

        TowerPlaceSelector = Instantiate(UnityManager.GetPrefab("Selector"));

        SelectorScript = TowerPlaceSelector.GetComponent<TowerPlaceSelectorScript>();

        //SelectorScript.SelectedTower = TowerPrefabs["ShootTower"].GetComponent<TowerObject>();

    }

    public void PlaceTower(string towerName)
    {
        var hitInfo = Physics2D.Raycast(TowerPlaceSelector.transform.position, Vector2.zero);

        ulong towerPrice = TowerPrefabs[towerName].GetComponent<TowerObject>().Price;

        if (towerPrice > PlayerInfo.Money)
            return;

        if (!hitInfo || hitInfo.transform.tag != "Tower")
        {
            var tower = Instantiate(TowerPrefabs[towerName]);

            PlayerInfo.Money -= towerPrice;

            tower.transform.position = TowerPlaceSelector.transform.position;
            tower.transform.rotation = TowerPlaceSelector.transform.rotation;
        }
    }
    void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.S))
            Global.RoundInProgress = true;
#endif

        if (Input.GetKeyDown(KeyCode.R))
            TowerPlaceSelector.transform.Rotate(new Vector3(0, 0, 90));

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectorScript.SetNextTower();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SelectorScript.SetPreviousTower();

        }

        if (!Global.RoundInProgress)
        {
            if(!TowerPlaceSelector.activeSelf)
                TowerPlaceSelector.SetActive(true);

            // Tower placement
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            TowerPlaceSelector.transform.position = new Vector3(Mathf.Round(mousePos.x+0.5f), Mathf.Round(mousePos.y+0.5f) ,0) - new Vector3(0.5f,0.5f,0);

            if (Input.GetMouseButtonDown((int)MouseButton.Left))
            {
                PlaceTower(SelectorScript.SelectedTower.gameObject.name);
            }
        }
        else
        {
            if(TowerPlaceSelector.activeSelf)
                TowerPlaceSelector.SetActive(false);
        }
    }
}
