namespace Yodo1.MAS
{
    using System.Collections.Generic;

    public class Yodo1U3dAdValue
    {
        public string NetworkName { get; private set; }

        /// <summary>
        /// The precision of the revenue value for this ad.
        ///
        /// Possible values are:
        /// - "publisher_defined" - If the revenue is the price assigned to the line item by the publisher.
        /// - "exact" - If the revenue is the resulting price of a real-time auction.
        /// - "estimated" - If the revenue is the price obtained by auto-CPM.
        /// - "undefined" - If we do not have permission from the ad network to share impression-level data.
        /// - "" - An empty string, if revenue and precision are not valid (for example, in test mode).
        /// </summary>
        public string RevenuePrecision { get; private set; }


        /// <summary>
        /// The ad's value.
        /// </summary>
        public double Revenue { get; private set; }

        /// <summary>
        /// The value's currency code.
        /// </summary>
        public string Currency { get; private set; }


        public Yodo1U3dAdValue(Dictionary<string, object> dict)
        {
            if (dict.ContainsKey("networkName"))
            {
                NetworkName = dict["networkName"].ToString();
            }
            if (dict.ContainsKey("revenuePrecision"))
            {
                RevenuePrecision = dict["revenuePrecision"].ToString();
            }

            if (dict.ContainsKey("revenue"))
            {
                Revenue = Yodo1U3dAdUtils.GetDoubleFromDictionary(dict, "revenue");
            }

            if (dict.ContainsKey("currency"))
            {
                Currency = dict["currency"].ToString();
            }
        }

        public override string ToString()
        {
            return "[AdValue networkName: " + NetworkName +
                   ", revenue: " + Revenue +
                   ", revenuePrecision: " + RevenuePrecision +
                   ", currency: " + Currency + "]";
        }

        public static Yodo1U3dAdValue CreateWithJson(string adValueString)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)Yodo1JSON.Deserialize(adValueString);
            Yodo1U3dAdValue adValue = new Yodo1U3dAdValue(dict);
            return adValue;
        }
    }
}