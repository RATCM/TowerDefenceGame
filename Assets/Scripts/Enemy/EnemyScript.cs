using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] public float Health = 100;
    [SerializeField] public float DefaultSpeed = 5; // Only change this if speed is slowed down permanentley
    [SerializeField] public ulong MoneyDrop = 5;

    /// <summary>
    /// CurrentSpeed is in units per second
    /// </summary>
    [HideInInspector] public float CurrentSpeed = 5; // Change this for tower that slows enemies down
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
