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
    private float distanzaMinima = 0.5f;
    [SerializeField] private Transform playerPivot;
    static List<ARRaycastHit> hitList = new List<ARRaycastHit>();
    private GameObject torre;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject countdownPanel;
    public TextMeshProUGUI countdownDisplay;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        ContaPezzi.OnGameOver += HandleGameOver;

    }
   
  

    private void Update()
    {
        
        if (!torrePosizionata && aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hitList, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hitList[0].pose;

            float distanzaDallaTelecamera = Vector3.Distance(Camera.main.transform.position, hitPose.position);

            if (distanzaDallaTelecamera >= distanzaMinima)
            {
                GameObject torrePrefab = towers[Menu.currentTowerIndex];
                torrePrefab.transform.localScale = new Vector3(1f, 1f, 1f);

                Vector3 offsetFromPlayerPivot = new Vector3(0, 2, 2);
                Vector3 torrePosition = playerPivot.position + offsetFromPlayerPivot;


               
                
                torre = Instantiate(torrePrefab, torrePosition, Quaternion.identity);
                


                torrePosizionata = true;
                OnTowerPlaced?.Invoke();
            }
            else
            {
                Debug.Log("La torre è troppo vicina alla camera.");
            }
        }
    }

    public void RestartTower()
    {
        if (torre != null)
        {
            Destroy(torre);
        }
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
                    Debug.Log("Hai perso");
                    gameOverPanel.SetActive(true);
                    if((countdownDisplay!=null) && (countdownPanel!=null))
                    {
                        countdownPanel.SetActive(false);
                        countdownDisplay.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Torre non trovata");
            gameOverPanel.SetActive(true);
        }
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        if((countdownDisplay!=null) && (countdownPanel!=null))
        {
            countdownPanel.SetActive(false);
            countdownDisplay.gameObject.SetActive(false);
        }
    }
}
