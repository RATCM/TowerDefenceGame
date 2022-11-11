using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using TMPro;
using System.Runtime.CompilerServices;

public partial class TowerInfoController : MonoBehaviour { } // This makes things easier, trust me

public class TowerSelectController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private List<Button> towerButtons;
    [HideInInspector] private Dictionary<string, GameObject> towers = new Dictionary<string, GameObject>();
    private GameObject selector;
    private TowerPlaceSelectorScript selectorScript;

    private TMP_Text population;
    private TMP_Text money;
    private TMP_Text day;

    private Button StartButton;
    void Awake()
    {
        var buttons = GetComponentsInChildren<Button>();
        towerButtons = buttons.Where(x => x.name.Contains("Tower")).ToList();

        StartButton = buttons.First(x => x.name == "StartButton");
        StartButton.onClick.AddListener(OnRoundStart);

        towerButtons.ForEach(x => x.onClick.AddListener(delegate { onClick(x); }));

        towers = UnityManager.GetAllPrefabsOfTag("Tower").ToDictionary(x => x.name);

        selector = Instantiate(UnityManager.GetPrefab("Selector"));

        selectorScript = selector.GetComponent<TowerPlaceSelectorScript>();

        selector.SetActive(false);

        //var obj = GetComponentsInChildren<TMP_Text>(true);

        population = GetComponentsInChildren<TMP_Text>(true).First(x => x.gameObject.name == "Populaton Label");
        money = GetComponentsInChildren<TMP_Text>(true).First(x => x.gameObject.name == "Money Label");
        day = GetComponentsInChildren<TMP_Text>(true).First(x => x.gameObject.name == "Day Label");

        UpdateStatus();
    }

    void OnRoundStart()
    {
        Global.RoundInProgress = true;
    }

    void onClick(Button btn)
    {
        if (Global.RoundInProgress) return;

        try
        {
            selectorScript.UpdateTower(towers[btn.name.Split(' ')[0]].GetComponent<TowerObject>());

            towerButtons.ToList().ForEach(x => x.GetComponent<Image>().color = new Color(1f,1f,1f,0.4f));
            btn.GetComponent<Image>().color = new Color(0f, 1f, 0f, 0.7f);
        }
        catch
        {
            Debug.Log("Tower not implemented yet");
        }

    }

    public void PlaceTower(string towerName)
    {
        var hitInfos = Physics2D.RaycastAll(selector.transform.position, Vector2.zero);

        ulong towerPrice = towers[towerName].GetComponent<TowerObject>().Price;

        if ((long)towerPrice > PlayerInfo.Money)
            return;


        if (hitInfos.Length > 0 && hitInfos.Any(x => x.collider.name == "Placable Area") && !hitInfos.Any(x => x.collider.tag == "Tower"))
        {
            var tower = Instantiate(towers[towerName]);

            PlayerInfo.Money -= (long)towerPrice;

            tower.transform.position = selector.transform.position;
            tower.transform.rotation = selector.transform.rotation;

            selectorScript.UpdateSprite();
        }
    }

    void TowerPlacement()
    {
        if (!Global.RoundInProgress)
        {
            if (!selector.activeSelf)
                return;

            if (Input.GetKeyDown(KeyCode.R))
                selector.transform.Rotate(new Vector3(0, 0, 90));

            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float gridSize = 1;

            selectorScript.Move(new Vector3((Mathf.Round(mousePos.x * gridSize + gridSize / 2) - gridSize / 2) / gridSize, (Mathf.Round(mousePos.y * gridSize + gridSize / 2) - gridSize / 2) / gridSize, 0));

            if (Input.GetMouseButtonDown((int)MouseButton.Left))
            {
                PlaceTower(selectorScript.SelectedTower.gameObject.name);
            }
        }
        else
        {
            if (selector.activeSelf)
                selector.SetActive(false);
        }
    }

    void TowerSelection()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selector.SetActive(false);
            selectorScript.UpdateTower(null);
            towerButtons.ForEach(x => x.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.4f));

            GameController.PlayerTowers
                .Select(x => x.GetComponentInChildren<TowerInfoController>(true).gameObject)
                .ToList()
                .ForEach(x => x.SetActive(false));
        }
    }

    void UpdateStatus()
    {
        population.text = $"{((long)PlayerInfo.Population)} in total\n{(long)PlayerInfo.Civilians} avaliable";
        money.text = PlayerInfo.Money.ToString() + "$";
        day.text = $"Day {PlayerInfo.CurrentRound}";

        // This mess is for performance enhancements so we dont use GetComponent when we dont have to
        if (Global.RoundInProgress && StartButton.enabled)
        {
            StartButton.enabled = false;
            StartButton.GetComponent<Image>().color = new Color32(0x7D, 0x7D, 0x7D, 0x82);
            StartButton.GetComponentInChildren<TMP_Text>().text = "Round in progress";
        }
        else if (!Global.RoundInProgress && !StartButton.enabled)
        {
            StartButton.enabled = true;
            StartButton.GetComponent<Image>().color = new Color32(0x00, 0xFF, 0x0F, 0xD7);
            StartButton.GetComponentInChildren<TMP_Text>().text = "Start";
        }
    }
    void Update()
    {
        UpdateStatus();

        //if (Input.GetKeyDown(KeyCode.S))
        //    Global.RoundInProgress = true;

        TowerSelection();

        if (selectorScript.SelectedTower == null || pointerOverUI)
            return;

        TowerPlacement();
    }

    bool pointerOverUI = false;
    public void OnPointerEnter(PointerEventData data)
    {
        pointerOverUI = true;
        selector.SetActive(false);
    }

    public void OnPointerExit(PointerEventData data)
    {
        pointerOverUI = false;
        selector.SetActive(selectorScript.SelectedTower != null);
    }
}
