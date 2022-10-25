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
    
    /// <summary>
    /// This method should always be called in the Start() method of all inheited Towers inhereited from TowerObject
    /// </summary>
    /// <param name="name"></param>
    public virtual void InstantiateUIPrefab(string name) // Always call this method when implementing TowerObject
    {
        var popup = UnityManager.GetPrefab(name);
        UIPanel = Instantiate(popup, transform);

        float camHeight = Camera.main.orthographicSize * 2f;

        float camWidth = camHeight * Camera.main.aspect;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

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

    /// <summary>
    /// This method changes the worker count for each tower with the value of the value in the parameter
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Boolean indicating weather the value was changed</returns>
    public bool ChangeWorkerCount(long value)
    {
        if (WorkerCount + value < 0) // Dont decreese workercount if the result is less than 0
            return false;

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