using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Internal.Privacy
{
    public class EditorConsentDialogUI : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rootTransform = null;

        [SerializeField]
        private RectTransform contentParent = null;

        [SerializeField]
        private Text titleText = null;

        [SerializeField]
        private Button backButton = null;

        [Header("Prefabs")]
        [SerializeField]
        private EditorConsentDialogToggleUI togglePrefab = null;

        [SerializeField]
        private EditorConsentDialogClickableText plainTextPrefab = null;

        [SerializeField]
        private EditorConsentDialogButtonUI buttonPrefab = null;

        public event Action<string, Dictionary<string, bool>> OnCompleteted;
        public event Action OnDismissed;
        public event Action<string, bool> OnToggleStateUpdated;

        public RectTransform RectTransform { get { return rootTransform; } }

        public bool IsShowing { get; set; }

        public bool IsDismissible { get; set; }

        public bool IsConstructed { get; set; }

        private Dictionary<string, EditorConsentDialogButtonUI> createdButtons;
        private Dictionary<string, EditorConsentDialogToggleUI> createdToggles;

        protected virtual void Awake()
        {
            createdButtons = new Dictionary<string, EditorConsentDialogButtonUI>();
            createdToggles = new Dictionary<string, EditorConsentDialogToggleUI>();
        }

        protected virtual void Start()
        {
            if (backButton != null)
                backButton.onClick.AddListener(() =>
                    {
                        if (IsDismissible)
                        {
                            Dismiss();
                        }
                    });
        }

        protected virtual void Update()
        {
            if (!IsConstructed || !IsShowing)
                return;

            // Android back button.
            if (Input.GetKeyDown(KeyCode.Escape) && IsDismissible)
            {
                Dismiss();
            }
        }

        public void SetButtonInteractable(string buttonId, bool interactble)
        {
            if (createdButtons == null || buttonId == null || !createdButtons.ContainsKey(buttonId))
                return;

            createdButtons[buttonId].Interactable = interactble;
        }

        public EditorConsentDialogUI Construct(string title, bool isDimissible)
        {
            if (titleText != null && title != null)
                titleText.text = title;

            if (backButton != null)
                backButton.gameObject.SetActive(isDimissible);

            IsConstructed = true;
            IsDismissible = isDimissible;

            return this;
        }

        public void Show()
        {
            if (!IsConstructed || IsShowing || rootTransform == null)
                return;

            rootTransform.gameObject.SetActive(true);
            IsShowing = true;
        }

        public void Hide()
        {
            if (!IsConstructed || !IsShowing || rootTransform == null)
                return;

            rootTransform.gameObject.SetActive(false);
            IsShowing = false;
        }

        public void Dismiss()
        {
            if (OnDismissed != null)
                OnDismissed();

            Hide();
        }

        public EditorConsentDialogClickableText AddPlainText(string text)
        {
            if (text == null || plainTextPrefab == null || contentParent == null)
                return null;

            EditorConsentDialogClickableText newText = GameObject.Instantiate(plainTextPrefab);
            newText.text = text;
            newText.rectTransform.SetParent(contentParent, false);
            newText.OnHyperlinkClicked += link =>
            {
                link = link.TrimStart('"').TrimEnd('"');
                Application.OpenURL(link);
            };

            return newText;
        }

        public EditorConsentDialogButtonUI AddButton(ConsentDialog.Button buttonData)
        {
            if (buttonPrefab == null || contentParent == null)
                return null;

            if (createdButtons.ContainsKey(buttonData.Id))
            {
                Debug.LogWarning("Ignored a button with duplicated id: " + buttonData.Id + "!!!");
                return null;
            }

            EditorConsentDialogButtonUI newButton = GameObject.Instantiate(buttonPrefab);
            newButton.Button.interactable = buttonData.IsInteractable;
            newButton.UpdateText(buttonData.Title);
            newButton.UpdateTextColor(buttonData.GetCurrentTitleColor());
            newButton.UpdateBackgroundColor(buttonData.GetCurrentBodyColor());
            newButton.AddListener(() =>
                {
                    Hide();

                    if (OnCompleteted != null)
                        OnCompleteted(buttonData.Id, GetTogglesResult());
                });
            newButton.Button.transform.SetParent(contentParent, false);

            createdButtons.Add(buttonData.Id, newButton);
            return newButton;
        }

        public EditorConsentDialogToggleUI AddToggle(ConsentDialog.Toggle toggleData)
        {
            if (togglePrefab == null || contentParent == null)
                return null;

            if (createdToggles.ContainsKey(toggleData.Id))
            {
                Debug.LogWarning("Ignored a toggle with duplicated id: " + toggleData.Id + "!!!");
                return null;
            }

            EditorConsentDialogToggleUI newToggle = GameObject.Instantiate(togglePrefab);
            newToggle.UpdateSettings(toggleData);
            newToggle.OnToggleStateUpdated += OnToggleStateUpdated;
            newToggle.transform.SetParent(contentParent, false);

            createdToggles.Add(toggleData.Id, newToggle);
            return newToggle;
        }

        private Dictionary<string, bool> GetTogglesResult()
        {
            if (createdToggles == null)
                return null;

            return createdToggles.ToDictionary(toggle => toggle.Key, toggle => toggle.Value.IsOn);
        }
    }
}
