using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Internal.Privacy
{
    public class EditorConsentDialogToggleUI : MonoBehaviour
    {
        [SerializeField]
        private Button expandButton = null;

        [SerializeField]
        private EditorConsentDialogToggleSwitch toggle = null;

        [SerializeField]
        private EditorConsentDialogClickableText descriptionText = null;

        [SerializeField]
        private Text title = null;

        [SerializeField]
        private Image expandArrow = null;

        [SerializeField]
        private Sprite collapseIcon = null, expandIcon = null;

        [SerializeField]
        private LayoutElement descriptionLayout = null;

        [SerializeField]
        private float expandAnimationDuration = 0.2f;

        public Action<string, bool> OnToggleStateUpdated;

        public bool IsDescriptionExpanded { get; set; }

        public bool IsOn
        {
            get { return toggle != null ? toggle.isOn : false; }
            set
            {
                if (toggle != null)
                    toggle.isOn = value;
            }
        }

        public float ExpandAnimationDuration
        {
            get { return expandAnimationDuration; }
            set
            {
                if (value < 0)
                    return;

                expandAnimationDuration = value;
            }
        }

        private Coroutine expandCoroutine;

        protected virtual void Awake()
        {
            if (descriptionText != null)
                descriptionText.OnHyperlinkClicked += link => Application.OpenURL(link);

            if (expandButton != null)
                expandButton.onClick.AddListener(() => ToggleDescription());

            IsDescriptionExpanded = false;
            descriptionLayout.preferredHeight = 0;           
        }

        protected virtual void OnValidate()
        {
            if (expandAnimationDuration < 0)
                expandAnimationDuration = 0;
        }

        #region Public Methods

        public void UpdateSettings(ConsentDialog.Toggle toggleData)
        {
            UpdateDescription(toggleData.Description);
            UpdateTitle(toggleData.Title);
            SetupToggle(toggleData);
        }

        public void ExpandDescription()
        {
            if (IsDescriptionExpanded)
                return;

            if (expandArrow != null && expandIcon != null)
                expandArrow.sprite = expandIcon;

            IsDescriptionExpanded = true;
            AnimateDescription(descriptionText.preferredHeight);
        }

        public void CollapseDescription()
        {
            if (!IsDescriptionExpanded)
                return;

            if (expandArrow != null && collapseIcon)
                expandArrow.sprite = collapseIcon;

            IsDescriptionExpanded = false;
            AnimateDescription(0);
        }

        public void ToggleDescription()
        {
            if (IsDescriptionExpanded)
            {
                CollapseDescription();
            }
            else
            {
                ExpandDescription();
            }
        }

        public void UpdateTitle(string text)
        {
            if (title == null || text == null)
                return;

            title.text = text;
        }

        public void UpdateDescription(string text)
        {
            if (descriptionText == null || text == null)
                return;

            descriptionText.text = text;
            
            // Resize the description component to fix with new value.
            // Note that we can't use ContentSizeFitter component because
            // we need to update the Text's height when animating it.
            descriptionLayout.preferredHeight = IsDescriptionExpanded ? descriptionText.preferredHeight : 0;
        }

        #endregion Public Methods

        #region Private Methods

        private void SetupToggle(ConsentDialog.Toggle toggleData)
        {
            if (toggle == null)
                return;

            toggle.isOn = toggleData.IsOn;

            if (!toggleData.IsInteractable)
            {
                toggle.interactable = false;
                return;
            }

            toggle.OnValueChanged.AddListener(isOn =>
                {
                    if (toggleData.ShouldToggleDescription)
                        UpdateDescription(isOn ? toggleData.OnDescription : toggleData.OffDescription);

                    if (OnToggleStateUpdated != null)
                        OnToggleStateUpdated(toggleData.Id, isOn);
                });
        }

        private void SetupExpandButton()
        {
            if (expandButton == null)
                return;

            expandButton.onClick.AddListener(() =>
                {
                    if (IsDescriptionExpanded)
                    {
                        CollapseDescription();
                    }
                    else
                    {
                        ExpandDescription();
                    }
                });
        }

        private void AnimateDescription(float targetHeight)
        {
            if (expandCoroutine != null)
                StopCoroutine(expandCoroutine);

            expandCoroutine = StartCoroutine(AnimateDescriptionCoroutine(targetHeight));
        }

        private IEnumerator AnimateDescriptionCoroutine(float targetHeight)
        {
            if (descriptionLayout == null || expandAnimationDuration < 0)
                yield break;

            float currentLerpValue = 0;
            float startHeight = descriptionLayout.preferredHeight;

            while (currentLerpValue < expandAnimationDuration && descriptionLayout != null)
            {
                currentLerpValue += Time.deltaTime;
                descriptionLayout.preferredHeight = Mathf.Lerp(startHeight, targetHeight, currentLerpValue / expandAnimationDuration);

                yield return null;
            }
            descriptionLayout.preferredHeight = targetHeight;

            expandCoroutine = null;
        }

        #endregion
    }
}
