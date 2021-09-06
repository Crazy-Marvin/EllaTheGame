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

namespace ShapeFX
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ShFX_EffectHandler : MonoBehaviour
	{
		//--------------------------------------------------------------------------------------------------
		
		public enum EndAction
		{
			DoNothing,
			DestroyGameObject,
			DeactivateGameObject,
		}

		//--------------------------------------------------------------------------------------------------
		// STATIC

		static public bool DisableCameraShake;

		//--------------------------------------------------------------------------------------------------
		// PUBLIC INSPECTOR PROPERTIES

		[Header("EFFECT")]
		[Tooltip("What to do when the Particle System has finished playing")]
		public EndAction endAction = EndAction.DestroyGameObject;

		[Header("SOUND")]
		[Tooltip("Sound effect to play with the Particle System")]
		public AudioClip soundEffect;

		[Header("CAMERA SHAKE")]
		[Tooltip("Perform a simple Camera shake when the effect plays")]
		public bool shakeCamera = false;
		[Tooltip("Will use Camera.main if empty")]
		public Camera cameraToShake;
		[Range(0, 180f)]
		public float shakeAngle = 90f;
		public bool randomShakeAngle = false;
		public float shakeDuration = 0.5f;
		public float shakeDelay = 0.0f;
		[Range(1, 20)]
		public int shakeRepeat = 1;
		[Range(0f, 0.1f), Tooltip("Will change the camera position every step seconds (every frame if 0)")]
		public float shakeStep = 0.03f;
		[Range(0.01f, 1f)]
		public float shakeStrength = 0.1f;
		[Tooltip("Decrease the shake strength as the camera moves further away from the effect")]
		public bool useFalloff = false;
		[Tooltip("Distance between effect and camera at which the shake starts to linearly decrease")]
		public float falloffMin = 10f;
		[Tooltip("Distance between effect and camera over which the shake effects is ignored")]
		public float falloffMax = 20f;

		//--------------------------------------------------------------------------------------------------
		// PRIVATE PROPERTIES

		private ParticleSystem ps;

		static private Vector3 camOriginalPosition;
		static private int lastShakeFrame = -1;
		static private float curFrameMaxStrength = -1;
		
		private Coroutine shakeCoroutine;
		private float shakeOffset;
		private bool cameraShaking;
		
		//--------------------------------------------------------------------------------------------------
		// UNITY EVENTS

		void Awake()
		{
			ps = this.GetComponent<ParticleSystem>();
		}

		void OnEnable()
		{
			if(cameraToShake == null)
				cameraToShake = Camera.main;

			StartCoroutine(CR_CheckParticleSystem());
		}

		void OnDisable()
		{
			StopAllCoroutines();

			if(shakeCamera && !DisableCameraShake)
			{
				Camera.onPreRender -= OnCamPreRender;
				Camera.onPostRender -= OnCamPostRender;
			}
		}


		//To avoid altering the original camera movement/position, shaking is performed
		//just before render and its position is restored just after render
		void OnCamPreRender(Camera cam)
		{
			if(cameraShaking)
			{
				if(cam == cameraToShake)
				{
					//store original position (only store at first frame across all effects)
					if(lastShakeFrame < Time.frameCount)
					{
						camOriginalPosition = cam.transform.position;
						lastShakeFrame = Time.frameCount;
						curFrameMaxStrength = -1f;
					}

					//apply shake (priorize higher strength)
					if(shakeOffset > curFrameMaxStrength)
					{
						curFrameMaxStrength = shakeOffset;
						Vector3 shakeVector = Quaternion.AngleAxis(shakeAngle, cam.transform.forward) * cam.transform.right;
						cam.transform.position = cam.transform.position + shakeVector.normalized * shakeOffset;
					}
				}
			}
		}
		
		void OnCamPostRender(Camera cam)
		{
			if(cameraShaking)
			{
				if(cam == cameraToShake)
				{
					//restore position
					cam.transform.position = camOriginalPosition;
				}
			}
		}
		
		//--------------------------------------------------------------------------------------------------
		// PUBLIC
		
		public void DoEndAction()
		{
			switch(endAction)
			{
				case EndAction.DestroyGameObject:
					Object.Destroy(this.gameObject);
					break;

				case EndAction.DeactivateGameObject:
					this.gameObject.SetActive(false);
					break;
			}
		}

		public void StartCameraShake()
		{
			StopCameraShake();

			Camera.onPreRender += OnCamPreRender;
			Camera.onPostRender += OnCamPostRender;

			shakeOffset = 0f;
			cameraShaking = true;
			shakeCoroutine = StartCoroutine(CR_ShakeCamera());
		}

		public void StopCameraShake()
		{
			cameraShaking = false;
			if(shakeCoroutine != null)
				StopCoroutine(shakeCoroutine);

			Camera.onPreRender -= OnCamPreRender;
			Camera.onPostRender -= OnCamPostRender;
		}

		//--------------------------------------------------------------------------------------------------
		// PRIVATE
		
		IEnumerator CR_CheckParticleSystem()
		{
			while(true)
			{
				//wait for the Particle System to start playing, if needed
				while(!ps.isPlaying)
					yield return null;

				//play sound effect, if any
				if(soundEffect != null)
					AudioSource.PlayClipAtPoint(soundEffect, this.transform.position);

				//camera shake
				if(shakeCamera && !DisableCameraShake)
					StartCameraShake();

				//wait for the Particle System to completely finish
				while(ps.IsAlive())
					yield return null;

				//wait for camera shaking to end, if any
				while(cameraShaking)
					yield return null;

				DoEndAction();
			}
		}

		IEnumerator CR_ShakeCamera()
		{
			yield return new WaitForSeconds(shakeDelay);

			float t = 0f;
			float sign = 1f;
			float step = 0f;
			float falloff = 0f;
			shakeOffset = 0f;
			int repeat = shakeRepeat;

			while(t < shakeDuration)
			{
				step += Time.deltaTime;
				if(step > shakeStep)
				{
					step = 0f;

					if(useFalloff)
					{
						float dist = Vector3.Distance(cameraToShake.transform.position, this.transform.position);
						falloff = 1f - Mathf.Clamp01((dist - falloffMin) / (falloffMax - falloffMin));
					}

					if(falloff > 0f || !useFalloff)
					{
						//adjust shake values
						shakeOffset = Mathf.Lerp(useFalloff ? shakeStrength * falloff : shakeStrength, 0f, Mathf.Clamp01(t/shakeDuration)) * sign;
						sign *= -1f;
						if(randomShakeAngle && sign > 0f)
							shakeAngle = Random.Range(0f, 180f);
					}
				}

				t += Time.deltaTime;
				yield return null;

				if(t >= shakeDuration)
				{
					repeat--;
					if(repeat > 0)
					{
						t -= shakeDuration;
					}
				}
			}
			shakeOffset = 0f;

			StopCameraShake();
		}
		
	}
}