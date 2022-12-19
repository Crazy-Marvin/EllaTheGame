using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace EasyMobile.Editor
{
    public static class EM_GUIStyleManager
    {
        #region Constants

        // Common button sizes.
        public static readonly float buttonHeight = EditorGUIUtility.singleLineHeight * 1.3f;
        public static readonly float smallButtonWidth = 24f;
        public static readonly float smallButtonHeight = 24f;
        public static readonly float toolboxWidth = 34;
        public static readonly float toolboxHeight = 86;

        #endregion

        #region Textures

        private static Texture2D _aboutEMTex;

        public static Texture2D AboutEMTex
        {
            get
            {
                if (_aboutEMTex == null)
                {
#if EASY_MOBILE_PRO
                    string imageName = "em-about-pro.psd";
#else
                    string imageName = "em-about-basic.psd";
#endif
                    _aboutEMTex = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + imageName, typeof(Texture2D)) as Texture2D;
                }
                return _aboutEMTex;
            }
        }

        private static Texture2D _homeIcon;

        public static Texture2D HomeIcon
        {
            get
            {
                if (_homeIcon == null)
                {
                    string iconName = EditorGUIUtility.isProSkin ? "icon-home-dark.psd" : "icon-home-light.psd";
                    _homeIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _homeIcon;
            }
        }

        private static Texture2D _backIcon;

        public static Texture2D BackIcon
        {
            get
            {
                if (_backIcon == null)
                {
                    string iconName = EditorGUIUtility.isProSkin ? "icon-back-dark.psd" : "icon-back-light.psd";
                    _backIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _backIcon;
            }
        }

        private static Texture2D _arrowUp;

        public static Texture2D ArrowUp
        {
            get
            {
                if (_arrowUp == null)
                {
                    string iconName = EditorGUIUtility.isProSkin ? "icon-up-arrow-dark.psd" : "icon-up-arrow-light.psd";
                    _arrowUp = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _arrowUp;
            }
        }

        private static Texture2D _arrowDown;

        public static Texture2D ArrowDown
        {
            get
            {
                if (_arrowDown == null)
                {
                    string iconName = EditorGUIUtility.isProSkin ? "icon-down-arrow-dark.psd" : "icon-down-arrow-light.psd";
                    _arrowDown = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _arrowDown;
            }
        }

        private static Texture2D _chevronDown;

        public static Texture2D ChevronDown
        {
            get
            {
                if (_chevronDown == null)
                {
                    string iconName = EditorGUIUtility.isProSkin ? "icon-chevron-down-dark.psd" : "icon-chevron-down-light.psd";
                    _chevronDown = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _chevronDown;
            }
        }

        private static Texture2D _chevronUp;

        public static Texture2D ChevronUp
        {
            get
            {
                if (_chevronUp == null)
                {
                    string iconName = EditorGUIUtility.isProSkin ? "icon-chevron-up-dark.psd" : "icon-chevron-up-light.psd";
                    _chevronUp = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _chevronUp;
            }
        }

        private static Texture2D _bottomSeparatorTex;

        public static Texture2D BottomSeparatorTex
        {
            get
            {
                if (_bottomSeparatorTex == null)
                {
                    string iconName = EditorGUIUtility.isProSkin ? "bottom-separator-dark.psd" : "bottom-separator-light.psd";
                    _bottomSeparatorTex = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _bottomSeparatorTex;
            }
        }

        private static Texture2D _uppercaseSectionHeaderBg;

        public static Texture2D UppercaseSectionHeaderBg
        {
            get
            {
                if (_uppercaseSectionHeaderBg == null)
                {
                    string iconName = EditorGUIUtility.isProSkin ? "uppercase-section-header-dark.psd" : "uppercase-section-header-light.psd";
                    _uppercaseSectionHeaderBg = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _uppercaseSectionHeaderBg;
            }
        }

        private static Texture2D _uppercaseSectionHeaderIcon;

        public static Texture2D UppercaseSectionHeaderIcon
        {
            get
            {
                if (_uppercaseSectionHeaderIcon == null)
                {
                    string iconName = EditorGUIUtility.isProSkin ? "uppercase-section-header-icon-dark.psd" : "uppercase-section-header-icon-light.psd";
                    _uppercaseSectionHeaderIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _uppercaseSectionHeaderIcon;
            }
        }

        private static Texture2D _easymobileIcon;

        public static Texture2D EasyMobileIcon
        {
            get
            {
                if (_easymobileIcon == null)
                {
#if EASY_MOBILE_PRO
                    string iconName = "easymobile-icon-pro.png";
#else
                    string iconName = "easymobile-icon-basic.png";
#endif
                    _easymobileIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }

                return _easymobileIcon;
            }
        }

        private static Texture2D _hyperlinkIcon;

        public static Texture2D HyperlinkIcon
        {
            get
            {
                if (_hyperlinkIcon == null)
                {
                    string iconName = EditorGUIUtility.isProSkin ? "icon-hyperlink-dark.psd" : "icon-hyperlink-light.psd";
                    _hyperlinkIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _hyperlinkIcon;
            }
        }

        private static Texture2D _hyperlinkIconActive;

        public static Texture2D HyperlinkIconActive
        {
            get
            {
                if (_hyperlinkIconActive == null)
                {
                    string iconName = "icon-hyperlink-active.psd";
                    _hyperlinkIconActive = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _hyperlinkIconActive;
            }
        }

        private static Texture2D _moduleOffIcon;

        public static Texture2D ModuleOffIcon
        {
            get
            {
                if (_moduleOffIcon == null)
                {
                    _moduleOffIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + "icon-module-off.psd", typeof(Texture2D)) as Texture2D;
                }
                return _moduleOffIcon;
            }
        }

        private static Texture2D _moduleOnIcon;

        public static Texture2D ModuleOnIcon
        {
            get
            {
                if (_moduleOnIcon == null)
                {
                    _moduleOnIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + "icon-module-on.psd", typeof(Texture2D)) as Texture2D;
                }
                return _moduleOnIcon;
            }
        }

        private static Texture2D _adIcon;

        public static Texture2D AdIcon
        {
            get
            {
                if (_adIcon == null)
                {
                    string iconName = "Module Icons/ad-icon.psd";
                    _adIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _adIcon;
            }
        }

        private static Texture2D _gameServiceIcon;

        public static Texture2D GameServiceIcon
        {
            get
            {
                if (_gameServiceIcon == null)
                {
                    string iconName = "Module Icons/game-service-icon.psd";
                    _gameServiceIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _gameServiceIcon;
            }
        }

        private static Texture2D _iapIcon;

        public static Texture2D IAPIcon
        {
            get
            {
                if (_iapIcon == null)
                {
                    string iconName = "Module Icons/iap-icon.psd";
                    _iapIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _iapIcon;
            }
        }

        private static Texture2D _notificationIcon;

        public static Texture2D NotificationIcon
        {
            get
            {
                if (_notificationIcon == null)
                {
                    string iconName = "Module Icons/notification-icon.psd";
                    _notificationIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _notificationIcon;
            }
        }

        private static Texture2D _selectModulePanelTexture;

        public static Texture2D SelectModulePanelTexture
        {
            get
            {
                if (_selectModulePanelTexture == null)
                {
                    var color = EditorGUIUtility.isProSkin ? new Color(.3f, .3f, .3f, 1.0f) : new Color(.92f, .92f, .92f, 1.0f);
                    _selectModulePanelTexture = new Texture2D(1, 1);
                    _selectModulePanelTexture.SetPixel(0, 0, color);
                    _selectModulePanelTexture.Apply();
                }
                return _selectModulePanelTexture;
            }
        }

        private static Texture2D _privacyIcon;

        public static Texture2D PrivacyIcon
        {
            get
            {
                if (_privacyIcon == null)
                {
                    string iconName = "Module Icons/privacy-icon.psd";
                    _privacyIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _privacyIcon;
            }
        }

        private static Texture2D _sharingIcon;

        public static Texture2D SharingIcon
        {
            get
            {
                if (_sharingIcon == null)
                {
                    string iconName = "Module Icons/sharing-icon.psd";
                    _sharingIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _sharingIcon;
            }
        }

        private static Texture2D _utilityIcon;

        public static Texture2D UtilityIcon
        {
            get
            {
                if (_utilityIcon == null)
                {
                    string iconName = "Module Icons/utility-icon.psd";
                    _utilityIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _utilityIcon;
            }
        }

        private static Texture2D _nativeApisIcon;

        public static Texture2D NativeApisIcon
        {
            get
            {
                if (_nativeApisIcon == null)
                {
                    string iconName = "Module Icons/native-apis-icon.psd";
                    _nativeApisIcon = AssetDatabase.LoadAssetAtPath(EM_Constants.SkinTextureFolder + "/" + iconName, typeof(Texture2D)) as Texture2D;
                }
                return _nativeApisIcon;
            }
        }

        private static Texture2D _permissionsIcon;

        public static Texture2D PermissionIcon
        {
            get
            {
                if (_permissionsIcon == null)
                {
                    string iconName = "Module Icons/permissions-icon.png";
                    _permissionsIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EM_Constants.SkinTextureFolder + "/" + iconName);
                }
                return _permissionsIcon;
            }
        }

        private static Texture2D _easymobileSmallIcon;

        public static Texture2D EasyMobileSmallIcon
        {
            get
            {
                if (_easymobileSmallIcon == null)
                {
                    string iconName = "em_icon_small.psd";
                    _easymobileSmallIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EM_Constants.SkinTextureFolder + "/" + iconName);
                }
                return _easymobileSmallIcon;
            }
        }

        private static Texture2D _toolReimpotGPSRIcon;

        public static Texture2D ToolReimportGPSRIcon
        {
            get
            {
                if (_toolReimpotGPSRIcon == null)
                {
                    string iconName = "Tool Icons/google-play.psd";
                    _toolReimpotGPSRIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EM_Constants.SkinTextureFolder + "/" + iconName);
                }
                return _toolReimpotGPSRIcon;
            }
        }

        private static Texture2D _toolImportPlaymakerActionsIcon;

        public static Texture2D ToolImportPlaymakerActionsIcon
        {
            get
            {
                if (_toolImportPlaymakerActionsIcon == null)
                {
                    string iconName = "Tool Icons/playmaker.psd";
                    _toolImportPlaymakerActionsIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EM_Constants.SkinTextureFolder + "/" + iconName);
                }
                return _toolImportPlaymakerActionsIcon;
            }
        }

        private static Texture2D _toolExportEMSettingsIcon;

        public static Texture2D ToolExportEMSettingsIcon
        {
            get
            {
                if (_toolExportEMSettingsIcon == null)
                {
                    string iconName = "Tool Icons/export.psd";
                    _toolExportEMSettingsIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EM_Constants.SkinTextureFolder + "/" + iconName);
                }
                return _toolExportEMSettingsIcon;
            }
        }

        private static Texture2D _toolUserGuideIcon;

        public static Texture2D ToolUserGuideIcon
        {
            get
            {
                if (_toolUserGuideIcon == null)
                {
                    string iconName = "Tool Icons/manual.psd";
                    _toolUserGuideIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EM_Constants.SkinTextureFolder + "/" + iconName);
                }
                return _toolUserGuideIcon;
            }
        }

        private static Texture2D _toolScriptingRefIcon;

        public static Texture2D ToolScriptingRefIcon
        {
            get
            {
                if (_toolScriptingRefIcon == null)
                {
                    string iconName = "Tool Icons/api.psd";
                    _toolScriptingRefIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EM_Constants.SkinTextureFolder + "/" + iconName);
                }
                return _toolScriptingRefIcon;
            }
        }

        private static Texture2D _toolVideoTutorialsIcon;

        public static Texture2D ToolVideoTutorialsIcon
        {
            get
            {
                if (_toolVideoTutorialsIcon == null)
                {
                    string iconName = "Tool Icons/video-tutorials.psd";
                    _toolVideoTutorialsIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EM_Constants.SkinTextureFolder + "/" + iconName);
                }
                return _toolVideoTutorialsIcon;
            }
        }

        private static Texture2D _toolSupportIcon;

        public static Texture2D ToolSupportIcon
        {
            get
            {
                if (_toolSupportIcon == null)
                {
                    string iconName = "Tool Icons/support.psd";
                    _toolSupportIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EM_Constants.SkinTextureFolder + "/" + iconName);
                }
                return _toolSupportIcon;
            }
        }

        private static Texture2D _toolRateIcon;

        public static Texture2D ToolRateIcon
        {
            get
            {
                if (_toolRateIcon == null)
                {
                    string iconName = "Tool Icons/fivestars.psd";
                    _toolRateIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EM_Constants.SkinTextureFolder + "/" + iconName);
                }
                return _toolRateIcon;
            }
        }

        private static Texture2D _toolGenerateManifestIcon;

        public static Texture2D ToolGenerateManifestIcon
        {
            get
            {
                if (_toolGenerateManifestIcon == null)
                {
                    string iconName = "Tool Icons/xml.psd";
                    _toolGenerateManifestIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EM_Constants.SkinTextureFolder + "/" + iconName);
                }
                return _toolGenerateManifestIcon;
            }
        }

        private static Texture2D _toolAboutIcon;

        public static Texture2D ToolAboutIcon
        {
            get
            {
                if (_toolAboutIcon == null)
                {
                    string iconName = "Tool Icons/about.psd";
                    _toolAboutIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EM_Constants.SkinTextureFolder + "/" + iconName);
                }
                return _toolAboutIcon;
            }
        }

        #endregion

        #region GUISkin

        private static GUISkin _skin;

        public static GUISkin Skin
        {
            get
            {
                if (_skin != null)
                {
                    return _skin;
                }
                else
                {
                    string skinName = EditorGUIUtility.isProSkin ? "EMSkin_Dark.guiskin" : "EMSkin_Light.guiskin";
                    string skinPath = EM_Constants.SkinFolder + "/" + skinName;
                    _skin = AssetDatabase.LoadAssetAtPath(skinPath, typeof(GUISkin)) as GUISkin;

                    if (_skin == null)
                    {
                        Debug.LogError("Couldn't load the GUISkin at " + skinPath);
                    }

                    return _skin;
                }
            }
        }

        #endregion

        #region GUIStyles

        // Inspector header icon
        private static GUIStyle _inspectorHeaderIcon;

        public static GUIStyle InspectorHeaderIcon
        {
            get
            {
                if (_inspectorHeaderIcon == null)
                {
                    _inspectorHeaderIcon = new GUIStyle(GUIStyle.none)
                    {
                        imagePosition = ImagePosition.ImageOnly,
                        margin = new RectOffset(0, 0, 3, 0),
                        fixedHeight = 30,
                        fixedWidth = 30
                    };
                }
                return _inspectorHeaderIcon;
            }
        }

        // Inspector header title
        private static GUIStyle _inspectorHeaderTitle;

        public static GUIStyle InspectorHeaderTitle
        {
            get
            {
                if (_inspectorHeaderTitle == null)
                {
                    _inspectorHeaderTitle = new GUIStyle(EditorStyles.boldLabel);
                    _inspectorHeaderTitle.alignment = TextAnchor.MiddleLeft;
                    var padding = _inspectorHeaderTitle.padding;
                    padding.top = 2;
                    _inspectorHeaderTitle.padding = padding;

                    if (EditorGUIUtility.isProSkin)
                    {
                        _inspectorHeaderTitle.normal = new GUIStyleState()
                        {
                            textColor = Color.white
                        };
                    }
                }
                return _inspectorHeaderTitle;
            }
        }

        // Inspector header subtitle
        private static GUIStyle _inspectorHeaderSubtitle;

        public static GUIStyle InspectorHeaderSubtitle
        {
            get
            {
                if (_inspectorHeaderSubtitle == null)
                {
                    _inspectorHeaderSubtitle = new GUIStyle(EditorStyles.miniLabel);
                    _inspectorHeaderSubtitle.alignment = TextAnchor.MiddleLeft;
                    var padding = _inspectorHeaderSubtitle.padding;
                    padding.bottom = 5;
                    _inspectorHeaderSubtitle.padding = padding;

                    if (EditorGUIUtility.isProSkin)
                    {
                        _inspectorHeaderSubtitle.normal = new GUIStyleState()
                        {
                            textColor = Color.white
                        };
                    }
                }
                return _inspectorHeaderSubtitle;
            }
        }

        // Module toobar button
        private static GUIStyle _moduleToolbarButton;

        public static GUIStyle ModuleToolbarButton
        {
            get
            {
                if (_moduleToolbarButton == null)
                {
                    _moduleToolbarButton = new GUIStyle(EditorStyles.toolbarButton);
                    _moduleToolbarButton.fontSize = 10;
                    _moduleToolbarButton.fontStyle = FontStyle.Bold;
                    _moduleToolbarButton.fixedHeight = EditorGUIUtility.singleLineHeight * 2;
                    _moduleToolbarButton.stretchWidth = true;
                }
                return _moduleToolbarButton;
            }
        }

        // Module toobar button left
        private static GUIStyle _moduleToolbarButtonLeft;

        public static GUIStyle ModuleToolbarButtonLeft
        {
            get
            {
                if (_moduleToolbarButtonLeft == null)
                {
                    _moduleToolbarButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
                    _moduleToolbarButtonLeft.fontSize = 10;
                    _moduleToolbarButtonLeft.fontStyle = FontStyle.Bold;
                    _moduleToolbarButtonLeft.fixedHeight = EditorGUIUtility.singleLineHeight * 2f;
                    _moduleToolbarButtonLeft.stretchWidth = true;
                }
                return _moduleToolbarButtonLeft;
            }
        }

        // Module toolbar button middle
        private static GUIStyle _moduleToolbarButtonMid;

        public static GUIStyle ModuleToolbarButtonMid
        {
            get
            {
                if (_moduleToolbarButtonMid == null)
                {
                    _moduleToolbarButtonMid = new GUIStyle(EditorStyles.miniButtonMid);
                    _moduleToolbarButtonMid.fontSize = 10;
                    _moduleToolbarButtonMid.fontStyle = FontStyle.Bold;
                    _moduleToolbarButtonMid.fixedHeight = EditorGUIUtility.singleLineHeight * 2f;
                    _moduleToolbarButtonMid.stretchWidth = true;
                }
                return _moduleToolbarButtonMid;
            }
        }

        // Module toolbar button right
        private static GUIStyle _moduleToolbarButtonRight;

        public static GUIStyle ModuleToolbarButtonRight
        {
            get
            {
                if (_moduleToolbarButtonRight == null)
                {
                    _moduleToolbarButtonRight = new GUIStyle(EditorStyles.miniButtonRight);
                    _moduleToolbarButtonRight.fontSize = 10;
                    _moduleToolbarButtonRight.fontStyle = FontStyle.Bold;
                    _moduleToolbarButtonRight.fixedHeight = EditorGUIUtility.singleLineHeight * 2f;
                    _moduleToolbarButtonRight.stretchWidth = true;
                }
                return _moduleToolbarButtonRight;
            }
        }

        // Module foldout
        private static GUIStyle _moduleFoldout;

        public static GUIStyle ModuleFoldout
        {
            get
            {
                if (_moduleFoldout == null)
                {
                    _moduleFoldout = new GUIStyle(EditorStyles.foldout);  // Extending default foldout style.
                    _moduleFoldout.fontSize = 14;
                    _moduleFoldout.fontStyle = FontStyle.Bold;
                    _moduleFoldout.alignment = TextAnchor.MiddleLeft;
                    _moduleFoldout.normal.textColor = Color.black;
                }
                return _moduleFoldout;
            }
        }

        // Textfield with wordwrap
        private static GUIStyle _wordwrapTextField;

        public static GUIStyle WordwrapTextField
        {
            get
            {
                if (_wordwrapTextField == null)
                {
                    _wordwrapTextField = new GUIStyle(EditorStyles.textField);  // Extending default textfield style.
                    _wordwrapTextField.wordWrap = true;
                }
                return _wordwrapTextField;
            }
        }

        // Bottom separator box.
        private static GUIStyle _fullWidthBottomSeparatorBox;

        public static GUIStyle FullWidthBottomSeparatorBox
        {
            get
            {
                if (_fullWidthBottomSeparatorBox == null)
                {
                    _fullWidthBottomSeparatorBox = new GUIStyle(EditorStyles.inspectorFullWidthMargins)
                    {
                        stretchWidth = true,
                        margin = new RectOffset(0, 0, 0, 0),
                        border = new RectOffset(0, 0, 0, 0),
                        padding = new RectOffset(0, 0, 0, 0),
                        overflow = new RectOffset(8, 0, 0, 0),
                        normal = new GUIStyleState()
                        {
                            background = BottomSeparatorTex
                        }
                    };
                }
                return _fullWidthBottomSeparatorBox;
            }
        }

        // Uppercase section box.
        private static GUIStyle _uppdercaseSectionBox;

        public static GUIStyle UppercaseSectionBox
        {
            get
            {
                if (_uppdercaseSectionBox == null)
                {
                    _uppdercaseSectionBox = new GUIStyle(EditorStyles.helpBox)
                    {
                        margin = new RectOffset(0, 10, 5, 5),
                    };
                }
                return _uppdercaseSectionBox;
            }
        }

        /// Uppercase section header when expanding.
        private static GUIStyle _uppercaseSectionHeaderExpand;

        public static GUIStyle UppercaseSectionHeaderExpand
        {
            get
            {
                if (_uppercaseSectionHeaderExpand == null)
                {
                    _uppercaseSectionHeaderExpand = GetCustomStyle("Uppercase Section Header");
                }
                return _uppercaseSectionHeaderExpand;
            }
        }

        /// Uppercase section header when collapse.
        private static GUIStyle _uppercaseSectionHeaderCollapse;

        public static GUIStyle UppercaseSectionHeaderCollapse
        {
            get
            {
                if (_uppercaseSectionHeaderCollapse == null)
                {
                    _uppercaseSectionHeaderCollapse = new GUIStyle(GetCustomStyle("Uppercase Section Header"))
                    {
                        normal = new GUIStyleState()
                    };
                }
                return _uppercaseSectionHeaderCollapse;
            }
        }

        #endregion

        #region Methods

        private static Dictionary<string, GUIStyle> _customStyles = new Dictionary<string, GUIStyle>();

        public static GUIStyle GetCustomStyle(string styleName)
        {
            if (_customStyles.ContainsKey(styleName))
            {
                return _customStyles[styleName];
            }
            else if (Skin != null)
            {
                GUIStyle style = Skin.FindStyle(styleName);

                if (style == null)
                {
                    Debug.LogError("Couldn't find style " + styleName);
                }
                else
                {
                    _customStyles.Add(styleName, style);
                }

                return style;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}

