using System;
using System.Collections;
using System.Collections.Generic;
using Supyrb;
using Supyrb.Common;
using UnityEngine;
using UnityEngine.Events;

public class Block : MonoBehaviour
{
	//Stati possibili per il blocco
	public enum State
	{
		Sticky,
		StickyCollided, //Ancora appiccicoso, ma solo per un po' di più
		Solid
	}

	[SerializeField]
	private BlockRigidbody blockRigidbody = null;

    //Tempo durante il quale il blocco rimane appiccicoso dopo una collisione
    [SerializeField]
	private float stickyTimeAfterCollision = 0.2f;

	//Stato attuale del blocco
	[SerializeField]
	private State state = State.Solid;

	//Indica se il blocco è un elemento persistente nel livello
	[SerializeField]
	private bool persistentLevelElement = true;

	//Evento chiamato al momento dell'impatto
	[SerializeField]
	private UnityEvent onImpact = null;

    //Evento chiamato in caso di collisione con altri oggetti diversi da un blocco
    [SerializeField]
	private UnityEvent onOtherCollision = null;
	
	private SimpleTransform awakeTransform;
	private Transform awakeParent;

	private Block controllerBlock;

	public Block ControllerBlock => controllerBlock;

    //Timeout massimo senza collisioni per considerare il blocco non colpito
    [SerializeField]
    private float noCollisionTimeout = 4.0f;

    //Flag che indica se il blocco ha avuto collisioni
    private bool hasCollided = false;

    //Contatore di blocchi
    private int contBlock = 0;

	//Spara il blocco con una forza data
	public void Shoot(Vector3 force)
	{
		blockRigidbody.Rb.AddForce(force, ForceMode.VelocityChange);
		persistentLevelElement = false;
		state = State.Sticky;
		StartCoroutine(CheckNoCollision());
	}

	//Verifica se il blocco ha avuto collisioni
	private IEnumerator CheckNoCollision()
    {
        yield return new WaitForSeconds(noCollisionTimeout);

        //Verifica se il blocco non ha ancora avuto collisioni
        if (!hasCollided)
        {
            //Esegue le azioni necessarie quando il blocco non ha colpito niente
            Debug.Log("Il blocco non ha colpito niente!");
			contBlock++;
			AutoPlacement autoPlacement = FindObjectOfType<AutoPlacement>();
            //Segnala la fine del gioco tramite l'evento
            autoPlacement.GameOver();
        }
    }

    //Rimuove il componente Rigidbody dal blocco
    public void RemoveRigidbody()
	{
		blockRigidbody.RemoveRigidbody();
	}
	
	private void Awake()
	{
		controllerBlock = this;
		awakeParent = transform.parent;
		awakeTransform = transform.GetSimpleTransform(TransformType.Local);
	}

	private void OnDestroy()
	{
	}

    //Chiamato quando avviene una collisione con un altro oggetto
    private void OnCollisionEnter(Collision collision)
	{
		AutoPlacement autoPlacement = FindObjectOfType<AutoPlacement>();
		//Il blocco ha avuto una collisione
        hasCollided = true;
		if (state == State.Solid)
		{
			onOtherCollision.Invoke();
			return;
		}
		
		//Se l'oggetto colliso è un blocco
		if (collision.gameObject.TryGetComponent(out Block otherBlock))
		{
			if (state == State.Sticky)
			{
				state = State.Solid;
			}

			var otherController = otherBlock.ControllerBlock;
			if (otherController != this)
			{
				//Unisce il blocco corrente con l'altro blocco
				MergeWithOtherBlock(collision, otherController);
				onImpact.Invoke();
				if (autoPlacement != null)
				{
					Debug.Log("Ha colpito la torre, ora verifico");
					StartCoroutine(autoPlacement.CheckTowerState());
				}
			}
		}
		else
		{
            //Se l'oggetto colliso non è un blocco, il gioco termina con una sconfitta
            Debug.Log("Non hai colpito la torre hai perso");
			autoPlacement.GameOver();
		}
	}

    //Unisce il blocco corrente con un altro blocco
    private void MergeWithOtherBlock(Collision collision, Block other)
	{
		Debug.Log("MergeWithOtherBlock called");	
		other.transform.parent = transform;
		//Imposta la massa a zero prima di rimuovere il corpo rigido
    	other.blockRigidbody.Rb.mass = 0f;
		blockRigidbody.Rb.Merge(other.blockRigidbody.Rb);
		other.RemoveRigidbody();
	}

    //Cambia lo stato a solido dopo un certo periodo di tempo
    private IEnumerator ChangeToSolid()
	{
		yield return new WaitForSeconds(stickyTimeAfterCollision);
		state = State.Solid;
	}

    //Chiamato quando si vuole ristartare il livello
    private void OnRestartLevel()
	{
		if (persistentLevelElement)
		{
			transform.parent = awakeParent;
			transform.ApplySimpleTransform(awakeTransform);
			blockRigidbody.ResetRigidbody();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	#if UNITY_EDITOR

	private void Reset()
	{
		if (blockRigidbody == null)
		{
			blockRigidbody = GetComponent<BlockRigidbody>();
		}
	}

	#endif
}