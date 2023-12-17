using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

//Gestisce il piazzamento automatico della torre
[RequireComponent(typeof(ARRaycastManager))]
public class AutoPlacement : MonoBehaviour
{
    public static event Action OnTowerPlaced;
    //Gestore del raycast
    private ARRaycastManager aRRaycastManager;
    static List<ARRaycastHit> hitList = new List<ARRaycastHit>();
    //Indica se la torre è piazzata o meno
    private bool torrePosizionata = false;
    //Lista delle torri disponibili
    [SerializeField] private GameObject[] towers;
    //Riferimento al mortaio
    [SerializeField] private GameObject playerPivot;
    //Riferimento alla torre
    private GameObject torre;

    //Riferimenti ai pannelli del menù
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject countdownPanel;
    public TextMeshProUGUI countdownDisplay;
    [SerializeField] public TextMeshProUGUI gameOverPanelText;

    //Riferimento all'esplosione
    [SerializeField] public Explosion explosion;
    //Gestore dei popup relativi al tutorial
    [SerializeField] public TutorialManager tutorialManager;
    //Distanza minima alla quale deve trovarsi la torre rispetto al mortaio
    private float distanzaMinima = 1.75f;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        //Istanzia la torre attraverso il rilevamento di un piano orizzontale
        if (!torrePosizionata && aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hitList, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hitList[0].pose;

            //Istanzia la torre corretta, posizionandola sul piano rilevato, e la scala
            GameObject torrePrefab = towers[Menu.currentTowerIndex];
            torrePrefab.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Vector3 towerPosition = hitPose.position + new Vector3(0, 0, 1.75f);

            torre = Instantiate(torrePrefab, towerPosition, Quaternion.identity);
            torre.tag = "Torre";
            torrePosizionata = true;
            OnTowerPlaced?.Invoke();

            //Si attivano dei popup nel caso in cui la torre selezionata sia quella relativa al tutorial
            if (Menu.currentTowerIndex == 0)
            {
                tutorialManager.ShowCurrentPopup();
            }
        }

        //Allontana la torre dal cannone nel caso in cui il giocatore, e di conseguenza il mortaio, si avvicina troppo (entro la soglia prestabilita di 1.75f)
        if (torrePosizionata)
        {
            float distanza = Vector3.Distance(playerPivot.transform.position, torre.transform.position);

            if (distanza < distanzaMinima)
            {
                Vector3 direzione = (playerPivot.transform.position - torre.transform.position).normalized;
                torre.transform.position -= direzione * 1f;
            }
        }
    }

    //Ricrea la torre
    public void RestartTower()
    {
        //Distrugge la torre istanziata se già esiste
        if (torre != null)
        {
            Destroy(torre);
        }
        //Reimposta il flag in modo da consentire l'istanziazione di una nuova torre
        torrePosizionata = false;
    }

    //Controlla lo stato della torre: se viene rilevato un movimento eccessivo dei blocchi che la compongono si ritiene la torre crollata, altrimenti la si ritiene intatta
    public IEnumerator CheckTowerState()
    {
        if (torre != null)
        {
            List<Block> towerBlocks = new List<Block>();
            foreach (Transform child in torre.gameObject.transform)
            {
                Block block = child.GetComponent<Block>();

                if (block != null)
                {
                    towerBlocks.Add(block);
                }
            }
            yield return new WaitForSeconds(2.5f);
            foreach (var block in towerBlocks)
            {
                Rigidbody blockRigidbody = block.GetComponent<Rigidbody>();
                Vector3 velocity = blockRigidbody.velocity;
                float speed = velocity.magnitude;
                Debug.Log("speed " + speed);
                if (speed > 0f)
                {
                    Vector3 position = blockRigidbody.position;
                    explosion.Explode(position);
                    Debug.Log("Hai perso");
                    playerPivot.SetActive(false);
                    gameOverPanelText.text = "You lost, tower destroyed!";
                    gameOverPanel.SetActive(true);
                    countdownPanel.SetActive(false);
                    countdownDisplay.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogError("Torre non trovata");
            playerPivot.SetActive(false);
            gameOverPanel.SetActive(true);
        }
    }

    //Genera la sconfitta, nel caso in cui il giocatore non colpisca la torre (condizione necessaria per considerare uno sparo valido)
    public void GameOver()
    {
        playerPivot.SetActive(false);
        gameOverPanelText.text = "You lost, you must hit the tower!";
        gameOverPanel.SetActive(true);
        countdownPanel.SetActive(false);
        countdownDisplay.gameObject.SetActive(false);
    }
}