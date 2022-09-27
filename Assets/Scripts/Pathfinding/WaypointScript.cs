using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WaypointScript : MonoBehaviour
{
    void Start()
    {
       var types = "Terrain".GetEnumValue();

        foreach(var type in types)
        {
            Debug.Log(type);
        }
        //Debug.Log("Terrain".GetEnumValue());
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
