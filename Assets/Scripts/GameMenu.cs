using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{ 
    [SerializeField] GameObject gameMenu;

    public void Resume()
    {
        //Si disattiva il men�
        gameMenu.SetActive(false);
        //Il gioco riprende 
        Time.timeScale = 1f;
    }
    public void Pause()
    {
        //Si attiva il men�
        gameMenu.SetActive(true);
        //Il gioco viene messo in pausa 
        Time.timeScale = 0f;
    }
    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

}
