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
	[CustomEditor(typeof(ShFX_EffectHandler)), CanEditMultipleObjects()]
	public class ShFX_EffectHandler_Editor : Editor
	{
		private SerializedProperty prop_endAction;
		private SerializedProperty prop_soundEffect;
		private SerializedProperty prop_shakeCamera;
		private SerializedProperty prop_cameraToShake;
		private SerializedProperty prop_shakeAngle;
		private SerializedProperty prop_randomShakeAngle;
		private SerializedProperty prop_shakeDuration;
		private SerializedProperty prop_shakeDelay;
		private SerializedProperty prop_shakeRepeat;
		private SerializedProperty prop_shakeStep;
		private SerializedProperty prop_shakeStrength;
		private SerializedProperty prop_useFalloff;
		private SerializedProperty prop_falloffMin;
		private SerializedProperty prop_falloffMax;

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
			prop_endAction = serializedObject.FindProperty("endAction");
			prop_soundEffect = serializedObject.FindProperty("soundEffect");
			prop_shakeCamera = serializedObject.FindProperty("shakeCamera");
			prop_cameraToShake = serializedObject.FindProperty("cameraToShake");
			prop_shakeAngle = serializedObject.FindProperty("shakeAngle");
			prop_randomShakeAngle = serializedObject.FindProperty("randomShakeAngle");
			prop_shakeDuration = serializedObject.FindProperty("shakeDuration");
			prop_shakeDelay = serializedObject.FindProperty("shakeDelay");
			prop_shakeRepeat = serializedObject.FindProperty("shakeRepeat");
			prop_shakeStep = serializedObject.FindProperty("shakeStep");
			prop_shakeStrength = serializedObject.FindProperty("shakeStrength");
			prop_useFalloff = serializedObject.FindProperty("useFalloff");
			prop_falloffMin = serializedObject.FindProperty("falloffMin");
			prop_falloffMax = serializedObject.FindProperty("falloffMax");
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
			bool guiEnabled = GUI.enabled;

			//--------

			EditorGUILayout.PropertyField(prop_endAction);
			EditorGUILayout.PropertyField(prop_soundEffect);
			EditorGUILayout.PropertyField(prop_shakeCamera);
			GUI.enabled &= prop_shakeCamera.boolValue;
			EditorGUILayout.PropertyField(prop_cameraToShake);
			if(prop_cameraToShake.objectReferenceValue == null)
				EditorGUILayout.HelpBox("Camera.main will be used if 'Camera To Shake' is not specified.", MessageType.Info, false);
			GUI.enabled &= !prop_randomShakeAngle.boolValue;
			EditorGUILayout.PropertyField(prop_shakeAngle);
			GUI.enabled = guiEnabled & prop_shakeCamera.boolValue;
			EditorGUILayout.PropertyField(prop_randomShakeAngle);
			EditorGUILayout.PropertyField(prop_shakeDuration);
			EditorGUILayout.PropertyField(prop_shakeDelay);
			EditorGUILayout.PropertyField(prop_shakeRepeat);
			EditorGUILayout.PropertyField(prop_shakeStep);
			EditorGUILayout.PropertyField(prop_shakeStrength);
			EditorGUILayout.PropertyField(prop_useFalloff);
			GUI.enabled &= prop_useFalloff.boolValue;
			EditorGUILayout.PropertyField(prop_falloffMin);
			EditorGUILayout.PropertyField(prop_falloffMax);
			GUI.enabled = guiEnabled;
			
			//--------
			
			if(GUI.changed)
			{
				prop_falloffMax.floatValue = Mathf.Max(prop_falloffMax.floatValue, prop_falloffMin.floatValue);

				serializedObject.ApplyModifiedProperties();
			}
			
			GUI.enabled = guiEnabled;
		}

		//--------------------------------------------------------------------------------------------------------------------------------
	}
}