using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class InstrumentationElement : AndroidManifestElement
    {
        public InstrumentationElement()
        {
            Style = AndroidManifestElementStyles.Instrumentation;
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
                yield return "android:functionalTest";
                yield return "android:handleProfiling";
                yield return "android:icon";
                yield return "android:label";
                yield return "android:name";
                yield return "android:targetPackage";
                yield return "android:targetProcesses";
            }
        }
    }
}
