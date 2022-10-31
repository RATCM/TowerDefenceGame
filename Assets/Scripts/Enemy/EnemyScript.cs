using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine;

[System.Flags]
public enum DamageType
{
    Laser = 1,
    Projectiles = 1 << 1,
    Explosion = 1 << 2,
    Freeze = 1 << 3,
}
public class EnemyScript : MonoBehaviour
{
    [Tooltip("The health of the enemy")]
    [SerializeField] public float Health = 100;

    [Tooltip("The base speed of the enemy")]
    [SerializeField] public float DefaultSpeed = 5; // Only change this if speed is slowed down permanentley

    [Tooltip("THe amount of money the enemy drops when dead")]
    [SerializeField] public ulong MoneyDrop = 5;

    [Tooltip("The damage caused by the enemy")]
    [SerializeField] public ulong Damage = 5;

    [Tooltip("This indicates which damage types this enemy is immune to")]
    [SerializeField] public DamageType Immunities;

    [HideInInspector] public float InitSpeed { get; private set; }

    [HideInInspector] public List<EnemyEffect> CurrentEffects = new List<EnemyEffect>();



    /// <summary>
    /// CurrentSpeed is in units per second
    /// </summary>
    [HideInInspector] public float CurrentSpeed = 5; // Change this for tower that slows enemies down

    private void Start()
    {
        InitSpeed = DefaultSpeed;
    }

    public void AddEffect(EnemyEffect effect)
    {
        var first = CurrentEffects.FirstOrDefault(x => x.GetType() == effect.GetType());
        if (first == null)
        {
            CurrentEffects.Add(effect);
            effect.ApplyEffect();
        }
        else
        {
            first.UpdateEffects(effect);
            first.ApplyEffect();
        }
    }
    void Update()
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
            PlayerInfo.Money += MoneyDrop;
        }
        // Movement is controlled in the PathFinding script attached to the Enemy prefab
    }
}
