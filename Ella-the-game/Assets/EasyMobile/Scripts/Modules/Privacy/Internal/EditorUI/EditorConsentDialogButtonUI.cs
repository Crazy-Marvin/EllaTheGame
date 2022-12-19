using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EasyMobile.Internal.Privacy
{
    public class EditorConsentDialogButtonUI : MonoBehaviour
    {
        [SerializeField]
        private Button button = null;

        [SerializeField]
        private Text text = null;

        public Button Button { get { return button; } }

        public Text Text { get { return text; } }

        public bool Interactable
        {
            get { return button != null ? button.interactable : false; }
            set
            {
                if (button != null)
                    button.interactable = value;
            }
        }

        public void AddListener(UnityAction action)
        {
            if (action != null)
                button.onClick.AddListener(action);
        }

        public void UpdateText(string newText)
        {
            if (text != null && newText != null)
                text.text = newText;
        }

        public void UpdateBackgroundColor(Color color)
        {
            if (button != null)
                button.image.color = color;
        }

        public void UpdateTextColor(Color color)
        {
            if (text != null)
                text.color = color;
        }
    }
}
