using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class ShootTower : DefenceTower
{
    [HideInInspector] protected ulong BulletCount = 0;
    [SerializeField] protected ulong MagazineSize = 10;
    [SerializeField] protected float ReloadTime = 3f;
    [SerializeField] protected float WaitTime = 1f;
    [HideInInspector] private float LastShotTime;
    [HideInInspector] private GameObject Gun;
    [HideInInspector] private Vector3 GunInitPos;
    void Start()
    {
        InstantiateUIPrefab("ShootTowerInfoPopup");
        LastShotTime = Time.realtimeSinceStartup;
        Gun = GetComponentsInChildren<Transform>().ToList().First(x => x.name == "Gun").gameObject;
        GunInitPos = Gun.transform.localPosition;
    }

    void FixedUpdate()
    {
        if (!IsActive)
            return;

        if (CanShoot())
            Shoot();

        LookAtTarget();
    }

    void LookAtTarget()
    {
        if (CurrentTargets.Count == 0)
        {
            Gun.transform.rotation = transform.rotation;
            Gun.transform.localPosition = GunInitPos;
            return;
        }

        var dir = gameObject.PointDircetion(CurrentTargets.GetClosest(gameObject));

        // https://answers.unity.com/questions/585035/lookat-2d-equivalent-.html
        Quaternion rotation = Quaternion.LookRotation
            (dir, transform.TransformDirection(Vector3.back));

        Gun.transform.rotation = new Quaternion(0, 0, rotation.z,rotation.w);

        Gun.transform.localPosition = GunInitPos.Rotate2D(Gun.transform.localEulerAngles.z);
    }

    bool CanShoot()
    {
        if (CurrentTargets.Count == 0)
            return false;

        if (BulletCount == 0){
            return LastShotTime + ReloadTime <= Time.realtimeSinceStartup;
        }
        return LastShotTime + WaitTime <= Time.realtimeSinceStartup;
    }

    void Shoot()
    {
        var bullet = UnityManager.GetPrefab("Bullet");
        var instance = Instantiate(bullet, transform);

        var script = instance.GetComponent<BulletScript>();

        script.SetValues(Vector2.up.Rotate(Gun.transform.localEulerAngles.z), 0.3f, DamagePerSecond/60 * 1/WaitTime, "Enemy", 2);

        BulletCount--;
        LastShotTime = Time.realtimeSinceStartup;
    }
}
