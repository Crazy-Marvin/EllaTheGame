using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace EasyMobile.Demo
{
    /// <summary>
    /// An simple panel with similar to <see cref="NativeUI.AlertPopup"/>.
    /// Currenly calling another alert in <see cref="NativeUI.Alert(string, string, string)"/>'s callback will cause NRE,
    /// so we need to use this class instead.
    /// </summary>
    public class AlertPopup : MonoBehaviour
    {
        public enum ButtonName
        {
            /// <summary>
            /// The alert popup is closed via <see cref="Hide"/> method of an error orcured.
            /// </summary>
            None = 0,

            /// <summary>
            /// The alert popup is clased via <see cref="buttonA"/>.
            /// </summary>
            ButtonA,

            /// <summary>
            /// The alert popup is clased via <see cref="buttonB"/>.
            /// </summary>
            ButtonB,

            /// <summary>
            /// The alert popup is clased via <see cref="buttonC"/>.
            /// </summary>
            ButtonC
        }

        [Serializable]
        public class IntEvent : UnityEvent<ButtonName> { }

        [SerializeField]
        private GameObject rootView = null;

        [SerializeField]
        private Button buttonA = null, buttonB = null, buttonC = null;

        [SerializeField]
        private Text buttonAText = null, buttonBText = null, buttonCText = null;

        [SerializeField]
        private Text titleText = null, messageText = null;

        [SerializeField]
        private string title = "Title", message = "Message";

        [SerializeField]
        private IntEvent onClosed = null;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                titleText.text = title;
            }
        }

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                messageText.text = message;
            }
        }

        /// <summary>
        /// Event raised everytime the alert is closed.
        /// 0: closed with <see cref="buttonA"/>
        /// 1: closed with <see cref="buttonB"/>
        /// 2: closed with <see cref="buttonC"/>
        /// -1: closed with <see cref="Hide()"/> method or if there was any error.
        /// </summary>
        public UnityEvent<ButtonName> OnClosed { get { return onClosed; } }

        public bool IsShowing { get { return isShowing; } }

        private const string defaultButtonText = "Ok";
        private bool isShowing;

        private void Awake()
        {
            titleText.text = Title;
            messageText.text = Message;

            buttonA.onClick.AddListener(() => Hide(ButtonName.ButtonA));
            buttonB.onClick.AddListener(() => Hide(ButtonName.ButtonB));
            buttonC.onClick.AddListener(() => Hide(ButtonName.ButtonC));
        }

        /// <summary>
        /// Show an alert popup with default title, message and 1 "Ok" button.
        /// </summary>
        public void Alert()
        {
            if (IsShowing)
                return;

            Alert(Title, Message, defaultButtonText);
        }

        /// <summary>
        /// Show an alert popup with specific title, message and 1 "Ok" button.
        /// </summary>
        public void Alert(string title, string message)
        {
            Alert(title, title, defaultButtonText);
        }

        /// <summary>
        /// Show an alert popup with specific title, message and 1 button with specific message.
        /// </summary>
        public void Alert(string title, string message, string buttonMessage)
        {
            if (IsShowing)
                return;

            isShowing = true;

            Title = title;
            Message = message;

            buttonAText.text = buttonMessage;
            buttonA.gameObject.SetActive(true);

            buttonB.gameObject.SetActive(false);
            buttonC.gameObject.SetActive(false);
            rootView.SetActive(true);
        }

        /// <summary>
        /// Show an alert popup with specific title, message and 2 buttons with specific messages.
        /// </summary>
        public void Alert(string title, string message, string buttonAMessage, string buttonBMessage)
        {
            if (IsShowing)
                return;

            Alert(title, message, buttonAMessage);

            buttonBText.text = buttonBMessage;
            buttonB.gameObject.SetActive(true);
        }

        /// <summary>
        /// Show an alert popup with specific title, message and 3 buttons with specific messages.
        /// </summary>
        public void Alert(string title, string message, string buttonAMessage, string buttonBMessage, string buttonCMessage)
        {
            if (IsShowing)
                return;

            Alert(title, message, buttonAMessage, buttonBMessage);

            buttonCText.text = buttonCMessage;
            buttonC.gameObject.SetActive(true);
        }

        public void Hide(bool keepListeners = false)
        {
            Hide(ButtonName.None, keepListeners);
        }

        private void Hide(ButtonName buttonName, bool keepListeners = false)
        {
            if (!IsShowing)
                return;

            isShowing = false;
            rootView.SetActive(false);

            if (!keepListeners)
                OnClosed.RemoveAllListeners();

            RaiseOnClosed(buttonName);
        }

        private void RaiseOnClosed(ButtonName buttonName)
        {
            if (onClosed != null)
                onClosed.Invoke(buttonName);
        }
    }
}
