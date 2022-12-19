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
    /// A mutable value object for the contact properties, such as the first name and the phone number of a contact.
    /// </summary>
    internal class CNMutableContact : CNContact
    {
        internal CNMutableContact(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        public CNMutableContact()
            : base(C.CNMutableContact_new())
        {
            CFFunctions.CFRelease(SelfPtr());
        }

        /// <summary>
        /// An enum identifying the contact type.
        /// </summary>
        /// <value>The type of the contact.</value>
        public new CNContactType ContactType
        {
            get
            {
                return base.ContactType;
            }
            set
            {
                mContactType = value;
                C.CNMutableContact_setContactType(SelfPtr(), mContactType.Value);
            }
        }

        /// <summary>
        /// A date component for the non-Gregorian birthday of the contact.
        /// </summary>
        /// <value>The birthday.</value>
        public new NSDateComponents Birthday
        {
            get
            {
                return base.Birthday;
            }
            set
            {
                mBirthday = value;
                C.CNMutableContact_setBirthday(SelfPtr(), mBirthday != null ? mBirthday.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// A date component for the Gregorian birthday of the contact.
        /// </summary>
        /// <value>The birthday.</value>
        public new NSDateComponents NonGregorianBirthday
        {
            get
            {
                return base.NonGregorianBirthday;   
            }
            set
            {
                mNonGregorianBirthday = value;
                C.CNMutableContact_setNonGregorianBirthday(SelfPtr(), mNonGregorianBirthday != null ? mNonGregorianBirthday.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// The name prefix of the contact.
        /// </summary>
        /// <value>The name prefix.</value>
        public new NSString NamePrefix
        {
            get
            {
                return base.NamePrefix;
            }
            set
            {
                mNamePrefix = value;
                C.CNMutableContact_setNamePrefix(SelfPtr(), mNamePrefix != null ? mNamePrefix.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// The given name of the contact.
        /// </summary>
        /// <value>The given name.</value>
        public new NSString GivenName
        {
            get
            {
                return base.GivenName;
            }
            set
            {
                mGivenName = value;
                C.CNMutableContact_setGivenName(SelfPtr(), mGivenName != null ? mGivenName.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// The middle name of the contact.
        /// </summary>
        /// <value>The middle name.</value>
        public new NSString MiddleName
        {
            get
            {
                return base.MiddleName;
            }
            set
            {
                mMiddleName = value;
                C.CNMutableContact_setMiddleName(SelfPtr(), mMiddleName != null ? mMiddleName.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// The family name of the contact.
        /// </summary>
        /// <value>The family name.</value>
        public new NSString FamilyName
        {
            get
            {
                return base.FamilyName;
            }
            set
            {
                mFamilyName = value;
                C.CNMutableContact_setFamilyName(SelfPtr(), mFamilyName != null ? mFamilyName.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// The name suffix of the contact.
        /// </summary>
        /// <value>The name suffix.</value>
        public new NSString NameSuffix
        {
            get
            {
                return base.NameSuffix;
            }
            set
            {
                mNameSuffix = value;
                C.CNMutableContact_setNameSuffix(SelfPtr(), mNameSuffix != null ? mNameSuffix.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// The nickname of the contact.
        /// </summary>
        /// <value>The nick name.</value>
        public new NSString NickName
        {
            get
            {
                return base.NickName;
            }
            set
            {
                mNickname = value;
                C.CNMutableContact_setNickname(SelfPtr(), mNickname != null ? mNickname.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// The name of the organization associated with the contact.
        /// </summary>
        /// <value>The organization name.</value>
        public new NSString OrganizationName
        {
            get
            {
                return base.OrganizationName;
            }
            set
            {
                mOrganizationName = value;
                C.CNMutableContact_setOrganizationName(SelfPtr(), mOrganizationName != null ? mOrganizationName.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// The name of the department associated with the contact.
        /// </summary>
        /// <value>The department name.</value>
        public new NSString DepartmentName
        {
            get
            {
                return base.DepartmentName;
            }
            set
            {
                mDepartmentName = value;
                C.CNMutableContact_setDepartmentName(SelfPtr(), mDepartmentName != null ? mDepartmentName.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// The contact’s job title.
        /// </summary>
        /// <value>The job title.</value>
        public new NSString JobTitle
        {
            get
            {
                return base.JobTitle;
            }
            set
            {
                mJobTitle = value;
                C.CNMutableContact_setJobTitle(SelfPtr(), mJobTitle != null ? mJobTitle.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// An array of labeled phone numbers for a contact.
        /// </summary>
        /// <value>The array of labeled phone numbers.</value>
        public new NSArray<CNLabeledValue<CNPhoneNumber>> PhoneNumbers
        {
            get
            {
                return base.PhoneNumbers;
            }
            set
            {
                mPhoneNumbers = value;
                C.CNMutableContact_setPhoneNumbers(SelfPtr(), mPhoneNumbers != null ? mPhoneNumbers.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// An array of labeled URL addresses for a contact.
        /// </summary>
        /// <value>The array of labeled URL addresses.</value>
        public new NSArray<CNLabeledValue<NSString>> UrlAddresses
        {
            get
            {
                return base.UrlAddresses;
            }
            set
            {
                mUrlAddresses = value;
                C.CNMutableContact_setUrlAddresses(SelfPtr(), mUrlAddresses != null ? mUrlAddresses.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// An array of labeled postal addresses for a contact.
        /// </summary>
        /// <value>The array of labeled postal addresses</value>
        public new NSArray<CNLabeledValue<CNPostalAddress>> PostalAddresses
        {
            get
            {
                return base.PostalAddresses;
            }
            set
            {
                mPostalAddresses = value;
                C.CNMutableContact_setPostalAddresses(SelfPtr(), mPostalAddresses != null ? mPostalAddresses.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// An array of labeled email addresses for the contact.
        /// </summary>
        /// <value>The array of labeled email addresses.</value>
        public new NSArray<CNLabeledValue<NSString>> EmailAddresses
        {
            get
            {
                return base.EmailAddresses;
            }
            set
            {
                mEmailAddresses = value;
                C.CNMutableContact_setEmailAddresses(SelfPtr(), mEmailAddresses != null ? mEmailAddresses.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// A string containing notes for the contact.
        /// </summary>
        /// <value>The notes for the contact.</value>
        public new NSString Note
        {
            get
            {
                return base.Note;
            }
            set
            {
                mNote = value;
                C.CNMutableContact_setNote(SelfPtr(), mNote != null ? mNote.ToPointer() : IntPtr.Zero);
            }
        }

        /// <summary>
        /// The profile picture of a contact.
        /// </summary>
        /// <value>The image data.</value>
        public new NSData ImageData
        {
            get
            {
                return base.ImageData;
            }
            set
            {
                mImageData = value;
                C.CNMutableContact_setImageData(SelfPtr(), mImageData != null ? mImageData.ToPointer() : IntPtr.Zero);
            }
        }

#region C wrapper

        private static class C
        {
            // Constructors
            [DllImport("__Internal")]
            internal static extern /* CNMutableContact */IntPtr CNMutableContact_new();

            // Contact Type
            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setContactType(HandleRef pointer, CNContactType contactType);

            // Date Properties
            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setBirthday(HandleRef pointer, /* NSDateComponents */IntPtr birthday);

            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setNonGregorianBirthday(HandleRef pointer, /* NSDateComponents */IntPtr nonGregorianBirthday);

            // Name Properties
            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setNamePrefix(HandleRef pointer, /* NSString */IntPtr namePrefix);

            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setGivenName(HandleRef pointer, /* NSString */IntPtr givenName);

            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setMiddleName(HandleRef pointer, /* NSString */IntPtr middleName);

            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setFamilyName(HandleRef pointer, /* NSString */IntPtr familyName);

            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setPreviousFamilyName(HandleRef pointer, /* NSString */IntPtr previousFamilyName);

            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setNameSuffix(HandleRef pointer, /* NSString */IntPtr nameSuffix);

            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setNickname(HandleRef pointer, /* NSString */IntPtr nickname);

            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setOrganizationName(HandleRef pointer, /* NSString */IntPtr organizationName);

            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setDepartmentName(HandleRef pointer, /* NSString */IntPtr departmentName);

            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setJobTitle(HandleRef pointer, /* NSString */IntPtr jobTitle);

            // Contact phone Number
            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setPhoneNumbers(HandleRef pointer, /* NSArray<CNLabeledValue<CNPhoneNumber *> *> */IntPtr phoneNumbers);

            // Online Addresses
            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setEmailAddresses(HandleRef pointer, /* NSArray<CNLabeledValue<NSString *> *> */IntPtr emailAddresses);

            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setUrlAddresses(HandleRef pointer, /* NSArray<CNLabeledValue<NSString *> *> */IntPtr urlAddresses);

            // Postal Addresses
            [DllImport("__Internal")]
            internal static extern /* NSArray<CNLabeledValue<CNPostalAddress *> *> */HandleRef CNContact_postalAddresses(IntPtr pointer);

            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setPostalAddresses(HandleRef pointer, /* NSArray<CNLabeledValue<CNPostalAddress *> *> */IntPtr postalAddresses);

            // Note Property
            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setNote(HandleRef pointer, /* NSString */IntPtr note);

            // Image Properties
            [DllImport("__Internal")]
            internal static extern void CNMutableContact_setImageData(HandleRef pointer, /* NSData */IntPtr imageData);
        }

#endregion
    }
}
#endif