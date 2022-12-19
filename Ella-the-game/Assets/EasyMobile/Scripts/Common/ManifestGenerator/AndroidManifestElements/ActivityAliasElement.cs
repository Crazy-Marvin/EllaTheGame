using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class ActivityAliasElement : AndroidManifestElement
    {
        public ActivityAliasElement()
        {
            Style = AndroidManifestElementStyles.ActivityAlias; 
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
                yield return "android:enabled";
                yield return "android:exported";
                yield return "android:icon";
                yield return "android:label";
                yield return "android:name";
                yield return "android:permission";
                yield return "android:targetActivity";
            }
        }
    }
}
