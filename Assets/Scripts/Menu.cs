using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject[] towers;
    public static int currentTowerIndex = 0;

    [SerializeField] private TMP_Text level;
    [SerializeField] private LevelListAsset levelList;

    public void onPlayButton()
    {
        if (levelList != null)
        {
            levelList.ChangeIndexRelative(currentTowerIndex);
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.LogError("LevelList is not assigned to Menu.");
        }
    }

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

    public void onNextButton()
    {
        towers[currentTowerIndex].SetActive(false);
        currentTowerIndex = (currentTowerIndex + 1) % towers.Length;
        towers[currentTowerIndex].SetActive(true);
        setLevelName(currentTowerIndex);
    }

    //Modifica il nome del livello
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