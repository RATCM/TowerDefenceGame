using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class FreezeTower : DefenceTower
{
    public override List<TowerUpgradePath> upgradePath { get; set; } = new List<TowerUpgradePath>();
    [HideInInspector] FreezeGunScript Gun;
    [HideInInspector] Vector3 GunInitPos;
    [HideInInspector] protected override DamageType damageType { get => DamageType.Freeze; }
    [HideInInspector] private LineRenderer radiusIndicator;
    void Start()
    {
        InstantiateUIPrefab(TowerUIPrefab.ShootTower);
        Gun = GetComponentInChildren<FreezeGunScript>();
        GunInitPos = Gun.gameObject.transform.localPosition;

        upgradePath.Add(new TowerUpgradePath(
           new TowerUpgrade("Upgrade Range 25%", this, 50, delegate { Range *= 1.25f; }),
           new TowerUpgrade("Upgrade Range 25%", this, 100, delegate { Range *= 1.25f; }),
           new TowerUpgrade("Upgrade Range 25%", this, 200, delegate { Range *= 1.25f; })));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("Upgrade duration 25%", this, 50, delegate { Gun.SlowDownDuration *= 1.25f; }),
            new TowerUpgrade("Upgrade duration 25%", this, 100, delegate { Gun.SlowDownDuration *= 1.25f; }),
            new TowerUpgrade("Upgrade duration 25%", this, 200, delegate { Gun.SlowDownDuration *= 1.25f; })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("Upgrade slowdown 25%", this, 50, delegate { Gun.SlowDownValue *= 0.75f; }),
            new TowerUpgrade("Upgrade slowdown 25%", this, 100, delegate { Gun.SlowDownValue *= 0.75f; }),
            new TowerUpgrade("Upgrade slowdown 25%", this, 200, delegate { Gun.SlowDownValue *= 0.75f; })
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
    protected override void Update()
    {
        base.Update();
    }
}
