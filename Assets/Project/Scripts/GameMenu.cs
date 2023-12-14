using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{ 
    [SerializeField] GameObject gameMenu;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject winPanel;
    public ShootBehaviour newBullet;
    public AutoPlacement autoPlacement;

    public void Resume()
    {
        //Si disattiva il men�
        gameMenu.SetActive(false);
        //Il gioco riprende 
        Time.timeScale = 1f;
    }
    public void Pause()
    {
        //Si attiva il menù
        gameMenu.SetActive(true);
        //Il gioco viene messo in pausa 
        Time.timeScale = 0f;
    }
    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void Restart()
    {
        //Si disattiva il menù
        gameMenu.SetActive(false);
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        //Il gioco riprende 
        Time.timeScale = 1f;
        newBullet.RestartBullet();
        autoPlacement.RestartTower();
    }

}
