using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class ActivityElement : AndroidManifestElement
    {
        public ActivityElement()
        {
            Style = AndroidManifestElementStyles.Activity;
        }

        public override IEnumerable<AndroidManifestElementStyles> ParentStyles
        {
            get
            {
                yield return AndroidManifestElementStyles.Application;
            }
        }

        public override IEnumerable<AndroidManifestElementStyles> ChildStyles
        {
            get
            {
                yield return AndroidManifestElementStyles.IntentFilter;
                yield return AndroidManifestElementStyles.MetaData;
            }
        }

        public override IEnumerable<string> AllAvailableAttributes
        {
            get
            {
                yield return "android:allowEmbedded";
                yield return "android:allowTaskReparenting";
                yield return "android:alwaysRetainTaskState";
                yield return "android:autoRemoveFromRecents";
                yield return "android:banner";
                yield return "android:clearTaskOnLaunch";
                yield return "android:colorMode";
                yield return "android:configChanges";
                yield return "android:directBootAware";
                yield return "android:documentLaunchMode";
                yield return "android:enabled";
                yield return "android:excludeFromRecents";
                yield return "android:exported";
                yield return "android:finishOnTaskLaunch";
                yield return "android:hardwareAccelerated";
                yield return "android:icon";
                yield return "android:label";
                yield return "android:launchMode";
                yield return "android:maxRecents";
                yield return "android:maxAspectRatio";
                yield return "android:multiprocess";
                yield return "android:name";
                yield return "android:noHistory";
                yield return "android:parentActivityName";
                yield return "android:persistableMode";
                yield return "android:permission";
                yield return "android:process";
                yield return "android:relinquishTaskIdentity";
                yield return "resizeableActivity";
                yield return "android:screenOrientation";
                yield return "android:showForAllUsers";
                yield return "android:stateNotNeeded";
                yield return "supportsPictureInPicture";
                yield return "android:taskAffinity";
                yield return "android:theme";
                yield return "android:uiOptions";
                yield return "android:windowSoftInputMode";
            }
        }
    }
}
