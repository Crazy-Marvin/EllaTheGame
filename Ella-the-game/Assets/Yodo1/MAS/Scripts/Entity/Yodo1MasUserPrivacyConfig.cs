using System.Collections.Generic;
using UnityEngine;

namespace Yodo1.MAS
{
    public class Yodo1MasUserPrivacyConfig
    {
        private Color defaultColor = new Color(0, 0, 0, 0);
        private Color _titleBackgroundColor;
        private Color _titleTextColor;
        private Color _contentBackgroundColor;
        private Color _contentTextColor;
        private Color _buttonBackgroundColor;
        private Color _buttonTextColor;

        public Yodo1MasUserPrivacyConfig titleBackgroundColor(Color color)
        {
            this._titleBackgroundColor = color;
            return this;
        }

        public Yodo1MasUserPrivacyConfig titleTextColor(Color color)
        {
            this._titleTextColor = color;
            return this;
        }

        public Yodo1MasUserPrivacyConfig contentBackgroundColor(Color color)
        {
            this._contentBackgroundColor = color;
            return this;
        }

        public Yodo1MasUserPrivacyConfig contentTextColor(Color color)
        {
            this._contentTextColor = color;
            return this;
        }

        public Yodo1MasUserPrivacyConfig buttonBackgroundColor(Color color)
        {
            this._buttonBackgroundColor = color;
            return this;
        }

        public Yodo1MasUserPrivacyConfig buttonTextColor(Color color)
        {
            this._buttonTextColor = color;
            return this;
        }

        public string toJson()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (_titleBackgroundColor.Equals(defaultColor))
            {
                dic.Add("titleBackgroundColor", string.Empty);
            }
            else
            {
                dic.Add("titleBackgroundColor", "#" + ColorUtility.ToHtmlStringRGB(_titleBackgroundColor));
            }

            if (_titleTextColor.Equals(defaultColor))
            {
                dic.Add("titleTextColor", string.Empty);
            }
            else
            {
                dic.Add("titleTextColor", "#" + ColorUtility.ToHtmlStringRGB(_titleTextColor));
            }

            if (_contentBackgroundColor.Equals(defaultColor))
            {
                dic.Add("contentBackgroundColor", string.Empty);
            }
            else
            {
                dic.Add("contentBackgroundColor", "#" + ColorUtility.ToHtmlStringRGB(_contentBackgroundColor));
            }

            if (_contentTextColor.Equals(defaultColor))
            {
                dic.Add("contentTextColor", string.Empty);
            }
            else
            {
                dic.Add("contentTextColor", "#" + ColorUtility.ToHtmlStringRGB(_contentTextColor));
            }

            if (_buttonBackgroundColor.Equals(defaultColor))
            {
                dic.Add("buttonBackgroundColor", string.Empty);
            }
            else
            {
                dic.Add("buttonBackgroundColor", "#" + ColorUtility.ToHtmlStringRGB(_buttonBackgroundColor));
            }

            if (_buttonTextColor.Equals(defaultColor))
            {
                dic.Add("buttonTextColor", string.Empty);
            }
            else
            {
                dic.Add("buttonTextColor", "#" + ColorUtility.ToHtmlStringRGB(_buttonTextColor));
            }
            return Yodo1JSON.Serialize(dic);
        }
    }
}
