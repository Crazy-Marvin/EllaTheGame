using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class UsesSdkElement : AndroidManifestElement
    {
        public UsesSdkElement()
        {
            Style = AndroidManifestElementStyles.UsesSdk;
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
                yield break;
            }
        }

        public override IEnumerable<string> AllAvailableAttributes
        {
            get
            {
                yield return "android:minSdkVersion";
                yield return "android:targetSdkVersion";
                yield return "android:maxSdkVersion";
            }
        }
    }
}
