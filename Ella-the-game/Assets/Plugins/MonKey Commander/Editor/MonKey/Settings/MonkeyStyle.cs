using System;
using MonKey.Editor.Console;
using MonKey.Extensions;
using MonKey.Internal;
using MonKey.Settings.Internal;
using UnityEditor;
using UnityEngine;

namespace MonKey.Editor.Internal
{
    [InitializeOnLoad]
    public class MonkeyStyle : EditorSingleton<MonkeyStyle>,IMonKeySingleton
    {
        #region STATIC

        static MonkeyStyle()
        {
            SessionID = "MC_MStyle";
        }

        public bool IsFakeTextureInUse;

        private static MonkeyStyle instance;
        
        public static string ArrayVisualPrefix = "(";
        public static string ArrayVisualSuffix = ")";
        public static string VariableSeparator = " , ";

        public static string StylizedNoResultFound => MonKeyLocManager.CurrentLoc.NoResultsFoundLabel.Italic();

        public static string StylizedNoAutoComplete => MonKeyLocManager.CurrentLoc.NoAutoCompleteFoundLabel.Italic();

        public static string StylizedGeneralHelp => MonKeyLocManager.CurrentLoc.HelpLabel;

        public static string LoadingLabelStylized => MonKeyLocManager.CurrentLoc.LoadingLabel.Italic();

        public static string StylizeWithLoadingLabelStyle(string word)
        {
            return word.Italic();
        }

        public static string NoResultsFoundLabelStylized => MonKeyLocManager.CurrentLoc.NoResultsFoundLabel.Italic();

        public static Texture2D ColorTexture(int width, int height, Color col,bool keep=false)
        {
            //remove color variation for versions under 2017
            var pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {

#if UNITY_2018_1_OR_NEWER
                pix[i] = QualitySettings.activeColorSpace == ColorSpace.Linear ? col.linear : col;
#else
                pix[i] =  col;
#endif
            }
            var result = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
            result.SetPixels(pix);
            result.Apply();

            if (keep)
                result.hideFlags = HideFlags.HideAndDontSave;
            return result;
        }

        private static Texture2D ColorHorizontalGradientTexture(int width, Color startColor, Color endColor,
            int endGradientPixel,bool keep)
        {
            var pix = new Color[width];
#if UNITY_2018_1_OR_NEWER
            startColor = QualitySettings.activeColorSpace == ColorSpace.Linear ? startColor.linear : startColor;
            endColor = QualitySettings.activeColorSpace == ColorSpace.Linear ? endColor.linear : endColor;
#endif
            for (int i = 0; i < pix.Length; i++)
            {
                if (i < endGradientPixel)
                {
                    pix[i] = Color.Lerp(startColor, endColor, (float)i / endGradientPixel);
                }
                else
                {
                    pix[i] = endColor;
                }
            }

            var result = new Texture2D(width, 1, TextureFormat.RGBA32, false, true);
            result.SetPixels(pix);
            result.Apply();
            result.hideFlags = HideFlags.HideAndDontSave;
            return result;
        }


