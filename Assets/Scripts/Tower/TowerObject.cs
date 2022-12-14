using System.Collections.Generic;
using UnityEngine;
using TowerTypes;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class TowerUpgrade
{
    public delegate void UpgradeAction();
    public string UpgradeName;
    public TowerObject Tower;
    public ulong UpgradePrice;
    private UpgradeAction Action;

    public TowerUpgrade(string upgradeName, TowerObject tower, ulong upgradePrice, UpgradeAction action)
    {
        UpgradeName = upgradeName;
        Tower = tower;
        UpgradePrice = upgradePrice;
        Action = action;
    }
    internal bool _applyUpgrade()
    {
        if ((long)UpgradePrice < PlayerInfo.Money)
        {
            Action.Invoke();
            PlayerInfo.Money -= (long)UpgradePrice;
            return true;
        }
        return false;
    }
}

public class TowerUpgradePath
{
    private Queue<TowerUpgrade> Upgrades = new Queue<TowerUpgrade>();
    public TowerUpgradePath(IEnumerable<TowerUpgrade> upgradePath) =>
        upgradePath.ToList().ForEach(x => Upgrades.Enqueue(x));
    public TowerUpgradePath(params TowerUpgrade[] upgradePath) =>
        upgradePath.ToList().ForEach(x => Upgrades.Enqueue(x));

    public TowerUpgrade GetNext()
    {
        Upgrades.TryPeek(out var result);
        return result;
    }
    public void ApplyUpgrade()
    {
        var check = Upgrades.Peek()._applyUpgrade();

        var tower = Upgrades.Peek().Tower;

        if (check) tower.currentUpgrades.Add(Upgrades.Dequeue());
    }
}

public abstract class TowerObject : MonoBehaviour, ITower
{
    public void Sell()
    {
        if (Global.RoundInProgress) return;

        // Calculate value of tower
        var value = Price + currentUpgrades.Sum(x => (float)x.UpgradePrice);

        PlayerInfo.Money += (long)(value/2); // Only give 50% of actual value back when selling

        Destroy(gameObject);
    }
    protected enum TowerUIPrefab
    {
        ShootTower,
        LaserTower,
    }

    [SerializeField] public string TowerName = "[Generic Tower Name]";
    [SerializeField] public string TowerDescription = "[Generic Tower Description]";
    [SerializeField] public float UpkeepPerWorker = 1f;
    [SerializeField] public float TowerUpkeep = 10f;

    [Tooltip("The base price of the tower")]
    [SerializeField] public ulong Price = 100;

    [HideInInspector] public abstract List<TowerUpgradePath> upgradePath { get; set; }
    [HideInInspector] public List<TowerUpgrade> currentUpgrades = new List<TowerUpgrade>();
    [HideInInspector] public ulong WorkerCount { get; protected set; } = 0;
    [HideInInspector] public ulong MinimumWorkerCount = 1;
    [HideInInspector] public ulong MaximumWorkerCount = 10;
    [HideInInspector] public bool IsActive { get { return WorkerCount >= MinimumWorkerCount && Global.RoundInProgress; } }
    [HideInInspector] public Vector2 Direction{ get { return Vector2.up.Rotate(transform.rotation.eulerAngles.z); } }
    [HideInInspector] public int TowerLevel = 1;
    [HideInInspector] protected GameObject UIPanel;

    [HideInInspector] protected Color DefaultTowerColor;
    [HideInInspector] protected Color DefaultGunColor;

    [HideInInspector] public abstract string TowerInfoDisplay { get; }
    protected virtual void Awake()
    {
        GameController.PlayerTowers.Add(this);
        DefaultTowerColor = GetComponent<SpriteRenderer>().color;
    }

    protected virtual void OnDestroy()
    {
        GameController.PlayerTowers.Remove(this);
    }

    /// <summary>
    /// This method should always be called in the Start() method of all inheited Towers inhereited from TowerObject
    /// </summary>
    /// <param name="name"></param>
    protected virtual void InstantiateUIPrefab(string name) // Always call this method when implementing TowerObject
    {
        var popup = UnityManager.GetPrefab(name);
        UIPanel = Instantiate(popup, transform);

        float camHeight = Camera.main.orthographicSize * 2f;

        float camWidth = camHeight * Camera.main.aspect;

        var canvas = UIPanel.GetComponentInChildren<Canvas>();

        var rectTransform = canvas.GetComponent<RectTransform>();

        var canvasWidth = rectTransform.lossyScale.x * rectTransform.sizeDelta.x;

        var canvasHeight = rectTransform.lossyScale.y * rectTransform.sizeDelta.y;

        if(UIPanel.transform.position.x + canvasWidth/2 + 3 > camWidth/2)
        {
            UIPanel.transform.Translate(new Vector2(-3, 0), Space.World);
        }
        else
        {
            UIPanel.transform.Translate(new Vector2(3, 0), Space.World);
        }

        if (UIPanel.transform.position.y/2 + canvasHeight/2 > camHeight/2 || UIPanel.transform.position.y/2 - canvasHeight/2 < -camHeight/2)
        {
            var posY = camHeight/2 - UIPanel.transform.position.y/2 - canvasHeight/2;
            UIPanel.transform.Translate(new Vector2(0,posY), Space.World);
        }

        UIPanel.transform.rotation = Quaternion.identity;
    }

    protected virtual void InstantiateUIPrefab(TowerUIPrefab prefab) =>
        InstantiateUIPrefab(prefab.ToString() + "InfoPopup");

