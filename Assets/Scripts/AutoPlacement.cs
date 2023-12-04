using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class AutoPlacement : MonoBehaviour
{
    private ARRaycastManager aRRaycastManager;
    private bool torrePosizionata = false;
    [SerializeField] private GameObject torre;
    private float distanzaMinima = 3.0f;

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
                torre.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                torre.transform.position = new Vector3(hitPose.position.x, hitPose.position.y, hitPose.position.z);
                torre.transform.rotation = hitPose.rotation;
                Instantiate(torre);

                torrePosizionata = true;
            }
            else
            {
                Debug.Log("La torre è troppo vicina alla camera.");
            }
        }
    }
}
