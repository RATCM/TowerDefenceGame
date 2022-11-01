using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static bool LoadScene(string name)
    {
        bool sucess = true;
        try
        {
            SceneManager.LoadScene(name);

        }
        catch (UnityException)
        {
            sucess = false;
        }

        return sucess;
    }
}