        public static string PrettifyTypeName(Type type)
        {
            if (type.IsArray)
                return "Array of " + PrettifyTypeName(type.GetElementType());

            if (!type.IsPrimitive)
                return type.Name.NicifyVariableName();
            string name = type.Name.ToLower();
            switch (name)
            {
                case "single":
                    return "Number (float)";
                case "int32":
                    return "Number (int)";
                case "int64":
                    return "Number (long)";
                case "boolean":
                    return "Bool";
                default:
                    return name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Texture2D GetIconForFile(string fileName)
        {
            return UnityEditorInternal.InternalEditorUtility.GetIconForFile(fileName);
        }

        #endregion

        public int CommandGroupHeight = 30;
        public int CommandGroupNoHelpHeight = 20;

        public Color WindowColor;
        public Color WindowStartGradientColor;

        public Color SelectedResultFieldColor;
        public Color ResultFieldColor;
        public Color SearchFieldColor;

        public Color SearchFieldInstructionTextColor;
        public Color SearchFieldTextColor;
        public Color SearchResultTextColor;
        public Color QuickNameTextColor;
        public Color CommandHelpTextColor;
        public Color CommandHelpTextColorPro;
        public Color CommandHelpHighlightTextColor;
        public Color GeneralHelpTextColor;
        public Color HighlightOnSelectedTextColor;

        public Color WarningColor;

        public Color SideColor;
        public Color SideColorSecond;

        public Color Line1Color;
        public Color Line2Color;
        public Color Line3Color;

        public Color AutoCompleteLine1Color;
        public Color AutoCompleteLine2Color;

        public Color ParametricMethodGradientColorStart;
        public Color ParametricMethodGradientColorEnd;

        public Color ParametricMethodHelpTextColor;

        public Color NonSelectedParameterGroupColor;
        public Color SelectedParameterGroupColor;
        public Color ErrorParameterGroupColor;

        public Color NonSelectedAutoCompleteGroupColor;

        public Color VariableNameSelectedTextColor;
        public Color VariableValueSelectedTextColor;

        public Color VariableNameNonSelectedTextColor;
        public Color VariableValueNonSelectedTextColor;

        public Color VariableValueErrorTextColor;

        public Color VariableHelpTextColor;

        public Color VariableHelpBackgroundColor;

        public Color ParametricTabShadow1Color;
        public Color ParametricTabShadow2Color;

        public Color ParametricTabOutline1Color;
        public Color ParametricTabOutline2Color;

        public Color ParametricTabOutlineUnfocusedColor;

        public Color ParametricTabUnderline1Color;
        public Color ParametricTabUnderline2Color;

        public Color ParametricTabUnderlineUnfocused1Color;
        public Color ParametricTabUnderlineUnfocused2Color;

        public Texture2D MonkeyHead;

        public Texture2D DebugColorTexture;

        public Texture2D NameLogo;

        public Texture2D MonkeyLogo;
        public Texture2D MonkeyLogoSmiling;
        public Texture2D MonkeyLogoHappy;

        public Texture2D MonkeySleepingLogo;
        public Texture2D MonkeySleepingPro;

        public Texture2D SearchIcon;

        public Texture2D ParametricIcon;
        public Texture2D ParametricTipIcon;
        public Texture2D ParametricSelectedIcon;

        public Texture2D NonParametricIcon;
        public Texture2D NonParametricTipIcon;
        public Texture2D NonParametricSelectedIcon;

        public Texture2D CategoryIcon;

        public Texture2D WarningIcon;

        public Texture2D TopPanelGradientTexture;

        public Texture2D SelectedResultFieldTex;

        public Texture2D InvertedSelectedResultFieldTex;

        public Texture2D ResultFieldTex;

        public Texture2D WindowBackgroundTex;

        public Texture2D BlackTex;

        public Texture2D SearchFieldBackgroundTex;

        public Texture2D SideTex;
        public Texture2D SideTexSecond;

        public Texture2D Line1Tex;

        public Texture2D Line2Tex;

        public Texture2D Line3Tex;

        public Texture2D AutoCompleteline1Tex;
        public Texture2D AutoCompleteline2Tex;

        public Texture2D SelectedVariableTex;
        public Texture2D NonSelectedVariableTex;
        public Texture2D HelpVariableTex;

        public Texture2D ParametricTabShadow1Texture;
        public Texture2D ParametricTabShadow2Texture;

        public Texture2D ParametricTabOutline1Texture;
        public Texture2D ParametricTabOutline2Texture;

        public Texture2D ParametricTabOutlineUnfocusedTexture;

        public Texture2D ParametricTabUnderline1Texture;
        public Texture2D ParametricTabUnderline2Texture;

        public Texture2D ParametricTabUnderlineUnfocused1Texture;
        public Texture2D ParametricTabUnderlineUnfocused2Texture;

        public GUIStyle NameLogoStyle;
        public GUIStyle NameLogoStyleGroup;

        public GUIStyle MonkeyLogoStyle;
        public GUIStyle MonkeyLogoStyleSmiling;
        public GUIStyle MonkeyLogoStyleHappy;

        public GUIStyle MonkeyLogoStyleSleeping;
        public GUIStyle MonkeyLogoStyleSleepingPro;
        public GUIStyle MonkeyLogoStyleSleepingGroup;

        public GUIStyle SearchIconStyle;

        public GUIStyle SearchLabelStyleEmpty;

        public GUIStyle WarningIconStyle;
        public GUIStyle WarningIconGroupStyle;

        public GUIStyle NonParamIconStyle;
        public GUIStyle NonParamIconTipStyle;
        public GUIStyle NonParamIconSelectedStyle;

        public GUIStyle ParamIconStyle;
        public GUIStyle ParamIconTipStyle;
        public GUIStyle ParamIconSelectedStyle;

        public GUIStyle CategoryIconStyle;


        public GUIStyle ParamIconGroupStyle;

        public GUIStyle SearchIconBackgroundStyle;

        public GUIStyle MonkeyLogoGroupStyle;

        public GUIStyle TopSearchPanelStyle;

        public GUIStyle SearchLabelGroupStyle;

        public GUIStyle AutoCompleteSearchLabelGroupStyle;

        public GUIStyle SearchLabelSubGroupStyle;

        public GUIStyle SearchLabelStyle;

        public GUIStyle SearchLabelHelpStyle;

        public GUIStyle ParameterHelpStyle;
        public GUIStyle HelpStyle;
        public GUIStyle HelpStyleProSleeping;

        public GUIStyle CommandResultScrollGroupStyle;

        public GUIStyle VerticalSideLineSecondStyle;

        public GUIStyle HelpTextStyle;

        public GUIStyle VerticalSideLineStyle;

        public GUIStyle HorizontalSideLineStyle;

        public GUIStyle HorizontalSideSecondLineStyle;

        public GUIStyle AutoCompleteLine1Style;
        public GUIStyle AutoCompleteLine2Style;

        public GUIStyle CommandResultGroupStyle;

        public GUIStyle CommandNameStyle;
        public GUIStyle CommandConfirmationStyle;

        public GUIStyle AssetNameStyle;

        public GUIStyle HorizontalSearchResultLine1Style;

        public GUIStyle HorizontalSearchResultLine2Style;

        public GUIStyle HorizontalSearchResultLine3Style;

        public GUIStyle NoResultStyle;

        public GUIStyle CommandHotKeyStyle;

        public GUIStyle CommandHotKeySelectedStyle;

        public GUIStyle CommandConflictHotKeyStyle;

        public GUIStyle CommandHelpStyle;

        public GUIStyle SceneCommandHelpStyle;

        public GUIStyle SceneCommandCrossStyle;

        public GUIStyle PathStyle;

        public GUIStyle CommandHelpParametricStyle;

        public GUIStyle ScrollBarStyle;

        public GUIStyle CommandHelpStyleSelected;

        public GUIStyle ValidationStyle;

        public GUIStyle CommandResultLayoutStyle;
        public GUIStyle CommandResultLayoutForcedHighlightStyle;
        public GUIStyle CommandResultLayoutNoHighlightStyle;

        public GUIStyle AutoCompleteResultLayoutStyle;
        public GUIStyle AutoCompleteResultLayoutForcedHighlightStyle;
        public GUIStyle AutoCompleteResultLayoutNoHighlightStyle;

        public GUISkin MonkeyScrollBarStyle;
        public GUISkin DefaultStyle;
        public GUIStyle CommandResultInsideLayoutStyle;

        public GUIStyle ParametricMethodMethodGroup;

        public GUIStyle MonkeyLogoParametricGroupStyle;
        public GUIStyle TopSearchParametricPanelStyle;
        public GUIStyle ParametricNameLogoStyle;

        public GUIStyle VariableGroupStyle;
        public GUIStyle VariableSelectedGroupStyle;
        public GUIStyle VariableNonSelectedGroupStyle;
        public GUIStyle ArrayVariableSelectedGroupStyle;
        public GUIStyle ArrayVariableNonSelectedGroupStyle;

        public GUIStyle VariableErrorGroupStyle;

        public GUIStyle VariableNameSelectedTextStyle;
        public GUIStyle VariableNameErrorTextStyle;
        public GUIStyle VariableValueSelectedTextStyle;

        public GUIStyle VariableNameNonSelectedTextStyle;
        public GUIStyle VariableValueNonSelectedTextStyle;

        public GUIStyle VariablePanelStyle;

        public GUIStyle VariableHelpGroupStyle;
        public GUIStyle VariableHelpTextStyle;
        public GUIStyle VariableTypeTextStyle;

        public GUIStyle ParametricTabShadow1Style;
        public GUIStyle ParametricTabShadow2Style;

        public GUIStyle ParametricTabOutline1HorizontalStyle;
        public GUIStyle ParametricTabOutline2HorizontalStyle;

        public GUIStyle ParametricTabOutline1VerticalStyle;
        public GUIStyle ParametricTabOutline2VerticalStyle;

        public GUIStyle ParametricTabOutlineUnfocusedHorizontalStyle;
        public GUIStyle ParametricTabOutlineUnfocusedVerticalStyle;

        public GUIStyle ParametricTabUnderline1Style;
        public GUIStyle ParametricTabUnderline2Style;

        public GUIStyle ParametricTabUnderlineUnfocused1Style;
        public GUIStyle ParametricTabUnderlineUnfocused2Style;

        public GUIStyle ParametricWindowBackgroundStyle;
        public GUIStyle ParameterWarningIconStyle;
        public GUIStyle ArrayWarningIconStyle;

        public GUIStyle SmallMonkey;
        public GUIStyle ArrayVariableNewGroupStyle;
        public GUIStyle ParameterLayoutForcedHighlightStyle;
        public GUIStyle ParameterLayoutNoHighlightStyle;

        public void InitDefaultStyle()
        {
            if (!DefaultStyle)
                DefaultStyle = GUI.skin;
        }

        public void PostInstanceCreation()
        {
            GetTexturesFromFile();
            CreateAllColors();
            CreateProceduralTextures();

            InitializeTextureRelatedStyles();

            MonkeyScrollBarStyle = CreateInstance<GUISkin>();

            MonkeyScrollBarStyle.verticalScrollbar.stretchWidth = true;
            MonkeyScrollBarStyle.verticalScrollbar.stretchHeight = true;

            MonkeyScrollBarStyle.verticalScrollbar.normal
                = new GUIStyleState() { background = WindowBackgroundTex };

            MonkeyScrollBarStyle.hideFlags = HideFlags.HideAndDontSave;
        }

        public Texture2D GetTextureFromName(string name)
        {
            string[] found;
            found = AssetDatabase.FindAssets(name);
            if (found.Length > 0)
                return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(found[0]));

            IsFakeTextureInUse = true;

            return Texture2D.whiteTexture;
        }


        public void GetTexturesFromFile()
        {
            IsFakeTextureInUse = false;

            MonkeyLogo = GetTextureFromName("MonKeyCommander-MonkeyLogo");

            MonkeyLogoSmiling = GetTextureFromName("MonKeyCommander-MonkeyLogoSmiling");

            MonkeyLogoHappy = GetTextureFromName("MonKeyCommander-MonkeyLogoHappy");

            MonkeySleepingLogo = GetTextureFromName("MonKeyCommander-MonkeySleeping");

            MonkeySleepingPro = GetTextureFromName("MonKeyCommander-MonkeySleepingPro");

            SearchIcon = GetTextureFromName("MonKeyCommander-SearchIcon");

            NonParametricIcon = GetTextureFromName("MonKeyCommander-NonParam");

            NonParametricTipIcon = GetTextureFromName("MonKeyCommander-NonParamTip");

            NonParametricSelectedIcon = GetTextureFromName("MonKeyCommander-NonParamSelected");

            ParametricIcon = GetTextureFromName("MonKeyCommander-Param");

            ParametricTipIcon = GetTextureFromName("MonKeyCommander-ParamTip");

            ParametricSelectedIcon = GetTextureFromName("MonKeyCommander-ParamSelected");

            WarningIcon = GetTextureFromName("MonKeyCommander-AlertIcon");

            NameLogo = GetTextureFromName("MonKeyCommander-Logo");

            MonkeyHead = GetTextureFromName("MonKeyCommander-MonkeyHead");

            CategoryIcon = GetTextureFromName("MonKeyCommander-ListIcon");
           

        }


        private static void LogDirectoryMoveError()
        {
            if (EditorApplication.isCompiling
                || EditorApplication.isUpdating || EditorApplication.timeSinceStartup < 20)
                return;
            Debug.LogWarning("The Monkey Commander Textures could not be found." +
                      "If you moved the folder of Monkey Commander, " +
                      "please update it accordingly in the preferences (Edit/Preferences)");
            if (CommandConsoleWindow.CurrentPanel)
                CommandConsoleWindow.CurrentPanel.Close();
        }

        private void CreateAllColors()
        {
            WindowColor = ColorExt.HTMLColor("#313131");
            WindowStartGradientColor = ColorExt.HTMLColor("#373737");

            SelectedResultFieldColor = ColorExt.HTMLColor("#262626");


            ResultFieldColor = ColorExt.HTMLColor("#434343");
            SearchFieldColor = ColorExt.HTMLColor("#979797");

            SearchFieldInstructionTextColor = ColorExt.HTMLColor("#656565");
            SearchFieldTextColor = Color.black;
            SearchResultTextColor = Color.white;
            QuickNameTextColor = ColorExt.HTMLColor("#ffc75a");
            CommandHelpTextColor = ColorExt.HTMLColor("#8d8d8d");
            CommandHelpTextColorPro = ColorExt.HTMLColor("#26251e");

            CommandHelpHighlightTextColor = ColorExt.HTMLColor("#a0a0a0");
            GeneralHelpTextColor = ColorExt.HTMLColor("#656565");
            HighlightOnSelectedTextColor = ColorExt.HTMLColor("#db7f1a");

            SideColor = ColorExt.HTMLColor("#2b2927");
            SideColorSecond = ColorExt.HTMLColor("#313131");

            WarningColor = ColorExt.HTMLColor("#f3aa1d");

            Line1Color = ColorExt.HTMLColor("#3a3a3a");
            Line2Color = ColorExt.HTMLColor("#4a4a4a");
            Line3Color = ColorExt.HTMLColor("#484848");

            AutoCompleteLine1Color = ColorExt.HTMLColor("#323232");
            AutoCompleteLine2Color = ColorExt.HTMLColor("#3f3f3f");

            ParametricMethodGradientColorStart = ColorExt.HTMLColor("#55432b");
            ParametricMethodGradientColorEnd = ColorExt.HTMLColor("#333230");

            ParametricMethodHelpTextColor = ColorExt.HTMLColor("#ab6821");

            NonSelectedParameterGroupColor = ColorExt.HTMLColor("#383838");
            SelectedParameterGroupColor = ColorExt.HTMLColor("#434343");

            NonSelectedAutoCompleteGroupColor = ColorExt.HTMLColor("#3a3a3a");

            VariableNameSelectedTextColor = Color.white;//ColorExt.HTMLColor("#999999");
            VariableValueSelectedTextColor = ColorExt.HTMLColor("#d3d3d3");

            VariableNameNonSelectedTextColor = ColorExt.HTMLColor("#999999");
            VariableValueNonSelectedTextColor = ColorExt.HTMLColor("#9b9b9b");

            VariableValueErrorTextColor = ColorExt.HTMLColor("#f3aa1d");

            VariableHelpTextColor = ColorExt.HTMLColor("#a0a0a0");
            VariableHelpBackgroundColor = ColorExt.HTMLColor("#3c3c3c");

            ParametricTabShadow1Color = ColorExt.HTMLColor("#212121");
            ParametricTabShadow2Color = ColorExt.HTMLColor("#232323");

            ParametricTabOutline1Color = ColorExt.HTMLColor("#575757");
            ParametricTabOutline2Color = ColorExt.HTMLColor("#474747");

            ParametricTabOutlineUnfocusedColor = ColorExt.HTMLColor("#464646");

            ParametricTabUnderline1Color = ColorExt.HTMLColor("#373737");
            ParametricTabUnderline2Color = ColorExt.HTMLColor("#4c4c4c");

            ParametricTabUnderlineUnfocused1Color = ColorExt.HTMLColor("#333333");
            ParametricTabUnderlineUnfocused2Color = ColorExt.HTMLColor("#3d3d3d");

        }

        private void CreateProceduralTextures()
        {
            DebugColorTexture = ColorTexture(1, 1, Color.cyan,true);

            TopPanelGradientTexture =
                ColorHorizontalGradientTexture(600, WindowStartGradientColor, WindowColor, 115,true);

            SelectedResultFieldTex = ColorTexture(1, 1, SelectedResultFieldColor,true);
            InvertedSelectedResultFieldTex= ColorTexture(1, 1, SelectedResultFieldColor.DarkerBrighter(0.2f), true);
            ResultFieldTex = ColorTexture(1, 1, ResultFieldColor,true);

            WindowBackgroundTex = ColorTexture(1, 1, WindowColor,true);
            BlackTex = ColorTexture(1, 1, Color.black,true);

            SearchFieldBackgroundTex = ColorTexture(1, 1, SearchFieldColor,true);

            SideTex = ColorTexture(1, 1, SideColor,true);
            SideTexSecond = ColorTexture(1, 1, SideColorSecond,true);

            Line1Tex = ColorTexture(1, 1, Line1Color,true);
            Line2Tex = ColorTexture(1, 1, Line2Color,true);
            Line3Tex = ColorTexture(1, 1, Line2Color,true);

            AutoCompleteline1Tex = ColorTexture(1, 1, AutoCompleteLine1Color,true);
            AutoCompleteline2Tex = ColorTexture(1, 1, AutoCompleteLine2Color,true);

            SelectedVariableTex = ColorTexture(1, 1, SelectedParameterGroupColor,true);
            NonSelectedVariableTex = ColorTexture(1, 1, NonSelectedParameterGroupColor,true);

            HelpVariableTex = ColorTexture(1, 1, VariableHelpBackgroundColor,true);

            ParametricTabShadow1Texture = ColorTexture(1, 1, ParametricTabShadow1Color);
            ParametricTabShadow2Texture = ColorTexture(1, 1, ParametricTabShadow2Color);

            ParametricTabOutline1Texture = ColorTexture(1, 1, ParametricTabOutline1Color,true);
            ParametricTabOutline2Texture = ColorTexture(1, 1, ParametricTabOutline2Color,true);

            ParametricTabOutlineUnfocusedTexture = ColorTexture(1, 1, ParametricTabOutlineUnfocusedColor, true);

            ParametricTabUnderline1Texture = ColorTexture(1, 1, ParametricTabUnderline1Color, true);
            ParametricTabUnderline2Texture = ColorTexture(1, 1, ParametricTabUnderline2Color, true);

            ParametricTabUnderlineUnfocused1Texture = ColorTexture(1, 1,
                ParametricTabUnderlineUnfocused1Color, true);
            ParametricTabUnderlineUnfocused2Texture = ColorTexture(1, 1,
                ParametricTabUnderlineUnfocused2Color, true);
        }

        private void InitializeTextureRelatedStyles()
        {
            NameLogoStyle = new GUIStyle()
            {
                fixedWidth = 122,
                fixedHeight = 14,
                normal = { background = NameLogo }
            };

            ParametricNameLogoStyle = new GUIStyle()
            {
                fixedWidth = 122,
                fixedHeight = 14,
                margin = new RectOffset(0, 10, 0, 0),
                normal = { background = NameLogo }
            };

            NameLogoStyleGroup = new GUIStyle()
            {
                margin = new RectOffset(0, 0, 10, 2),
            };

            MonkeyLogoStyle = new GUIStyle()
            {
                fixedWidth = 67,
                fixedHeight = 63,
                stretchWidth = true,
                stretchHeight = true,
                normal = { background = MonkeyLogo }
            };

            MonkeyLogoStyleSmiling = new GUIStyle()
            {
                fixedWidth = 67,
                fixedHeight = 63,
                stretchWidth = true,
                stretchHeight = true,
                normal = { background = MonkeyLogoSmiling }
            };

            SmallMonkey = new GUIStyle()
            {
                fixedWidth = 35,
                fixedHeight = 35,
                stretchWidth = true,
                stretchHeight = true,
                margin = new RectOffset(2, 2, 0, 0),
                normal = { background = MonkeyLogo }
            };


            MonkeyLogoStyleHappy = new GUIStyle()
            {
                fixedWidth = 67,
                fixedHeight = 63,
                stretchWidth = true,
                stretchHeight = true,
                normal = { background = MonkeyLogoHappy }
            };

            MonkeyLogoStyleSleeping = new GUIStyle()
            {
                fixedWidth = 208,
                fixedHeight = 194,
                normal = { background = MonkeySleepingLogo }
            };

            MonkeyLogoStyleSleepingPro = new GUIStyle()
            {
                fixedWidth = 208,
                fixedHeight = 194,
                normal = { background = MonkeySleepingPro }
            };

            MonkeyLogoStyleSleepingGroup = new GUIStyle()
            {
                stretchWidth = true,
                stretchHeight = true,
            };

            SearchIconBackgroundStyle = new GUIStyle()
            {
                fixedWidth = 30,
                padding = new RectOffset(10, 5, 4, 2),
                normal = { background = SearchFieldBackgroundTex }
            };

            ParamIconGroupStyle = new GUIStyle()
            {
                fixedWidth = 21,
                stretchHeight = true,
                margin = new RectOffset(0, 10, 0, 0)
            };

            ParamIconStyle = new GUIStyle()
            {
                fixedWidth = 18,
                fixedHeight = 16,
                alignment = TextAnchor.MiddleCenter,
                normal = { background = ParametricIcon }
            };

            CategoryIconStyle = new GUIStyle()
            {
                fixedWidth = 18,
                fixedHeight = 16,
                alignment = TextAnchor.MiddleCenter,
                normal = { background = CategoryIcon }
            };


            ParamIconTipStyle = new GUIStyle()
            {
                fixedWidth = 18,
                fixedHeight = 16,
                normal = { background = ParametricTipIcon }
            };

            ParamIconSelectedStyle = new GUIStyle()
            {
                fixedWidth = 18,
                fixedHeight = 16,
                normal = { background = ParametricSelectedIcon }
            };

            NonParamIconStyle = new GUIStyle()
            {
                fixedWidth = 18,
                normal = { background = NonParametricIcon }
            };

            NonParamIconTipStyle = new GUIStyle()
            {
                fixedWidth = 18,
                fixedHeight = 14,
                normal = { background = NonParametricTipIcon }
            };

            NonParamIconSelectedStyle = new GUIStyle()
            {
                fixedWidth = 18,
                normal = { background = NonParametricSelectedIcon }
            };

            WarningIconStyle = new GUIStyle()
            {
                fixedWidth = 24,
                fixedHeight = 21,
                normal = { background = WarningIcon }
            };

            ParameterWarningIconStyle = new GUIStyle()
            {
                fixedWidth = 24,
                fixedHeight = 21,
                margin = new RectOffset(0, 10, 10, 10),
                normal = { background = WarningIcon }
            };

            ArrayWarningIconStyle = new GUIStyle()
            {
                fixedWidth = 16,
                fixedHeight = 14,
                margin = new RectOffset(0, 0, 5, 5),
                normal = { background = WarningIcon }
            };

            WarningIconGroupStyle = new GUIStyle()
            {
                fixedWidth = 24,
                margin = new RectOffset(6, 0, 0, 0)
            };

            TopSearchPanelStyle = new GUIStyle()
            {
                margin = new RectOffset(0, 20, 0, 0),
                stretchWidth = true,
                fixedHeight = 75,
                normal = { background = TopPanelGradientTexture }
            };

            TopSearchParametricPanelStyle = new GUIStyle()
            {
                margin = new RectOffset(0, 0, 0, 0),
                stretchWidth = true,
                fixedHeight = 75,
                normal = { background = TopPanelGradientTexture }
            };

            SearchIconStyle = new GUIStyle()
            {
                fixedWidth = 14,
                fixedHeight = 14,
                stretchWidth = true,
                stretchHeight = true,
                normal = { background = SearchIcon }
            };

            SearchLabelStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 15,
                    alignment = TextAnchor.MiddleLeft,
                    stretchWidth = true,
                    padding = new RectOffset(0, 0, 1, 1),
                    normal =
                    {
                        textColor = SearchFieldTextColor,
                        background = SearchFieldBackgroundTex,
                    }
                };



            SearchLabelStyleEmpty =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 15,
                    fixedWidth = 0,
                    stretchWidth = false,
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(0, 0, 1, 1),
                    normal =
                    {
                        textColor = SearchFieldTextColor,
                          background = SearchFieldBackgroundTex,
                    },
                };

            SearchLabelHelpStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 15,
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(0, 0, 1, 1),
                    stretchWidth = true,
                    normal =
                    {
                        textColor = WindowColor,
                        background = SearchFieldBackgroundTex,
                    },
                };

            CommandResultScrollGroupStyle =
                new GUIStyle
                {
                    margin = new RectOffset(0, 0, 0, 0),
                    stretchWidth = true,
                    normal = { background = WindowBackgroundTex }
                };

            VerticalSideLineSecondStyle =
                new GUIStyle
                {
                    fixedWidth = 2,
                    stretchHeight = true,
                    normal = { background = SideTexSecond }
                };

            VerticalSideLineStyle =
                new GUIStyle
                {
                    fixedWidth = 2,
                    stretchHeight = true,
                    normal = { background = SideTex }
                };

            HorizontalSideLineStyle =
                new GUIStyle
                {
                    fixedHeight = 1,
                    stretchWidth = true,
                    normal = { background = SideTex }
                };

            HorizontalSideSecondLineStyle =
                new GUIStyle
                {
                    fixedHeight = 1,
                    stretchWidth = true,
                    normal = { background = SideTexSecond }
                };

            HorizontalSearchResultLine1Style =
                new GUIStyle
                {
                    fixedHeight = 1,
                    stretchWidth = true,
                    normal = { background = Line1Tex }
                };

