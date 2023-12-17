using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using TMPro;

[RequireComponent(typeof(ARRaycastManager))]
public class AutoPlacement : MonoBehaviour
{
    public static event Action OnTowerPlaced;
    private ARRaycastManager aRRaycastManager;
    private bool torrePosizionata = false;
    [SerializeField] private GameObject[] towers;
    [SerializeField] private GameObject playerPivot;
    static List<ARRaycastHit> hitList = new List<ARRaycastHit>();
    private GameObject torre;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject countdownPanel;
    public TextMeshProUGUI countdownDisplay;
    [SerializeField] public TextMeshProUGUI gameOverPanelText;
    [SerializeField] public Explosion explosion;
    [SerializeField] public TutorialManager tutorialManager;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (!torrePosizionata && aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hitList, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hitList[0].pose;

            GameObject torrePrefab = towers[Menu.currentTowerIndex];
            torrePrefab.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Vector3 towerPosition = hitPose.position + new Vector3(0, 0, 1f);

            torre = Instantiate(torrePrefab, towerPosition, Quaternion.identity);
            torre.tag = "Torre";
            torrePosizionata = true;
            OnTowerPlaced?.Invoke();

            if (Menu.currentTowerIndex == 0)
            {
                tutorialManager.ShowCurrentPopup();
            }
        }
    }

    public void RestartTower()
    {
        // Distruggi la torre istanziata se esiste
        if (torre != null)
        {
            Destroy(torre);
        }
        // Reimposta il flag per consentire la posizione di una nuova torre
        torrePosizionata = false;
    }

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
                    gameOverPanelText.text ="You lost, tower destroyed!";
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

    public void GameOver()
    {
        playerPivot.SetActive(false);
        gameOverPanelText.text = "You lost, you must hit the tower!";
        gameOverPanel.SetActive(true);
        countdownPanel.SetActive(false);
        countdownDisplay.gameObject.SetActive(false);
    }
}