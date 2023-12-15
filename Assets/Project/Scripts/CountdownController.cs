using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownController : MonoBehaviour
{
    public int countdownTime = 5;
    public TextMeshProUGUI countdownDisplay;
    public GameObject countdownPanel;
    public GameObject winPanel;

    [SerializeField] AudioManager audioManager; //Oggetto che fa riferimento al gestore dell'audio

    void OnEnable()
    {
        countdownTime = 5;
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        while(countdownTime > 0)
        {
            countdownDisplay.text = countdownTime.ToString();
            audioManager.PlaySFX(audioManager.countdownTic); //Fa partire il tic del countdown

            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        audioManager.PlaySFX(audioManager.countdownEnd); //Fa partire il tic finale del countdown

        countdownPanel.SetActive(false);
        countdownDisplay.gameObject.SetActive(false);
        winPanel.SetActive(true);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

}
