using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class ShootTower : DefenceTower, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] protected ulong BulletCount = 0;
    [SerializeField] protected ulong MagazineSize = 10;
    [SerializeField] protected float ReloadTime = 3f;
    [SerializeField] protected float WaitTime = 1f;
    [HideInInspector] private float LastShotTime;
    [HideInInspector] private GameObject Gun;
    [HideInInspector] private Vector3 GunInitPos;

    void Awake()
    {
        InstantiateUIPrefab("ShootTowerInfoPopup");
        LastShotTime = Time.realtimeSinceStartup;
        Gun = GetComponentsInChildren<Transform>().ToList().First(x => x.name == "Gun").gameObject;
        GunInitPos = Gun.transform.localPosition;
    }

    void FixedUpdate()
    {
        if (CanShoot())
            Shoot();

        LookAtTarget();
    }

    void LookAtTarget()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();

        enemies = enemies.Where(x => Vector2.Angle(Direction, gameObject.PointDircetion(x)) <= MaxTargetingAngle).ToList();

        enemies = enemies.Where(x => Vector2.Distance(x.transform.position, this.transform.position) <= Range).ToList();

        if (enemies.Count() == 0)
        {
            Gun.transform.rotation = Quaternion.identity;
            Gun.transform.localPosition = GunInitPos;
            return;
        }

        enemies = enemies.SortByClosest(gameObject).ToList();

        var dir = gameObject.PointDircetion(enemies[0]);

        // https://answers.unity.com/questions/585035/lookat-2d-equivalent-.html
        Quaternion rotation = Quaternion.LookRotation
            (dir, transform.TransformDirection(Vector3.back));

        Gun.transform.rotation = new Quaternion(0, 0, rotation.z,rotation.w);

        Gun.transform.localPosition = GunInitPos.Rotate2D(Gun.transform.rotation.eulerAngles.z);
    }

    bool CanShoot()
    {
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

        script.SetValues(Vector2.up.Rotate(Gun.transform.rotation.eulerAngles.z), 0.3f, DamagePerSecond/60 * 1/WaitTime, "Enemy", 2);

        BulletCount--;
        LastShotTime = Time.realtimeSinceStartup;
    }

    public void OnPointerClick(PointerEventData eventData) => 
        UIPanel.SetActive(!UIPanel.activeSelf);
    public void OnPointerEnter(PointerEventData eventData) =>
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
    public void OnPointerExit(PointerEventData eventData) =>
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
}
