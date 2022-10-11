using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootTower : DefenceTower, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] protected ulong BulletCount = 0;
    [SerializeField] protected ulong MagazineSize = 10;
    [SerializeField] protected ulong ReloadTime = 3;

    private void Awake()
    {
        InstantiateUIPrefab("ShootTowerInfoPopup");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerClick(PointerEventData eventData) => 
        UIPanel.SetActive(!UIPanel.activeSelf);
    public void OnPointerEnter(PointerEventData eventData) =>
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
    public void OnPointerExit(PointerEventData eventData) =>
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
}
