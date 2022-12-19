using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class ProviderElement : AndroidManifestElement
    {
        public ProviderElement()
        {
            Style = AndroidManifestElementStyles.Provider;
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
                yield return AndroidManifestElementStyles.MetaData;
                yield return AndroidManifestElementStyles.GrantUriPermission;
                yield return AndroidManifestElementStyles.PathPermission;
            }
        }

        public override IEnumerable<string> AllAvailableAttributes
        {
            get
            {
                yield return "android:authorities";
                yield return "android:enabled";
                yield return "android:directBootAware";
                yield return "android:exported";
                yield return "android:grantUriPermissions";
                yield return "android:icon";
                yield return "android:initOrder";
                yield return "android:label";
                yield return "android:multiprocess";
                yield return "android:name";
                yield return "android:permission";
                yield return "android:process";
                yield return "android:readPermission";
                yield return "android:syncable";
                yield return "android:writePermission";
            }
        }
    }
}
