#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using EasyMobile.MiniJSON;

namespace EasyMobile.Internal.Privacy
{
    internal class EditorConsentDialog : IPlatformConsentDialog
    {
        internal const string RootCanvasPrefabPath = "Assets/EasyMobile/Prefabs/EditorConsentDialog/ConsentDialogCanvas.prefab";
        internal const string ConsentDialogPrefabPath = "Assets/EasyMobile/Prefabs/EditorConsentDialog/ConsentDialogUI.prefab";

        private RectTransform rootCanvasPrefab;
        private EditorConsentDialogUI consentDialogUIPrefab;

        private RectTransform rootCanvas;
        private EditorConsentDialogUI consentDialogUI;
        private bool isShowing;

        public EditorConsentDialog()
        {
            rootCanvasPrefab = LoadAndLogWarning<RectTransform>(RootCanvasPrefabPath);
            consentDialogUIPrefab = LoadAndLogWarning<EditorConsentDialogUI>(ConsentDialogPrefabPath);
        }

        private T LoadAndLogWarning<T>(string path) where T : UnityEngine.Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset == null)
                Debug.LogWarning("Couldn't load asset at path: " + path);

            return asset;
        }

        private void BuildDialogFromContent(EditorConsentDialogUI dialog, string contents)
        {
            foreach (var content in DeserializeContents(contents))
            {
                if (content.IsPlainText())
                {
                    dialog.AddPlainText(content.content);
                }
                else if (content.IsButton())
                {
                    var buttonData = JsonUtility.FromJson<ConsentDialog.Button>(content.content);
                    dialog.AddButton(buttonData);
                }
                else if (content.IsToggle())
                {
                    var toggleData = JsonUtility.FromJson<ConsentDialog.Toggle>(content.content);
                    dialog.AddToggle(toggleData);
                }
                else
                {
                    Debug.Log("Unexpected type of content: " + content.type);
                }
            }
        }

        private IEnumerable<ConsentDialogContentSerializer.SplitContent> DeserializeContents(string contents)
        {
            if (contents == null)
                yield break;

            var jsonArray = Json.Deserialize(contents) as List<object>;

            if (jsonArray == null)
                yield break;

            foreach (object element in jsonArray)
            {
                if (element == null)
                    continue;

                var serializedObject = JsonUtility.FromJson<ConsentDialogContentSerializer.SplitContent>(element.ToString());

                if (serializedObject != null)
                    yield return serializedObject;
            }
        }

        private void OnToggleStateUpdatedHandler(string toggleId, bool isOn)
        {
            if (ToggleStateUpdated != null)
                ToggleStateUpdated(this, toggleId, isOn);
        }

        private void OnDismissedHandler()
        {
            if (Dismissed != null)
                Dismissed(this);

            DestroyOldDialog();
        }

        private void OnCompletedHandler(string buttonId, Dictionary<string, bool> toggleResults)
        {
            if (Completed != null)
                Completed(this, buttonId, toggleResults);

            DestroyOldDialog();
        }

        private void DestroyOldDialog()
        {
            if (rootCanvas != null)
                GameObject.Destroy(rootCanvas.gameObject);

            isShowing = false;
        }

        #region IPlatformConsentDialog implementation

        #pragma warning disable         // Empty events warning
        public event Action<IPlatformConsentDialog, string, bool> ToggleStateUpdated;
        public event Action<IPlatformConsentDialog, string, Dictionary<string, bool>> Completed;
        public event Action<IPlatformConsentDialog> Dismissed;
        #pragma warning restore 0067

        public bool IsShowing()
        {
            return isShowing;
        }

        public void Show(string title, string content, bool isDimissible)
        {
            if (rootCanvasPrefab == null || consentDialogUIPrefab == null)
                return;

            isShowing = true;

            rootCanvas = GameObject.Instantiate(rootCanvasPrefab);
            consentDialogUI = GameObject.Instantiate(consentDialogUIPrefab);
            consentDialogUI.transform.SetParent(rootCanvas, false);

            consentDialogUI.OnCompleteted += OnCompletedHandler;
            consentDialogUI.OnDismissed += OnDismissedHandler;
            consentDialogUI.OnToggleStateUpdated += OnToggleStateUpdatedHandler;

            consentDialogUI.Construct(title, isDimissible);
            BuildDialogFromContent(consentDialogUI, content);
            consentDialogUI.Show();
        }

        public void SetButtonInteractable(string buttonId, bool interactble)
        {
            if (consentDialogUI == null)
                return;

            consentDialogUI.SetButtonInteractable(buttonId, interactble);
        }

        public void SetToggleIsOn(string toggleId, bool isOn, bool animated)
        {
            // TODO: implement this.
        }

        public void SetToggleInteractable(string toggleId, bool interactable)
        {
            // TODO: implement this.
        }

        #endregion
    }
}
#endif