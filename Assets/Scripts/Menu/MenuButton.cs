using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour, IPointerClickHandler
{
    private GameObject ThisMenu;

    [SerializeField] private GameObject NewMenu;
    public void Update()
    {
        ThisMenu = transform.root.gameObject;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        NewMenu.GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
        ThisMenu.GetComponentInChildren<Canvas>(true).gameObject.SetActive(false);
    }
}
