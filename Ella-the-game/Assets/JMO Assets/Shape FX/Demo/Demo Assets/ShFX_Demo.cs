//========================================
//
// SHAPE FX
// (c) 2016 - Jean Moreno
// http://www.jeanmoreno.com/unity
//
//========================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ShapeFX
{
	namespace Demo
	{
		public class ShFX_Demo : MonoBehaviour
		{
			//--------------------------------------------------------------------------------------------------
			// PUBLIC INSPECTOR PROPERTIES
			
			public Transform EffectsRoot;
			public Text TxtEffectName;
			public Text TxtEffectCount;
			public Text TxtAmbienceName;
			public Text TxtEffectCaption;
			public Text TxtEffectWaiting;
			public Canvas UICanvas;
			public Ambience[] Ambiences;

			public bool killLoopEffects { get; set; }
			public bool shakeCamera
			{
				get { return !ShFX_EffectHandler.DisableCameraShake;  }
				set { ShFX_EffectHandler.DisableCameraShake = !value; }
			}

			//--------------------------------------------------------------------------------------------------
			// PRIVATE PROPERTIES

			private float effectCaptionTargetAlpha = 1f;
			private float effectWaitingTargetAlpha = 0f;

			private int effectIndex = 0;
			private List<ParticleSystem> particlesSystemsList;
			private ParticleSystem currentEffect;

			[System.Serializable]
			public class Ambience
			{
				public string Name;
				public Material Skybox;
				public Color AmbientColor;
				public Color FogColor;
				public GameObject LightingGroup;
			}
			
			private int selectedAmbiance = 0;

			private bool waitingForEffect;
			private bool skipEffectStop;

			//--------------------------------------------------------------------------------------------------
			// UNITY EVENTS
			
			void Awake()
			{
				particlesSystemsList = new List<ParticleSystem>();
				for(int i = 0; i < EffectsRoot.childCount; i++)
				{
					var ps = EffectsRoot.GetChild(i).GetComponent<ParticleSystem>();
					if(ps != null)
					{
						particlesSystemsList.Add(ps);
					}
				}

				killLoopEffects = true;
				shakeCamera = true;

				UpdateCurrentEffect(0);
			}

			void Update()
			{
				if(Input.GetKeyDown(KeyCode.RightArrow))
				{
					UpdateCurrentEffect(+1);
				}
				else if(Input.GetKeyDown(KeyCode.LeftArrow))
				{
					UpdateCurrentEffect(-1);
				}

				else if(Input.GetKeyDown(KeyCode.UpArrow))
				{
					UpdateAmbience(+1);
				}
				else if(Input.GetKeyDown(KeyCode.DownArrow))
				{
					UpdateAmbience(-1);
				}

				else if(Input.GetKeyDown(KeyCode.Delete))
				{
					if(waitingForEffect)
						skipEffectStop = true;
				}
				else if(Input.GetKeyDown(KeyCode.H))
				{
					UICanvas.enabled = !UICanvas.enabled;
				}

				//Text fading
				float diff = Mathf.Abs(effectCaptionTargetAlpha - TxtEffectCaption.color.a);
				if(diff > 0.01f)
				{
					Color col = TxtEffectCaption.color;
					col.a = effectCaptionTargetAlpha;
					TxtEffectCaption.color = Color.Lerp(TxtEffectCaption.color, col, Time.deltaTime * 4f);

					if(diff < 0.03f)
					{
						col.a = effectCaptionTargetAlpha;
						TxtEffectCaption.color = col;
					}
				}

				diff = Mathf.Abs(effectWaitingTargetAlpha - TxtEffectWaiting.color.a);
				if(diff > 0.01f)
				{
					Color col = TxtEffectWaiting.color;
					col.a = effectWaitingTargetAlpha;
					TxtEffectWaiting.color = Color.Lerp(TxtEffectWaiting.color, col, Time.deltaTime * 4f);
					
					if(diff < 0.03f)
					{
						col.a = effectWaitingTargetAlpha;
						TxtEffectWaiting.color = col;
					}
				}

			}
			
			//--------------------------------------------------------------------------------------------------

			public void UpdateCurrentEffect(int indexOffset)
			{
				effectIndex += indexOffset;
				if(indexOffset < 0 && effectIndex < 0) effectIndex = particlesSystemsList.Count-1;
				if(indexOffset > 0 && effectIndex > particlesSystemsList.Count-1) effectIndex = 0;

				StartCoroutine(CR_PlayNextEffect());
			}

			private IEnumerator CR_PlayNextEffect()
			{
				if(waitingForEffect) yield break;
				waitingForEffect = true;

				StopCoroutine("CR_CheckCurrentEffect");

				if(currentEffect != null)
				{
					if(!killLoopEffects && !skipEffectStop)
					{
						currentEffect.Stop();
						var lightEffect = currentEffect.GetComponentInChildren<ShFX_EffectLight>();
						if(lightEffect != null)
							lightEffect.StopLightEffect();

#if UNITY_5_5_OR_NEWER
						while(currentEffect.main.loop && (currentEffect.IsAlive() || (lightEffect != null && lightEffect.isPlaying)))
#else
						while(currentEffect.loop && (currentEffect.IsAlive() || (lightEffect != null && lightEffect.isPlaying)))
#endif
						{
							TxtEffectName.text = particlesSystemsList[effectIndex].name;
							TxtEffectCount.text = GetEffectCountLabel();
							
							effectCaptionTargetAlpha = 0f;
							effectWaitingTargetAlpha = 1f;

							if(skipEffectStop || killLoopEffects)
							{
								skipEffectStop = false;
								currentEffect.gameObject.SetActive(false);
								break;
							}

							yield return new WaitForSeconds(0.1f);
						}
					}

					currentEffect.gameObject.SetActive(false);
				}

				effectCaptionTargetAlpha = 1f;
				effectWaitingTargetAlpha = 0f;

				currentEffect = particlesSystemsList[effectIndex];
				currentEffect.gameObject.SetActive(true);
				currentEffect.Play(true);

				TxtEffectName.text = currentEffect.name;
				TxtEffectCount.text = GetEffectCountLabel();

				yield return new WaitForSeconds(0.1f);
				StartCoroutine("CR_CheckCurrentEffect");
				
				waitingForEffect = false;
			}

			private string GetEffectCountLabel()
			{
				return string.Format("{0}/{1}", (effectIndex+1).ToString("00"), particlesSystemsList.Count);
			}

			private IEnumerator CR_CheckCurrentEffect()
			{
				while(true)
				{
					yield return null;

					if(currentEffect != null && !currentEffect.IsAlive())
					{
						yield return new WaitForSeconds(0.5f);
						currentEffect.Play();
					}
				}
			}

			public void UpdateAmbience(int indexOffset)
			{
				selectedAmbiance += indexOffset;
				if(selectedAmbiance >= Ambiences.Length) selectedAmbiance = 0;
				if(selectedAmbiance < 0) selectedAmbiance = Ambiences.Length-1;

				if(Ambiences[selectedAmbiance] == null) return;

				TxtAmbienceName.text = "Ambience: " + Ambiences[selectedAmbiance].Name;

				RenderSettings.skybox = Ambiences[selectedAmbiance].Skybox;
				RenderSettings.fogColor = Ambiences[selectedAmbiance].FogColor;
				RenderSettings.ambientSkyColor = Ambiences[selectedAmbiance].AmbientColor;
				
				for(int i = 0; i < Ambiences.Length; i++)
				{
					Ambiences[i].LightingGroup.SetActive( i == selectedAmbiance );
				}
			}
		}
	}
}
