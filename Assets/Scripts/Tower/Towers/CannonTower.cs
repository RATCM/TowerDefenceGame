using System.Collections;
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
    // Start is called before the first frame update
    void Start()
    {
        // TODO: Change prefab
        InstantiateUIPrefab("ShootTowerInfoPopup");

        LastShotTime = Time.time;
        Gun = GetComponentsInChildren<Transform>().ToList().First(x => x.name == "Gun").gameObject;
        GunInitPos = Gun.transform.localPosition;

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

        var script = instance.GetComponent<CannonBallScript>();

        script.SetValues(Vector2.up.Rotate(Gun.transform.localEulerAngles.z), ProjectileSpeed, DamagePerSecond / currentReloadTime, DamageRadius, "Enemy");

        LastShotTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CanShoot())
            Shoot();

        LookAtTarget();
    }
}
