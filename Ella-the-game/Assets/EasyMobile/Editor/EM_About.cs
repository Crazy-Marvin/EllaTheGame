using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EasyMobile.Editor
{
    public class EM_About : EditorWindow
    {
        const int WINDOW_WIDTH = 512;
        const int WINDOW_HEIGHT = 240;
        const int IMAGE_WIDTH = 512;
        const int IMAGE_HEIGHT = 200;

        Texture2D mainImage;

        void OnEnable()
        {
            // Set the window title
            #if UNITY_PRE_5_1
            title = "About";
            #else
            titleContent = new GUIContent("About");            
            #endif

            // Load the main image
            mainImage = EM_GUIStyleManager.AboutEMTex;

            // Set sizes
            Vector2 fixedSizes = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
            maxSize = fixedSizes;
            minSize = fixedSizes;
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();
            if (mainImage != null)
                GUI.DrawTexture(new Rect(0f, 0f, IMAGE_WIDTH, IMAGE_HEIGHT), mainImage, ScaleMode.ScaleAndCrop);

            GUILayout.FlexibleSpace();
            GUILayout.Label("Version " + EM_Constants.versionString);
            GUILayout.Label(EM_Constants.Copyright);

            GUILayout.EndVertical();
        }
    }
}
