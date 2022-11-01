using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour, IPointerClickHandler
{
    public string NewSceneName = "";
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(NewSceneName,LoadSceneMode.Single);
    }
}
