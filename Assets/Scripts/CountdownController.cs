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

    // Start is called before the first frame update
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
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        countdownPanel.SetActive(false);
        countdownDisplay.gameObject.SetActive(false);
        winPanel.SetActive(true);
    }

}
