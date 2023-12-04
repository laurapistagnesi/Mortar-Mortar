using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    [SerializeField]
    GameObject spawnablePrefab;
    Camera arCam;
    GameObject spawnedObject;
    bool hasSpawned = false; // Per tenere traccia se hai già creato un oggetto
    float freezeTime = 2.0f; // Durata del freeze in secondi
    float elapsedTime = 0.0f; // Tempo trascorso

    // Start is called before the first frame update
    void Start()
    {
        spawnedObject = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasSpawned) // Se hai già creato un oggetto, esci dall'Update
        {
            if (elapsedTime < freezeTime)
            {
                // Applica il freeze solo per il periodo specificato
                FreezeObject();
                elapsedTime += Time.deltaTime;
            }
            else
            {
                // Rimuovi il freeze dopo il periodo specificato
                UnfreezeObject();
            }

            return;
        }

        if (m_RaycastManager.Raycast(arCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), m_Hits)) // Rileva il piano nel centro della fotocamera
        {
            // Verifica se è stato rilevato un piano e se non hai già creato un oggetto
            if (m_Hits.Count > 0 && !hasSpawned)
            {
                SpawnPrefab(m_Hits[0].pose.position);
                hasSpawned = true; // Imposta il flag a true per evitare ulteriori creazioni
            }
        }
    }

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        spawnedObject = Instantiate(spawnablePrefab, spawnPosition, Quaternion.identity);
    }

    private void FreezeObject()
    {
        // Ottieni tutti i Rigidbody dei blocchi nell'oggetto
        Rigidbody[] blockRigidbodies = spawnedObject.GetComponentsInChildren<Rigidbody>();

        // Applica il freeze della posizione e della rotazione a tutti i blocchi
        foreach (Rigidbody blockRigidbody in blockRigidbodies)
        {
            blockRigidbody.constraints = RigidbodyConstraints.FreezePositionX |
                                        RigidbodyConstraints.FreezePositionY |
                                        RigidbodyConstraints.FreezePositionZ |
                                        RigidbodyConstraints.FreezeRotationX |
                                        RigidbodyConstraints.FreezeRotationY |
                                        RigidbodyConstraints.FreezeRotationZ;
        }
    }

    private void UnfreezeObject()
    {
        // Ottieni tutti i Rigidbody dei blocchi nell'oggetto
        Rigidbody[] blockRigidbodies = spawnedObject.GetComponentsInChildren<Rigidbody>();

        // Rimuovi il freeze della posizione e della rotazione a tutti i blocchi
        foreach (Rigidbody blockRigidbody in blockRigidbodies)
        {
            blockRigidbody.constraints = RigidbodyConstraints.None;
        }
    }
}


