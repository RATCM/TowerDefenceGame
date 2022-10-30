using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FreezeTower : DefenceTower
{
    [HideInInspector] FreezeGunScript Gun;
    [HideInInspector] Vector3 GunInitPos;
    //protected new List<GameObject> CurrentTargets
    //{
    //    get
    //    {
    //        return base.CurrentTargets.Where(x => !x.GetComponent<EnemyScript>().CurrentEffects.Any(x => x is TempFreeze)).ToList();
    //    }
    //}
    // Start is called before the first frame update
    void Start()
    {
        InstantiateUIPrefab(TowerUIPrefab.ShootTower);
        Gun = GetComponentInChildren<FreezeGunScript>();
        GunInitPos = Gun.gameObject.transform.localPosition;
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
}
