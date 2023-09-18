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
#if UNITY_EDITOR
using UnityEditor;
#endif

// Handles a Light attached to an Effect

namespace ShapeFX
{
	[RequireComponent(typeof(Light))]
	[ExecuteInEditMode()]
	public class ShFX_EffectLight : MonoBehaviour
	{
		//--------------------------------------------------------------------------------------------------
		// PUBLIC INSPECTOR PROPERTIES
		
		public ParticleSystem linkedEffect;
		public bool playOnEnable = false;
		[Tooltip("Detect when the linked Particle System starts playing to play along with it")]
		public bool autoPlayFromParticleSystem = true;
		public float delay = 0.0f;
		public float duration = 1.0f;
		public float playbackSpeed = 1.0f;
		public bool loop = false;
		public float fadeIn = 0f;
		public float fadeOut = 0f;
		[Range(0f, 8f)]
		public float peakIntensity = 2.0f;
		public AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
		public bool useColorGradient = false;
		public Gradient colorGradient;
		[HideInInspector]
		public bool colorFromParticleSystem = false;
		[HideInInspector]
		public Color cachedColor = new Color(1,1,1,1);
		[HideInInspector, System.NonSerialized]
		public float editorPlaybackSpeed = 1.0f;
		[HideInInspector, System.NonSerialized]
		public bool isPlaying;

		private float PlaybackSpeed { get { return playbackSpeed * editorPlaybackSpeed; } }

		//--------------------------------------------------------------------------------------------------
		// PRIVATE PROPERTIES

		private Light _light;
		public Light lightComponent
		{
			get
			{
				if(_light == null)
					_light = this.GetComponent<Light>();
				return _light;
			}
		}

		private float playbackTime;
		private float fadeTime;
		private bool particleSystemPlaying;

		//--------------------------------------------------------------------------------------------------
		// PUBLIC
		
		public void PlayLightEffect()
		{
			lightComponent.enabled = true;
			playbackTime = 0f;
			fadeTime = Mathf.Clamp(fadeIn, 0f, int.MaxValue);
			isPlaying = true;

#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				GetEffectMaxDuration();
				
				//take back to where the particle system is at
				if(editorParticlePreviewTime > 0f)
				{
					playbackTime = editorParticlePreviewTime;
					fadeTime = fadeIn - Mathf.Clamp(editorParticlePreviewTime, 0f, fadeIn);
				}
			}
#endif
		}

#if UNITY_EDITOR
		public void Editor_RestartLightEffect()
		{
			lightComponent.enabled = true;
			playbackTime = 0f;
			fadeTime = Mathf.Clamp(fadeIn, 0f, int.MaxValue);
			isPlaying = true;
		}
#endif
		
		public void StopLightEffect(bool immediate = false)
		{
			if(!loop || immediate)
			{
				isPlaying = false;
				lightComponent.enabled = false;
				lightComponent.intensity = 0f;
			}
			else
			{
				//start fading out
				fadeTime = Mathf.Clamp(-fadeOut - 0.0001f, int.MinValue, 0f);
			}
		}

		//--------------------------------------------------------------------------------------------------
		// UNITY EVENTS

		void Reset()
		{
			if(linkedEffect == null)
			{
				linkedEffect = this.GetComponentInParent<ParticleSystem>();
			}
		}

		void OnEnable()
		{
			if(linkedEffect == null)
			{
				linkedEffect = this.GetComponentInParent<ParticleSystem>();
			}

			if(playOnEnable)
			{
				PlayLightEffect();
			}
		}

		void OnDisable()
		{
			particleSystemPlaying = false;
			StopLightEffect(true);
		}

