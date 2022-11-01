using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [HideInInspector] private bool ValuesSet = false;
    [HideInInspector] private Vector2 Direction;
    [HideInInspector] private float Speed;
    [HideInInspector] private float Damage;
    [HideInInspector] private string TargetTag;
    [HideInInspector] private int HitsToGoThrough;
    [HideInInspector] private List<GameObject> EnemiesHit = new List<GameObject>();

    [SerializeField] private GameObject MuzzleFlash;
    [SerializeField] private GameObject BulletImpact;
    public void SetValues(Vector2 direction,float speed, float damage, string targetTag = "Enemy", int hitsToGoThrough = 1)
    {
        Direction = direction.normalized;
        Speed = speed;
        Damage = damage;
        TargetTag = targetTag;
        HitsToGoThrough = hitsToGoThrough;
        ValuesSet = true;

        GameObject effect = Instantiate(MuzzleFlash, transform.position + (Vector3)Direction*0.3f, Quaternion.identity);
        effect.transform.localScale = Vector3.one * 0.5f;
        

        //effect.transform.localScale = Vector3.one;
        Destroy(effect, 0.2f);
    }
    private void Start()
    {
    }
    void FixedUpdate()
    {
        if (!ValuesSet)
            return;

        if (EnemiesHit.Count == HitsToGoThrough)
        {
            Destroy(gameObject);
            GameObject effect = Instantiate(BulletImpact, transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one;
            Destroy(effect, 10);
            return;
        }

        transform.Translate(Direction * Speed * Time.fixedDeltaTime, Space.World);
        
        var box = GetComponent<BoxCollider2D>();

        var hit = Physics2D.BoxCast(transform.position, box.size * 0.5f, 0f, Vector3.zero, 0, LayerMask.GetMask("Enemy"));

        if(hit)
        {
            var instance = hit.collider.gameObject;

            if (EnemiesHit.Contains(instance))
                return;

            EnemiesHit.Add(instance);

            var enemy = instance.GetComponent<EnemyScript>();

            enemy.Health -= Damage;
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
