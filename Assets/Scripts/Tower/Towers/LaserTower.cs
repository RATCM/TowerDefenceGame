using System.Collections.Generic;
using UnityEngine;
using TowerTypes;
using System;
using System.Linq;

public class LaserTower : DefenceTower, ILaser
{
    [HideInInspector] public float MinimumEnergy = 50f; 
    [HideInInspector] public float MaximumEnergy = 100f;
    [HideInInspector] public float OptimalEnergy = 50f;
    [HideInInspector] protected float CurrentTemprature = 0;
    [HideInInspector] private GameObject rayPrefab;
    [HideInInspector] private GameObject laserRay;
    [HideInInspector] private GameObject Gun;
    [HideInInspector] protected bool OnCooldown = false;
    [HideInInspector] protected override DamageType damageType { get => DamageType.Laser; }
    [HideInInspector] public override List<TowerUpgradePath> upgradePath { get; set; } = new List<TowerUpgradePath>();

    [SerializeField] GameObject SmokeEffect;

    [Tooltip("The the lowest temperature the tower can be at one time")]
    [SerializeField] protected float MinimumTemperature = 0;

    [Tooltip("The maximum temperature the tower can be, before it has to cool down")]
    [SerializeField] protected float MaximumTemperature = 100;

    [Tooltip("The energy used by the tower, the lowest value wont increese the temperature while also causing the least damage")]
    [Range(50f, 100f)] [SerializeField] public float EnergyUse = 50f;

    public override string TowerInfoDisplay =>
        $"Range: {MathF.Round(Range,1)}\n" +
        $"DPS: {(long)(WorkerCount >= MinimumWorkerCount ? DamagePerSecond : 0)}\n" +
        $"Minimum Temperature: {(long)MinimumTemperature}\n" +
        $"Maximum Temperature: {(long)MaximumTemperature}\n" +
        $"Current Temperature: {(long)CurrentTemprature}\n" +
        $"Upkeep: {(long)(WorkerCount >= MinimumWorkerCount ? UpkeepPerWorker * WorkerCount + TowerUpkeep : 0)}$ per round\n" +
        $"Is Active: {(WorkerCount >= MinimumWorkerCount ? "Yes" : "No")}";

    void Start()
    {
        InstantiateUIPrefab("LaserTowerInfoPopup");
        rayPrefab = UnityManager.GetPrefab("LaserRay");
        Gun = GetComponentsInChildren<Transform>().ToList().First(x => x.name == "Gun").gameObject;

        laserRay = Instantiate(rayPrefab, Gun.transform);
        laserRay.SetActive(false);

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +25% Range", this, 50, delegate { Range *= 1.25f; }),
            new TowerUpgrade("(2) +25% Range", this, 100, delegate { Range *= 1.25f; }),
            new TowerUpgrade("(3) +25% Range", this, 200, delegate { Range *= 1.25f; })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +25% DPS", this, 100, delegate { DamagePerSecond *= 1.25f; }),
            new TowerUpgrade("(2) +25% DPS", this, 200, delegate { DamagePerSecond *= 1.25f; }),
            new TowerUpgrade("(3) +25% DPS", this, 400, delegate { DamagePerSecond *= 1.25f; })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +50% Tempearture capacity 50%", this, 100, delegate { MaximumTemperature *= 1.5f; }),
            new TowerUpgrade("(2) +50% Tempearture capacity 50%", this, 200, delegate { MaximumTemperature *= 1.5f; }),
            new TowerUpgrade("(3) +50% Tempearture capacity 50%", this, 400, delegate { MaximumTemperature *= 1.5f; })
            ));
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
        var freezeTowers = GameController.PlayerTowers
            .Where(x => x.GetType() == typeof(FreezeTower))
            .Where(x => x.IsActive)
            .Where(x => Vector2.Distance(x.transform.position, transform.position) < ((FreezeTower)x).Range);

        float decreseMultiplier = 0.005f;
        decreseMultiplier *= freezeTowers.Count() + 1f;

        // more workers makes the temperature increese more slowly
        // This could divide by zero but it wont matter in this case
        float increseMultiplier = decreseMultiplier / (WorkerCount - MinimumWorkerCount + 1);
        increseMultiplier *= 1f / (freezeTowers.Count() + 1);
        
        if (laserRay.activeSelf)
        {
            CurrentTemprature += (EnergyUse - OptimalEnergy) * increseMultiplier;
        }
        else if(CurrentTemprature > MinimumTemperature)
        {
            CurrentTemprature -= (EnergyUse - OptimalEnergy) * decreseMultiplier;
        }

        if (IsActive)
        {
            var b = "ruh";
        }
        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, CurrentTemprature / MaximumTemperature);
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
            var ins = Instantiate(SmokeEffect, transform.position, Quaternion.identity);
            ins.transform.localScale = Vector3.one * 0.25f;

            var ps = ins.GetComponentsInChildren<ParticleSystem>().ToList();
            ps.ForEach(x => x.Stop());
            var main = ps.Select(x => x.main).ToList();
            main.ForEach(x => x.duration = (MaximumTemperature - MinimumTemperature) * 0.025f);
            ps.ForEach(x => x.Play());

            Destroy(ins, (MaximumTemperature - MinimumTemperature)*0.075f);
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