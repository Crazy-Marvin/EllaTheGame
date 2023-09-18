using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class ApplicationElement : AndroidManifestElement
    {
        public ApplicationElement()
        {
            Style = AndroidManifestElementStyles.Application;
        }

        public override IEnumerable<AndroidManifestElementStyles> ParentStyles
        {
            get
            {
                yield return AndroidManifestElementStyles.Manifest;
            }
        }

        public override IEnumerable<AndroidManifestElementStyles> ChildStyles
        {
            get
            {
                yield return AndroidManifestElementStyles.Activity;
                yield return AndroidManifestElementStyles.ActivityAlias;
                yield return AndroidManifestElementStyles.MetaData;
                yield return AndroidManifestElementStyles.Service;
                yield return AndroidManifestElementStyles.Receiver;
                yield return AndroidManifestElementStyles.Provider;
                yield return AndroidManifestElementStyles.UsesLibrary;
            }
        }

        public override IEnumerable<string> AllAvailableAttributes
        {
            get
            {
                yield return "android:allowTaskReparenting";
                yield return "android:allowBackup";
                yield return "android:allowClearUserData";
                yield return "android:backupAgent";
                yield return "android:backupInForeground";
                yield return "android:banner";
                yield return "android:debuggable";
                yield return "android:description";
                yield return "android:directBootAware";
                yield return "android:enabled";
                yield return "android:extractNativeLibs";
                yield return "android:fullBackupContent";
                yield return "android:fullBackupOnly";
                yield return "android:hasCode";
                yield return "android:hardwareAccelerated";
                yield return "android:icon";
                yield return "android:isGame";
                yield return "android:killAfterRestore";
                yield return "android:largeHeap";
                yield return "android:label";
                yield return "android:logo";
                yield return "android:manageSpaceActivity";
                yield return "android:name";
                yield return "android:networkSecurityConfig";
                yield return "android:permission";
                yield return "android:persistent";
                yield return "android:process";
                yield return "android:restoreAnyVersion";
                yield return "android:requiredAccountType";
                yield return "resizeableActivity";
                yield return "android:restrictedAccountType";
                yield return "android:supportsRtl";
                yield return "android:taskAffinity";
                yield return "android:testOnly";
                yield return "android:theme";
                yield return "android:uiOptions";
                yield return "android:usesCleartextTraffic";
                yield return "android:vmSafeMode";
            }
        }
    }
}
