using System.Collections.Generic;

namespace Yodo1.MAS
{
    public class Yodo1AdBuildConfig
    {
        private bool _enableAdaptiveBanner;
        private bool _enableUserPrivacyDialog;
        private string _userAgreementUrl;
        private string _privacyPolicyUrl;

        public Yodo1AdBuildConfig enableAdaptiveBanner(bool adaptiveBanner)
        {
            this._enableAdaptiveBanner = adaptiveBanner;
            return this;
        }

        public Yodo1AdBuildConfig enableUserPrivacyDialog(bool userPrivacyDialog)
        {
            this._enableUserPrivacyDialog = userPrivacyDialog;
            return this;
        }

        public Yodo1AdBuildConfig userAgreementUrl(string url)
        {
            this._userAgreementUrl = url;
            return this;
        }

        public Yodo1AdBuildConfig privacyPolicyUrl(string url)
        {
            this._privacyPolicyUrl = url;
            return this;
        }

        public string toJson()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("enableAdaptiveBanner", _enableAdaptiveBanner);
            dic.Add("enableUserPrivacyDialog", _enableUserPrivacyDialog);
            if (string.IsNullOrEmpty(_userAgreementUrl))
            {
                dic.Add("userAgreementUrl", "");
            }
            else
            {
                dic.Add("userAgreementUrl", _userAgreementUrl);
            }

            if (string.IsNullOrEmpty(_privacyPolicyUrl))
            {
                dic.Add("privacyPolicyUrl", "");
            }
            else
            {
                dic.Add("privacyPolicyUrl", _privacyPolicyUrl);
            }
            return Yodo1JSON.Serialize(dic);
        }
    }
}



