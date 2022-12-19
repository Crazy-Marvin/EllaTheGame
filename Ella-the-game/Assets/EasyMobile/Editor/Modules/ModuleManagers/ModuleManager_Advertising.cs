using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace EasyMobile.Editor
{
    internal class ModuleManager_Advertising : ModuleManager
    {
        #region Singleton

        private static ModuleManager_Advertising sInstance;

        private ModuleManager_Advertising()
        {
        }

        public static ModuleManager_Advertising Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new ModuleManager_Advertising();
                return sInstance;
            }
        }

        #endregion

        #region implemented abstract members of ModuleManager

        protected override void InternalEnableModule()
        {
            // Check ad network plugins' availability and define appropriate scripting symbols.
            // Note that UnityAds symbol is added automatically by Unity engine.
            List<string> symbols = new List<string>();

            // AdColony
            if (EM_ExternalPluginManager.IsAdColonyAvail())
            {
                symbols.Add(EM_ScriptingSymbols.AdColony);
            }

            // AdMob
            if (EM_ExternalPluginManager.IsAdMobAvail())
            {
                symbols.Add(EM_ScriptingSymbols.AdMob);
            }

            // AdMob
            if (EM_ExternalPluginManager.IsAppLovinAvail())
            {
                symbols.Add(EM_ScriptingSymbols.AppLovin);
            }

            // FB Audience
            if (EM_ExternalPluginManager.IsFBAudienceAvail())
            {
                symbols.Add(EM_ScriptingSymbols.FBAudience);
            }

            // Chartboost
            if (EM_ExternalPluginManager.IsChartboostAvail())
            {
                symbols.Add(EM_ScriptingSymbols.Chartboost);
            }

            // FairBid
            if (EM_ExternalPluginManager.IsFairBidAvail())
            {
                symbols.Add(EM_ScriptingSymbols.FairBid);
            }

            // IronSource
            if (EM_ExternalPluginManager.IsIronSourceAvail())
            {
                symbols.Add(EM_ScriptingSymbols.IronSource);
            }

            // MoPub
            if (EM_ExternalPluginManager.IsMoPubAvail())
            {
                symbols.Add(EM_ScriptingSymbols.MoPub);
            }

            // TapJoy
            if (EM_ExternalPluginManager.IsTapJoyAvail())
            {
                symbols.Add(EM_ScriptingSymbols.TapJoy);
            }

            // Unity Monetization
            if (EM_ExternalPluginManager.IsUnityMonetizationAvail())
            {
                symbols.Add(EM_ScriptingSymbols.UnityMonetization);
            }

            // Unity Ads
            if (EM_ExternalPluginManager.IsUnityAdAvail())
            {
                symbols.Add(EM_ScriptingSymbols.UnityAds);
            }

            // Vungle
            if (EM_ExternalPluginManager.IsVungleAvail())
            {
                symbols.Add(EM_ScriptingSymbols.Vungle);
            }

            // Defines all ad symbols on all platforms.
            GlobalDefineManager.SDS_AddDefinesOnAllPlatforms(symbols.ToArray());
        }

        protected override void InternalDisableModule()
        {
            // Remove associated scripting symbols on all platforms if any was defined on that platform.
            GlobalDefineManager.SDS_RemoveDefinesOnAllPlatforms(
                new string[]
                {
                    EM_ScriptingSymbols.AdColony,
                    EM_ScriptingSymbols.AdMob,
                    EM_ScriptingSymbols.AppLovin,
                    EM_ScriptingSymbols.FBAudience,
                    EM_ScriptingSymbols.Chartboost,
                    EM_ScriptingSymbols.FairBid,
                    EM_ScriptingSymbols.IronSource,
                    EM_ScriptingSymbols.MoPub,
                    EM_ScriptingSymbols.TapJoy,
                    EM_ScriptingSymbols.UnityMonetization
                });
        }

        public override List<string> AndroidManifestTemplatePaths
        {
            get
            {
                return null;
            }
        }

        public override IAndroidPermissionRequired AndroidPermissionsHolder
        {
            get
            {
                return EM_Settings.Advertising as IAndroidPermissionRequired;
            }
        }

        public override IIOSInfoItemRequired iOSInfoItemsHolder
        {
            get
            {
                return EM_Settings.Advertising as IIOSInfoItemRequired;
            }
        }

        public override Module SelfModule
        {
            get
            {
                return Module.Advertising;
            }
        }

        #endregion
    }
}
