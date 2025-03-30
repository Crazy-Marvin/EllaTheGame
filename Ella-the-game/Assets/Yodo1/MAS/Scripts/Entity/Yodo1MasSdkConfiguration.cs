using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yodo1.MAS
{
    /// <summary>
    /// This enum represents the user's geography used to determine the type of consent flow shown to the user.
    /// </summary>
    public enum Yodo1MasConsentFlowUserGeography
    {
        /// <summary>
        /// User's geography is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The user is in GDPR region.
        /// </summary>
        Gdpr = 1,

        /// <summary>
        /// The user is in a non-GDPR region.
        /// </summary>
        Other = 2,
    }

    /// <summary>
    /// MAS SDK-defined app tracking transparency status values (extended to include "unavailable" state on iOS before iOS14).
    /// </summary>
    public enum Yodo1MasAttTrackingStatus
    {
        /// <summary>
        /// Device is on iOS before iOS14, AppTrackingTransparency.framework is not available.
        /// </summary>
        SystemLow = -1,

        /// <summary>
        /// The user has not yet received an authorization request to authorize access to app-related data that can be used for tracking the user or the device.
        /// </summary>
        NotDetermined = 0,

        /// <summary>
        /// Authorization to access app-related data that can be used for tracking the user or the device is restricted.
        /// </summary>
        Restricted = 1,

        /// <summary>
        /// The user denies authorization to access app-related data that can be used for tracking the user or the device.
        /// </summary>
        Denied = 2,

        /// <summary>
        /// The user authorizes access to app-related data that can be used for tracking the user or the device.
        /// </summary>
        Authorized = 3,
    }

    /// <summary>
    /// This class contains various properties of the MAS SDK configuration.
    /// </summary>
    public class Yodo1MasSdkConfiguration
    {
        /// <summary>
        /// Get the user's geography used to determine the type of consent flow shown to the user.
        /// If no such determination could be made, `Yodo1MasConsentFlowUserGeography` will be returned.
        /// </summary>
        public Yodo1MasConsentFlowUserGeography ConsentFlowUserGeography { get; private set; }

        /// <summary>
        /// Get the user's age when you’re using age gate by MAS.
        /// </summary>
        public int UserAge { get; private set; }

        /// <summary>
        /// Indicates whether or not the user authorizes access to app-related data that can be used for tracking the user or the device.
        /// Note: Users can revoke permission at any time through the "Allow Apps To Request To Track" privacy setting on the device.
        /// </summary>
        public Yodo1MasAttTrackingStatus AttTrackingStatus { get; private set; }

        public Yodo1MasSdkConfiguration()
        {
            UserAge = 0;
            AttTrackingStatus = Yodo1MasAttTrackingStatus.NotDetermined;
            ConsentFlowUserGeography = Yodo1MasConsentFlowUserGeography.Unknown;
        }

        public static Yodo1MasSdkConfiguration CreateWithJson(string message)
        {
            Yodo1MasSdkConfiguration configuration = new Yodo1MasSdkConfiguration();
            Dictionary<string, object> dict = (Dictionary<string, object>)Yodo1JSON.Deserialize(message);
            if (dict != null)
            {
                if (dict.ContainsKey("userAge"))
                {
                    configuration.UserAge = int.Parse(dict["userAge"].ToString());
                }
                if (dict.ContainsKey("attrackingStatus"))
                {
                    configuration.AttTrackingStatus = (Yodo1MasAttTrackingStatus)int.Parse(dict["attrackingStatus"].ToString());
                }
                if (dict.ContainsKey("consentFlowUserGeography"))
                {
                    configuration.ConsentFlowUserGeography = (Yodo1MasConsentFlowUserGeography)int.Parse(dict["consentFlowUserGeography"].ToString());
                }
            }
            return configuration;
        }

        override
        public string ToString()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("userAge", UserAge);
            dict.Add("attrackingStatus", AttTrackingStatus);
            dict.Add("consentFlowUserGeography", ConsentFlowUserGeography);
            return Yodo1JSON.Serialize(dict);
        }
    }
}

