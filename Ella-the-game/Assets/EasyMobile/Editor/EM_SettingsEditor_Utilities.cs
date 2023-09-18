using UnityEngine;
using UnityEditor;
using System.Collections;
using EasyMobile;

namespace EasyMobile.Editor
{
    // Partial editor class for Utilities module.
    internal partial class EM_SettingsEditor
    {
        const string UtilityModuleLabel = "UTILITIES";
        const string UtilityModuleIntro = "The Utilities module offers useful miscellaneous features such as the native rating dialog, an effective tool to solicit user ratings and reviews.";

        void UtilityModuleGUI()
        {
            DrawModuleHeader();

            // Rating Request settings
            EditorGUILayout.Space();
            DrawUppercaseSection("REQUEST_DIALOG_CONFIG_FOLDOUT_KEY", "STORE REVIEW | RATING DIALOG", () =>
                {
                    EditorGUILayout.HelpBox("Since Easy Mobile Pro 2.8.0 we use native review popups provided by iOS and Android which do not require any setup here.", MessageType.Info);

                    // // Appearance
                    // EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);
                    // EditorGUILayout.HelpBox("All instances of " + RatingDialogContent.PRODUCT_NAME_PLACEHOLDER + " in titles and messages will be replaced by the actual Product Name given in PlayerSettings.", MessageType.Info);
                    // EditorGUI.indentLevel++;
                    // EditorGUILayout.PropertyField(RatingRequestProperties.defaultRatingDialogContent.property, RatingRequestProperties.defaultRatingDialogContent.content, true);
                    // EditorGUI.indentLevel--;

                    // // Behaviour
                    // EditorGUILayout.Space();
                    // EditorGUILayout.LabelField("Behaviour", EditorStyles.boldLabel);
                    // EditorGUILayout.PropertyField(RatingRequestProperties.minimumAcceptedStars.property, RatingRequestProperties.minimumAcceptedStars.content);
                    // EditorGUILayout.PropertyField(RatingRequestProperties.supportEmail.property, RatingRequestProperties.supportEmail.content);
                    // EditorGUILayout.PropertyField(RatingRequestProperties.iosAppId.property, RatingRequestProperties.iosAppId.content);

                    // // Display constraints
                    // EditorGUILayout.Space();
                    // EditorGUILayout.LabelField("Display Constraints", EditorStyles.boldLabel);
                    // EditorGUILayout.PropertyField(RatingRequestProperties.annualCap.property, RatingRequestProperties.annualCap.content);
                    // EditorGUILayout.PropertyField(RatingRequestProperties.delayAfterInstallation.property, RatingRequestProperties.delayAfterInstallation.content);
                    // EditorGUILayout.PropertyField(RatingRequestProperties.coolingOffPeriod.property, RatingRequestProperties.coolingOffPeriod.content);
                    // EditorGUILayout.PropertyField(RatingRequestProperties.ignoreConstraintsInDevelopment.property, RatingRequestProperties.ignoreConstraintsInDevelopment.content);
                });
        }
    }
}
