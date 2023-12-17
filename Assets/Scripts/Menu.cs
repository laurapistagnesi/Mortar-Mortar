using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //Torri disponibili nel menù
    [SerializeField] private GameObject[] towers;
    //Indice della torre corrente
    public static int currentTowerIndex = 0;

    //Testo per mostrare il nome della torre corrente
    [SerializeField] private TMP_Text level;

    public void OnEnable()
    {
        //Attiva la prima torre e disattiva le altre
        currentTowerIndex = 0;
        towers[currentTowerIndex].SetActive(true);

        for (int i = 1; i < towers.Length; i++)
        {
            towers[i].SetActive(false);
        }
    }

    //Avvia il gioco
    public void onPlayButton()
    {
        SceneManager.LoadScene(1);
    }

    //Cambia la torre corrente con quella precedente
    public void onPreviousButton()
    {
        towers[currentTowerIndex].SetActive(false);
        currentTowerIndex--;
        if (currentTowerIndex < 0)
        {
            currentTowerIndex += towers.Length;
        }

        towers[currentTowerIndex].SetActive(true);
        setLevelName(currentTowerIndex);
    }

    //Cambia la torre corrente con quella successiva
    public void onNextButton()
    {
        towers[currentTowerIndex].SetActive(false);
        currentTowerIndex = (currentTowerIndex + 1) % towers.Length;
        towers[currentTowerIndex].SetActive(true);
        setLevelName(currentTowerIndex);
    }

    //Modifica il testo in base al nome della torre corrente
    public void setLevelName(int index)
    {
        switch (index)
        {
            case 0:
                level.text = "TUTORIAL";
                break;
            case 1:
                level.text = "ROOFMORT";
                break;
            case 2:
                level.text = "RICK AND MORTAR";
                break;
            case 3:
                level.text = "MORTALLICA";
                break;
            case 4:
                level.text = "MORTARPUNK2077";
                break;
            case 5:
                level.text = "MORTARHEAD";
                break;
            case 6:
                level.text = "GAMES OF MORTS";
                break;
            case 7:
                level.text = "MORTINI";
                break;
        }
    }
}