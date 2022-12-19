using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class IntentFilterElement : AndroidManifestElement
    {
        public IntentFilterElement()
        {
            Style = AndroidManifestElementStyles.IntentFilter;
        }

        public override IEnumerable<AndroidManifestElementStyles> ParentStyles
        {
            get
            {
                yield return AndroidManifestElementStyles.Activity;
                yield return AndroidManifestElementStyles.ActivityAlias;
                yield return AndroidManifestElementStyles.Service;
                yield return AndroidManifestElementStyles.Receiver;
            }
        }

        public override IEnumerable<AndroidManifestElementStyles> ChildStyles
        {
            get
            {
                yield return AndroidManifestElementStyles.Action; // Must contain.
                yield return AndroidManifestElementStyles.Category;
                yield return AndroidManifestElementStyles.Data;
            }
        }

        public override IEnumerable<string> AllAvailableAttributes
        {
            get
            {
                yield return "android:icon";
                yield return "android:label";
                yield return "android:priority";
            }
        }
    }
}
