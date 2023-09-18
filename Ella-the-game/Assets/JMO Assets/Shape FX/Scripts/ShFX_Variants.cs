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

// This class is only used to show the ShFX_Variants_Editor in the Inspector.
// It will only be an empty script in the build.

namespace ShapeFX
{
	[DisallowMultipleComponent, RequireComponent(typeof(ParticleSystem))]
	public class ShFX_Variants : MonoBehaviour
	{
#if UNITY_EDITOR

		public string mainName = "Main";

		public bool isMobile;

		public bool rndShape = true;
		public bool rndStyle = true;
		public bool rndBlendMode = false;

		//Density
		public bool useDensity;
		public Vector3 densityCachedScale = Vector3.one;
		public float densityCachedScaleVolume = 0f;
		[System.Serializable]
		public class EffectDensity
		{
			public ParticleSystem particleSystem;
			public bool useDensity;
			public float density;
			public float densityCachedEmissionRate = 0f;
		}
		public EffectDensity[] effectDensities;

#endif
	}
}
