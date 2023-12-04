using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class AutoPlacementController : MonoBehaviour
{
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject projectilePrefab; // Prefab del proiettile
    [SerializeField] private float launchForce = 1000.0f; // Forza di lancio
    [SerializeField] private float swipeThreshold = 100.0f; // La soglia per considerare uno swipe

    private ARRaycastManager aRRaycastManager;
    private Camera mainCamera;
    private GameObject targetTower; // La torre da colpire
    private bool towerReady = false;
    private Vector3 swipeStartPos;
    private Vector3 swipeEndPos;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        aRRaycastManager = GetComponent <ARRaycastManager>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!towerReady && aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            tower.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            AddFixedJointsToTower(tower);
            tower.transform.position = new Vector3(hitPose.position.x, hitPose.position.y, hitPose.position.z);
            tower.transform.rotation = hitPose.rotation;

            // Istanzia la torre
            targetTower = Instantiate(tower);
            towerReady = true;

            RemoveFixedJointsFromTower(tower);
        }

        if (towerReady && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    swipeStartPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    swipeEndPos = touch.position;
                    if (Vector3.Distance(swipeEndPos, swipeStartPos) > swipeThreshold)
                    {
                        LaunchProjectile();
                    }
                    break;
            }
        }
    }

    private void AddFixedJointsToTower(GameObject tower)
    {
        Rigidbody[] rigidbodies = tower.GetComponentsInChildren<Rigidbody>();
        for (int i = 1; i < rigidbodies.Length; i++)
        {
            FixedJoint fixedJoint = rigidbodies[i].gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = rigidbodies[i - 1];
        }
    }

    private void RemoveFixedJointsFromTower(GameObject tower)
    {
        FixedJoint[] fixedJoints = tower.GetComponentsInChildren<FixedJoint>();
        foreach (FixedJoint joint in fixedJoints)
        {
            Destroy(joint);
        }
    }

    private void LaunchProjectile()
    {
        if (targetTower == null)
        {
            Debug.LogWarning("Torre non assegnata come bersaglio.");
            return;
        }

        // Calcola la direzione del lancio
        Vector3 launchDirection = (targetTower.transform.position - transform.position).normalized;
        projectilePrefab.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        // Istanzia il proiettile e applica una forza per lanciarlo verso la torre
        GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
        rb.AddForce(launchDirection * launchForce);
    }
}
