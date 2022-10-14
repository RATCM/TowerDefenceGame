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
    public void SetValues(Vector2 direction,float speed, float damage, string targetTag = "Enemy", int hitsToGoThrough = 1)
    {
        Direction = direction.normalized;
        Speed = speed;
        Damage = damage;
        TargetTag = targetTag;
        HitsToGoThrough = hitsToGoThrough;
        ValuesSet = true;
    }
    void FixedUpdate()
    {
        if (!ValuesSet)
            return;

        if (EnemiesHit.Count == HitsToGoThrough)
        {
            Destroy(gameObject);
            return;
        }

        transform.Translate(Direction * Speed);
        
        var box = GetComponent<BoxCollider2D>();

        var hit = Physics2D.BoxCast(transform.position, box.size, 0f, Vector3.zero);

        if(hit && hit.collider.tag == TargetTag)
        {
            var instance = hit.collider.gameObject;

            if (EnemiesHit.Contains(instance))
                return;

            EnemiesHit.Add(instance);

            var enemy = instance.GetComponent<EnemyScript>();

            enemy.Health -= Damage;
        }
        //Debug.Log(count);
    //    if (count > 0)
    //    {
    //        var instance = results[0].gameObject;

    //        if (EnemiesHit.Contains(instance))
    //            return;

    //        EnemiesHit.Add(instance);

    //        var enemy = instance.GetComponent<EnemyScript>();

    //        enemy.Health -= Damage;
    //    }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
