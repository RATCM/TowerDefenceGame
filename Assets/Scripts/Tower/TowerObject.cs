using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerTypes;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine.UIElements;
using System;
using Mono.Cecil.Cil;
using System.IO;
using Microsoft.CodeAnalysis;
using System.Linq;

public abstract class TowerObject : MonoBehaviour, ITower
{

    protected enum TowerUIPrefab
    {
        ShootTower,
        LaserTower,
    }
    [SerializeField] public string TowerName = "[Generic Tower Name]";
    [SerializeField] public string TowerDescription = "[Generic Tower Description]";

    [Tooltip("The base price of the tower")]
    [SerializeField] public ulong Price = 100;
    [HideInInspector] public ulong WorkerCount { get; protected set; } = 0;
    [HideInInspector] public ulong MinimumWorkerCount = 1;
    [HideInInspector] public ulong MaximumWorkerCount = 10;
    [HideInInspector] public bool IsActive { get { return WorkerCount >= MinimumWorkerCount && Global.RoundInProgress; } }
    [HideInInspector] public Vector2 Direction{ get { return Vector2.up.Rotate(transform.rotation.eulerAngles.z); } }
    [HideInInspector] public int TowerLevel = 1;
    [HideInInspector] protected GameObject UIPanel;
    
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

    protected virtual void InstantiateUIPrefab(TowerUIPrefab prefab)
    {
        InstantiateUIPrefab(prefab.ToString() + "InfoPopup");
    }

    /// <summary>
    /// This method changes the worker count for each tower with the value of the value in the parameter
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Boolean indicating weather the value was changed</returns>
    public bool ChangeWorkerCount(long value)
    {
        if ((long)WorkerCount + value < 0) // Dont decreese workercount if the result is less than 0
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

    // The Update() method should only be used here
    void Update()
    {
        // This is nessecary due to a unity bug where the OnMouse events are not invoked properly
        // The issue is probably described here http://t-machine.org/index.php/2015/03/14/fix-unity3ds-broken-onmousedown/
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
        
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse) && hits.Any(x => x.collider.gameObject == gameObject))
        {
            UIPanel.SetActive(!UIPanel.activeSelf);
        }
    }

    void OnMouseEnter() =>
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);

    void OnMouseExit() =>
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
    
}

public abstract class DefenceTower : TowerObject, IDefenceTower
{
    [Tooltip("The base DPS caused by the tower on enemies")]
    [SerializeField] public float DamagePerSecond = 100f;

    [Tooltip("The angle the tower can attack anemies")]
    [SerializeField] public float MaxTargetingAngle = 360f;

    [Tooltip("The range of the tower")]
    [SerializeField] protected float Range = 100f;

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

    protected abstract DamageType damageType { get; }

}

/// <summary>
/// This class currentley contains noting and is only used for type checking
/// </summary>
public abstract class WorkerTower : TowerObject, IWorkerTower
{

}