    /// <summary>
    /// This method changes the worker count for each tower with the value of the value in the parameter
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Boolean indicating weather the value was changed</returns>
    public bool ChangeWorkerCount(long value)
    {
        if ((long)WorkerCount + value < 0) // Dont decreese workercount if the result is less than 0
            return false;

        if (value > 0 && WorkerCount + (ulong)value > MaximumWorkerCount)
            return false;

        if (PlayerInfo.Money < 0 && value > 0) // can't increse workers when you are in debt
            return false;

        if((long)GameController.AvaliableWorkers - value >= 0)
        {
            WorkerCount = (ulong)((long)WorkerCount + value);
            return true;
        }
        return false;
    }

    public void RemoveWorkers(ulong value)
    {
        if ((long)WorkerCount - (long)value < 0) // Dont decreese workercount if the result is less than 0
            return;

        WorkerCount -= value;

    }
    private bool mouseOver = false;
    // The Update() method should genereally only be used here, if overriding, call base.Update()
    protected virtual void Update()
    {
        // This is nessecary due to a unity bug where the OnMouse events are not invoked properly
        // The issue is probably described here http://t-machine.org/index.php/2015/03/14/fix-unity3ds-broken-onmousedown/
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        if (hits.Any(x => x.collider.gameObject == gameObject) && !hits.Any(x => x.collider.gameObject.TryGetComponent<Button>(out _)))
        {
            if (!mouseOver)
            {
                MouseOver();
                mouseOver = true;
            }

            if (Input.GetMouseButtonDown((int)UnityEngine.UIElements.MouseButton.LeftMouse))
            {
                // Make all other inactive:
                bool activate = !UIPanel.activeSelf;
                GameController.PlayerTowers.ForEach(x => x.UIPanel.SetActive(false));
                UIPanel.SetActive(activate);
            }
        }
        else if (mouseOver)
        {
            MouseNotOver();
            mouseOver = false;
        }
    }

    protected virtual void MouseOver() =>
        gameObject.GetComponent<SpriteRenderer>().color = DefaultTowerColor * 0.8f;

    protected virtual void MouseNotOver() =>
        gameObject.GetComponent<SpriteRenderer>().color = DefaultTowerColor;
}

public abstract class DefenceTower : TowerObject, IDefenceTower
{
    [Tooltip("The base DPS caused by the tower on enemies")]
    [SerializeField] public float DamagePerSecond = 100f;

    [Tooltip("The angle the tower can attack anemies")]
    [SerializeField] public float MaxTargetingAngle = 360f;

    [Tooltip("The range of the tower")]
    [SerializeField] public float Range = 100f;

    [Tooltip("The amount of targets the tower can have at once")]
    [SerializeField] protected int MaxTargets = 1;
    [HideInInspector] protected List<GameObject> CurrentTargets
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("Enemy")
                .Where(x => (x.GetComponent<EnemyScript>().Immunities & damageType) == 0) // Dont include enemies which are immune
                .Where(x => Vector2.Angle(Direction, gameObject.PointDirection(x)) <= MaxTargetingAngle/2) // Remove targets outside of tower view
                .Where(x => Vector2.Distance(x.transform.position, this.transform.position) <= Range).ToList(); // Remove targets outside of tower range
        }
    }

    [HideInInspector] private Transform RangeIndicator;
    [HideInInspector] private GameObject Gun;
    private (LineRenderer, LineRenderer) Lines;
    protected override void InstantiateUIPrefab(string name)
    {
        base.InstantiateUIPrefab(name);
        RangeIndicator = GetComponentsInChildren<Transform>(true).First(x => x.name == "RangeIndicator");
    }
    protected override void InstantiateUIPrefab(TowerUIPrefab prefab)
    {
        base.InstantiateUIPrefab(prefab);
        RangeIndicator = GetComponentsInChildren<Transform>(true).First(x => x.name == "RangeIndicator");
    }
    protected override void Update()
    {
        base.Update();
        RangeIndicator.localScale = Vector3.one * Range * 2;

        // Probably should be in Awake() or Start() to save on performance, but this is better for future upgrades for MaxTargetingAngle
        if (Lines.Item1.gameObject.activeSelf && MaxTargetingAngle < 360)
        {
            Lines.Item1.SetPositions(new Vector3[] { Vector3.zero, (Vector2.up * Range).Rotate(MaxTargetingAngle / 2) });
            Lines.Item2.SetPositions(new Vector3[] { Vector3.zero, (Vector2.up * Range).Rotate(-MaxTargetingAngle / 2) });
        }
    }
    protected override void MouseOver()
    {
        base.MouseOver();
        RangeIndicator.gameObject.SetActive(true);
        Lines.Item1.gameObject.SetActive(true);
        Lines.Item2.gameObject.SetActive(true);

        Gun.GetComponent<SpriteRenderer>().color = DefaultGunColor * 0.8f;
    }

    protected override void MouseNotOver()
    {
        base.MouseNotOver();
        RangeIndicator.gameObject.SetActive(false);
        Lines.Item1.gameObject.SetActive(false);
        Lines.Item2.gameObject.SetActive(false);

        Gun.GetComponent<SpriteRenderer>().color = DefaultGunColor;
    }

    protected override void Awake()
    {
        base.Awake();

        Gun = GetComponentsInChildren<Transform>().First(x => x.name == "Gun").gameObject;
        DefaultGunColor = Gun.GetComponent<SpriteRenderer>().color;

        var prefab = UnityManager.GetPrefab("AngleRay");

        Lines.Item1 = Instantiate(prefab, transform).GetComponent<LineRenderer>();
        Lines.Item2 = Instantiate(prefab, transform).GetComponent<LineRenderer>();
    }

    protected abstract DamageType damageType { get; }
}

/// <summary>
/// This class currentley contains noting and is only used for type checking
/// </summary>
public abstract class WorkerTower : TowerObject, IWorkerTower
{

}