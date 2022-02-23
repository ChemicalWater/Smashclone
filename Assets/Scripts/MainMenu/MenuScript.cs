using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class MenuScript : MonoBehaviour
{
    [SerializeField] GameObject homeScreen;
    [SerializeField] GameObject profielScreen;

    public void switchScreen(string toScreen)
    {
      if (toScreen == "Home")
        {
            homeScreen.SetActive(true);
            profielScreen.SetActive(false);
        }
      if(toScreen == "Profiel")
        {
            homeScreen.SetActive(false);
            profielScreen.SetActive(true);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
