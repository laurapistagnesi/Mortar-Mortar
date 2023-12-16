using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] popups;
    private int currentPopupIndex = 0;

    public GameObject popupTutorial;

    public void ShowCurrentPopup()
    {
        popupTutorial.SetActive(true);
        // Attiva solo il popup corrente
        popups[currentPopupIndex].SetActive(true);
    }

    void Update()
    {
        // Controlla il touch per avanzare nel tutorial
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            popups[currentPopupIndex].SetActive(false);
            // Avanza al popup successivo
            currentPopupIndex++;

            // Controlla se ci sono ancora popup da mostrare
            if (currentPopupIndex <= popups.Length)
            {
                ShowCurrentPopup();
            }
            else
            {
                // Tutorial completato
                popupTutorial.SetActive(false);
            }
        }
    }
}

