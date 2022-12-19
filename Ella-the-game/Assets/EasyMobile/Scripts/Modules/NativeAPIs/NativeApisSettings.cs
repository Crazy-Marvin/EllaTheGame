using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile
{
    /// <summary>
    /// All settings related to native APIs.
    /// </summary>
    [Serializable]
    public class NativeApisSettings
    {
        [SerializeField]
        private bool mIsMediaEnabled = false;

        [SerializeField]
        private MediaApiSettings mMediaSettings = null;

        [SerializeField]
        private bool mIsContactsEnabled = false;

        [SerializeField]
        private ContactsApiSettings mContactsSettings = null;

        /// <summary>
        /// Is Media (Camera & Gallery) submodule enabled?
        /// </summary>
        public bool IsMediaEnabled { get { return mIsMediaEnabled; } }

        /// <summary>
        /// Gets the Media submodule settings.
        /// </summary>
        /// <value>The media settings.</value>
        public MediaApiSettings Media{ get { return mMediaSettings; } }

        /// <summary>
        /// Is the Contacts submodule enabled?
        /// </summary>
        public bool IsContactsEnabled { get { return mIsContactsEnabled; } }

        /// <summary>
        /// Gets the Contacts submodule settings.
        /// </summary>
        /// <value>The contacts settings.</value>
        public ContactsApiSettings Contacts { get { return mContactsSettings; } }
    }
}
