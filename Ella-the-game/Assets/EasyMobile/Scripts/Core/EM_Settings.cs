using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using EasyMobile.Internal;

namespace EasyMobile
{
    public class EM_Settings : ScriptableObject
    {
        public static EM_Settings Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = LoadSettingsAsset();

                    if (sInstance == null)
                    {
                        #if !UNITY_EDITOR
                        Debug.LogError("Easy Mobile settings not found! " +
                            "Please go to menu Windows > Easy Mobile > Settings to setup the plugin.");
                        #endif
                        sInstance = CreateInstance<EM_Settings>();   // Create a dummy scriptable object for temporary use.
                    }
                }

                return sInstance;
            }
        }

        public static EM_Settings LoadSettingsAsset()
        {
            return Resources.Load("EM_Settings") as EM_Settings;
        }

        #region Module Settings

        public static AdSettings Advertising { get { return Instance.mAdvertisingSettings; } }

        public static GameServicesSettings GameServices { get { return Instance.mGameServiceSettings; } }

        public static IAPSettings InAppPurchasing { get { return Instance.mInAppPurchaseSettings; } }

        public static PrivacySettings Privacy { get { return Instance.mPrivacySettings; } }

        public static NotificationsSettings Notifications { get { return Instance.mNotificationSettings; } }

        public static SharingSettings Sharing { get { return Instance.mSharingSettings; } }

        #if EASY_MOBILE_PRO
        public static NativeApisSettings NativeApis { get { return Instance.mNativeApisSettings; } }
        #endif

        // Rating Request (Store Review) belongs to Utilities module.
        public static RatingRequestSettings RatingRequest { get { return Instance.mRatingRequestSettings; } }

        public static bool IsRuntimeAutoInitializationEnabled { get { return Instance.mRuntimeAutoInitialization; } }
        public static bool IsAdModuleEnable { get { return Instance.mIsAdModuleEnable; } }

        public static bool IsIAPModuleEnable { get { return Instance.mIsIAPModuleEnable; } }

        public static bool IsGameServicesModuleEnable { get { return Instance.mIsGameServiceModuleEnable; } }

        public static bool IsNotificationsModuleEnable { get { return Instance.mIsNotificationModuleEnable; } }

        public static bool IsCompositeModule(Module mod)
        {
            if (mod == Module.NativeApis || mod == Module.Utilities)
                return true;
            else
                return false;
        }

        public static bool IsModuleEnable(Module mod)
        {
            switch (mod)
            {
                case Module.Advertising:
                    return Instance.mIsAdModuleEnable;
                case Module.GameServices:
                    return Instance.mIsGameServiceModuleEnable;
                case Module.Gif:
                    return true;
                case Module.InAppPurchasing:
                    return Instance.mIsIAPModuleEnable;
                case Module.NativeApis:
                    return true;
                case Module.Notifications:
                    return Instance.mIsNotificationModuleEnable;
                case Module.Privacy:
                    return true;
                case Module.Sharing:
                    return Instance.mIsSharingModuleEnable;
                case Module.Utilities:
                    return true;
                default:
                    return false;   
            }
        }

        public static bool IsSubmoduleEnable(Submodule submod)
        {
            switch (submod)
            {
                case Submodule.Contacts:
                    #if EASY_MOBILE_PRO
                    return NativeApis.IsContactsEnabled;
                    #else
                    return false;
                    #endif
                case Submodule.Media:
                    #if EASY_MOBILE_PRO
                    return NativeApis.IsMediaEnabled;
                    #else
                    return false;
                    #endif
                case Submodule.RatingRequest:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Private members

        private static EM_Settings sInstance;

        [SerializeField]
        private AdSettings mAdvertisingSettings = null;
        [SerializeField]
        private GameServicesSettings mGameServiceSettings = null;
        [SerializeField]
        private IAPSettings mInAppPurchaseSettings = null;
        [SerializeField]
        private NotificationsSettings mNotificationSettings = null;
        [SerializeField]
        private PrivacySettings mPrivacySettings = null;
        [SerializeField]
        private RatingRequestSettings mRatingRequestSettings = null;
        [SerializeField]
        private SharingSettings mSharingSettings = null;

        #if EASY_MOBILE_PRO
        [SerializeField]
        private NativeApisSettings mNativeApisSettings = null;
        #endif

        [SerializeField] 
        private bool mRuntimeAutoInitialization = true;
        [SerializeField]
        private bool mIsAdModuleEnable = false;
        [SerializeField]
        private bool mIsIAPModuleEnable = false;
        [SerializeField]
        private bool mIsGameServiceModuleEnable = false;
        [SerializeField]
        private bool mIsNotificationModuleEnable = false;
        [SerializeField]
        private bool mIsSharingModuleEnable = false;

#if UNITY_EDITOR

        // These fields are only used as a SerializedProperty in the editor scripts, hence the warning suppression.
        #pragma warning disable 0414
        [SerializeField]
        private int mActiveModuleIndex = 0;
        // Index of the active module on the toolbar.
        [SerializeField]
        private bool mIsSelectingModule = true;
        #pragma warning restore 0414
       
        #endif

        #endregion
    }
}

