using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawnerScript : MonoBehaviour
{
    [SerializeField] private GameObject starObject;
    // Update is called once per frame
    void Update()
    {
        if ( ((int) StarController.TimePassed) % 3 == 0)
        {
            Instantiate(starObject, GetComponent<Transform>());
        }
    }
}
