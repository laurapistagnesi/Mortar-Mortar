using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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
    private void HandleGameOver()
    {
        gameOverPanel.SetActive(true);

    }
}
