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

public class LaserTower : DefenceTower, ILaser
{
    #region Fields in inspector
    [HideInInspector] public float MinimumEnergy = 50f; 
    [HideInInspector] public float MaximumEnergy = 100f;
    [HideInInspector] public float OptimalEnergy = 50f;
    [HideInInspector] protected float CurrentTemprature = 0;
    [HideInInspector] private GameObject rayPrefab;
    [HideInInspector] private GameObject laserRay;
    [HideInInspector] private GameObject Gun;
    [HideInInspector] protected bool OnCooldown = false;

    [Tooltip("The the lowest temperature the tower can be at one time")]
    [SerializeField] protected float MinimumTemperature = 0;

    [Tooltip("The maximum temperature the tower can be, before it has to cool down")]
    [SerializeField] protected float MaximumTemperature = 100;

    [Tooltip("The energy used by the tower, the lowest value wont increese the temperature while also causing the least damage")]
    [Range(50f, 100f)] [SerializeField] public float EnergyUse = 50f;


    #endregion
    void Start()
    {
        InstantiateUIPrefab("LaserTowerInfoPopup");
        rayPrefab = UnityManager.GetPrefab("LaserRay");
        Gun = GetComponentsInChildren<Transform>().ToList().First(x => x.name == "Gun").gameObject;

        laserRay = Instantiate(rayPrefab, Gun.transform);
        laserRay.SetActive(false);
    }

    bool CanShoot() =>
        IsActive && CurrentTargets.Count > 0 && !OnCooldown;

    public void StartBeam()
    {
        laserRay.SetActive(true);
    }
    public void EndBeam()
    {
        laserRay.SetActive(false);
    }

    public void UpdateTemperature()
    {
        float multiplier = 0.005f;
        if (laserRay.activeSelf)
        {
            CurrentTemprature += (EnergyUse - OptimalEnergy) * multiplier/(WorkerCount - MinimumWorkerCount + 1); // more workers makes the temperature increese more slowly
        }
        else
        {
            CurrentTemprature -= (EnergyUse - OptimalEnergy) * multiplier;
        }

        UpdateLaserStatus();
    }
    void UpdateLaserStatus()
    {
        if (CurrentTemprature <= MinimumTemperature)// Remove cooldown when temperature is at minimum
        {
            OnCooldown = false;
            return;
        }
        else if(CurrentTemprature >= MaximumTemperature) // Activates cooldown when the temperature exeeds the max temperature
        {
            OnCooldown = true;
            return;
        }
    }

    void AimAtTarget()
    {
        if (!CanShoot())
        {
            Gun.transform.rotation = transform.rotation;
            return;
        }

        LineRenderer lr = laserRay.GetComponent<LineRenderer>();

        var enemy = CurrentTargets.GetClosest(gameObject).GetComponent<EnemyScript>();

        var dir = enemy.transform.position - transform.position;

        // https://answers.unity.com/questions/585035/lookat-2d-equivalent-.html
        Quaternion rotation = Quaternion.LookRotation
            (dir, transform.TransformDirection(Vector3.back));

        Gun.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

        lr.SetPositions(
            new Vector3[] {
                    Vector3.zero,
                    Vector3.up * (transform.position-enemy.transform.position).magnitude}); // Update the position of laser

        enemy.Health -= DamagePerSecond / 60f * (EnergyUse / MinimumEnergy); // damage enemy
    }

    void FixedUpdate()
    {
        if (CanShoot())
            StartBeam();
        else
            EndBeam();

        AimAtTarget();
        UpdateTemperature();
        UpdateLaserStatus();
    }
}