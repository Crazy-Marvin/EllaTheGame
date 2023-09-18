#if UNITY_IOS && EM_CONTACTS
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.CoreFoundation;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;

namespace EasyMobile.iOS.Contacts
{
    /// <summary>
    /// An immutable representation of the postal address for a contact.
    /// </summary>
    internal class CNPostalAddress : iOSObjectProxy
    {
        private NSString mStreet;
        private NSString mCity;
        private NSString mState;
        private NSString mPostalCode;
        private NSString mCountry;
        private NSString mISOCountryCode;
        private NSString mSubAdministrativeArea;
        private NSString mSubLocality;

        internal CNPostalAddress(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// The street name in a postal address.
        /// </summary>
        /// <value>The street.</value>
        public NSString Street
        {
            get
            {
                if (mStreet == null)
                {
                    var ptr = C.CNPostalAddress_street(SelfPtr());

                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mStreet = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mStreet;
            }
        }

        /// <summary>
        /// The city name in a postal address.
        /// </summary>
        /// <value>The city.</value>
        public NSString City
        {
            get
            {
                if (mCity == null)
                {
                    var ptr = C.CNPostalAddress_city(SelfPtr());

                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mCity = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mCity;
            }
        }

        /// <summary>
        /// The state name in a postal address.
        /// </summary>
        /// <value>The state.</value>
        public NSString State
        {
            get
            {
                if (mState == null)
                {
                    var ptr = C.CNPostalAddress_state(SelfPtr());

                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mState = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mState;
            }
        }

        /// <summary>
        /// The postal code in a postal address.
        /// </summary>
        /// <value>The postal code.</value>
        public NSString PostalCode
        {
            get
            {
                if (mPostalCode == null)
                {
                    var ptr = C.CNPostalAddress_postalCode(SelfPtr());

                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mPostalCode = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mPostalCode;
            }
        }

        /// <summary>
        /// The country name in a postal address.
        /// </summary>
        /// <value>The country.</value>
        public NSString Country
        {
            get
            {
                if (mCountry == null)
                {
                    var ptr = C.CNPostalAddress_country(SelfPtr());

                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mCountry = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mCountry;
            }
        }

        /// <summary>
        /// The ISO country code for the country in a postal address, using the ISO 3166-1 alpha-2 
        /// standard.
        /// </summary>
        /// <value>The ISO country code.</value>
        public NSString ISOCountryCode
        {
            get
            {
                if (mISOCountryCode == null)
                {
                    var ptr = C.CNPostalAddress_ISOCountryCode(SelfPtr());

                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mISOCountryCode = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mISOCountryCode;
            }
        }

        /// <summary>
        /// The subadministrative area (such as a county or other region) in a postal address.
        /// </summary>
        /// <value>The subadministrative areas.</value>
        public NSString SubAdministrativeArea
        {
            get
            {
                if (mSubAdministrativeArea == null)
                {
                    var ptr = C.CNPostalAddress_subAdministrativeArea(SelfPtr());

                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mSubAdministrativeArea = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mSubAdministrativeArea;
            }
        }

        /// <summary>
        /// Additional information associated with the location, typically defined at the city or town 
        /// level, in a postal address.
        /// </summary>
        /// <value>The additional information associated with the location.</value>
        public NSString SubLocality
        {
            get
            {
                if (mSubLocality == null)
                {
                    var ptr = C.CNPostalAddress_subLocality(SelfPtr());

                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mSubLocality = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mSubLocality;
            }
        }

        /// <summary>
        /// Returns the localized name for the property associated with the specified key.
        /// </summary>
        /// <returns>The string for key.</returns>
        /// <param name="key">Key.</param>
        public static NSString LocalizedStringForKey(NSString key)
        {
            if (key == null)
                return null;

            var ptr = C.CNPostalAddress_localizedStringForKey(key.ToPointer());
            NSString localizedStr = null;

            if (PInvokeUtil.IsNotNull(ptr))
            {
                localizedStr = new NSString(ptr);
                CFFunctions.CFRelease(ptr);
            }
            return localizedStr;
        }

#region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNPostalAddress_street(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNPostalAddress_city(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNPostalAddress_state(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNPostalAddress_postalCode(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNPostalAddress_country(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNPostalAddress_ISOCountryCode(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNPostalAddress_subAdministrativeArea(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNPostalAddress_subLocality(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNPostalAddress_localizedStringForKey(/* NSString */IntPtr key);

        }

#endregion
    }
}
#endif
