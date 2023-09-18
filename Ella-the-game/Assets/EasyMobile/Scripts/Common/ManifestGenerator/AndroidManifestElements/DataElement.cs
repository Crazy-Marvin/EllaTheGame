using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class DataElement : AndroidManifestElement
    {
        public DataElement()
        {
            Style = AndroidManifestElementStyles.Data;
        }

        public override IEnumerable<AndroidManifestElementStyles> ParentStyles
        {
            get
            {
                yield return AndroidManifestElementStyles.IntentFilter;
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
                yield return "android:host";
                yield return "android:port";
                yield return "android:path";
                yield return "android:pathPrefix";
                yield return "android:pathPattern";
            }
        }
    }
}
