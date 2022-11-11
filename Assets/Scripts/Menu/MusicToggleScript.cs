using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MusicToggleScript : MonoBehaviour
{
    [HideInInspector] AudioSource musicAudio;
    //[HideInInspector] float defaultVolume;
    // Start is called before the first frame update
    void Start()
    {
        var toggle = gameObject.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate { OnToggle(toggle); });

        musicAudio = FindObjectOfType<AudioSource>();
        toggle.SetIsOnWithoutNotify(!musicAudio.mute);
    }

    void OnToggle(Toggle toggle)
    {
        musicAudio.mute = !toggle.isOn;
        PlayerPrefs.SetInt("MusicOn", toggle.isOn ? 1 : 0);
    }
}
