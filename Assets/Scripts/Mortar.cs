using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Imposta la posizione della fionda nella parte inferiore dello schermo
        Vector3 bottomScreenPosition = new Vector3(Screen.width / 2, 0, 15); // L'asse Z può essere regolato a seconda della tua scena
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(bottomScreenPosition);
        transform.position = worldPosition;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0); // Imposta la coordinata Z a zero
        Debug.Log("Mortar at position: " + worldPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
