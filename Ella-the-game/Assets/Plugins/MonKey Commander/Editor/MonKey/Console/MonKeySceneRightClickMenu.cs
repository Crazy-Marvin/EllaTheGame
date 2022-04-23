using MonKey.Editor.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonKey.Editor.Console
{
    internal class MonKeySceneRightClickMenu : EditorWindow
    {
        public static List<CommandInfo> RightClickContextualCommand = new List<CommandInfo>();

        public static bool ControlClick = true;

        public static MonKeySceneRightClickMenu Instance;

        [InitializeOnLoadMethod]
        public static void PlugToEvent()
        {

           /* SceneView.onSceneGUIDelegate -= CheckMonKeyRightClick;
            SceneView.onSceneGUIDelegate += CheckMonKeyRightClick;*/

        }

        private static void CheckMonKeyRightClick(SceneView sceneview)
        {
            if (Event.current.button != 1 || RightClickContextualCommand.Count==0)
                return;

            if (Event.current.type == EventType.MouseUp && Event.current.control)
            {
                if(Instance)
                    Instance.Close();

                Instance = CreateInstance<MonKeySceneRightClickMenu>();
                Vector2 pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                Instance.ShowAsDropDown(new Rect(pos.x, pos.y, 50, 50), new Vector2(220, Mathf.Clamp(RightClickContextualCommand.Count, 4, 10) * 32));
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            int i = 0;
            foreach (CommandInfo commandInfo in RightClickContextualCommand)
            {
                CommandDisplay.DisplayCommandInSearch(commandInfo,i,-1,true,true,"");
                i++;


            }
            GUILayout.EndVertical();
        }
    }
}