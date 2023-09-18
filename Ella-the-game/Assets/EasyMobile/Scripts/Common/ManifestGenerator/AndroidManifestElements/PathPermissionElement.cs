using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class PathPermissionElement : AndroidManifestElement
    {
        public PathPermissionElement()
        {
            Style = AndroidManifestElementStyles.PathPermission;
        }

        public override IEnumerable<AndroidManifestElementStyles> ParentStyles
        {
            get
            {
                yield return AndroidManifestElementStyles.Provider;
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
                yield return "android:path";
                yield return "android:pathPrefix";
                yield return "android:pathPattern";
                yield return "android:permission";
                yield return "android:readPermission";
                yield return "android:writePermission";
            }
        }
    }
}
