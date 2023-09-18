using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MonKey.Internal
{
    public static class EditorGUIExt
    {
        /// <summary>
        /// Thanks to https://forum.unity.com/threads/how-to-copy-and-paste-in-a-custom-editor-textfield.261087/
        /// Add copy-paste functionality to any text field
        /// Returns changed text or NULL.
        /// Usage: text = HandleCopyPaste (controlID) ?? text;
        /// </summary>
        public static string HandleCopyPaste(int controlID)
        {

            if (Event.current.type == EventType.KeyUp && (Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command))
            {
                if (Event.current.keyCode == KeyCode.C)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.Copy();
                }
                else if (Event.current.keyCode == KeyCode.V)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                   /* Debug.Log(editor.text);

                    editor.Paste();
                    Debug.Log(editor.text);*/

                    return editor.text;
                }
                else if (Event.current.keyCode == KeyCode.A)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.SelectAll();
                    return editor.text;
                }
            }

            return null;
        }

        /// <summary>
        /// Thanks to https://forum.unity.com/threads/how-to-copy-and-paste-in-a-custom-editor-textfield.261087/
        /// TextField with copy-paste support
        /// </summary>
        public static string TextField(string value, string controlName, GUIStyle style, params GUILayoutOption[] options)
        {
            int textFieldID = GUIUtility.keyboardControl;

            // Handle custom copy-paste
            value = HandleCopyPaste(textFieldID) ?? value;

            return GUILayout.TextField(value, style, options);
        }
    }
}
