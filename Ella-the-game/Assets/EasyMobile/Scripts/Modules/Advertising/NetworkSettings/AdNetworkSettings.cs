using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    public class AdNetworkSettings
    {
        [SerializeField]
        private bool mEnable;
        /// <summary>
        /// Check if ad module is enabled
        /// </summary>
        public bool Enable
        {
            get { return mEnable; }
            set { mEnable = value; }
        }
    }
}