#if UNITY_IOS && EM_CONTACTS
using System;
using System.Collections;
using System.Collections.Generic;
using AOT;
using System.Runtime.InteropServices;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.CoreFoundation;

namespace EasyMobile.iOS.Contacts
{
    /// <summary>
    /// The CNContactStore class is a thread-safe class that can fetch and save contacts, groups, and containers.
    /// </summary>
    internal class CNContactStore : iOSObjectProxy
    {
        public delegate void EnumerateContactsWithFetchRequestBlock(CNContact contact,out bool stop);

        /// <summary>
        /// An authorization status the user can grant for an app to access the specified entity type.
        /// </summary>
        public enum CNAuthorizationStatus
        {
            /// <summary>
            /// The user has not yet made a choice regarding whether the application may access contact data.
            /// </summary>
            CNAuthorizationStatusNotDetermined = 0,
            /// <summary>
            /// The application is not authorized to access contact data. The user cannot change this application’s status, 
            /// possibly due to active restrictions such as parental controls being in place.
            /// </summary>
            CNAuthorizationStatusRestricted,
            /// <summary>
            /// The user explicitly denied access to contact data for the application.
            /// </summary>
            CNAuthorizationStatusDenied,
            /// <summary>
            /// The application is authorized to access contact data.
            /// </summary>
            CNAuthorizationStatusAuthorized
        }

        /// <summary>
        /// The entities the user can grant access to.
        /// </summary>
        public enum CNEntityType
        {
            /// <summary>
            /// The user's contacts.
            /// </summary>
            CNEntityTypeContacts = 0
        }