            HorizontalSearchResultLine2Style =
                new GUIStyle
                {
                    fixedHeight = 1,
                    stretchWidth = true,
                    normal = { background = Line2Tex }
                };

            HorizontalSearchResultLine3Style =
                new GUIStyle
                {
                    fixedHeight = 1,
                    stretchWidth = true,
                    normal = { background = Line3Tex }
                };

            CommandResultInsideLayoutStyle = new GUIStyle
            {
                richText = true,
                stretchWidth = true,
                margin = new RectOffset(0, 0, 2, 2)
            };

            CommandResultLayoutStyle = new GUIStyle
            {
                richText = true,
                stretchWidth = true,
                normal = { textColor = SearchResultTextColor, background = ResultFieldTex },
                hover = { background = SelectedResultFieldTex, textColor = SearchResultTextColor }

            };

            CommandResultLayoutForcedHighlightStyle = new GUIStyle
            {
                richText = true,
                stretchWidth = true,
                normal = { background = SelectedResultFieldTex, textColor = SearchResultTextColor },
            };

            CommandResultLayoutNoHighlightStyle = new GUIStyle
            {
                richText = true,
                stretchWidth = true,
                normal = { textColor = SearchResultTextColor, background = ResultFieldTex },
            };

            ParameterLayoutForcedHighlightStyle = new GUIStyle
            {
                richText = true,
                stretchWidth = true,
                normal = { background = InvertedSelectedResultFieldTex, textColor = SearchResultTextColor },
            };

