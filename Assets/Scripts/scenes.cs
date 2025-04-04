using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scenes : MonoBehaviour
{
    

    public void LoadScene(int sceneName)
    {
        SceneManager.LoadScene(sceneName);

    }
}
