using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    [Serializable]
    public class UsesConfigurationElement : AndroidManifestElement
    {
        public UsesConfigurationElement()
        {
            Style = AndroidManifestElementStyles.UsesConfiguration;
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
                yield return "android:reqFiveWayNav";
                yield return "android:reqHardKeyboard";
                yield return "android:reqKeyboardType";
                yield return "android:reqNavigation";
                yield return "android:reqTouchScreen";
            }
        }
    }
}
