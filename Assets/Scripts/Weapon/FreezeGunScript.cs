using UnityEngine;

public class FreezeGunScript : MonoBehaviour
{
    [SerializeField] public float SlowDownValue = 0.5f;
    [SerializeField] public float SlowDownDuration = 2f;
    [SerializeField] GameObject freezeEffect;
    [HideInInspector] GameObject freezeEffectInstance;
    [HideInInspector] Vector3 InitialPos;
    [HideInInspector] FreezeTower Parent;
    [HideInInspector] public float actualFreezeDuration =>
        Parent.WorkerCount >= Parent.MinimumWorkerCount
        ? SlowDownDuration * (1 + ((Parent.WorkerCount - Parent.MinimumWorkerCount) * 0.1f))
        : SlowDownDuration;
    public void Activate(GameObject closest)
    {
        freezeEffectInstance.SetActive(true);

        var enemy = closest.GetComponent<EnemyScript>();

        enemy.AddEffect(new TempFreeze(SlowDownValue, actualFreezeDuration, enemy.gameObject));
    }
    public void UnActivate()
    {
        freezeEffectInstance.SetActive(false);
    }

    public void ResetPosition()
    {
        transform.rotation = Parent.gameObject.transform.rotation;
        transform.localPosition = InitialPos;
    }
    void Start()
    {
        freezeEffectInstance = Instantiate(freezeEffect, transform);
        freezeEffectInstance.transform.rotation = transform.rotation;
        freezeEffectInstance.SetActive(false);
        InitialPos = transform.localPosition;
        Parent = GetComponentInParent<FreezeTower>();
    }
}
