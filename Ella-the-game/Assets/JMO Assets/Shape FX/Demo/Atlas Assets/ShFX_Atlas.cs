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
		public class ShFX_Atlas : MonoBehaviour
		{
			//--------------------------------------------------------------------------------------------------
			// PUBLIC INSPECTOR PROPERTIES
			
			public Camera noGroupCam;
			public Image Fader;
			public Transform TextEffectsRoot;
			public Text TextPage;
			public Text TextGroupModel;
			public bool includeInactiveGroups;
			public bool playbackLoop;
			public bool autoplayParticles;

			//--------------------------------------------------------------------------------------------------
			// PRIVATE PROPERTIES

			public int currentGroup = 0;
			private List<GameObject> groups = new List<GameObject>();
			private List<ParticleSystem> currentParticles = new List<ParticleSystem>();
			private Camera currentCam;

			private Text[] textsEffectNames;
			private Color textDefaultColor = new Color(0.8f,0.8f,0.8f,0.25f);
			private Color textClearColor = new Color(0.8f,0.8f,0.8f,0f);

			private Text[] textsGroupNames;
			private RectTransform textsGroupNamesRoot;
			private Color colorGroupSelected = new Color(0.6f,0.6f,0.6f,1f);
			private Color colorGroupIdle = new Color(0.2f,0.2f,0.2f,1f);
			private bool groupNameFixed;

			private bool tweening;
			private Coroutine currentCoroutine;
			private Dictionary<ParticleSystem, Coroutine> playParticleCoroutines = new Dictionary<ParticleSystem, Coroutine>();

			//--------------------------------------------------------------------------------------------------
			// UNITY EVENTS

			IEnumerator Start()
			{
				//Needed for soft particles
				noGroupCam.depthTextureMode = DepthTextureMode.Depth;

				ShFX_EffectHandler.DisableCameraShake = true;

				//Get groups
				groups.Clear();
				int index = 0;
				for(int i = 0; i < this.transform.childCount; i++)
				{
					GameObject go = this.transform.GetChild(i).gameObject;
					if(go.activeSelf || includeInactiveGroups)
					{
						Camera[] cams = go.GetComponentsInChildren<Camera>(true);
						if(cams != null && cams.Length > 0 && cams[0] != null)
							cams[0].depthTextureMode = DepthTextureMode.Depth;

						groups.Add(go);
						go.SetActive(index == currentGroup);
						index++;
					}
				}

				//Get effects texts
				textsEffectNames = TextEffectsRoot.GetComponentsInChildren<Text>(true);
				if(textsEffectNames.Length > 0)
				{
					textDefaultColor = textsEffectNames[0].color;
					textClearColor = textDefaultColor;
					textClearColor.a = 0f;
				}

				//Get groups texts
				textsGroupNamesRoot = TextGroupModel.rectTransform.parent as RectTransform;
				textsGroupNames = new Text[groups.Count];
				for(int i = 0; i < groups.Count; i++)
				{
					textsGroupNames[i] = Instantiate<Text>(TextGroupModel);
					textsGroupNames[i].transform.SetParent(textsGroupNamesRoot, false);
					textsGroupNames[i].name = string.Format("TextGroup {0}", i+1);
					textsGroupNames[i].text = groups[i].name;
					textsGroupNames[i].gameObject.SetActive(true);
				}

				//Reposition group texts
				Canvas.ForceUpdateCanvases();
				for(int i = 0; i < textsGroupNames.Length; i++)
				{
					if(i == 0)
						textsGroupNames[i].rectTransform.anchoredPosition = Vector2.zero;
					else
						textsGroupNames[i].rectTransform.anchoredPosition = textsGroupNames[i-1].rectTransform.anchoredPosition + new Vector2(textsGroupNames[i-1].rectTransform.rect.width/2 + textsGroupNames[i].rectTransform.rect.width/2 + 100f, 0f);
				}

				yield return new WaitForSeconds(0.1f);

				GetCurrentParticles();
				UpdateToNewGroup(currentGroup);
			}

			void Update()
			{
				//Input
				if(Input.GetKeyDown(KeyCode.RightArrow))
				{
					NextGroup();
				}
				else if(Input.GetKeyDown(KeyCode.LeftArrow))
				{
					PreviousGroup();
				}

				if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
				{
					foreach(KeyValuePair<ParticleSystem, Coroutine> kvp in playParticleCoroutines)
					{
						StopCoroutine(kvp.Value);
					}
					playParticleCoroutines.Clear();
					foreach(ParticleSystem ps in currentParticles)
					{
						ps.Clear();
						ps.Play();

						var lightFx = ps.GetComponentInChildren<ShFX_EffectLight>();
						if(lightFx != null)
							lightFx.PlayLightEffect();
					}
				}

				//Loop effects
				if(playbackLoop)
				{
					foreach(ParticleSystem ps in currentParticles)
					{
						if(!ps.IsAlive() && !playParticleCoroutines.ContainsKey(ps))
						{
							playParticleCoroutines.Add(ps, StartCoroutine(CR_PlayParticle(ps)));
						}
					}
				}

				//Tween group name
				Vector2 targetPosition = new Vector2(-textsGroupNames[currentGroup].rectTransform.anchoredPosition.x, 0f);
				float dist = Vector2.Distance(textsGroupNamesRoot.anchoredPosition, targetPosition);
				if(dist > 0.01f)
					textsGroupNamesRoot.anchoredPosition = Vector2.Lerp(textsGroupNamesRoot.anchoredPosition, targetPosition, 4 * Time.deltaTime);
				else
					textsGroupNamesRoot.anchoredPosition = targetPosition;

				groupNameFixed = dist < 25f;
			}

			IEnumerator CR_PlayParticle(ParticleSystem particle)
			{
				yield return new WaitForSeconds(0.5f);
				particle.Play();
				playParticleCoroutines.Remove(particle);
			}

			//--------------------------------------------------------------------------------------------------
			// PRIVATE

			public void NextGroup()
			{
				int newGroup = currentGroup;
				newGroup++;
				if(newGroup >= groups.Count)
					newGroup = 0;

				if(!tweening)
				{
					if(currentCoroutine != null)
						StopCoroutine(currentCoroutine);
					currentCoroutine = StartCoroutine(CR_ChangeGroup(currentGroup));
				}

				UpdateToNewGroup(newGroup);
			}
			
			public void PreviousGroup()
			{
				int newGroup = currentGroup;
				newGroup--;
				if(newGroup < 0)
					newGroup = groups.Count-1;

				if(!tweening)
				{
					if(currentCoroutine != null)
						StopCoroutine(currentCoroutine);
					currentCoroutine = StartCoroutine(CR_ChangeGroup(currentGroup));
				}

				UpdateToNewGroup(newGroup);
			}

			private void UpdateToNewGroup(int newGroup)
			{
				textsGroupNames[currentGroup].color = colorGroupIdle;
				currentGroup = newGroup;
				textsGroupNames[currentGroup].color = colorGroupSelected;
				TextPage.text = string.Format("Page {0}/{1}", (groups.Count > 10) ? (currentGroup+1).ToString().PadLeft(2,'0') : (currentGroup+1).ToString(), groups.Count);
			}

			IEnumerator CR_ChangeGroup(int prevGroup)
			{
				tweening = true;

				float duration = 0.33f;
				float t = 0;

				Fader.gameObject.SetActive(true);
				while(t < duration)
				{
					t += Time.deltaTime;
					float d = Mathf.Clamp01(t/duration);
					Fader.color = Color.Lerp(Color.clear, Color.black, d);
					yield return null;
				}

				foreach(KeyValuePair<ParticleSystem, Coroutine> kvp in playParticleCoroutines)
				{
					StopCoroutine(kvp.Value);
				}
				playParticleCoroutines.Clear();

				DeactivateGroup(prevGroup);
				while(!groupNameFixed)
					yield return null;
				ActivateGroup(currentGroup);
				GetCurrentParticles();
				tweening = false;
				
				t = 0f;
				while(t < duration)
				{
					t += Time.deltaTime;
					float d = Mathf.Clamp01(t/duration);
					Fader.color = Color.Lerp(Color.black, Color.clear, d);
					yield return null;
				}
				Fader.gameObject.SetActive(false);
			}

			private void GetCurrentParticles()
			{
				currentCam = groups[currentGroup].GetComponentInChildren<Camera>();

				foreach(Text txt in textsEffectNames)
					txt.gameObject.SetActive(false);

				currentParticles.Clear();
				int index = 0;

				float lowerLabels = 0f;
				bool centerText = false;
				foreach(Transform child in groups[currentGroup].transform)
				{
					if(child.name == "[center text]")
					{
						centerText = true;
					}

					if(child.name.StartsWith("[lower labels:"))
					{
						lowerLabels = float.Parse(child.name.Substring(14, 3));
					}
				}

				for(int i = 0; i < groups[currentGroup].transform.childCount; i++)
				{
					ParticleSystem ps = groups[currentGroup].transform.GetChild(i).GetComponent<ParticleSystem>();
					if(ps != null && ps.gameObject.activeSelf)
					{
						if(!autoplayParticles)
						{
#if UNITY_5_5_OR_NEWER
							var main = ps.main;
							main.playOnAwake = false;
#else
							ps.playOnAwake = false;
#endif
							ps.Stop();
							ps.Clear();
						}

						currentParticles.Add(ps);

						textsEffectNames[index].gameObject.SetActive(true);

						if(centerText)
							textsEffectNames[index].rectTransform.anchoredPosition = Vector2.zero;
						else
						{
							textsEffectNames[index].transform.position = currentCam.WorldToScreenPoint(ps.transform.position);
							if(ps.transform.eulerAngles.x != 0f)
							{
								Vector3 pos = textsEffectNames[index].transform.position;
								pos.y -= 50;
								textsEffectNames[index].transform.position = pos;
							}

							if(lowerLabels > 0f)
							{
								Vector3 pos = textsEffectNames[index].transform.position;
								pos.y -= lowerLabels;
								textsEffectNames[index].transform.position = pos;
							}
						}
						textsEffectNames[index].text = ps.name;
						index++;
					}
				}
			}

			private void DeactivateGroup(int index)
			{
				groups[index].SetActive(false);
				noGroupCam.gameObject.SetActive(true);

				foreach(ParticleSystem ps in currentParticles)
				{
					ps.Clear();
				}
			}
			private void ActivateGroup(int index)
			{
				groups[index].SetActive(true);
				noGroupCam.gameObject.SetActive(false);
			}
		}
	}
}