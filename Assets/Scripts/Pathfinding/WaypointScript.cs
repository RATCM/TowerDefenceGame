using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WaypointScript : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
