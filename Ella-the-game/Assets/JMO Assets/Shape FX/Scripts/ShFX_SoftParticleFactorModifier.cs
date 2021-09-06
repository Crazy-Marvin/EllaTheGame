//========================================
//
// SHAPE FX
// (c) 2016 - Jean Moreno
// http://www.jeanmoreno.com/unity
//
//========================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Changes the material's "Soft Particle Factor" without needing to create another material

namespace ShapeFX
{
	[RequireComponent(typeof(ParticleSystem))]
	[ExecuteInEditMode()]
	public class ShFX_SoftParticleFactorModifier : MonoBehaviour
	{
		//--------------------------------------------------------------------------------------------------
		// PUBLIC INSPECTOR PROPERTIES

		[Range(0f,10f)]
		public float SoftParticleFactor = 1.0f;
		
		//--------------------------------------------------------------------------------------------------
		// PRIVATE PROPERTIES

		ParticleSystemRenderer psr;
		
		//--------------------------------------------------------------------------------------------------
		// UNITY EVENTS
		
		void OnEnable()
		{
			UpdatePropertyBlock();
		}

		void OnDisable()
		{
			ClearPropertyBlock();
		}

	#if UNITY_EDITOR
		void Update()
		{
			UpdatePropertyBlock();
		}
	#endif
		
		//--------------------------------------------------------------------------------------------------
		// PUBLIC
		
		
		
		//--------------------------------------------------------------------------------------------------
		// PRIVATE
		
		private void UpdatePropertyBlock()
		{
			if(psr == null)
			{
				psr = this.GetComponent<ParticleSystemRenderer>();
			}
			if(psr != null)
			{
				MaterialPropertyBlock mpb = new MaterialPropertyBlock();
				psr.GetPropertyBlock(mpb);
				mpb.SetFloat("_InvFade", SoftParticleFactor);
				psr.SetPropertyBlock(mpb);
			}
		}

		private void ClearPropertyBlock()
		{
			if(psr == null)
			{
				psr = this.GetComponent<ParticleSystemRenderer>();
			}
			if(psr != null)
			{
				MaterialPropertyBlock mpb = new MaterialPropertyBlock();
				psr.GetPropertyBlock(mpb);
				mpb.Clear();
				psr.SetPropertyBlock(mpb);
			}
		}
	}
}