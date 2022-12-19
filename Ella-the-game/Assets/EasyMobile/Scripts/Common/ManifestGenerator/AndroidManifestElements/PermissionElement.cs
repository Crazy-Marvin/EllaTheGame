using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class PermissionElement : AndroidManifestElement
    {
        public PermissionElement()
        {
            Style = AndroidManifestElementStyles.Permission;
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
                yield return "android:description";
                yield return "android:icon";
                yield return "android:label";
                yield return "android:name";
                yield return "android:permissionGroup";
                yield return "android:protectionLevel";
            }
        }
    }
}