           ParameterLayoutNoHighlightStyle = new GUIStyle
            {
                richText = true,
                stretchWidth = true,
                normal = { textColor = SearchResultTextColor, background = ResultFieldTex },
            };



            AutoCompleteResultLayoutForcedHighlightStyle = new GUIStyle
            {
                richText = true,
                stretchWidth = true,
                normal = { background = SelectedResultFieldTex, textColor = SearchResultTextColor },
            };

            AutoCompleteResultLayoutStyle = new GUIStyle
            {
                richText = true,
                stretchWidth = true,
                normal = { textColor = SearchResultTextColor, background = HelpVariableTex },
                hover = { background = SelectedResultFieldTex, textColor = SearchResultTextColor }
            };

            AutoCompleteResultLayoutNoHighlightStyle = new GUIStyle
            {
                richText = true,
                stretchWidth = true,
                normal = { textColor = SearchResultTextColor, background = HelpVariableTex },
            };

            MonkeyLogoGroupStyle = new GUIStyle()
            {
                fixedWidth = 66,
                fixedHeight = 64,
                margin = new RectOffset(5, 12, 5, 5),
                stretchWidth = true,
                stretchHeight = true,
            };

            MonkeyLogoParametricGroupStyle = new GUIStyle()
            {
                fixedWidth = 66,
                fixedHeight = 64,
                margin = new RectOffset(9, 12, 7, 0),
                stretchWidth = true,
                stretchHeight = true,
            };

            HelpStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 12,
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 10, 10),
                    stretchWidth = true,
                    normal =
                    {
                        textColor = CommandHelpTextColor,
                    },
                };

            HelpStyleProSleeping =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 12,
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 10, 10),
                    stretchWidth = true,
                    normal =
                    {
                        textColor = CommandHelpTextColorPro,
                    },
                };

            ParameterHelpStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 11,
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 10, 10),
                    stretchWidth = true,
                    normal =
                    {
                        textColor = CommandHelpTextColor,
                    },
                };

            HelpTextStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    margin = new RectOffset(5, 5, 0, 0),
                    normal =
                    {
                        textColor = CommandHelpTextColor,
                    },
                };

            NoResultStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 14,
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 0, 0),
                    normal =
                    {
                        textColor = SearchResultTextColor
                    }
                };

            CommandHotKeyStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 12,
                    stretchHeight = true,
                    margin = new RectOffset(0, 0, 0, 0),
                    alignment = TextAnchor.MiddleRight,
                    normal =
                    {
                        textColor = CommandHelpTextColor,
                    }
                };

            CommandHotKeySelectedStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 12,
                    margin = new RectOffset(0, 0, 0, 0),
                    stretchHeight = true,
                    alignment = TextAnchor.MiddleRight,
                    normal =
                    {
                        textColor = CommandHelpHighlightTextColor
                    }
                };

            CommandConflictHotKeyStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 10,
                    margin = new RectOffset(0, 0, 0, 0),
                    alignment = TextAnchor.MiddleRight,
                    normal =
                    {
                        textColor = WarningColor
                    },
                    wordWrap = true
                };

            CommandHelpStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 11,
                    margin = new RectOffset(0, 0, 4, 0),
                    stretchWidth = true,
                    normal =
                    {
                        textColor = CommandHelpTextColor
                    },
                    wordWrap = true

                };

            SceneCommandHelpStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 11,
                    margin = new RectOffset(0, 0, 4, 0),
                    stretchWidth = true,
                    normal =
                    {
                        textColor = CommandHelpTextColor
                    },
                    wordWrap = false

                };

            SceneCommandCrossStyle = new GUIStyle
            {
                richText = true,
                fontSize = 11,
                margin = new RectOffset(0, 0, 4, 0),
                stretchWidth = true,
                normal =
                {
                    textColor = CommandHelpTextColor
                },
                hover =
                {
                    background = BlackTex
                },
                wordWrap = true

            };

            PathStyle = new GUIStyle
            {
                richText = true,
                fontSize = 12,
                margin = new RectOffset(0, 5, 0, 0),
                stretchWidth = true,
                normal =
                {
                    textColor = CommandHelpTextColor
                },
            };

            CommandHelpParametricStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 11,
                    margin = new RectOffset(0, 0, 4, 0),
                    stretchWidth = true,
                    normal =
                    {
                        textColor = ParametricMethodHelpTextColor
                    },
                    wordWrap = true

                };

            ScrollBarStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 12,
                    margin = new RectOffset(0, 0, 0, 0),
                    normal =
                    {
                        textColor = CommandHelpTextColor
                    },
                };

            CommandHelpStyleSelected =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 11,
                    margin = new RectOffset(0, 0, 4, 0),
                    normal =
                    {
                        textColor = CommandHelpHighlightTextColor
                    },
                    wordWrap = true
                };

            ValidationStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 12,
                    alignment = TextAnchor.MiddleRight,
                    margin = new RectOffset(0, 0, 0, 0),
                    normal = { textColor = WarningColor },
                    wordWrap = true
                };

            SearchLabelGroupStyle =
                new GUIStyle
                {
                    stretchWidth = true,
                    stretchHeight = true,
                };

            AutoCompleteSearchLabelGroupStyle = new GUIStyle
            {
                stretchWidth = true,
                stretchHeight = true,
                padding = new RectOffset(5, 5, 2, 2),
                normal = { background = SearchFieldBackgroundTex }
            };

            CommandResultGroupStyle =
                new GUIStyle
                {
                    margin = new RectOffset(10, 10, 5, 5),
                };

            CommandNameStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 14,
                    alignment = TextAnchor.MiddleLeft,
                    margin = new RectOffset(0, 0, 0, 0),
                    wordWrap = true
                };

            CommandConfirmationStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 11,
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 5, 0),
                    wordWrap = true
                };

            AssetNameStyle =
                new GUIStyle
                {
                    richText = true,
                    fontSize = 14,
                    alignment = TextAnchor.MiddleLeft,
                    margin = new RectOffset(0, 0, 0, 0),
                    wordWrap = false
                };


            SearchLabelSubGroupStyle =
                new GUIStyle
                {
                    stretchWidth = true,
                    stretchHeight = true,
                };

            ParametricMethodMethodGroup = new GUIStyle()
            {
                alignment = TextAnchor.MiddleLeft,
                stretchHeight = true,
                stretchWidth = true,
                padding = new RectOffset(10, 0, 0, 0),
            };


            VariableGroupStyle = new GUIStyle()
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true,
                margin = new RectOffset(5, 5, 0, 0),
            };

            VariableSelectedGroupStyle = new GUIStyle()
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true,
                normal =
                {
                    background = SelectedVariableTex,
                }
            };

            VariableNonSelectedGroupStyle = new GUIStyle()
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true,
                normal =
                {
                    background = NonSelectedVariableTex,
                }
            };

            ArrayVariableSelectedGroupStyle = new GUIStyle()
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true,
                padding = new RectOffset(2, 2, 0, 0),
                margin = new RectOffset(2, 2, 0, 0),
                normal =
                {
                    background = SelectedResultFieldTex,
                }
            };

            ArrayVariableNonSelectedGroupStyle = new GUIStyle()
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true,
                padding = new RectOffset(2, 2, 0, 0),
                margin = new RectOffset(2, 2, 0, 0),
                normal =
                {
                    background = NonSelectedVariableTex,
                }
            };

            ArrayVariableNewGroupStyle = new GUIStyle()
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true,
                padding = new RectOffset(2, 2, 0, 0),
                margin = new RectOffset(2, 2, 0, 0),
                normal =
                {
                    background = SelectedResultFieldTex,
                }
            };

            VariableHelpGroupStyle = new GUIStyle()
            {
                richText = true,
                padding = new RectOffset(10, 10, 7, 10),
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(0, 10, 5, 0),
                stretchHeight = true,
                normal =
                {

                    //background = helpVariableTex,
                }
            };

            VariableHelpTextStyle = new GUIStyle()
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true,
                fontSize = 12,
                margin = new RectOffset(0, 0, 9, 9),
                normal =
                {
                    textColor = VariableNameNonSelectedTextColor,
                },
                wordWrap = true
            };

            VariableTypeTextStyle = new GUIStyle()
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true,
                fontSize = 12,
                margin = new RectOffset(5, 5, 5, 5),
                normal =
                {
                    textColor =VariableValueSelectedTextColor
                }
            };

            ParametricTabShadow1Style =
                new GUIStyle
                {
                    fixedWidth = 1,
                    stretchHeight = true,
                    margin = new RectOffset(0, 0, 2, 0),
                    normal = { background = ParametricTabShadow1Texture }
                };

            ParametricTabShadow2Style =
                 new GUIStyle
                 {
                     fixedWidth = 1,
                     stretchHeight = true,
                     margin = new RectOffset(0, 0, 2, 0),
                     normal = { background = ParametricTabShadow2Texture }
                 };

            ParametricTabOutline1HorizontalStyle =
                new GUIStyle
                {
                    fixedHeight = 1,
                    stretchWidth = true,
                    normal = { background = ParametricTabOutline1Texture }
                };

            ParametricTabOutline2HorizontalStyle =
                new GUIStyle
                {
                    fixedHeight = 1,
                    stretchWidth = true,
                    margin = new RectOffset(0, 0, 0, 0),
                    normal = { background = ParametricTabOutline2Texture }
                };

            ParametricTabOutlineUnfocusedHorizontalStyle =
                new GUIStyle
                {
                    fixedHeight = 1,
                    stretchWidth = true,
                    normal = { background = ParametricTabOutlineUnfocusedTexture }
                };

            ParametricTabOutline1VerticalStyle =
                new GUIStyle
                {
                    fixedWidth = 1,
                    stretchHeight = true,
                    normal = { background = ParametricTabOutline1Texture }
                };

            ParametricTabOutline2VerticalStyle =
                new GUIStyle
                {
                    fixedWidth = 1,
                    stretchHeight = true,
                    margin = new RectOffset(0, 0, 0, 0),
                    normal = { background = ParametricTabOutline2Texture }
                };

            ParametricTabOutlineUnfocusedVerticalStyle =
                new GUIStyle
                {
                    fixedWidth = 1,
                    stretchHeight = true,
                    normal = { background = ParametricTabOutlineUnfocusedTexture }
                };

            ParametricTabUnderline1Style =
                new GUIStyle
                {
                    fixedHeight = 1,
                    stretchWidth = true,
                    margin = new RectOffset(8, 8, 0, 0),
                    normal = { background = ParametricTabUnderline1Texture }
                };

            ParametricTabUnderline2Style =
                new GUIStyle
                {
                    fixedHeight = 2,
                    stretchWidth = true,
                    margin = new RectOffset(8, 8, 0, 0),
                    normal = { background = ParametricTabUnderline2Texture }
                };

            ParametricTabUnderlineUnfocused1Style =
                new GUIStyle
                {
                    fixedHeight = 1,
                    stretchWidth = true,
                    margin = new RectOffset(8, 8, 0, 0),
                    normal = { background = ParametricTabUnderlineUnfocused1Texture }
                };

            ParametricTabUnderlineUnfocused2Style =
                new GUIStyle
                {
                    fixedHeight = 2,
                    stretchWidth = true,
                    margin = new RectOffset(8, 8, 0, 0),
                    normal = { background = ParametricTabUnderlineUnfocused2Texture }
                };

            ParametricWindowBackgroundStyle = new GUIStyle()
            {
                padding = new RectOffset(8, 8, 8, 0),
                normal = { background = SelectedResultFieldTex }
            };

            VariableNameSelectedTextStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                margin = new RectOffset(0, 0, 10, 10),
                normal = { textColor = VariableNameSelectedTextColor }
            };

            VariableNameErrorTextStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                margin = new RectOffset(0, 0, 10, 10),
                normal = { textColor = VariableValueErrorTextColor }
            };

            VariableValueSelectedTextStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                margin = new RectOffset(2, 2, 5, 2),
                normal = { textColor = VariableValueSelectedTextColor }
            };

            VariableNameNonSelectedTextStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                margin = new RectOffset(0, 0, 10, 10),
                normal = { textColor = VariableValueNonSelectedTextColor }
            };

            VariableValueNonSelectedTextStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                margin = new RectOffset(2, 2, 5, 5),
                normal = { textColor = VariableValueNonSelectedTextColor }
            };

            VariablePanelStyle = new GUIStyle()
            {
                margin = new RectOffset(0, 0, 0, 0),
                stretchHeight = true,
                padding = new RectOffset(0, 0, 0, 5),
                normal = { background = SelectedVariableTex },
            };

            AutoCompleteLine1Style = new GUIStyle()
            {
                stretchWidth = true,
                fixedHeight = 1,
                normal = { background = AutoCompleteline1Tex }
            };

            AutoCompleteLine2Style = new GUIStyle()
            {
                stretchWidth = true,
                fixedHeight = 2,
                normal = { background = AutoCompleteline2Tex }
            };

        }


    }
}
