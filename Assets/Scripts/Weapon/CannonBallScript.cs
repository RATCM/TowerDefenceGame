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

        var circle = GetComponent<CircleCollider2D>();

        var hit = Physics2D.CircleCast(transform.position, circle.radius, Vector3.zero,0,LayerMask.GetMask("Enemy"));

        if (hit)
        {
            var enemies = Physics2D.CircleCastAll(hit.transform.position, Radius, Vector2.zero,0,LayerMask.GetMask("Enemy"));

            foreach(var enemy in enemies)
            {
                var enemyScript = enemy.collider.gameObject.GetComponent<EnemyScript>();

                enemyScript.Health -= Damage;
            }

            // Explosion effect
            GameObject effect = Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * Radius * 0.5f;
            Destroy(effect, 10);

            Destroy(gameObject);
        }
    }
}