        internal CNContactStore(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        public CNContactStore()
            : base(C.CNContactStore_new())
        {
            CFFunctions.CFRelease(SelfPtr());
        }

        /// <summary>
        /// Fetches a unified contact for the specified contact identifier.
        /// </summary>
        /// <returns>The contact with identifier.</returns>
        /// <param name="identifier">Identifier.</param>
        /// <param name="keys">Keys.</param>
        /// <param name="error">Error.</param>
        public CNContact UnifiedContactWithIdentifier(NSString identifier, NSArray<NSString> keys, out NSError error)
        {
            Util.NullArgumentTest(identifier);
            Util.NullArgumentTest(keys);

            IntPtr errorPtr = new IntPtr();
            var contactPtr = C.CNContactStore_unifiedContactWithIdentifier(
                                 SelfPtr(),
                                 identifier.ToPointer(),
                                 keys.ToPointer(),
                                 ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            CNContact contact = null;
            if (PInvokeUtil.IsNotNull(contactPtr))
            {
                contact = new CNContact(contactPtr);
                CFFunctions.CFRelease(contactPtr);
            }

            return contact;
        }

        /// <summary>
        /// Fetches all unified contacts matching the specified predicate.
        /// </summary>
        /// <returns>The contacts matching predicate.</returns>
        /// <param name="predicate">Predicate.</param>
        /// <param name="keys">Keys.</param>
        /// <param name="error">Error.</param>
        public NSArray<CNContact> UnifiedContactsMatchingPredicate(NSPredicate predicate, NSArray<NSString> keys, out NSError error)
        {
            Util.NullArgumentTest(predicate);
            Util.NullArgumentTest(keys);

            IntPtr errorPtr = new IntPtr();
            var contactsPtr = C.CNContactStore_unifiedContactsMatchingPredicate(
                                  SelfPtr(),
                                  predicate.ToPointer(),
                                  keys.ToPointer(),
                                  ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            NSArray<CNContact> contacts = null;
            if (PInvokeUtil.IsNotNull(contactsPtr))
            {
                contacts = new NSArray<CNContact>(contactsPtr);
                CFFunctions.CFRelease(contactsPtr);
            }

            return contacts;
        }

        /// <summary>
        /// Returns the current authorization status to access the contact data.
        /// </summary>
        /// <returns>The status for entity type.</returns>
        /// <param name="entityType">Entity type.</param>
        public static CNAuthorizationStatus AuthorizationStatusForEntityType(CNEntityType entityType)
        {
            return C.CNContactStore_authorizationStatusForEntityType(entityType);
        }

        /// <summary>
        /// Requests access to the user's contacts.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void RequestAccessForEntityType(CNEntityType entityType, Action<bool, NSError> completionHandler)
        {
            C.CNContactStore_requestAccessForEntityType(
                SelfPtr(),
                entityType,
                InternalRequestAccessForEntityTypeCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Returns a Boolean value that indicates whether the enumeration of 
        /// all contacts matching a contact fetch request executed successfully.
        /// </summary>
        /// <returns><c>true</c>, if contacts with fetch request was enumerated, <c>false</c> otherwise.</returns>
        /// <param name="fetchRequest">Fetch request.</param>
        /// <param name="error">Error.</param>
        /// <param name="block">Block.</param>
        public bool EnumerateContactsWithFetchRequest(CNContactFetchRequest fetchRequest, out NSError error, EnumerateContactsWithFetchRequestBlock block)
        {
            Util.NullArgumentTest(fetchRequest);
            Util.NullArgumentTest(block);

            IntPtr errorPtr = new IntPtr();
            bool success = C.CNContactStore_enumerateContactsWithFetchRequest(
                               SelfPtr(),
                               fetchRequest.ToPointer(),
                               ref errorPtr,
                               InternalEnumerateContactsBlock,
                               PInvokeCallbackUtil.ToIntPtr((CNContact contact) =>
                    {
                        bool shouldStop;
                        block(contact, out shouldStop);
                        return shouldStop;
                    }));

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);     // balance out ref count of a pointer returned directly from native side.
            }

            return success;
        }

        /// <summary>
        /// Executes a save request and returns success or failure.
        /// </summary>
        /// <returns><c>true</c>, if save request was executed, <c>false</c> otherwise.</returns>
        /// <param name="saveRequest">Save request.</param>
        /// <param name="error">Error.</param>
        public bool ExecuteSaveRequest(CNSaveRequest saveRequest, out NSError error)
        {
            Util.NullArgumentTest(saveRequest);

            IntPtr errorPtr = new IntPtr();
            bool success = C.CNContactStore_executeSaveRequest(SelfPtr(), saveRequest.ToPointer(), ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);     // balance out ref count of a pointer returned directly from native side.
            }

            return success;
        }

#region Internal Callbacks

        [MonoPInvokeCallback(typeof(C.RequestAccessForEntityTypeCallback))]
        private static void InternalRequestAccessForEntityTypeCallback(bool granted,/* NSError */IntPtr error, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            NSError nsError = PInvokeUtil.IsNotNull(error) ? new NSError(error) : null;

            // Invoke consumer callback.
            PInvokeCallbackUtil.PerformInternalCallback(
                "CNContactStore#InternalRequestAccessForEntityTypeCallback",
                PInvokeCallbackUtil.Type.Temporary,
                granted, 
                nsError, 
                secondaryCallback);
        }

        [MonoPInvokeCallback(typeof(C.EnumerateContactsBlock))]
        private static void InternalEnumerateContactsBlock(/* CNContact */IntPtr contact, out bool stop, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
            {
                stop = true;
                return;
            }
        
            CNContact ct = PInvokeUtil.IsNotNull(contact) ? new CNContact(contact) : null;
        
            // Invoke consumer callback.
            stop = PInvokeCallbackUtil.PerformInternalFunction<CNContact, bool>(
                "CNContactStore#InternalEnumerateContactsBlock",
                PInvokeCallbackUtil.Type.Permanent, // make sure the callback can be called repeatedly
                ct, 
                secondaryCallback);

            // Release callback handle if stopping.
            if (stop)
                PInvokeCallbackUtil.UnpinCallbackHandle(secondaryCallback);
        }

#endregion

#region C wrapper

        private static class C
        {
            // Constructor
            [DllImport("__Internal")]
            internal static extern /* CNContactStore */IntPtr CNContactStore_new();

            // Fetching Unified Contacts
            [DllImport("__Internal")]
            internal static extern /* CNContact */IntPtr CNContactStore_unifiedContactWithIdentifier(
                HandleRef selfPointer, 
                /* NSString */IntPtr identifier, 
                /* NSArray<id<CNKeyDescriptor>> */IntPtr keys, 
                /* NSError * __Nullable * */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern /* NSArray<CNContact *> */IntPtr CNContactStore_unifiedContactsMatchingPredicate(
                HandleRef selfPointer, 
                /* NSPredicate */IntPtr predicate, 
                /* NSArray<id<CNKeyDescriptor>> */IntPtr keys, 
                /* NSError * __Nullable * */[In, Out] ref IntPtr error);

            // Privacy Access
            public delegate void RequestAccessForEntityTypeCallback(bool granted,/* NSError */IntPtr error,IntPtr secondaryCallback);

            public delegate void EnumerateContactsBlock(/* CNContact */IntPtr contact,out bool stop,IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern CNAuthorizationStatus CNContactStore_authorizationStatusForEntityType(CNEntityType entityType);

            [DllImport("__Internal")]
            internal static extern void CNContactStore_requestAccessForEntityType(HandleRef selfPointer, 
                                                                                  CNEntityType entityType, 
                                                                                  RequestAccessForEntityTypeCallback callback, 
                                                                                  IntPtr secondaryCallback);

            // Fetching and Saving
            [DllImport("__Internal")]
            internal static extern bool CNContactStore_enumerateContactsWithFetchRequest(
                HandleRef selfPointer, 
                /* CNContactFetchRequest */IntPtr fetchRequest, 
                /* NSError * __Nullable * */[In,Out] ref IntPtr error, 
                EnumerateContactsBlock enumerateBlock, 
                IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern bool CNContactStore_executeSaveRequest(
                HandleRef selfPointer, 
                /* CNSaveRequest */IntPtr saveRequest, 
                /* NSError * _Nullable * */[In,Out] ref IntPtr error);
        }

#endregion
    }
}
#endif