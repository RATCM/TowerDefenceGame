using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IPointerHandler : IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    void IPointerUpHandler.OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData) { }
    void IPointerDownHandler.OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData) { }
    void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData) { }
    void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData) { }
}