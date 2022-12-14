using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CannonTower : DefenceTower
{
    [SerializeField] protected float DamageRadius = 1f;
    [SerializeField] protected float ReloadTime = 3f;
    [SerializeField] protected float ReloadTimeMultiplier = 0.1f;
    [SerializeField] protected float ProjectileSpeed = 15f;
    [HideInInspector] private float currentReloadTime { get { return 1f/(1f/(ReloadTime) + ReloadTimeMultiplier * (WorkerCount - MinimumWorkerCount)); } }
    [HideInInspector] private float LastShotTime = 0;
    [HideInInspector] private GameObject Gun;
    [HideInInspector] private Vector3 GunInitPos;
    [HideInInspector] protected override DamageType damageType { get => DamageType.Explosion; }
    [HideInInspector] public override List<TowerUpgradePath> upgradePath { get; set; } = new List<TowerUpgradePath>();

    public override string TowerInfoDisplay =>
        $"Range: {MathF.Round(Range,1)}\n units\n" +
        $"DPS: {(long)(DamagePerSecond * (WorkerCount >= MinimumWorkerCount ? ReloadTime / currentReloadTime : 0))}\n" +
        $"Blast Radius: {MathF.Round(DamageRadius,1)} units\n" +
        $"Upkeep: {(long)(WorkerCount >= MinimumWorkerCount ? UpkeepPerWorker * WorkerCount + TowerUpkeep : 0)}$ per round\n" +
        $"Is Active: {(WorkerCount >= MinimumWorkerCount ? "Yes" : "No")}";


    // Start is called before the first frame update
    void Start()
    {
        // TODO: Change prefab
        InstantiateUIPrefab("ShootTowerInfoPopup");

        LastShotTime = Time.time;
        Gun = GetComponentsInChildren<Transform>().ToList().First(x => x.name == "Gun").gameObject;
        GunInitPos = Gun.transform.localPosition;

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +25% Range", this, 400, delegate { Range *= 1.25f; DefaultTowerColor = Color.green; MouseNotOver(); }),
            new TowerUpgrade("(2) +25% Range", this, 800, delegate { Range *= 1.25f; DefaultTowerColor = Color.blue; MouseNotOver(); }),
            new TowerUpgrade("(3) +25% Range", this, 1600, delegate { Range *= 1.25f; DefaultTowerColor = Color.red; MouseNotOver(); })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +25% DPS", this, 2000, delegate { DamagePerSecond *= 1.25f; DefaultGunColor = Color.green; MouseNotOver(); }),
            new TowerUpgrade("(2) +25% DPS", this, 4000, delegate { DamagePerSecond *= 1.25f; DefaultGunColor = Color.blue; MouseNotOver(); }),
            new TowerUpgrade("(3) +25% Blast Radius", this, 8000, delegate { DamageRadius *= 1.25f; DefaultGunColor = Color.red; MouseNotOver(); })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +25% Maximum worker count", this, 500, delegate { MaximumWorkerCount = (ulong)(MaximumWorkerCount * 1.25f); Gun.gameObject.transform.localScale *= 1.1f; }),
            new TowerUpgrade("(2) +25% Maximum worker count", this, 1000, delegate { MaximumWorkerCount = (ulong)(MaximumWorkerCount * 1.25f); Gun.gameObject.transform.localScale *= 1.1f; }),
            new TowerUpgrade("(3) +25% Maximum worker count", this, 2000, delegate { MaximumWorkerCount = (ulong)(MaximumWorkerCount * 1.25f); Gun.gameObject.transform.localScale *= 1.1f; })
            ));
    }

    float CalculateBulletTravelTime(Vector2 delta, Vector2 vr, float muzzleV)
    {
        float a = Vector2.Dot(vr, vr) - muzzleV * muzzleV;
        float b = 2f * Vector2.Dot(vr, delta);
        float c = Vector2.Dot(delta, delta);

        float det = b * b - 4f * a * c;

        if (det > 0f)
            return 2f * c / (Mathf.Sqrt(det) - b);
        else
            return -1f;
    }

    /// <summary>
    /// This calculates the end position of a target after a bullet has been shot
    /// </summary>
    /// <returns>The position of the target when it gets hit by bullet</returns>
    Vector2 CalculateTargetPos()
    {
        // This requires some fancy math

        var closest = CurrentTargets.GetClosest(gameObject);

        if (closest == null)
            return Vector2.down;

        //var dist = gameObject.Distance2D(closest);


        //var bulletTravelTime = BulletSpeed * dist; // In seconds



        var enemyScript = closest.GetComponent<EnemyScript>();
        var pathFindingScript = closest.GetComponent<PathFinding>();

        Vector2 delta = (closest.transform.position - gameObject.transform.position);

        Vector2 dir;
        if (closest.transform.position.x < 2.5f) // The map gets a little weird after this point
        {
            dir = pathFindingScript.CurrentDirection;
        }
        else
        {
            dir = new Vector2(1, -1).normalized;
        }

        Vector2 vr = dir * enemyScript.CurrentSpeed;

        var t = CalculateBulletTravelTime(delta, vr, ProjectileSpeed);

        if (t < 0f)
            return Vector2.down;

        var enemyEndPos = (Vector2)closest.transform.position + dir * enemyScript.CurrentSpeed * t;

        return (enemyEndPos - (Vector2)gameObject.transform.position).normalized;
    }
    void LookAtTarget()
    {
        if (CurrentTargets.Count == 0)
        {
            Gun.transform.rotation = transform.rotation;
            Gun.transform.localPosition = GunInitPos;
            return;
        }

        var dir = CalculateTargetPos();

        // https://answers.unity.com/questions/585035/lookat-2d-equivalent-.html
        Quaternion rotation = Quaternion.LookRotation
            (dir, transform.TransformDirection(Vector3.back));

        Gun.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

        Gun.transform.localPosition = GunInitPos.Rotate2D(Gun.transform.localEulerAngles.z);
    }

    bool CanShoot() =>
        IsActive && CurrentTargets.Count > 0 && LastShotTime + currentReloadTime <= Time.time;

    void Shoot()
    {
        var bullet = UnityManager.GetPrefab("CannonBall");
        var instance = Instantiate(bullet, transform);
        instance.transform.position = Gun.transform.position;

        var script = instance.GetComponent<CannonBallScript>();

        script.SetValues(Vector2.up.Rotate(Gun.transform.localEulerAngles.z), ProjectileSpeed, DamagePerSecond * ReloadTime, DamageRadius, "Enemy");

        LastShotTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LookAtTarget();

        if (CanShoot())
            Shoot();

    }
}
