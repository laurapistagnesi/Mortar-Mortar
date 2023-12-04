using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class InstantiateObjects : MonoBehaviour
{
    public GameObject tower;
    //La distanza desiderata dalla torre rispetto alla fotocamera
    public float distanceFromCamera = 2f;

    private ARRaycastManager raycastManager;
    private ARAnchor towerAnchor;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    private void Start()
    {
        // Lancia un raggio a partire dal centro dello schermo per posizionare la torre all'inizio
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            PlaceObject(hitPose.position);
        }
    }

    private void PlaceObject(Vector3 position)
    {
        if (towerAnchor != null)
            Destroy(towerAnchor.gameObject);

        towerAnchor = new GameObject("TowerAnchor").AddComponent<ARAnchor>();
        towerAnchor.transform.position = position;

        GameObject placedObject = Instantiate(tower, towerAnchor.transform);
        placedObject.transform.localPosition = new Vector3(0f, 0f, -distanceFromCamera);
    }
}