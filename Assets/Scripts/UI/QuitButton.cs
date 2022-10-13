using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuitButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected string NewSceneName;
    public void OnPointerClick(PointerEventData eventData)
    {
        Application.Quit();
    }
}

