using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected string NewSceneName;

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(NewSceneName, LoadSceneMode.Single);
    }
}
