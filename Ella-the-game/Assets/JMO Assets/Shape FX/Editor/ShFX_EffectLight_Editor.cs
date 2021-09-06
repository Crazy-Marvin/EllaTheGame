//========================================
//
// SHAPE FX
// (c) 2016 - Jean Moreno
// http://www.jeanmoreno.com/unity
//
//========================================

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ShapeFX
{
	[CustomEditor(typeof(ShFX_EffectLight)), CanEditMultipleObjects()]
	public class ShFX_EffectLight_Editor : Editor
	{
		private ShFX_EffectLight lightEffect { get { return this.target as ShFX_EffectLight; } }
		
		private SerializedProperty prop_linkedEffect;
		private SerializedProperty prop_playOnEnable;
		private SerializedProperty prop_autoPlayFromParticleSystem;
		private SerializedProperty prop_delay;
		private SerializedProperty prop_duration;
		private SerializedProperty prop_playbackSpeed;
		private SerializedProperty prop_loop;
		private SerializedProperty prop_fadeIn;
		private SerializedProperty prop_fadeOut;
		private SerializedProperty prop_maxIntensity;
		private SerializedProperty prop_intensityCurve;
		private SerializedProperty prop_useGradient;
		private SerializedProperty prop_gradient;
		private SerializedProperty prop_linkedColor;
		private SerializedProperty prop_cachedColor;

		private GenericMenu revertValueMenu;

		//--------------------------------------------------------------------------------------------------------------------------------
		// Callbacks

		void OnEnable()
		{
			GetProperties();
		}

		void OnDisable()
		{

		}

		private void GetProperties()
		{
			prop_linkedEffect = serializedObject.FindProperty("linkedEffect");
			prop_playOnEnable = serializedObject.FindProperty("playOnEnable");
			prop_autoPlayFromParticleSystem = serializedObject.FindProperty("autoPlayFromParticleSystem");
			prop_delay = serializedObject.FindProperty("delay");
			prop_duration = serializedObject.FindProperty("duration");
			prop_playbackSpeed = serializedObject.FindProperty("playbackSpeed");
			prop_loop = serializedObject.FindProperty("loop");
			prop_fadeIn = serializedObject.FindProperty("fadeIn");
			prop_fadeOut = serializedObject.FindProperty("fadeOut");
			prop_maxIntensity = serializedObject.FindProperty("peakIntensity");
			prop_intensityCurve = serializedObject.FindProperty("intensityCurve");
			prop_useGradient = serializedObject.FindProperty("useColorGradient");
			prop_gradient = serializedObject.FindProperty("colorGradient");
			prop_linkedColor = serializedObject.FindProperty("colorFromParticleSystem");
			prop_cachedColor = serializedObject.FindProperty("cachedColor");

			revertValueMenu = new GenericMenu();
			revertValueMenu.AddItem(new GUIContent("Revert Value to Prefab"), false, OnRevertCurve);
		}

		void OnRevertCurve()
		{
			if(prop_intensityCurve != null && prop_intensityCurve.prefabOverride)
			{
				prop_intensityCurve.prefabOverride = false;
				prop_intensityCurve.serializedObject.ApplyModifiedProperties();
				prop_intensityCurve.serializedObject.Update();
			}
		}

		//--------------------------------------------------------------------------------------------------------------------------------
		// GUI

		public override void OnInspectorGUI()
		{
#if UNITY_5_6_OR_NEWER
			serializedObject.UpdateIfRequiredOrScript();
#else
			serializedObject.UpdateIfDirtyOrScript();
#endif
			//Light shortcuts
			SerializedObject lightSo = new SerializedObject(lightEffect.lightComponent);
			SerializedProperty prop_lightColor = lightSo.FindProperty("m_Color");

			bool guiEnabled = GUI.enabled;

			GUILayout.Space(6f);

			GUI.enabled &= prop_autoPlayFromParticleSystem.boolValue;
			EditorGUILayout.PropertyField(prop_linkedEffect);
			GUI.enabled = guiEnabled;
			EditorGUILayout.PropertyField(prop_autoPlayFromParticleSystem, new GUIContent("Auto Play from Linked Effect"));
			
			GUILayout.Label("LIGHT ANIMATION", EditorStyles.boldLabel);

			GUI.enabled &= !prop_autoPlayFromParticleSystem.boolValue;
			EditorGUILayout.PropertyField(prop_playOnEnable);
			GUI.enabled = guiEnabled;
			EditorGUILayout.PropertyField(prop_duration);
			EditorGUILayout.PropertyField(prop_playbackSpeed);
			EditorGUILayout.PropertyField(prop_delay);
			EditorGUILayout.PropertyField(prop_loop);
			GUI.enabled &= prop_loop.boolValue;
			EditorGUILayout.PropertyField(prop_fadeIn);
			EditorGUILayout.PropertyField(prop_fadeOut);
			GUI.enabled = guiEnabled;
			GUILayout.Space(6f);
			EditorGUILayout.PropertyField(prop_maxIntensity);
			//custom animation curve property to force [0:1] range
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(prop_intensityCurve.displayName, EditorStyles.label, prop_intensityCurve.prefabOverride ? EditorStyles.boldLabel : EditorStyles.label);
			prop_intensityCurve.animationCurveValue = EditorGUILayout.CurveField(prop_intensityCurve.animationCurveValue, Color.yellow, new Rect(0f, 0f, 1f, 1f));
			EditorGUILayout.EndHorizontal();
			if(prop_intensityCurve.isInstantiatedPrefab)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				if(Event.current.type == EventType.MouseDown && Event.current.button == 1 && lastRect.Contains(Event.current.mousePosition))
				{
					revertValueMenu.ShowAsContext();
				}
			}

			GUILayout.Space(6f);
			GUILayout.Label("LIGHT COLOR", EditorStyles.boldLabel);

			bool prop_linkedColor_previous = prop_linkedColor.boolValue;
			EditorGUILayout.PropertyField(prop_linkedColor, new GUIContent("Color from PS", "Set Light Color from Particle System"));
			
			GUI.enabled &= !prop_linkedColor.boolValue;
			bool prop_useGradient_previous = prop_useGradient.boolValue;
			EditorGUILayout.PropertyField(prop_useGradient);
			GUI.enabled = guiEnabled;
			
			// restore previous light color if reverted from linked color or gradient color
			if((prop_linkedColor_previous != prop_linkedColor.boolValue)
			   ||(prop_useGradient_previous != prop_useGradient.boolValue))
			{
				prop_lightColor.colorValue = lightEffect.cachedColor;
				lightSo.ApplyModifiedProperties();
			}
			
			GUI.enabled &= !prop_linkedColor.boolValue;
			if(!prop_linkedColor.boolValue && prop_useGradient.boolValue)
				EditorGUILayout.PropertyField(prop_gradient);
			else
				EditorGUILayout.PropertyField(prop_lightColor, new GUIContent("Light Color"));
			GUI.enabled = guiEnabled;
			
			//--------
			
			if(GUI.changed)
			{
				if(!prop_linkedColor.boolValue && !prop_useGradient.boolValue)
				{
					prop_cachedColor.colorValue = prop_lightColor.colorValue;
				}
				
				serializedObject.ApplyModifiedProperties();
				lightSo.ApplyModifiedProperties();
			}
			
			GUI.enabled = guiEnabled;
		}

		//--------------------------------------------------------------------------------------------------------------------------------
	}
}