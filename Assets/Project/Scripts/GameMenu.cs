using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{ 
    [SerializeField] GameObject gameMenu;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject playerPivot;
    public ShootBehaviour newBullet;
    public AutoPlacement autoPlacement;

    public void Resume()
    {
        //Si disattiva il menù
        gameMenu.SetActive(false);
        //Si riattiva il cannone
        playerPivot.SetActive(true);
        //Il gioco riprende 
        Time.timeScale = 1f;
    }
    public void Pause()
    {
        //Si attiva il menù
        gameMenu.SetActive(true);
        //Si disattiva il cannone
        playerPivot.SetActive(false);
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
        //Si riattiva il cannone
        playerPivot.SetActive(true);
        //Il gioco riprende 
        Time.timeScale = 1f;
        newBullet.RestartBullet();
        autoPlacement.RestartTower();
    }

}
