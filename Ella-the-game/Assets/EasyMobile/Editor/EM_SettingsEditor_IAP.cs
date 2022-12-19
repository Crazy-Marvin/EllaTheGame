using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace EasyMobile.Editor
{
    // Partial editor class for In-App Purchasing module.
    internal partial class EM_SettingsEditor
    {
        const string IAPModuleLabel = "IN-APP PURCHASING";
        const string IAPModuleIntro = "The In-App Purchasing module leverages Unity IAP to help you quickly setup and sell digital goods in your game.";
        const string UnityIAPEnableInstruction = "Unity In-App Purchasing service is required. Please go to Window > Services to enable it.";
        const string IAPManualInitInstruction = "You can initialize manually from script by calling the InAppPurchasing.InitializePurchasing() method.";
        const string IAPConstantGenerationIntro = "Generate the static class " + EM_Constants.RootNameSpace + "." + EM_Constants.IAPConstantsClassName + " that contains the constants of product names." +
                                                  " Remember to regenerate if you make changes to these names.";

        const string IAPProduct_NameProperty = "_name";
        const string IAPProduct_TypeProperty = "_type";
        const string IAPProduct_IdProperty = "_id";
        const string IAPProduct_PriceProperty = "_price";
        const string IAPProduct_DescriptionProperty = "_description";
        const string IAPProduct_StoreSpecificIdsProperty = "_storeSpecificIds";
        const string StoreSpecificIds_StoreProperty = "store";
        const string StoreSpecificIds_IdProperty = "id";
        const string IAPProduct_IsEditingAdvanced = "_isEditingAdvanced";

        GUIContent IAPProduct_NameContent = new GUIContent("Name", "Product name can be used when making purchases");
        GUIContent IAPProduct_TypeContent = new GUIContent("Type", "Product type");
        GUIContent IAPProduct_IdContent = new GUIContent("Id", "Unified product Id, this Id will be used for stores that don't have a specific Id provided in Store Specific Ids array");
        GUIContent IAPProduct_PriceContent = new GUIContent("Price", "Product price string for displaying purpose");
        GUIContent IAPProduct_DescriptionContent = new GUIContent("Description", "Product description for displaying purpose");
        GUIContent IAPProduct_StoreSpecificIdsContent = new GUIContent("Store-Specific Ids", "Product Id that is specific to a certain store (and will override the unified Id for that store)");

#if EM_UIAP
        static IAPAndroidStore currentAndroidStore;
        static bool isIAPProductsFoldout = false;
#endif

        static Dictionary<string, bool> iapFoldoutStates = new Dictionary<string, bool>();
        IAPAndroidStore androidStore;

        void IAPModuleGUI()
        {
            DrawModuleHeader();

            // Now draw the GUI.
            if (!isIAPModuleEnable.boolValue)
                return;

#if !EM_UIAP
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EM_GUIStyleManager.UppercaseSectionBox);
            EditorGUILayout.HelpBox(UnityIAPEnableInstruction, MessageType.Error, true);
            EditorGUILayout.EndVertical();
#else
            EditorGUILayout.Space();
            DrawUppercaseSection("IAP_AUTO_INIT_CONFIG", "AUTO INITIALIZATION", () =>
            {
                EditorGUILayout.PropertyField(IAPProperties.autoInit.property, IAPProperties.autoInit.content);
                if (!IAPProperties.autoInit.property.boolValue)
                {
                    EditorGUILayout.HelpBox(IAPManualInitInstruction, MessageType.Info);
                }
            });

            // Select target Android store, like using the Window > Unity IAP > Android > Target ... menu item.
            EditorGUILayout.Space();
            DrawUppercaseSection("ANDROID_TARGET_STORE_FOLDOUT_KEY", "TARGET ANDROID STORE", () =>
                {
                    EditorGUI.BeginChangeCheck();
                    IAPProperties.targetAndroidStore.property.enumValueIndex = EditorGUILayout.Popup(
                        IAPProperties.targetAndroidStore.content.text,
                        IAPProperties.targetAndroidStore.property.enumValueIndex,
                        IAPProperties.targetAndroidStore.property.enumDisplayNames
                    );
                    if (EditorGUI.EndChangeCheck())
                    {
                        SetTargetAndroidStore((IAPAndroidStore)IAPProperties.targetAndroidStore.property.enumValueIndex);
                    }

                    // Enable Amazon sandbox testing.
                    androidStore = (IAPAndroidStore)IAPProperties.targetAndroidStore.property.enumValueIndex;
                    if (androidStore == IAPAndroidStore.AmazonAppStore)
                        EditorGUILayout.PropertyField(IAPProperties.enableAmazoneSandboxTesting.property, IAPProperties.enableAmazoneSandboxTesting.content);
                });

            // Apple's Ask To Buy settings.
            EditorGUILayout.Space();
            DrawUppercaseSection("APPLE_ASK_TO_BUY_FOLDOUT_KEY", "APPLE ASK-TO-BUY", () =>
                {
                    // Apple Ask To Buy simulation in sandbox.
                    EditorGUILayout.PropertyField(IAPProperties.simulateAppleAskToBuy.property, IAPProperties.simulateAppleAskToBuy.content);
                });

            // Apple's promotional purchases settings.
            EditorGUILayout.Space();
            DrawUppercaseSection("APPLE_PROMOTIONAL_PURCHASES_FOLDOUT_KEY", "APPLE PROMOTIONAL PURCHASES", () =>
                {
                    // Whether to intercept Apple's promotional purchases.
                    EditorGUILayout.PropertyField(IAPProperties.interceptApplePromotionalPurchases.property, IAPProperties.interceptApplePromotionalPurchases.content);
                });

            // Receipt validation
            EditorGUILayout.Space();
            DrawUppercaseSection("RECEIPT_VALIDATION_FOLDOUT_KEY", "RECEIPT VALIDATION", () =>
                {
                    EditorGUILayout.HelpBox("Unity IAP offers local receipt validation for extra security. Apple stores and Google Play store only.", MessageType.None);

                    // iOS store.
                    EditorGUI.BeginDisabledGroup(!isAppleTangleValid);
                    IAPProperties.validateAppleReceipt.property.boolValue = EditorGUILayout.Toggle(IAPProperties.validateAppleReceipt.content, IAPProperties.validateAppleReceipt.property.boolValue);
                    EditorGUI.EndDisabledGroup();

                    // Always disable the option if AppleTangle is not valid.
                    if (!isAppleTangleValid)
                    {
                        IAPProperties.validateAppleReceipt.property.boolValue = false;
                    }

                    // Google Play store.
                    bool isTargetingGooglePlay = androidStore == IAPAndroidStore.GooglePlay;
                    EditorGUI.BeginDisabledGroup(!isGooglePlayTangleValid);
                    IAPProperties.validateGooglePlayReceipt.property.boolValue = EditorGUILayout.Toggle(IAPProperties.validateGooglePlayReceipt.content, IAPProperties.validateGooglePlayReceipt.property.boolValue);
                    EditorGUI.EndDisabledGroup();

                    // Always disable the option if GooglePlayTangle is not valid.
                    if (!isGooglePlayTangleValid)
                    {
                        IAPProperties.validateGooglePlayReceipt.property.boolValue = false;
                    }

                    if (!isAppleTangleValid || (!isGooglePlayTangleValid && isTargetingGooglePlay))
                    {
                        string rvMsg = "Please go to Window > Unity IAP > IAP Receipt Validation Obfuscator and create obfuscated secrets to enable receipt validation for Apple stores and Google Play store.";

                        if (!isAppleTangleValid)
                        {
                            rvMsg += " Note that you don't need to provide a Google Play public key if you're only targeting Apple stores.";
                        }
                        else
                        {
                            rvMsg = rvMsg.Replace("Apple stores and ", "");
                        }

                        if (isGooglePlayTangleValid || !isTargetingGooglePlay)
                        {
                            rvMsg = rvMsg.Replace(" and Google Play store", "");
                        }

                        EditorGUILayout.HelpBox(rvMsg, MessageType.Warning);
                    }
                });

            // Product list
            EditorGUILayout.Space();
            DrawUppercaseSection("PRODUCTS_FOLDOUT_KEY", "PRODUCTS", () =>
                {
                    EMProperty products = IAPProperties.products;

                    if (products.property.arraySize > 0)
                    {
                        EditorGUI.indentLevel++;
                        isIAPProductsFoldout = EditorGUILayout.Foldout(isIAPProductsFoldout, products.property.arraySize + " " + products.content.text, true);
                        EditorGUI.indentLevel--;

                        if (isIAPProductsFoldout)
                        {
                            // Draw the array of IAP products.
                            DrawArrayProperty(products.property, DrawIAPProduct);

                            // Detect duplicate product names.
                            string duplicateName = EM_EditorUtil.FindDuplicateFieldInArrayProperty(products.property, IAPProduct_NameProperty);
                            if (!string.IsNullOrEmpty(duplicateName))
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.HelpBox("Found duplicate name of \"" + duplicateName + "\".", MessageType.Warning);
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No products added.", MessageType.None);
                    }

                    if (GUILayout.Button("Add New Product", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                    {
                        // Add new IAP product.
                        AddNewProduct(products.property);

                        // Open the foldout if it's closed.
                        isIAPProductsFoldout = true;
                    }
                });

            // Constant generation.
            EditorGUILayout.Space();
            DrawUppercaseSection("IAP_CONTANTS_GENERATION_FOLDOUT_KEY", "CONSTANTS GENERATION", () =>
                {
                    EditorGUILayout.HelpBox(IAPConstantGenerationIntro, MessageType.None);
                    if (GUILayout.Button("Generate Constants Class", GUILayout.Height(EM_GUIStyleManager.buttonHeight)))
                    {
                        GenerateIAPConstants();
                    }
                });
#endif
        }

#if EM_UIAP
        void SetTargetAndroidStore(IAPAndroidStore store)
        {
            UnityEngine.Purchasing.AppStore androidStore = InAppPurchasing.GetAppStore(store);
            UnityEditor.Purchasing.UnityPurchasingEditor.TargetAndroidStore(androidStore);
        }
#endif

        // Generate a static class containing constants of IAP product names.
        void GenerateIAPConstants()
        {
            // First create a hashtable containing all the names to be stored as constants.
            SerializedProperty productsProp = IAPProperties.products.property;

            // First check if there're duplicate names.
            string duplicateName = EM_EditorUtil.FindDuplicateFieldInArrayProperty(productsProp, IAPProduct_NameProperty);
            if (!string.IsNullOrEmpty(duplicateName))
            {
                EM_EditorUtil.Alert("Error: Duplicate Names", "Found duplicate product name of \"" + duplicateName + "\".");
                return;
            }

            // Proceed with adding resource keys.
            Hashtable resourceKeys = new Hashtable();

            // Add the product names.
            for (int i = 0; i < productsProp.arraySize; i++)
            {
                SerializedProperty element = productsProp.GetArrayElementAtIndex(i);
                string name = element.FindPropertyRelative(IAPProduct_NameProperty).stringValue;

                // Ignore all items with an empty name.
                if (!string.IsNullOrEmpty(name))
                {
                    string key = "Product_" + name;
                    resourceKeys.Add(key, name);
                }
            }

            if (resourceKeys.Count > 0)
            {
                // Now build the class.
                EM_EditorUtil.GenerateConstantsClass(
                    EM_Constants.GeneratedFolder,
                    EM_Constants.RootNameSpace + "." + EM_Constants.IAPConstantsClassName,
                    resourceKeys,
                    true
                );
            }
            else
            {
                EM_EditorUtil.Alert("Constants Class Generation", "Please fill in required information for all products.");
            }
        }

        bool DrawIAPProduct(SerializedProperty property)
        {
            // Get members.
            SerializedProperty name = property.FindPropertyRelative(IAPProduct_NameProperty);
            SerializedProperty type = property.FindPropertyRelative(IAPProduct_TypeProperty);
            SerializedProperty id = property.FindPropertyRelative(IAPProduct_IdProperty);
            SerializedProperty price = property.FindPropertyRelative(IAPProduct_PriceProperty);
            SerializedProperty description = property.FindPropertyRelative(IAPProduct_DescriptionProperty);
            SerializedProperty storeSpecificIds = property.FindPropertyRelative(IAPProduct_StoreSpecificIdsProperty);
            SerializedProperty isEditingAdvanced = property.FindPropertyRelative(IAPProduct_IsEditingAdvanced);

            // Main content section.
            EditorGUILayout.BeginVertical(EM_GUIStyleManager.GetCustomStyle("Item Box"));

            // Foldout
            string key = property.propertyPath;
            string foldoutLabel = string.IsNullOrEmpty(name.stringValue) ? "[Untitled Product]" : name.stringValue;

            if (!iapFoldoutStates.ContainsKey(key))
                iapFoldoutStates.Add(key, false);

            EditorGUI.indentLevel++;
            iapFoldoutStates[key] = EditorGUILayout.Foldout(iapFoldoutStates[key], foldoutLabel, true);

            if (iapFoldoutStates[key])
            {
                // Required settings
                name.stringValue = EditorGUILayout.TextField(IAPProduct_NameContent, name.stringValue);
                type.enumValueIndex = EditorGUILayout.Popup(IAPProduct_TypeContent.text, type.enumValueIndex, type.enumDisplayNames);
                id.stringValue = EditorGUILayout.TextField(IAPProduct_IdContent, id.stringValue);

                // Advanced settings
                EditorGUI.indentLevel++;
                isEditingAdvanced.boolValue = EditorGUILayout.Foldout(isEditingAdvanced.boolValue, "More (Optional)", true);

                if (isEditingAdvanced.boolValue)
                {
                    price.stringValue = EditorGUILayout.TextField(IAPProduct_PriceContent, price.stringValue);
                    description.stringValue = EditorGUILayout.TextField(IAPProduct_DescriptionContent, description.stringValue, EM_GUIStyleManager.WordwrapTextField, GUILayout.Height(EditorGUIUtility.singleLineHeight * 3));

                    // Store-specific Ids section.
                    int storeNum = System.Enum.GetNames(typeof(IAPStore)).Length;
                    GUIContent plusButton = EditorGUIUtility.IconContent("Toolbar Plus");
                    GUIContent minusButton = EditorGUIUtility.IconContent("Toolbar Minus");
                    GUIStyle buttonsStyle = new GUIStyle(GUIStyle.none)
                    {
                        fixedHeight = EM_GUIStyleManager.smallButtonHeight,
                        fixedWidth = EM_GUIStyleManager.smallButtonWidth,
                    };

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(IAPProduct_StoreSpecificIdsContent);

                    // Draw plus button.
                    bool canAdd = true;
                    if (storeSpecificIds.arraySize >= storeNum) // Won't allow values larger than the number of available stores.
                        canAdd = false;

                    EditorGUI.BeginDisabledGroup(!canAdd);
                    if (GUILayout.Button(plusButton, buttonsStyle))
                        storeSpecificIds.arraySize++;
                    EditorGUI.EndDisabledGroup();

                    // Draw minus button.
                    bool canDelete = true;
                    if (storeSpecificIds.arraySize < 1)
                        canDelete = false;

                    EditorGUI.BeginDisabledGroup(!canDelete);
                    if (GUILayout.Button(minusButton, buttonsStyle))
                        storeSpecificIds.arraySize--;
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < storeSpecificIds.arraySize; i++)
                    {
                        SerializedProperty element = storeSpecificIds.GetArrayElementAtIndex(i);
                        SerializedProperty specificStore = element.FindPropertyRelative(StoreSpecificIds_StoreProperty);
                        SerializedProperty specificId = element.FindPropertyRelative(StoreSpecificIds_IdProperty);

                        EditorGUILayout.BeginHorizontal();
                        specificStore.enumValueIndex = EditorGUILayout.Popup(specificStore.enumValueIndex, specificStore.enumDisplayNames);
                        specificId.stringValue = EditorGUILayout.TextField(specificId.stringValue);

                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

            return iapFoldoutStates[key];
        }

        void AddNewProduct(SerializedProperty productsArrayProp)
        {
            if (productsArrayProp.isArray)
            {
                productsArrayProp.InsertArrayElementAtIndex(productsArrayProp.arraySize);

                // Reset the fields of newly added element or it will take the values of the preceding one.
                SerializedProperty newProp = productsArrayProp.GetArrayElementAtIndex(productsArrayProp.arraySize - 1);
                SerializedProperty name = newProp.FindPropertyRelative(IAPProduct_NameProperty);
                SerializedProperty type = newProp.FindPropertyRelative(IAPProduct_TypeProperty);
                SerializedProperty id = newProp.FindPropertyRelative(IAPProduct_IdProperty);
                SerializedProperty storeSpecificIds = newProp.FindPropertyRelative(IAPProduct_StoreSpecificIdsProperty);

                name.stringValue = string.Empty;
                id.stringValue = string.Empty;
                type.enumValueIndex = (int)IAPProductType.Consumable;
                storeSpecificIds.ClearArray();
            }
        }
    }
}

