using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{
    public static float TimePassed = 0; // For stars in Menu - Should be hidden from IDE but can't remember how to hide it.

    // Update is called once per frame
    void Update()
    {
        TimePassed += Time.deltaTime;
    }
}
