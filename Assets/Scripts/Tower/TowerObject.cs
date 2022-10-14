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
    [SerializeField] public string TowerName = "[Generic Tower Name]";
    [SerializeField] public string TowerDescription = "[Generic Tower Description]";
    [SerializeField] public float Price = 100;
    [HideInInspector] public long WorkerCount { get; protected set; } = 0;
    [HideInInspector] public long MinimumWorkerCount = 1;
    [HideInInspector] public long MaximumWorkerCount = 10;
    [HideInInspector] public bool IsActive { get { return WorkerCount >= MinimumWorkerCount; } }
    [HideInInspector] public Vector2 Direction = Vector2.right;
    [HideInInspector] public int TowerLevel = 1;
    [HideInInspector] protected GameObject UIPanel;
    
    public virtual void InstantiateUIPrefab(string name) // Always use this Method when implementing TowerObject
    {
        var popup = UnityManager.GetPrefab(name);
        UIPanel = Instantiate(popup, transform);
        UIPanel.transform.Translate(new Vector2(-3, 0));
    }
    public bool ChangeWorkerCount(long value)
    {
        if(GameController.AvaliableWorkers - value >= 0)
        {
            WorkerCount += value;
            return true;
        }
        return false;
    }
}

public abstract class DefenceTower : TowerObject, IDefenceTower
{
    [SerializeField] public float DamagePerSecond = 100f;
    [SerializeField] protected float Range = 100f;
    [SerializeField] protected float MaxTargetingAngle = 360f;
    [SerializeField] protected int MaxTargets = 1;
    [HideInInspector] protected List<GameObject> CurrentTargets = new List<GameObject>();
}

public abstract class WorkerTower : TowerObject, IWorkerTower
{

}