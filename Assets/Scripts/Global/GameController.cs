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
    [HideInInspector] private TowerPlaceSelectorScript SelectorScript;
    public static List<TowerObject> PlayerTowers { get => FindObjectsOfType<TowerObject>().ToList(); }

    // Func is nessecary due to dumb C# compiler stuff https://stackoverflow.com/questions/41179792/why-is-linq-sum-with-decimals-ambiguous-between-int-and-int
    public static ulong ActiveWorkers => (ulong)PlayerTowers.Sum(new Func<TowerObject, float>(x => x.WorkerCount));

    //public static long TotalWorkerCount;
    public static ulong AvaliableWorkers { get => (ulong)PlayerInfo.Population - ActiveWorkers; }

    // Start is called before the first frame update
    void Awake()
    {
        TowerPrefabs = UnityManager.GetAllPrefabsOfTag("Tower").ToDictionary(x => x.name);
        //var towers = UnityManager.GetAllPrefabsOfTag("Tower");
        //foreach (var tower in towers)
        //    TowerPrefabs.Add(tower.name, tower);

        TowerPlaceSelector = Instantiate(UnityManager.GetPrefab("Selector"));

        SelectorScript = TowerPlaceSelector.GetComponent<TowerPlaceSelectorScript>();
    }

    public void PlaceTower(string towerName)
    {
        var hitInfo = Physics2D.Raycast(TowerPlaceSelector.transform.position, Vector2.zero);

        var hitInfos = Physics2D.RaycastAll(TowerPlaceSelector.transform.position, Vector2.zero);

        ulong towerPrice = TowerPrefabs[towerName].GetComponent<TowerObject>().Price;

        if (towerPrice > PlayerInfo.Money)
            return;


        if (hitInfos.Length > 0 && hitInfos.Any(x => x.collider.name == "Placable Area") && !hitInfos.Any(x => x.collider.tag == "Tower"))
        {
            var tower = Instantiate(TowerPrefabs[towerName]);

            PlayerInfo.Money -= towerPrice;

            tower.transform.position = TowerPlaceSelector.transform.position;
            tower.transform.rotation = TowerPlaceSelector.transform.rotation;

            SelectorScript.UpdateSprite();
        }
    }

    void TowerPlacement()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.S))
            Global.RoundInProgress = true;
#endif


        if (!Global.RoundInProgress)
        {
            if (!TowerPlaceSelector.activeSelf)
                return;


            if (Input.GetKeyDown(KeyCode.R))
                TowerPlaceSelector.transform.Rotate(new Vector3(0, 0, 90));

            //if (!TowerPlaceSelector.activeSelf)
                //TowerPlaceSelector.SetActive(true);

            // Tower placement
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float gridSize = 1;

            SelectorScript.Move(new Vector3((Mathf.Round(mousePos.x * gridSize + gridSize/2) - gridSize/2)/gridSize, (Mathf.Round(mousePos.y*gridSize + gridSize/2) - gridSize/2)/gridSize, 0));

            if (Input.GetMouseButtonDown((int)MouseButton.Left))
            {
                PlaceTower(SelectorScript.SelectedTower.gameObject.name);
            }
        }
        else
        {
            if (TowerPlaceSelector.activeSelf)
                TowerPlaceSelector.SetActive(false);
        }
    }

    void TowerSelection()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TowerPlaceSelector.SetActive(!TowerPlaceSelector.activeSelf);

        if (!TowerPlaceSelector.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            SelectorScript.SetNextTower();
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            SelectorScript.SetPreviousTower();

    }

    void Update()
    {
        TowerPlacement();
        TowerSelection();
    }
}
