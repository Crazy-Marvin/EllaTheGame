using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class UsesPermissionElement : AndroidManifestElement
    {
        public UsesPermissionElement()
        {
            Style = AndroidManifestElementStyles.UsesPermission; 
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
                yield return "android:name";
                yield return "android:maxSdkVersion";
            }
        }
    }
}
