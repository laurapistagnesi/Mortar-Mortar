using System;
using NaughtyAttributes;
using Supyrb;
using UnityEngine;
using UnityEngine.Events;

public class Explosion : MonoBehaviour
{
	//Raggio dell'esplosione
	[SerializeField]
	private float radius = 2.0f;

	//Forza dell'esplosione
	[SerializeField]
	private float force = 1000f;

	[SerializeField]
	private float upwardsModifier = 3.0f;
	
	[SerializeField]
	private LayerMask affectedLayers = default;

    //Numero massimo di rigidbodies interessati dall'esplosione
    [SerializeField]
	private int maxAffectedRigidbodies = 30;

    //Evento invocato al termine dell'esplosione
    [SerializeField]
	private UnityEvent onExplosion = null;
	
	private Collider[] colliders = null;
	
	private void Awake()
	{
		colliders = new Collider[maxAffectedRigidbodies];
	}

	//Realizza l'esplosione
	[Button()]
	public void Explode(Vector3 position)
	{
		var explosionPosition = position;
		var hits = Physics.OverlapSphereNonAlloc(explosionPosition, radius, colliders, affectedLayers, QueryTriggerInteraction.Ignore);
		for (int i = 0; i < hits; i++)
		{
			var hitCollider = colliders[i];
			var rb = hitCollider.attachedRigidbody;
			if (rb == null)
			{
				continue;
			}
			rb.AddExplosionForce(force, explosionPosition, radius, upwardsModifier);
		}
		onExplosion.Invoke();
	}
	
	#if UNITY_EDITOR

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector3.zero, radius);
	}

	#endif
}
