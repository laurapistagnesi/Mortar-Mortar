using System;
using UnityEngine;

namespace Supyrb
{

	//Gestisce il Rigidbody di un blocco
	public class BlockRigidbody : MonoBehaviour
	{
		[SerializeField]
		private Rigidbody rb = null;

		[SerializeField]
		private float sleepThreshold = 0.4f;

		[SerializeField]
		private bool sleepOnEnable = true;

		public Rigidbody Rb => rb;

		void Awake()
		{
			rb.sleepThreshold = sleepThreshold;
		}

		private void OnEnable()
		{
			if (sleepOnEnable)
			{
				ResetRigidbody();
			}
		}

		//Rimuove il componente Rigidbody
		public void RemoveRigidbody()
		{
			if (rb == null)
			{
				return;
			}
			Destroy(rb);
		}
		
		//Resetta il componente Rigidbody
		public void ResetRigidbody()
		{
			if (rb == null)
			{
				rb = gameObject.AddComponent<Rigidbody>();
			}
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.Sleep();
		}

		#if UNITY_EDITOR
		private void Reset()
		{
			if(rb == null) {
				rb = GetComponent<Rigidbody>();
			}
		}
		#endif
	}
}