using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerTypes;

public abstract class TowerObject : MonoBehaviour, ITower
{
    [SerializeField] public string TowerName = "[Generic Tower Name]";
    [SerializeField] public string TowerDescription = "[Generic Tower Description]";
    [SerializeField] public float Price = 100;
    [HideInInspector] public long WorkerCount = 0;
    [HideInInspector] public long MinimumWorkerCount = 1;
    [HideInInspector] public long MaximumWorkerCount = 10;
    [HideInInspector] public bool IsActive = true;
    [HideInInspector] public Vector2 Direction = Vector2.right;
    [HideInInspector] public int TowerLevel = 1;
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