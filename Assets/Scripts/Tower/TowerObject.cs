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
    [SerializeField] public ulong Price = 100;
    [HideInInspector] public long WorkerCount { get; protected set; } = 0;
    [HideInInspector] public long MinimumWorkerCount = 1;
    [HideInInspector] public long MaximumWorkerCount = 10;
    [HideInInspector] public bool IsActive { get { return WorkerCount >= MinimumWorkerCount; } }
    [HideInInspector] public Vector2 Direction{ get { return Vector2.up.Rotate(transform.rotation.eulerAngles.z); } }
    [HideInInspector] public int TowerLevel = 1;
    [HideInInspector] protected GameObject UIPanel;
    
    public virtual void InstantiateUIPrefab(string name) // Always use this Method when implementing TowerObject
    {
        var popup = UnityManager.GetPrefab(name);
        UIPanel = Instantiate(popup, transform);

        UIPanel.transform.Translate(new Vector2(-3, 0), Space.World);
        UIPanel.transform.rotation = Quaternion.identity;
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
    [SerializeField] public float MaxTargetingAngle = 360f;
    [SerializeField] protected float Range = 100f;
    [SerializeField] protected int MaxTargets = 1;
    [HideInInspector] protected List<GameObject> CurrentTargets
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("Enemy")
                .Where(x => Vector2.Angle(Direction, gameObject.PointDircetion(x)) <= MaxTargetingAngle/2) // Remove targets outside of tower view
                .Where(x => Vector2.Distance(x.transform.position, this.transform.position) <= Range).ToList(); // Remove targets outside of tower range
        }
    }

}

/// <summary>
/// This class currentley contains noting and is only used for type checking
/// </summary>
public abstract class WorkerTower : TowerObject, IWorkerTower
{

}