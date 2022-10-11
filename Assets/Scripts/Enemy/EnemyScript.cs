using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float Health = 100;

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0)
            Destroy(gameObject);
    }
}
