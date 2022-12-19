using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class SupportsScreensElement : AndroidManifestElement
    {
        public SupportsScreensElement()
        {
            Style = AndroidManifestElementStyles.SupportsScreens; 
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
                yield return "android:resizeable";
                yield return "android:smallScreens";
                yield return "android:normalScreens";
                yield return "android:largeScreens";
                yield return "android:xlargeScreens";
                yield return "android:anyDensity";
                yield return "android:requiresSmallestWidthDp";
                yield return "android:compatibleWidthLimitDp";
                yield return "android:largestWidthLimitDp";
            }
        }
    }
}
