using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile
{
    [Serializable]
    public class PrivacySettings
    {
        /// <summary>
        /// Gets the default consent dialog.
        /// </summary>
        /// <value>The default consent dialog.</value>
        public ConsentDialog DefaultConsentDialog { get { return mDefaultConsentDialog; } }

        [SerializeField]
        private ConsentDialog mDefaultConsentDialog = null;

        [SerializeField]
        private ConsentDialogComposerSettings mConsentDialogComposerSettings;
    }

    [Serializable]
    public class ConsentDialogComposerSettings
    {
        [SerializeField]
        private int mToggleSelectedIndex;
        [SerializeField]
        private int mButtonSelectedIndex;
        [SerializeField]
        private bool mEnableCopyPasteMode;
    }
}
