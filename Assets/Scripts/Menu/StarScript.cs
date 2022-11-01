using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScript : MonoBehaviour
{
    [SerializeField] private float speed;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        if (((int)StarController.TimePassed) % 20 == 0)
        {
            Destroy(this);
        }
    }
}
