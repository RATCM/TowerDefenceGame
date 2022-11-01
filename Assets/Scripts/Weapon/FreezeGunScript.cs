using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeGunScript : MonoBehaviour
{
    [SerializeField] public float SlowDownValue = 0.5f;
    [SerializeField] public float SlowDownDuration = 2f;
    [SerializeField] GameObject freezeEffect;
    [HideInInspector] GameObject freezeEffectInstance;
    [HideInInspector] Vector3 InitialPos;
    [HideInInspector] GameObject Parent;
    public void Activate(GameObject closest)
    {
        freezeEffectInstance.SetActive(true);

        var enemy = closest.GetComponent<EnemyScript>();

        enemy.AddEffect(new TempFreeze(SlowDownValue, SlowDownDuration, enemy.gameObject));
    }
    public void UnActivate()
    {
        freezeEffectInstance.SetActive(false);
    }

    public void ResetPosition()
    {
        transform.rotation = Parent.transform.rotation;
        transform.localPosition = InitialPos;
    }
    void Start()
    {
        freezeEffectInstance = Instantiate(freezeEffect, transform);
        freezeEffectInstance.transform.rotation = transform.rotation;
        freezeEffectInstance.SetActive(false);
        InitialPos = transform.localPosition;
        Parent = GetComponentInParent<FreezeTower>().gameObject;
    }
}
