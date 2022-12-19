using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class MetaDataElement : AndroidManifestElement
    {
        public MetaDataElement()
        {
            Style = AndroidManifestElementStyles.MetaData;
        }

        public override IEnumerable<AndroidManifestElementStyles> ParentStyles
        {
            get
            {
                yield return AndroidManifestElementStyles.Activity;
                yield return AndroidManifestElementStyles.ActivityAlias;
                yield return AndroidManifestElementStyles.Application;
                yield return AndroidManifestElementStyles.Provider;
                yield return AndroidManifestElementStyles.Receiver;
                yield return AndroidManifestElementStyles.Service;
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
                yield return "android:resource";
                yield return "android:value";
            }
        }
    }
}
