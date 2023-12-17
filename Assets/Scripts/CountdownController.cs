using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Gestore del countdown relativo alla vittoria
public class CountdownController : MonoBehaviour
{
    //Tempo iniziale del countdown
    public int countdownTime = 5;
    //Riferimenti ai pannelli del menù
    public TextMeshProUGUI countdownDisplay;
    public GameObject countdownPanel;
    public GameObject winPanel;

    //Riferimento al gestore dell'audio
    [SerializeField] AudioManager audioManager;

    void OnEnable()
    {
        countdownTime = 5;
        StartCoroutine(CountdownToStart());
    }

    //Gestisce il countdown
    IEnumerator CountdownToStart()
    {
        while(countdownTime > 0)
        {
            //Aggiorna il testo del conteggio
            countdownDisplay.text = countdownTime.ToString();
            //Riproduce un suono relativo al tic del countdown
            audioManager.PlaySFX(audioManager.countdownTic);

            yield return new WaitForSeconds(1f);
            //Decrementa il tempo del countdown
            countdownTime--;
        }

        //Riproduce un suono di completamento del countdown
        audioManager.PlaySFX(audioManager.countdownEnd);

        //Disattiva il pannello del countdown
        countdownPanel.SetActive(false);
        countdownDisplay.gameObject.SetActive(false);
        //Attiva il pannello relativo alla vittoria
        winPanel.SetActive(true);
    }

    void OnDisable()
    {
        StopCoroutine(CountdownToStart());
    }

}
