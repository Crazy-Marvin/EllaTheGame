using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    /// <summary>
    /// Base class for all the ads UI sections.
    /// </summary>
    [Serializable]
    public class AdsSection
    {
        [SerializeField]
        protected GameObject defaultRoot, customRoot;

        [SerializeField]
        protected Button enableDefaultSectionButton, enableCustomSectionButton;

        [SerializeField]
        protected bool useDefaultUIAsDefault = true;

        /// <summary>
        /// True: the default section is being displayed.
        /// False: the custom section is.
        /// </summary>
        public bool IsUsingDefaultSection { get; private set; }

        public virtual void Start()
        {
            enableDefaultSectionButton.onClick.AddListener(Switch);
            enableCustomSectionButton.onClick.AddListener(Switch);

            IsUsingDefaultSection = useDefaultUIAsDefault;

            Switch(IsUsingDefaultSection);
        }

        public void Switch()
        {
            Switch(!IsUsingDefaultSection);
        }

        public void Switch(bool isUsingDefaultSection)
        {
            IsUsingDefaultSection = isUsingDefaultSection;

            if (IsUsingDefaultSection)
            {
                ShowDefaultSection();
            }
            else
            {
                ShowCustomSection();
            }
        }

        protected virtual void ShowDefaultSection()
        {
            defaultRoot.gameObject.SetActive(true);
            customRoot.gameObject.SetActive(false);

            enableDefaultSectionButton.interactable = false;
            enableCustomSectionButton.interactable = true;
        }

        protected virtual void ShowCustomSection()
        {
            defaultRoot.gameObject.SetActive(false);
            customRoot.gameObject.SetActive(true);

            enableDefaultSectionButton.interactable = true;
            enableCustomSectionButton.interactable = false;
        }
    }
}