		void Update()
		{
			//--------------------------------
			// AUTOPLAY DETECTION
			if(autoPlayFromParticleSystem)
			{
				if(linkedEffect.isPlaying != particleSystemPlaying)
				{
					if(linkedEffect.isPlaying)
					{
						PlayLightEffect();
					}
				}
				
				particleSystemPlaying = linkedEffect.isPlaying;
			}

#if UNITY_EDITOR
			if(autoPlayFromParticleSystem)
			{
				//handle scrubbing through particle system editor preview
				if(linkedEffect.isPaused)
				{
					lightComponent.enabled = true;
					isPlaying = true;
					playbackTime = editorParticlePreviewTime;
					fadeTime = fadeIn - Mathf.Clamp(editorParticlePreviewTime, 0f, fadeIn);
				}

				if(!Application.isPlaying)
				{
					//get editor particle preview values (through reflection)
					TryUpdateEditorParticleValues();
					if(hasEditorValues)
					{
						//try to detect if user manually stopped the Particle System
						if(editorParticlePreviewTime == 0f && editorParticlePreviewTime < lastEditorParticlePreviewTime)
						{
							float diff = lastEditorParticlePreviewTime - editorParticlePreviewTime;
							if(!linkedEffect.isPlaying && (diff < effectMaxDuration || loop))
							{
								//manual stop
								StopLightEffect(true);
							}
						}
						lastEditorParticlePreviewTime = editorParticlePreviewTime;
					}
				}
			}
			
			//handle editor deltaTime
			editorDeltaTime = Time.realtimeSinceStartup - lastRealtime;
			lastRealtime = Time.realtimeSinceStartup;
#endif

			//--------------------------------
			// PLAYBACK
			if(isPlaying)
			{
				float d = Mathf.Clamp01( (playbackTime % duration) / (duration+float.Epsilon) );	// range [0:1]

				//adjust light color
				if(useColorGradient)
				{
					this.lightComponent.color = colorGradient.Evaluate(d);
				}

				//target intensity (if no fading)
				float targetIntensity = Mathf.Lerp(0f, peakIntensity, intensityCurve.Evaluate(d));

				//fading in
				if(fadeTime > 0f)
				{
					fadeTime -= deltaTime;
					
					//fade in end
					if(fadeTime <= 0f)
						fadeTime = 0f;
					else
						targetIntensity = Mathf.Lerp(targetIntensity, 0f, fadeTime/fadeIn);
				}
				//fading out
				else if(fadeTime < 0f)
				{
					fadeTime += deltaTime;
					
					//fade out end
					if(fadeTime >= 0f)
					{
						fadeTime = 0f;
						StopLightEffect(true);
					}
					else
						targetIntensity = Mathf.Lerp(0f, targetIntensity, -fadeTime/fadeOut);
				}

				//actually set light intensity
				lightComponent.intensity = targetIntensity;

				//advance playback time
				playbackTime += deltaTime;

				//stop
				if(!loop && playbackTime > duration)
				{
					StopLightEffect(true);
				}
			}
		}

		//--------------------------------------------------------------------------------------------------
		// EDITOR ONLY

		private float deltaTime
		{
			get
			{
#if UNITY_EDITOR
				return (Application.isPlaying ? Time.deltaTime : editorDeltaTime) * PlaybackSpeed;
#else
				return Time.deltaTime * PlaybackSpeed;
#endif
			}
		}

#if UNITY_EDITOR
		[System.NonSerialized]
		private float editorDeltaTime;
		[System.NonSerialized]
		private float lastRealtime;
		[System.NonSerialized]
		private float lastEditorParticlePreviewTime;
		[System.NonSerialized]
		private float editorParticlePreviewTime;
		[System.NonSerialized]
		private bool hasEditorValues;
		[System.NonSerialized]
		private float effectMaxDuration;

		private void GetEffectMaxDuration()
		{
			ParticleSystem[] particleSystems = linkedEffect.GetComponentsInChildren<ParticleSystem>();
			effectMaxDuration = -1f;
			foreach(ParticleSystem ps in particleSystems)
			{
#if UNITY_5_5_OR_NEWER
				effectMaxDuration = Mathf.Max(effectMaxDuration, ps.main.duration + ps.main.startDelay.constantMax);
#else
				effectMaxDuration = Mathf.Max(effectMaxDuration, ps.duration + ps.startDelay);
#endif
			}
		}

		//get particle playback state in Editor
		private void TryUpdateEditorParticleValues()
		{
			hasEditorValues = false;
			System.Reflection.Assembly unityEditorAssembly = typeof(UnityEditor.Editor).Assembly;
			if(unityEditorAssembly != null)
			{
				System.Type pseu = unityEditorAssembly.GetType("UnityEditor.ParticleSystemEditorUtils");
				if(pseu != null)
				{
					//playback time
					System.Reflection.PropertyInfo editorPlaybackTime = pseu.GetProperty("editorPlaybackTime", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
					if(editorPlaybackTime != null)
					{
						object oValue = editorPlaybackTime.GetValue(null, null);
						if(oValue != null && oValue is float)
						{
							editorParticlePreviewTime = (float)oValue;
							hasEditorValues = true;
						}
					}

					//simulation speed
					System.Reflection.PropertyInfo editorSimulationSpeed = pseu.GetProperty("editorSimulationSpeed", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
					if(editorSimulationSpeed != null)
					{
						object oValue = editorSimulationSpeed.GetValue(null, null);
						if(oValue != null && oValue is float)
						{
							this.editorPlaybackSpeed = (float)oValue;
						}
					}
				}
			}
		}
#endif
	}
}