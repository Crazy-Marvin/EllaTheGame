#if UNITY_IOS && EM_CONTACTS
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.CoreFoundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EasyMobile.iOS.Contacts
{
    /// <summary>
    /// A thread-safe class that represents an immutable value object for contact properties, 
    /// such as the first name and phone numbers of a contact.
    /// </summary>
    internal class CNContact : iOSObjectProxy
    {
        /// <summary>
        /// The types a contact can be.
        /// </summary>
        public enum CNContactType
        {
            /// <summary>
            ///The contact is a person. 
            /// </summary>
            Person = 0,
            /// <summary>
            /// The contact is an Organization.
            /// </summary>
            Organization
        }

        protected NSString mIdentifier;
        protected CNContactType? mContactType;
        protected NSDateComponents mBirthday;
        protected NSDateComponents mNonGregorianBirthday;
        protected NSString mNamePrefix;
        protected NSString mGivenName;
        protected NSString mMiddleName;
        protected NSString mFamilyName;
        protected NSString mNameSuffix;
        protected NSString mNickname;
        protected NSString mOrganizationName;
        protected NSString mDepartmentName;
        protected NSString mJobTitle;
        protected NSArray<CNLabeledValue<CNPhoneNumber>> mPhoneNumbers;
        protected NSArray<CNLabeledValue<NSString>> mUrlAddresses;
        protected NSArray<CNLabeledValue<CNPostalAddress>> mPostalAddresses;
        protected NSArray<CNLabeledValue<NSString>> mEmailAddresses;
        protected NSString mNote;
        protected NSData mImageData;
        protected NSData mThumbnailImageData;
        protected bool? mImageDataAvailable;

        internal CNContact(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// A value that uniquely identifies a contact on the device.
        /// </summary>
        /// <value>The identifier.</value>
        public NSString Identifier
        {
            get
            {
                if (mIdentifier == null)
                {
                    var ptr = C.CNContact_identifier(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mIdentifier = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mIdentifier;
            }
        }

        /// <summary>
        /// An enum identifying the contact type.
        /// </summary>
        /// <value>The type of the contact.</value>
        public CNContactType ContactType
        {
            get
            {
                if (mContactType == null)
                    mContactType = C.CNContact_contactType(SelfPtr());

                return mContactType.Value;
            }
        }

        /// <summary>
        /// A date component for the non-Gregorian birthday of the contact.
        /// </summary>
        /// <value>The birthday.</value>
        public NSDateComponents Birthday
        {
            get
            {
                if (mBirthday == null)
                {
                    var ptr = C.CNContact_birthday(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mBirthday = new NSDateComponents(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mBirthday;
            }
        }

        /// <summary>
        /// A date component for the Gregorian birthday of the contact.
        /// </summary>
        /// <value>The birthday.</value>
        public NSDateComponents NonGregorianBirthday
        {
            get
            {
                if (mNonGregorianBirthday == null)
                {
                    var ptr = C.CNContact_nonGregorianBirthday(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mNonGregorianBirthday = new NSDateComponents(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mNonGregorianBirthday;
            }
        }

        /// <summary>
        /// The name prefix of the contact.
        /// </summary>
        /// <value>The name prefix.</value>
        public NSString NamePrefix
        {
            get
            {
                if (mNamePrefix == null)
                {
                    var ptr = C.CNContact_namePrefix(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mNamePrefix = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mNamePrefix;
            }
        }

        /// <summary>
        /// The given name of the contact.
        /// </summary>
        /// <value>The given name.</value>
        public NSString GivenName
        {
            get
            {
                if (mGivenName == null)
                {
                    var ptr = C.CNContact_givenName(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mGivenName = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mGivenName;
            }
        }


        /// <summary>
        /// The middle name of the contact.
        /// </summary>
        /// <value>The middle name.</value>
        public NSString MiddleName
        {
            get
            {
                if (mMiddleName == null)
                {
                    var ptr = C.CNContact_middleName(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mMiddleName = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mMiddleName;
            }
        }

        /// <summary>
        /// The family name of the contact.
        /// </summary>
        /// <value>The family name.</value>
        public NSString FamilyName
        {
            get
            {
                if (mFamilyName == null)
                {
                    var ptr = C.CNContact_familyName(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mFamilyName = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mFamilyName;
            }
        }

        /// <summary>
        /// The name suffix of the contact.
        /// </summary>
        /// <value>The name suffix.</value>
        public NSString NameSuffix
        {
            get
            {
                if (mNameSuffix == null)
                {
                    var ptr = C.CNContact_nameSuffix(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mNameSuffix = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mNameSuffix;
            }
        }

        /// <summary>
        /// The nickname of the contact.
        /// </summary>
        /// <value>The nick name.</value>
        public NSString NickName
        {
            get
            {
                if (mNickname == null)
                {
                    var ptr = C.CNContact_nickname(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mNickname = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mNickname;
            }
        }

        /// <summary>
        /// The name of the organization associated with the contact.
        /// </summary>
        /// <value>The organization name.</value>
        public NSString OrganizationName
        {
            get
            {
                if (mOrganizationName == null)
                {
                    var ptr = C.CNContact_organizationName(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mOrganizationName = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mOrganizationName;
            }
        }

        /// <summary>
        /// The name of the department associated with the contact.
        /// </summary>
        /// <value>The department name.</value>
        public NSString DepartmentName
        {
            get
            {
                if (mDepartmentName == null)
                {
                    var ptr = C.CNContact_departmentName(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mDepartmentName = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mDepartmentName;
            }
        }

        /// <summary>
        /// The contact’s job title.
        /// </summary>
        /// <value>The job title.</value>
        public NSString JobTitle
        {
            get
            {
                if (mJobTitle == null)
                {
                    var ptr = C.CNContact_jobTitle(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mJobTitle = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mJobTitle;
            }
        }

        /// <summary>
        /// Returns a string containing the localized contact property name.
        /// </summary>
        /// <returns>The localized string.</returns>
        /// <param name="key">A string containing the contact property key.</param>
        public NSString LocalizedStringForKey(NSString key)
        {
            if (key == null)
                return null;

            NSString localizedStr = null; 
            var ptr = C.CNContact_localizedStringForKey(key.ToPointer());
            if (PInvokeUtil.IsNotNull(ptr))
            {
                localizedStr = new NSString(ptr);
                CFFunctions.CFRelease(ptr);
            }
            return localizedStr;
        }

        /// <summary>
        /// An array of labeled phone numbers for a contact.
        /// </summary>
        /// <value>The array of labeled phone numbers.</value>
        public NSArray<CNLabeledValue<CNPhoneNumber>> PhoneNumbers
        {
            get
            {
                if (mPhoneNumbers == null)
                {
                    var ptr = C.CNContact_phoneNumbers(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mPhoneNumbers = new NSArray<CNLabeledValue<CNPhoneNumber>>(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mPhoneNumbers;
            }
        }

        /// <summary>
        /// An array of labeled URL addresses for a contact.
        /// </summary>
        /// <value>The array of labeled URL addresses.</value>
        public NSArray<CNLabeledValue<NSString>> UrlAddresses
        {
            get
            {
                if (mUrlAddresses == null)
                {
                    var ptr = C.CNContact_urlAddresses(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mUrlAddresses = new NSArray<CNLabeledValue<NSString>>(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mUrlAddresses;
            }
        }

        /// <summary>
        /// An array of labeled postal addresses for a contact.
        /// </summary>
        /// <value>The array of labeled postal addresses</value>
        public NSArray<CNLabeledValue<CNPostalAddress>> PostalAddresses
        {
            get
            {
                if (mPostalAddresses == null)
                {
                    var ptr = C.CNContact_postalAddresses(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mPostalAddresses = new NSArray<CNLabeledValue<CNPostalAddress>>(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mPostalAddresses;
            }
        }

        /// <summary>
        /// An array of labeled email addresses for the contact.
        /// </summary>
        /// <value>The array of labeled email addresses.</value>
        public NSArray<CNLabeledValue<NSString>> EmailAddresses
        {
            get
            {
                if (mEmailAddresses == null)
                {
                    var ptr = C.CNContact_emailAddresses(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mEmailAddresses = new NSArray<CNLabeledValue<NSString>>(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mEmailAddresses;
            }
        }

        /// <summary>
        /// A string containing notes for the contact.
        /// </summary>
        /// <value>The notes for the contact.</value>
        public NSString Note
        {
            get
            {
                if (mNote == null)
                {
                    var ptr = C.CNContact_note(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mNote = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mNote;
            }
        }

        /// <summary>
        /// The profile picture of a contact.
        /// </summary>
        /// <value>The image data.</value>
        public NSData ImageData
        {
            get
            {
                if (mImageData == null)
                {
                    var ptr = C.CNContact_imageData(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mImageData = new NSData(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mImageData;
            }
        }

        /// <summary>
        /// The thumbnail version of the contact’s profile picture.
        /// </summary>
        /// <value>The image data.</value>
        public NSData ThumbnailImageData
        {
            get
            {
                if (mThumbnailImageData == null)
                {
                    var ptr = C.CNContact_thumbnailImageData(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mThumbnailImageData = new NSData(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mThumbnailImageData;
            }
        }

        /// <summary>
        /// Indicates whether a contact has a profile picture.
        /// </summary>
        /// <value>The availability of the profile picture</value>
        public bool ImageDataAvailable
        {
            get
            { 
                if (mImageDataAvailable == null)
                    mImageDataAvailable = C.CNContact_imageDataAvailable(SelfPtr()); 
                return mImageDataAvailable.Value;
            }
        }

        /// <summary>
        /// Returns a new instance that’s a mutable copy of the receiver.
        /// </summary>
        /// <returns>The copy.</returns>
        public CNMutableContact MutableCopy()
        {
            CNMutableContact mutCopy = null;
            var ptr = C.CNContact_mutableCopy(SelfPtr());
            if (PInvokeUtil.IsNotNull(ptr))
            {
                mutCopy = new CNMutableContact(ptr);
                CFFunctions.CFRelease(ptr);
            }
            return mutCopy;
        }

#region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContact_identifier(HandleRef self);

            [DllImport("__Internal")]
            internal static extern CNContactType CNContact_contactType(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSDateComponents */ IntPtr CNContact_birthday(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSDateComponents */ IntPtr CNContact_nonGregorianBirthday(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContact_namePrefix(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContact_givenName(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContact_middleName(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContact_familyName(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContact_nameSuffix(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContact_nickname(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContact_organizationName(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContact_departmentName(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContact_jobTitle(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContact_localizedStringForKey(/* NSString */IntPtr key);

            [DllImport("__Internal")]
            internal static extern /* NSArray<CNLabeledValue<CNPhoneNumber *> *>  */ IntPtr CNContact_phoneNumbers(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSArray<CNLabeledValue<NSString *> *>  */ IntPtr CNContact_urlAddresses(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSArray<CNLabeledValue<CNPostalAddress *> *>  */ IntPtr CNContact_postalAddresses(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSArray<CNLabeledValue<NSString *> *>  */ IntPtr CNContact_emailAddresses(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr CNContact_note(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSData  */IntPtr CNContact_imageData(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSData  */IntPtr CNContact_thumbnailImageData(HandleRef self);

            [DllImport("__Internal")]
            internal static extern bool CNContact_imageDataAvailable(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* CNMutableContact */IntPtr CNContact_mutableCopy(HandleRef self);
        }

#endregion

    }
}
#endif