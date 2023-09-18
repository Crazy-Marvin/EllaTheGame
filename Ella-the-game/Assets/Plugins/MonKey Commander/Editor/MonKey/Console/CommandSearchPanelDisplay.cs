using MonKey.Editor.Internal;
using MonKey.Extensions;
using MonKey.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MonKey.Editor.Console
{
    internal static class CommandSearchPanelDisplay
    {
        internal static void DisplayCommandPanel(CommandConsoleWindow window)
        {
            try
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    window.Repaint();
                    window.Focus();
                    return;
                }

                DisplayCommandSearchPanel(window);

                if (CommandManager.Instance.IsLoading)
                    LoadingNoticeDisplay.DisplayLoadingNotice(window);
            }
            catch (Exception e)
            {
                if (CommandConsoleWindow.LogExceptions)
                    Debug.LogWarning(e);
                GUIUtility.ExitGUI();
            }

            window.CheckSearch();

            try
            {
                DisplayCommandResultPanel(window);
                DisplayCommandSearchHelp(window);
            }
            catch (Exception e)
            {
                if (CommandConsoleWindow.LogExceptions)
                    Debug.LogWarning(e);
                GUIUtility.ExitGUI();
            }
        }

        private static void DisplayCommandSearchPanel(CommandConsoleWindow window)
        {
            EditorGUILayout.BeginHorizontal(MonkeyStyle.Instance.TopSearchPanelStyle);
            EditorGUILayout.BeginHorizontal(MonkeyStyle.Instance.MonkeyLogoGroupStyle);
            EditorGUILayout.LabelField("", window.MonkeyFocusLogoStyle);
            EditorGUILayout.EndHorizontal();
            DisplayCommandSearchField(window);
            EditorGUILayout.EndHorizontal();
        }

        private static void DisplayCommandSearchField(CommandConsoleWindow window)
        {
            EditorGUILayout.BeginVertical(MonkeyStyle.Instance.SearchLabelGroupStyle);

            EditorGUILayout.BeginHorizontal(MonkeyStyle.Instance.NameLogoStyleGroup);
            GUILayout.FlexibleSpace();
            GUILayout.Label("", MonkeyStyle.Instance.NameLogoStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(MonkeyStyle.Instance.SearchLabelStyle);

            GUILayout.BeginHorizontal(MonkeyStyle.Instance.SearchLabelStyle);
            GUILayout.BeginHorizontal(MonkeyStyle.Instance.SearchIconBackgroundStyle);
            EditorGUILayout.LabelField("", MonkeyStyle.Instance.SearchIconStyle);
            GUILayout.EndHorizontal();

            DisplayTextInputField(window);

            if (window.SearchTerms.IsNullOrEmpty())
                GUILayout.Label(MonKeyLocManager.CurrentLoc.CommandSearchLabel,
                    MonkeyStyle.Instance.SearchLabelHelpStyle);

            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }


        private static void DisplayTextInputField(CommandConsoleWindow window)
        {
            GUI.skin.settings.cursorFlashSpeed = 1;

            const string controlName = "searchCommand";
            GUI.SetNextControlName(controlName);
            bool wasEmpty = window.SearchTerms.IsNullOrEmpty();
            window.SearchTerms = EditorGUIExt.TextField((window.SearchTerms ?? "").Replace("`", ""),
                controlName,
                (window.SearchTerms.IsNullOrEmpty())
                    ? MonkeyStyle.Instance.SearchLabelStyleEmpty
                    : MonkeyStyle.Instance.SearchLabelStyle);

            if (wasEmpty && !window.SearchTerms.IsNullOrEmpty())
            {
                window.CommandResult = new List<CommandInfo>();
            }

            if (window.PreventSearchMovement)
                CommandConsoleWindow.ForceEditSearchAtEnd(window.SearchTerms);

            if (window.Focused)
            {
                if (!window.SearchTerms.IsNullOrEmpty() && !window.CommandResult.Any())
                    window.MonkeyFocusLogoStyle = MonkeyStyle.Instance.MonkeyLogoStyle;
                else
                {
                    window.MonkeyFocusLogoStyle = window.SearchTerms.IsNullOrEmpty()
                        ? MonkeyStyle.Instance.MonkeyLogoStyleSmiling
                        : MonkeyStyle.Instance.MonkeyLogoStyleHappy;
                }

                window.Repaint();
            }

            if (!window.IsDocked || window.JustOpenedActiveMode || window.Focused)
                GUI.FocusControl(controlName);
        }


        public static void DisplayCommandSearchHelp(CommandConsoleWindow window)
        {
            EditorGUILayout.BeginHorizontal(MonkeyStyle.Instance.HelpStyle);
            GUILayout.FlexibleSpace();
            if (!window.Focused)
            {
                GUILayout.BeginHorizontal(MonkeyStyle.Instance.NonParamIconTipStyle);
                GUILayout.Label("");
                GUILayout.EndHorizontal();

                GUILayout.Label(MonKeyLocManager.CurrentLoc.NotFocusedHelpLabel,
                    MonkeyStyle.Instance.HelpTextStyle);
            }
            else
            {
                if (window.SelectedIndex >= 0 &&
                    window.CommandResult.ElementAt(window.SelectedIndex).IsParametric)
                {
                    DisplayParametricCommandTip(window);
                }
                else
                {
                    GUILayout.BeginHorizontal(MonkeyStyle.Instance.NonParamIconTipStyle);
                    GUILayout.Label("");
                    GUILayout.EndHorizontal();

                    GUILayout.Label(MonkeyStyle.StylizedGeneralHelp,
                        MonkeyStyle.Instance.HelpTextStyle);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        internal static void DisplayParametricCommandTip(CommandConsoleWindow window)
        {
            GUILayout.BeginHorizontal(MonkeyStyle.Instance.ParamIconTipStyle);
            GUILayout.Label("");
            GUILayout.EndHorizontal();

            GUILayout.Label(
                window.Focused
                    ? MonKeyLocManager.CurrentLoc.ParametricHelp
                    : MonKeyLocManager.CurrentLoc.NotFocusedHelpLabel,
                MonkeyStyle.Instance.HelpTextStyle);
        }

        private static void DisplayCommandResultPanel(CommandConsoleWindow window)
        {
            if (window.DisplayNoResult)
                DisplayNoCommandsFound();
            else
            {
                DisplayAllFoundCommands(window);
            }
        }

        internal static bool InitResultSelectionStyle(bool mouseActivity, float minHeight)
        {
            bool selected = false;
            if (mouseActivity)
            {
                EditorGUILayout.BeginVertical(MonkeyStyle.Instance.CommandResultLayoutForcedHighlightStyle,
                    GUILayout.ExpandHeight(true)
                    , GUILayout.MinHeight(minHeight));
                selected = true;
            }
            else
                EditorGUILayout.BeginVertical(MonkeyStyle.Instance.CommandResultLayoutNoHighlightStyle,
                    GUILayout.ExpandHeight(true), GUILayout.MinHeight(minHeight));

            return selected;
        }

        private static void DisplayCategory(string catID, string catName, CommandConsoleWindow window,
            bool back = false, string backName = "")
        {
            int height = MonkeyStyle.Instance.CommandGroupNoHelpHeight;

            bool selected = InitResultSelectionStyle(catID == window.HoveredCategory
                                                     || (back && backName == window.HoveredCategory),
                height);

            EditorGUILayout.BeginVertical(MonkeyStyle.Instance.CommandResultInsideLayoutStyle,
                GUILayout.ExpandHeight(true));

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(MonkeyStyle.Instance.CommandResultGroupStyle);

            EditorGUILayout.BeginVertical(MonkeyStyle.Instance.ParamIconGroupStyle,
                GUILayout.Height(height), GUILayout.ExpandHeight(true));
            GUILayout.FlexibleSpace();
            GUILayout.Label("", MonkeyStyle.Instance.CategoryIconStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            GUILayout.FlexibleSpace();

            GUILayout.Label(catName.Bold().Colored(Color.white),
                MonkeyStyle.Instance.CommandNameStyle);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(MonkeyStyle.Instance.CommandHotKeyStyle,
                GUILayout.ExpandHeight(true));

            EditorGUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSearchResultLine1Style);
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSearchResultLine2Style);
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSearchResultLine3Style);

            EditorGUILayout.EndVertical();

            if (selected)
            {
                CommandConsoleWindow.RowHeight = GUILayoutUtility.GetLastRect();
            }

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                window.BackCategory = back;
                window.HoveredCategory = !back ? catID : backName;
            }
        }

        private static void DisplayAllFoundCommands(CommandConsoleWindow window)
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideSecondLineStyle);
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideLineStyle);

            GUILayout.BeginHorizontal();

            GUILayout.Label("", MonkeyStyle.Instance.VerticalSideLineStyle);
            GUILayout.Label("", MonkeyStyle.Instance.VerticalSideLineSecondStyle);
            GUILayout.Label("", MonkeyStyle.Instance.VerticalSideLineStyle);

            GUI.skin = MonkeyStyle.Instance.MonkeyScrollBarStyle;

            window.ScrollIndex = EditorGUILayout.BeginScrollView(window.ScrollIndex, new GUIStyle()
            {
                normal = {background = MonkeyStyle.Instance.ResultFieldTex}
            });

            if (CommandConsoleWindow.DrawDebugAssemblies)
                PrintAllAssemblies();

            if (window.SearchTerms.IsNullOrEmpty())
            {
                if (window.CurrentCategory.IsNullOrEmpty())
                {
                    foreach (string key in CommandManager.Instance.BaseCategories)
                    {
                        if (CommandManager.Instance.CategoriesByName[key].IsMainCategory)
                            DisplayCategory(key, CommandManager.Instance.CategoriesByName[key].CategoryName, window);
                    }
                }
                else
                {
                    //acts as a back button
                    DisplayCategory("", "Back", window, true,
                        CommandManager.Instance.CategoriesByName[window.CurrentCategory].ParentCategoryName);

                    foreach (string key in CommandManager.Instance.CategoriesByName[window.CurrentCategory]
                        .SubCategories)
                    {
                        DisplayCategory(key, CommandManager.Instance.CategoriesByName[key].CategoryName, window);
                    }

                    DisplayCommands(window, true);
                }
            }
            else
                DisplayCommands(window);

            EditorGUILayout.EndScrollView();

            GUILayout.Label("", MonkeyStyle.Instance.VerticalSideLineStyle);
            GUILayout.Label("", MonkeyStyle.Instance.VerticalSideLineSecondStyle);
            GUILayout.Label("", MonkeyStyle.Instance.VerticalSideLineStyle);

            GUI.skin = MonkeyStyle.Instance.DefaultStyle;

            GUILayout.EndHorizontal();

            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideLineStyle);
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideSecondLineStyle);

            EditorGUILayout.EndVertical();
        }

        private static void PrintAllAssemblies()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(asm.GetName().Name
                    , MonkeyStyle.Instance.HelpStyle);
                GUILayout.EndHorizontal();
            }
        }

        private static void DisplayNoCommandsFound()
        {
            GUILayout.BeginVertical(MonkeyStyle.Instance.CommandResultLayoutNoHighlightStyle);

            GUILayout.Label(MonkeyStyle.StylizedNoResultFound,
                MonkeyStyle.Instance.NoResultStyle, GUILayout.ExpandHeight(true));

            GUILayout.EndVertical();
        }


        private static void DisplayCommands(CommandConsoleWindow window, bool ignoreMaxCommands = false)
        {
            int index = 0;

            if (window.MouseActivity)
                window.MouseOverField = false;

            if (window.CommandResult == null)
                return;

            foreach (var x in window.CommandResult)
            {
                if (index >= CommandManager.MaxCommandShown && !ignoreMaxCommands)
                    break;

                CommandDisplay.DisplayCommandInSearch(x, index, window.SelectedIndex,
                    window.MouseActivity, window.ForceValidationKeyPressed, window.SearchTerms);

                if (window.MouseActivity &&
                    GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    window.SelectedIndex = index;
                    window.MouseOverField = true;
                    window.Repaint();
                }

                index++;
            }
        }
    }
}