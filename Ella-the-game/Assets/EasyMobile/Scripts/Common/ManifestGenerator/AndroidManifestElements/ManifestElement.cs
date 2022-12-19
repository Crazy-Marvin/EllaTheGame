using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class ManifestElement : AndroidManifestElement
    {
        public ManifestElement()
        {
            Style = AndroidManifestElementStyles.Manifest;
            AddAttribute("xmlns:android", "http://schemas.android.com/apk/res/android");
        }

        public override IEnumerable<AndroidManifestElementStyles> ParentStyles
        {
            get
            {
                yield break;
            }
        }

        public override IEnumerable<AndroidManifestElementStyles> ChildStyles
        {
            get
            {
                yield return AndroidManifestElementStyles.Application; // Must contain.
                yield return AndroidManifestElementStyles.Instrumentation;
                yield return AndroidManifestElementStyles.Permission;
                yield return AndroidManifestElementStyles.PermissionGroup;
                yield return AndroidManifestElementStyles.PermissionTree;
                yield return AndroidManifestElementStyles.SupportsGlTexture;
                yield return AndroidManifestElementStyles.SupportsScreens;
                yield return AndroidManifestElementStyles.UsesConfiguration;
                yield return AndroidManifestElementStyles.UsesFeature;
                yield return AndroidManifestElementStyles.UsesPermission;
                yield return AndroidManifestElementStyles.UsesPermissionSdk23;
                yield return AndroidManifestElementStyles.UsesSdk;
            }
        }

        public override IEnumerable<string> AllAvailableAttributes
        {
            get
            {
                yield return "xmlns:android";
                yield return "package";
                yield return "android:sharedUserId";
                yield return "android:targetSandboxVersion";
                yield return "android:sharedUserLabel";
                yield return "android:versionCode";
                yield return "android:versionName";
                yield return "android:installLocation";
            }
        }
    }
}
