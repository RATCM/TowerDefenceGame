using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerTypes;
using Unity.VisualScripting;
using System;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class LaserTower : DefenceTower, ILaser, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Fields in inspector
    [HideInInspector] public float MinimumEnergy = 50f; 
    [HideInInspector] public float MaximumEnergy = 100f;
    [HideInInspector] public float OptimalEnergy = 50f;
    [HideInInspector] protected float CurrentTemprature = 0;
    [HideInInspector] protected bool BeamRunning = false;
    [HideInInspector] private GameObject rayPrefab;
    [HideInInspector] private GameObject laserRay;
    [HideInInspector] protected bool OnCooldown = false;

    [SerializeField] protected float MinimumTemperature = 0;
    [SerializeField] protected float MaximumTemperature = 100;
    [Range(50f, 100f)] [SerializeField] public float EnergyUse = 50f;
    #endregion

    public void StartBeam()
    {
        if (!BeamRunning && !CurrentTargets.IsNullOrEmpty())
        {
            laserRay = Instantiate(rayPrefab, gameObject.transform);

            LineRenderer lr = laserRay.GetComponent<LineRenderer>();

            lr.SetPositions(new Vector3[] { Vector3.zero, CurrentTargets[0].transform.position - gameObject.transform.position });
            BeamRunning = true;
        }
    }
    public void EndBeam()
    {
        BeamRunning = false;

        if(laserRay != null)
            Destroy(laserRay);
    }

    public void UpdateTemperature()
    {
        const float multiplier = 0.005f;
        if (BeamRunning)
        {
            CurrentTemprature += (EnergyUse - OptimalEnergy) * multiplier;
        }
        else
        {
            CurrentTemprature -= (EnergyUse - OptimalEnergy) * multiplier;
        }

        UpdateBeam();
    }
    public void UpdateTargets()
    {
        // Get all enemies in scene
        var enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();

        // Remove all enemies not in view
        enemies = enemies.Where(x => Vector2.Angle(Direction, gameObject.PointDircetion(x)) <= MaxTargetingAngle).ToList();

        enemies = enemies.Where(x => Vector2.Distance(x.transform.position, this.transform.position) <= Range).ToList();

        // Sort enemies by closest
        enemies = enemies.SortByClosest(gameObject).ToList();

        // Reset current targets
        CurrentTargets = new List<GameObject>(); 

        int total = MaxTargets < enemies.Count()
            ? MaxTargets
            : enemies.Count();

        for (int i = 0; i < total; i++)
        {
            CurrentTargets.Add(enemies[i]);
        }
    }
    void UpgradeTower()
    {
        TowerLevel++;
    }
    void UpdateBeam()
    {
        if (CurrentTemprature <= MinimumTemperature) // Remove cooldown when temperature is at minimum
            OnCooldown = false;

        if (OnCooldown) // Asserts that the beam is off during cooldown
        {
            BeamRunning = false;

            return;
        }

        if (CurrentTargets.IsNullOrEmpty()) // Asserts that beam is off is there is no avaliable targets
        {
            BeamRunning = false;
            return;
        }

        if(CurrentTemprature > MaximumEnergy) // Activates cooldown when the temperature exeeds the max temperature
        {
            OnCooldown = true;

            return;
        }

        BeamRunning = true;

    }
    void UpdateLaser()
    {
        if (BeamRunning)
        {
            // Instantiate laser
            if(laserRay  == null)
            {
                laserRay = Instantiate(rayPrefab, gameObject.transform);

                LineRenderer lr = laserRay.GetComponent<LineRenderer>();

                lr.SetPositions(new Vector3[] { Vector3.zero, CurrentTargets[0].transform.position - gameObject.transform.position });
                BeamRunning = true;
            }
            else // Update laser
            {
                LineRenderer lr = laserRay.GetComponent<LineRenderer>();

                lr.SetPositions(new Vector3[] { Vector3.zero, CurrentTargets[0].transform.position - gameObject.transform.position }); // Update the position of laser
                var enemy = CurrentTargets[0].GetComponent<EnemyScript>();

                enemy.Health -= DamagePerSecond * 1 / 60f; // remove health of enemy
            }
        }
        else if(laserRay != null) // Destroy the laser
        {
            Destroy(laserRay);
            laserRay = null;
        }
    }
    void Awake()
    {
        InstantiateUIPrefab("LaserTowerInfoPopup");
    }
    void Start()
    {
        rayPrefab = UnityManager.GetPrefab("LaserRay");
        //if (WorkerCount < MinimumWorkerCount)
        //    IsActive = false;
        //else
        //    IsActive = true;
    }
    void FixedUpdate()
    {
        if (!IsActive)
            return;

        UpdateTemperature();
        UpdateTargets();
        UpdateBeam();
        UpdateLaser();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) =>
        UIPanel.SetActive(!UIPanel.activeSelf);
    void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData) =>
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
    void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData) =>
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
}