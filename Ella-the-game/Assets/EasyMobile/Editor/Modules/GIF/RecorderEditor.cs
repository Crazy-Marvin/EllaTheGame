using UnityEngine;
using UnityEditor;

namespace EasyMobile.Editor
{
    [CustomEditor(typeof(Recorder))]
    public sealed class RecorderEditor : UnityEditor.Editor
    {

        SerializedProperty autoHeight;
        SerializedProperty width;
        SerializedProperty height;
        SerializedProperty framePerSecond;
        SerializedProperty length;
        SerializedProperty state;

        Camera attachedCam;

        void OnEnable()
        {
            autoHeight = serializedObject.FindProperty("_autoHeight");
            width = serializedObject.FindProperty("_width");
            height = serializedObject.FindProperty("_height");
            framePerSecond = serializedObject.FindProperty("_framePerSecond");
            length = serializedObject.FindProperty("_length");
            state = serializedObject.FindProperty("_state");
            attachedCam = ((Recorder)serializedObject.targetObject).GetComponent<Camera>();
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            // Not allow tweaking settings while playing as it may break everything.
            if (Application.isEditor && Application.isPlaying)
                GUI.enabled = false; 

            EditorGUILayout.PropertyField(autoHeight, new GUIContent("Auto Height", "Automatically calculate clip height based on clip width and camera's aspect ratio"));
            EditorGUILayout.PropertyField(width, new GUIContent("Width", "Width in pixels"));

            if (!autoHeight.boolValue)
                EditorGUILayout.PropertyField(height, new GUIContent("Height", "Height in pixels"));
            else
            {
                height.intValue = Recorder.CalculateAutoHeight(width.intValue, (Camera)attachedCam);
                EditorGUILayout.LabelField(new GUIContent("Height", "Height in pixels, computed automatically based on current width and camera's aspect ratio"), new GUIContent(height.intValue.ToString()));
            }

            EditorGUILayout.PropertyField(framePerSecond, new GUIContent("Frames Per Second", "The target FPS of the clip"));
            EditorGUILayout.PropertyField(length, new GUIContent("Length", "Clip length in seconds, the recorder automatically discards old content if needed to preserve this length"));

            float memUsed = Recorder.EstimateMemoryUse(width.intValue, height.intValue, framePerSecond.intValue, length.floatValue);
            EditorGUILayout.LabelField(new GUIContent("Estimated VRam Usage", "The estimated memory used for recording"), new GUIContent(memUsed.ToString("F3") + " MB"));

            // Display current state
            EditorGUILayout.LabelField("Current State", state.enumDisplayNames[state.enumValueIndex]);

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
    }
}
