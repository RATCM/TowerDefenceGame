using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControllerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("MusicOn"))
        {
            var musicAudio = FindObjectOfType<AudioSource>();
            musicAudio.mute = PlayerPrefs.GetInt("MusicOn") == 0;
        }
    }
}
