using UnityEngine;
using System;

public class ContaPezzi : MonoBehaviour
{
    
    public static event Action OnGameOver;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("pezzo"))
        {
           
           
             OnGameOver.Invoke();
                
            
        }
    }
}
