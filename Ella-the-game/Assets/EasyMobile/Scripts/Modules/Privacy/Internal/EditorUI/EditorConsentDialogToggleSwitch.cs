using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EasyMobile.Internal.Privacy
{
    [RequireComponent(typeof(RectTransform))]
    public class EditorConsentDialogToggleSwitch : Selectable, IEventSystemHandler, IPointerClickHandler, ISubmitHandler, ICanvasElement
    {
        [Serializable]
        public class ToggleEvent : UnityEvent<bool>
        {

        }

        [Header("Custom settings")]
        [SerializeField]
        private Image switchObject = null;

        [SerializeField]
        private Image background = null;

        [SerializeField, FormerlySerializedAs("m_IsActive")]
        private bool m_IsOn = false;

        [SerializeField, Tooltip("Position of the \"SwitchObject\" when this toggle is on.")]
        private Vector2 isOnPosition = Vector2.zero;

        [SerializeField, Tooltip("Position of the \"SwitchObject\" when this toggle is off.")]
        private Vector2 isOffPosition = Vector2.zero;

        [SerializeField]
        private float animationDuration = 3;

        [SerializeField, Tooltip("Change switch and background color when toggle state is changed?"), Space]
        private bool toggleColor;

        [SerializeField]
        private Color switchOnColor = Color.white,
            switchOffColor = Color.white,
            backgroundOnColor = Color.white,
            backgroundOffColor = Color.white;

        [SerializeField]
        private ToggleEvent onValueChanged = new ToggleEvent();

        public ToggleEvent OnValueChanged
        {
            get { return onValueChanged; }
        }

        public bool isOn
        {
            get
            {
                return m_IsOn;
            }
            set
            {
                Set(value);
            }
        }

        /// <summary>
        /// Change switch and background color when toggle state is changed?
        /// </summary>
        public bool ShouldToggleColor
        {
            get { return toggleColor; }
            set { toggleColor = value; }
        }

        public float AnimationDuration
        {
            get { return animationDuration; }
            set
            {
                if (value < 0)
                    return;

                animationDuration = value;
            }
        }

        private Coroutine switchCoroutine;
        private RectTransform switchTransform;

        #region Mono Messages

        protected override void Awake()
        {
            base.Awake();
            switchTransform = switchObject != null ? switchObject.rectTransform : null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayEffect();
        }

        protected override void Start()
        {
            PlayEffect();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Set(m_IsOn, false);
            PlayEffect();

#if UNITY_2018_3_OR_NEWER
            bool isNotPrefab = PrefabUtility.GetPrefabAssetType(this) != PrefabAssetType.Regular;
#else
            bool isNotPrefab = PrefabUtility.GetPrefabType(this) != PrefabType.Prefab;
#endif

            if (isNotPrefab && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);

            if (animationDuration < 0)
                animationDuration = 0;
        }
#endif

        #endregion // Mono Messages

        #region Public Methods

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Toggle();
            }
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Toggle();
        }

        public virtual void Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Prelayout)
            {
                onValueChanged.Invoke(m_IsOn);
            }
        }

        public void Toggle()
        {
            if (IsActive() && IsInteractable())
            {
                isOn = !isOn;
            }
        }

        public void LayoutComplete()
        {
        }

        public void GraphicUpdateComplete()
        {
        }

        #endregion // Public Methods

        #region Private Methods

        private void PlayEffect()
        {
            if (switchTransform == null)
                return;

            Vector2 targetPosition = (m_IsOn) ? isOnPosition : isOffPosition;

            if (!Application.isPlaying)
            {
                switchTransform.anchoredPosition = targetPosition;
                return;
            }

            if (switchCoroutine != null)
                StopCoroutine(switchCoroutine);

            switchCoroutine = StartCoroutine(SwitchCoroutine(targetPosition, m_IsOn));
        }

        private void Set(bool value)
        {
            Set(value, true);
        }

        private void Set(bool value, bool sendCallback)
        {
            if (m_IsOn != value)
            {
                m_IsOn = value;
                PlayEffect();
                if (sendCallback)
                {
                    onValueChanged.Invoke(m_IsOn);
                }
            }
        }

        private IEnumerator SwitchCoroutine(Vector2 targetPosition, bool isOn)
        {
            if (switchTransform == null || animationDuration < 0)
                yield break;

            UpdateColor(isOn);

            float currentLerpValue = 0f;
            Vector2 startPosition = switchTransform.anchoredPosition;
            while (currentLerpValue <= animationDuration && switchTransform != null)
            {
                currentLerpValue += Time.deltaTime;
                switchTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, currentLerpValue / animationDuration);
                yield return null;
            }
            switchTransform.anchoredPosition = targetPosition;

            switchCoroutine = null;
        }

        private void UpdateColor(bool isOn)
        {
            if (!toggleColor)
                return;

            if (switchObject != null)
                switchObject.color = isOn ? switchOnColor : switchOffColor;

            if (background != null)
                background.color = isOn ? backgroundOnColor : backgroundOffColor;
        }

        bool ICanvasElement.IsDestroyed()
        {
            return base.IsDestroyed();
        }

        #endregion // Private Methods

    }
}