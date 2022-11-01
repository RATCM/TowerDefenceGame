using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShootTower : DefenceTower
{
    [HideInInspector] protected ulong BulletCount = 0;
    [Tooltip("The amount of bullets of the magazine of the gun before having to reload")]
    [SerializeField] protected ulong MagazineSize = 10;

    [Tooltip("The time it takes to reload the gun in seconds")]
    [SerializeField] protected float ReloadTime = 3f;

    [Tooltip("The time it takes between each shot in the gun")]
    [SerializeField] protected float WaitTime = 1f;

    [Tooltip("The amound of time to decreese the WaitTime by with each new worker")]
    [SerializeField] protected float WaitTimeMultiplier = 0.1f;

    [Tooltip("The speed of the bullets in units/second")]
    [SerializeField] protected float BulletSpeed = 18f;
    [HideInInspector] protected float currentWaitTime { get { return 1f / (1f / (ReloadTime) + WaitTimeMultiplier * (WorkerCount - MinimumWorkerCount)); } }
    [HideInInspector] private float LastShotTime;
    [HideInInspector] private GameObject Gun;
    [HideInInspector] private Vector3 GunInitPos;
    [HideInInspector] protected override DamageType damageType { get => DamageType.Projectiles; }
    [HideInInspector] public override List<TowerUpgradePath> upgradePath { get; set; } = new List<TowerUpgradePath>();

    void Start()
    {
        InstantiateUIPrefab("ShootTowerInfoPopup");
        LastShotTime = Time.time;
        Gun = GetComponentsInChildren<Transform>().ToList().First(x => x.name == "Gun").gameObject;
        GunInitPos = Gun.transform.localPosition;

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +25 Range", this, 200, delegate { Range *= 1.25f; }),
            new TowerUpgrade("(2) +25 Range", this, 400, delegate { Range *= 1.25f; }),
            new TowerUpgrade("(3) +25 Range", this, 800, delegate { Range *= 1.25f; })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +25% DPS", this, 1000, delegate { DamagePerSecond *= 1.25f; }),
            new TowerUpgrade("(2) +25% DPS", this, 2000, delegate { DamagePerSecond *= 1.25f; }),
            new TowerUpgrade("(3) +25% DPS", this, 4000, delegate { DamagePerSecond *= 1.25f; })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) -25% Upkeep", this, 100, delegate { UpkeepPerWorker *= 0.75f; }),
            new TowerUpgrade("(2) -25% Upkeep", this, 200, delegate { UpkeepPerWorker *= 0.75f; }),
            new TowerUpgrade("(3) -25% Upkeep", this, 400, delegate { UpkeepPerWorker *= 0.75f; })
            ));
    }

    void FixedUpdate()
    {
        LookAtTarget();

        if (CanShoot())
            Shoot();
    }
    // Code for this is from https://www.gamedeveloper.com/programming/shooting-a-moving-target
    float CalculateBulletTravelTime(Vector2 delta, Vector2 vr, float muzzleV)
    {
        float a = Vector2.Dot(vr, vr) - muzzleV * muzzleV;
        float b = 2f * Vector2.Dot(vr, delta);
        float c = Vector2.Dot(delta, delta);

        float det = b * b - 4f * a * c;

        if (det > 0f)
            return 2f * c / (Mathf.Sqrt(det)-b);
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

        var enemyScript = closest.GetComponent<EnemyScript>();
        var pathFindingScript = closest.GetComponent<PathFinding>();

        Vector2 delta = (closest.transform.position - gameObject.transform.position);

        Vector2 dir;
        if(closest.transform.position.x < 2.5f)
        {
            dir = pathFindingScript.CurrentDirection;
        }
        else
        {
            dir = new Vector2(1, -1).normalized;
        }

        Vector2 vr = dir * enemyScript.CurrentSpeed;

        var t = CalculateBulletTravelTime(delta, vr, BulletSpeed);

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

        Gun.transform.rotation = new Quaternion(0, 0, rotation.z,rotation.w);

        Gun.transform.localPosition = GunInitPos.Rotate2D(Gun.transform.localEulerAngles.z);
    }

    bool CanShoot()
    {
        if (!IsActive || CurrentTargets.Count == 0)
            return false;

        if (BulletCount <= 0){
            return LastShotTime + ReloadTime <= Time.time;
        }
        return LastShotTime + currentWaitTime <= Time.time;
    }

    void Shoot()
    {
        if (BulletCount == 0)
            BulletCount = MagazineSize;

        var bullet = UnityManager.GetPrefab("Bullet");
        var instance = Instantiate(bullet, Gun.transform.position,Gun.transform.rotation);

        var script = instance.GetComponent<BulletScript>();

        script.SetValues(Vector2.up.Rotate(Gun.transform.rotation.eulerAngles.z), BulletSpeed, DamagePerSecond * WaitTime, "Enemy", 1);

        BulletCount--;
        LastShotTime = Time.time;
    }
}
