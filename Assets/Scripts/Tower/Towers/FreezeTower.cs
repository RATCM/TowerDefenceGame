using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FreezeTower : DefenceTower
{
    public override List<TowerUpgradePath> upgradePath { get; set; } = new List<TowerUpgradePath>();
    [HideInInspector] FreezeGunScript Gun;
    [HideInInspector] Vector3 GunInitPos;
    [HideInInspector] protected override DamageType damageType { get => DamageType.Freeze; }
    [HideInInspector] private LineRenderer radiusIndicator;
    public override string TowerInfoDisplay =>
        $"Range: {MathF.Round(Range)} units\n" +
        $"Duration: {MathF.Round(Gun.actualFreezeDuration,1)} seconds\n" +
        $"Slowdown value: {(long)(Gun.SlowDownValue*100)}%\n" +
        $"Upkeep: {(long)(WorkerCount >= MinimumWorkerCount ? UpkeepPerWorker * WorkerCount + TowerUpkeep : 0)}$ per round\n" +
        $"Is Active: {(WorkerCount >= MinimumWorkerCount ? "Yes" : "No")}";
    void Start()
    {
        InstantiateUIPrefab(TowerUIPrefab.ShootTower);
        Gun = GetComponentInChildren<FreezeGunScript>();
        GunInitPos = Gun.gameObject.transform.localPosition;

        upgradePath.Add(new TowerUpgradePath(
           new TowerUpgrade("(1) +25% Range", this, 50, delegate { Range *= 1.25f; DefaultTowerColor = Color.green; MouseNotOver(); }), // MouseOver() invocation is to update the sprite
           new TowerUpgrade("(2) +25% Range", this, 100, delegate { Range *= 1.25f; DefaultTowerColor = Color.blue; MouseNotOver(); }),
           new TowerUpgrade("(3) +25% Range", this, 200, delegate { Range *= 1.25f; DefaultTowerColor = Color.red; MouseNotOver(); })
           ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +15% Duration", this, 50, delegate { Gun.SlowDownDuration *= 1.15f; DefaultGunColor = Color.green; MouseNotOver(); }),
            new TowerUpgrade("(2) +15% Duration", this, 100, delegate { Gun.SlowDownDuration *= 1.15f; DefaultGunColor = Color.blue; MouseNotOver(); }),
            new TowerUpgrade("(3) +15% Duration", this, 200, delegate { Gun.SlowDownDuration *= 1.15f; DefaultGunColor = Color.red; MouseNotOver(); })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +10% Slowdown", this, 50, delegate { Gun.SlowDownValue *= 0.9f; Gun.gameObject.transform.localScale *= 1.1f; }),
            new TowerUpgrade("(2) +10% Slowdown", this, 100, delegate { Gun.SlowDownValue *= 0.9f; Gun.gameObject.transform.localScale *= 1.1f; }),
            new TowerUpgrade("(3) +10% Slowdown", this, 200, delegate { Gun.SlowDownValue *= 0.9f; Gun.gameObject.transform.localScale *= 1.1f; })
            ));
    }

    void LookAtTarget()
    {
        var dir = (CurrentTargets.GetClosest(gameObject).transform.position - transform.position).normalized;

        Quaternion rotation = Quaternion.LookRotation
            (dir, transform.TransformDirection(Vector3.back));

        Gun.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

        Gun.transform.localPosition = GunInitPos.Rotate2D(Gun.transform.localEulerAngles.z);
    }

    bool CanShoot() =>
        IsActive && CurrentTargets.Count > 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CanShoot())
        {
            Gun.Activate(CurrentTargets.GetClosest(gameObject));
            LookAtTarget();
        }
        else
        {
            Gun.UnActivate();
            Gun.ResetPosition();
        }
    }
    //protected override void Update()
    //{
    //    base.Update();
    //}
}
