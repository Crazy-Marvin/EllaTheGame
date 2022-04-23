using System.Text;
using MonKey.Editor.Internal;
using MonKey.Internal;
using UnityEditor;
using UnityEngine;

namespace MonKey.Editor.Console
{
    internal static class LoadingNoticeDisplay
    {
        internal static void DisplayLoadingNotice(EditorWindow window)
        {
            lock (CommandManager.Instance)
            {
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.Height(20));

                if (CommandManager.Instance.TotalAssemblies == 0)
                    GUILayout.Label(
                        MonkeyStyle
                            .StylizeWithLoadingLabelStyle(MonKeyLocManager.CurrentLoc.PreLoadingNotice),
                        MonkeyStyle.Instance.SearchLabelStyle);
                else
                {
                    StringBuilder noticeBuilder = new StringBuilder();


                    noticeBuilder.Append(MonKeyLocManager.CurrentLoc.LoadingLabel);

                    noticeBuilder.Append(MonKeyLocManager.CurrentLoc.EndLoadingLabel);
                    noticeBuilder.Append(CommandManager.Instance.AssemblyAnalyzed);
                    noticeBuilder.Append("/");
                    noticeBuilder.Append(CommandManager.Instance.TotalAssemblies);
                    noticeBuilder.Append(" | Checking Class:");
                    noticeBuilder.Append(CommandManager.Instance.CurrentClassLoading);
                
                    GUILayout.Label(
                        MonkeyStyle.StylizeWithLoadingLabelStyle(noticeBuilder.ToString()),
                        MonkeyStyle.Instance.SearchLabelStyle);
                }
                GUILayout.EndHorizontal();
                window.Repaint();
            }           
        }
    }
}
