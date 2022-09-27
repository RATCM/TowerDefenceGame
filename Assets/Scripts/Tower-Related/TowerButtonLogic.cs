using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerButtonLogic : MonoBehaviour, IPointerHandler
{
    [SerializeField] private Image towerImage;
    [SerializeField] private GameObject SelectTower;

    [SerializeField] private GameObject TowerUnplacedObject;
    public void OnPointerDown(PointerEventData eventData)
    {
        // Make tower slightly more darker, when mouse enters and presses down.
        towerImage.color = new Color(0.6f, 0.6f, 0.6f);
        Global.SelectedTower = SelectTower;
        Global.PointerState  = "Selecting Tower";

        TowerUnplacedObject.GetComponent<TowerPlacerScript>().UpdateSelectedTower();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        // Make tower slightly more darker, when mouse enters and no longer presses down.
        towerImage.color = new Color(0.8f, 0.8f, 0.8f);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Make tower slightly dark, when mouse enters.
        towerImage.color = new Color(0.8f, 0.8f, 0.8f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // Default to original color, when mouse leaves.
        towerImage.color = new Color(1,1,1);
    }
}
