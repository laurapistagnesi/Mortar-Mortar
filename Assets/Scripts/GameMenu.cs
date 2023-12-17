using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{ 
    //Riferimenti ai pannelli del menù
    [SerializeField] GameObject gameMenu;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject winPanel;
    //Riferimento al mortaio
    [SerializeField] GameObject playerPivot;
    //Riferimenti alle altre classi
    public ShootBehaviour newBullet;
    public AutoPlacement autoPlacement;

    //Gestore della ripresa del gioco
    public void Resume()
    {
        //Si disattiva il menù
        gameMenu.SetActive(false);
        //Si riattiva il cannone
        playerPivot.SetActive(true);
        //Il gioco riprende 
        Time.timeScale = 1f;
    }
    //Gestore della messa in pausa del gioco
    public void Pause()
    {
        //Si attiva il menù
        gameMenu.SetActive(true);
        //Si disattiva il cannone
        playerPivot.SetActive(false);
        //Il gioco viene messo in pausa 
        Time.timeScale = 0f;
    }
    //Gestore del ritorno alla scena di partenza del gioco
    public void Home()
    {
        //Il gioco riprende
        Time.timeScale = 1f;
        //Carica la scena corretta
        SceneManager.LoadScene("Menu");
    }
    //Gestore del riavvio della partita corrente
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
        //Si resettano i colpi a disposizione
        newBullet.RestartBullet();
        //Si istanzia nuovamente la torre
        autoPlacement.RestartTower();
    }

}
