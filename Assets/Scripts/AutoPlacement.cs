using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class AutoPlacement : MonoBehaviour
{
    private ARRaycastManager aRRaycastManager;
    private bool torrePosizionata = false;
    [SerializeField] private GameObject torrePrefab;
    private float distanzaMinima = 0.5f;
    [SerializeField] private Transform playerPivot;
    static List<ARRaycastHit> hitList = new List<ARRaycastHit>();

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (!torrePosizionata && aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hitList, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hitList[0].pose;

            float distanzaDallaTelecamera = Vector3.Distance(Camera.main.transform.position, hitPose.position);

            if (distanzaDallaTelecamera >= distanzaMinima)
            {
                torrePrefab.transform.localScale = new Vector3(1f, 1f, 1f);

                // Posiziona la torre a una certa distanza dal PlayerPivot
                Vector3 offsetFromPlayerPivot = new Vector3(0, 2, 2); // Imposta la distanza desiderata
                Vector3 torrePosition = playerPivot.position + offsetFromPlayerPivot;

                // Istanzia la torre con la scala già impostata
                GameObject torre = Instantiate(torrePrefab, torrePosition, Quaternion.identity);
                torre.tag = "Torre";

                torrePosizionata = true;
            }
            else
            {
                Debug.Log("La torre è troppo vicina alla camera.");
            }
        }
    }
}
