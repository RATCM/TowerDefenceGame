using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallScript : MonoBehaviour
{
    [HideInInspector] private bool ValuesSet = false;
    [HideInInspector] private Vector2 Direction;
    [HideInInspector] private float Speed;
    [HideInInspector] private float Damage;
    [HideInInspector] private float Radius;
    [HideInInspector] private string TargetTag;

    [SerializeField] private GameObject ExplosionEffect;

    public void SetValues(Vector2 direction, float speed, float damage, float radius, string targetTag = "Enemy")
    {
        Direction = direction.normalized;
        Speed = speed;
        Damage = damage;
        Radius = radius;
        TargetTag = targetTag;
        ValuesSet = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!ValuesSet)
            return;

        transform.Translate(Direction * Speed * Time.fixedDeltaTime);

        var box = GetComponent<CircleCollider2D>();

        var hit = Physics2D.CircleCast(transform.position, box.radius, Vector3.zero);

        if (hit && hit.collider.tag == TargetTag)
        {
            var enemies = Physics2D.CircleCastAll(hit.transform.position, Radius, Vector2.zero,0,LayerMask.GetMask("Enemy"));

            foreach(var enemy in enemies)
            {
                var enemyScript = enemy.collider.gameObject.GetComponent<EnemyScript>();

                enemyScript.Health -= Damage;
            }

            Destroy(gameObject);
        }
    }

    private void OnDestroy() // explosion
    {
        GameObject effect = Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
        Destroy(effect, 10);
    }
}
