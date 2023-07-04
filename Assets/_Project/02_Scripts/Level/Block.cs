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
	private RestartLevelSignal restartLevelSignal;
	private ToMenuSignal toMenuSignal;
	private Block controllerBlock;

	public Block ControllerBlock => controllerBlock;

	public void Shoot(Vector3 force)
	{
		blockRigidbody.Rb.AddForce(force, ForceMode.VelocityChange);
		persistentLevelElement = false;
		state = State.Sticky;
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
		
		Signals.Get(out restartLevelSignal);
		Signals.Get(out toMenuSignal);
		
		restartLevelSignal.AddListener(OnRestartLevel);
		toMenuSignal.AddListener(OnRestartLevel);
	}

	private void OnDestroy()
	{
		restartLevelSignal.RemoveListener(OnRestartLevel);
		toMenuSignal.RemoveListener(OnRestartLevel);
	}
	
	private void OnCollisionEnter(Collision collision)
	{
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
			}
		}
	}

	private void MergeWithOtherBlock(Collision collision, Block other)
	{
		
		other.transform.parent = transform;
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