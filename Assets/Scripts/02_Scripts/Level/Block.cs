// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Block.cs" company="Supyrb">
//   Copyright (c) 2020 Supyrb. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Supyrb;
using Supyrb.Common;
using UnityEngine;
using UnityEngine.Events;

public class Block : MonoBehaviour
{
	public enum State
	{
		Sticky,
		StickyCollided, // Still sticky, but only for a bit more
		Solid
	}

	[SerializeField]
	private BlockRigidbody blockRigidbody = null;

	[SerializeField]
	private float stickyTimeAfterCollision = 0.2f;

	[SerializeField]
	private State state = State.Solid;

	[SerializeField]
	private bool persistentLevelElement = true;

	[SerializeField]
	private UnityEvent onImpact = null;

	[SerializeField]
	private UnityEvent onOtherCollision = null;
	
	private SimpleTransform awakeTransform;
	private Transform awakeParent;
	//private RestartLevelSignal restartLevelSignal;
	//private ToMenuSignal toMenuSignal;
	private Block controllerBlock;

	public Block ControllerBlock => controllerBlock;
	
	[SerializeField]
    private float noCollisionTimeout = 2.0f; // Tempo massimo senza collisioni per considerare il blocco come non colpito

    private bool hasCollided = false;

	private int contBlock = 0;

	public void Shoot(Vector3 force)
	{
		blockRigidbody.Rb.AddForce(force, ForceMode.VelocityChange);
		persistentLevelElement = false;
		state = State.Sticky;
		StartCoroutine(CheckNoCollision());
	}

	private IEnumerator CheckNoCollision()
    {
        yield return new WaitForSeconds(noCollisionTimeout);

        // Verifica se il blocco non ha ancora avuto collisioni
        if (!hasCollided)
        {
            // Esegui le azioni necessarie quando il blocco non ha colpito niente
            Debug.Log("Il blocco non ha colpito niente!");
			contBlock++;
			AutoPlacement autoPlacement = FindObjectOfType<AutoPlacement>();
            // Segnala la fine del gioco tramite l'evento
            autoPlacement.GameOver();
        }
    }

	public void RemoveRigidbody()
	{
		blockRigidbody.RemoveRigidbody();
	}
	
	private void Awake()
	{
		controllerBlock = this;
		awakeParent = transform.parent;
		awakeTransform = transform.GetSimpleTransform(TransformType.Local);
		
		//Signals.Get(out restartLevelSignal);
		//Signals.Get(out toMenuSignal);
		
		//restartLevelSignal.AddListener(OnRestartLevel);
		//toMenuSignal.AddListener(OnRestartLevel);
	}

	private void OnDestroy()
	{
		//restartLevelSignal.RemoveListener(OnRestartLevel);
		//toMenuSignal.RemoveListener(OnRestartLevel);
	}
	
	private void OnCollisionEnter(Collision collision)
	{
		AutoPlacement autoPlacement = FindObjectOfType<AutoPlacement>();
		// Il blocco ha avuto una collisione
        hasCollided = true;
		if (state == State.Solid)
		{
			onOtherCollision.Invoke();
			return;
		}
		
		if (collision.gameObject.TryGetComponent(out Block otherBlock))
		{
			if (state == State.Sticky)
			{
				//StartCoroutine(ChangeToSolid());
				state = State.Solid;
			}

			var otherController = otherBlock.ControllerBlock;
			if (otherController != this)
			{
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
			Debug.Log("Non hai colpito la torre hai perso");
			autoPlacement.GameOver();
		}
	}

	private void MergeWithOtherBlock(Collision collision, Block other)
	{
		Debug.Log("MergeWithOtherBlock called");	
		other.transform.parent = transform;
		// Imposta la massa a zero prima di rimuovere il corpo rigido
    	other.blockRigidbody.Rb.mass = 0f;
		blockRigidbody.Rb.Merge(other.blockRigidbody.Rb);
		other.RemoveRigidbody();
	}

	private IEnumerator ChangeToSolid()
	{
		yield return new WaitForSeconds(stickyTimeAfterCollision);
		state = State.Solid;
	}

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