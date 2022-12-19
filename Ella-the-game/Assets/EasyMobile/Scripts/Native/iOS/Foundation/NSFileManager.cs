#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.CoreFoundation;

namespace EasyMobile.iOS.Foundation
{
    /// <summary>
    /// A convenient interface to the contents of the file system, 
    /// and the primary means of interacting with it.
    /// </summary>
    internal class NSFileManager : iOSObjectProxy
    {
        private const string FrameworkName = "Foundation";

        /// <summary>
        /// The location of significant directories.
        /// </summary>
        public enum NSSearchPathDirectory
        {
            /// <summary>
            /// Supported applications (/Applications).
            /// </summary>
            NSApplicationDirectory = 1,
            /// <summary>
            /// Unsupported applications and demonstration versions.
            /// </summary>
            NSDemoApplicationDirectory,
            /// <summary>
            /// Developer applications (/Developer/Applications).
            /// </summary>
            NSDeveloperApplicationDirectory,
            /// <summary>
            /// System and network administration applications.
            /// </summary>
            NSAdminApplicationDirectory,
            /// <summary>
            /// Various user-visible documentation, support, and configuration files (/Library).
            /// </summary>
            NSLibraryDirectory,
            /// <summary>
            /// Developer resources (/Developer).
            /// </summary>
            NSDeveloperDirectory,
            /// <summary>
            /// User home directories (/Users).
            /// </summary>
            NSUserDirectory,
            /// <summary>
            /// Documentation.
            /// </summary>
            NSDocumentationDirectory,
            /// <summary>
            /// Document directory.
            /// </summary>
            NSDocumentDirectory,
            /// <summary>
            /// Core services (System/Library/CoreServices).
            /// </summary>
            NSCoreServiceDirectory,
            /// <summary>
            /// The user’s autosaved documents (Library/Autosave Information).
            /// </summary>
            NSAutosavedInformationDirectory = 11,
            /// <summary>
            /// The user’s desktop directory.
            /// </summary>
            NSDesktopDirectory = 12,
            /// <summary>
            /// Discardable cache files (Library/Caches).
            /// </summary>
            NSCachesDirectory = 13,
            /// <summary>
            /// Application support files (Library/Application Support).
            /// </summary>
            NSApplicationSupportDirectory = 14,
            /// <summary>
            /// The user’s downloads directory.
            /// </summary>
            NSDownloadsDirectory = 15,
            /// <summary>
            /// Input Methods (Library/Input Methods).
            /// </summary>
            NSInputMethodsDirectory = 16,
            /// <summary>
            /// The user’s Movies directory (~/Movies).
            /// </summary>
            NSMoviesDirectory = 17,
            /// <summary>
            /// The user’s Music directory (~/Music).
            /// </summary>
            NSMusicDirectory = 18,
            /// <summary>
            /// The user’s Pictures directory (~/Pictures).
            /// </summary>
            NSPicturesDirectory = 19,
            /// <summary>
            /// The system’s PPDs directory (Library/Printers/PPDs).
            /// </summary>
            NSPrinterDescriptionDirectory = 20,
            /// <summary>
            /// The user’s Public sharing directory (~/Public).
            /// </summary>
            NSSharedPublicDirectory = 21,
            /// <summary>
            /// The PreferencePanes directory for use with System Preferences (Library/PreferencePanes).
            /// </summary>
            NSPreferencePanesDirectory = 22,
            /// <summary>
            /// The user scripts folder for the calling application (~/Library/Application Scripts/<code-signing-id>.
            /// </summary>
            NSApplicationScriptsDirectory = 23,
            /// <summary>
            /// The constant used to create a temporary directory.
            /// </summary>
            NSItemReplacementDirectory = 99,
            /// <summary>
            /// All directories where applications can be stored.
            /// </summary>
            NSAllApplicationsDirectory = 100,
            /// <summary>
            /// All directories where resources can be stored.
            /// </summary>
            NSAllLibrariesDirectory = 101,
            /// <summary>
            /// The trash directory.
            /// </summary>
            NSTrashDirectory = 102
        }

        /// <summary>
        /// Domain constants specifying base locations to use when you search for significant directories.
        /// </summary>
        public enum NSSearchPathDomainMask
        {
            /// <summary>
            /// The user’s home directory—the place to install user’s personal items (~).
            /// </summary>
            NSUserDomainMask = 1,
            /// <summary>
            /// The place to install items available to everyone on this machine.
            /// </summary>
            NSLocalDomainMask = 2,
            /// <summary>
            /// The place to install items available on the network (/Network).
            /// </summary>
            NSNetworkDomainMask = 4,
            /// <summary>
            /// A directory for system files provided by Apple (/System) .
            /// </summary>
            NSSystemDomainMask = 8,
            /// <summary>
            /// All domains.
            /// </summary>
            NSAllDomainsMask = 0x0ffff
        }

        /// <summary>
        /// Options for specifying the behavior of file replacement operations.
        /// </summary>
        public enum NSFileManagerItemReplacementOptions
        {
            /// <summary>
            /// Only metadata from the new item is used, and metadata from the original item isn’t preserved (default).
            /// </summary>
            NSFileManagerItemReplacementUsingNewMetadataOnly = 1 << 0,
            /// <summary>
            /// The backup item remains in place after a successful replacement.
            /// </summary>
            NSFileManagerItemReplacementWithoutDeletingBackupItem = 1 << 1
        }

        #region NSFileAttributeKey

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates whether the file is read-only.
        /// </summary>
        /// <value>The NS file append only.</value>
        public static NSString NSFileAppendOnly
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileAppendOnly, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates whether the file is busy.
        /// </summary>
        /// <value>The NS file busy.</value>
        public static NSString NSFileBusy
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileBusy, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the file's creation date.
        /// </summary>
        /// <value>The NS file creation date.</value>
        public static NSString NSFileCreationDate
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileCreationDate, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the identifier for the device on which the file resides.
        /// </summary>
        /// <value>The NS file device identifier.</value>
        public static NSString NSFileDeviceIdentifier
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileDeviceIdentifier, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates whether the file’s extension is hidden.
        /// </summary>
        /// <value>The NS file extension hidden.</value>
        public static NSString NSFileExtensionHidden
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileExtensionHidden, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the file’s group ID.
        /// </summary>
        /// <value>The NS file group owner account I.</value>
        public static NSString NSFileGroupOwnerAccountID
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileGroupOwnerAccountID, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the group name of the file’s owner.
        /// </summary>
        /// <value>The name of the NS file group owner account.</value>
        public static NSString NSFileGroupOwnerAccountName
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileGroupOwnerAccountName, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the file’s HFS creator code.
        /// </summary>
        /// <value>The NS file HFS creator code.</value>
        public static NSString NSFileHFSCreatorCode
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileHFSCreatorCode, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the file’s HFS type code.
        /// </summary>
        /// <value>The NS file HFS type code.</value>
        public static NSString NSFileHFSTypeCode
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileHFSTypeCode, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates whether the file is mutable.
        /// </summary>
        /// <value>The NS file immutable.</value>
        public static NSString NSFileImmutable
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileImmutable, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the file’s last modified date.
        /// </summary>
        /// <value>The NS file modification date.</value>
        public static NSString NSFileModificationDate
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileModificationDate, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the file’s owner's account ID.
        /// </summary>
        /// <value>The NS file owner account I.</value>
        public static NSString NSFileOwnerAccountID
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileOwnerAccountID, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the name of the file’s owner.
        /// </summary>
        /// <value>The name of the NS file owner account.</value>
        public static NSString NSFileOwnerAccountName
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileOwnerAccountName, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the file’s Posix permissions.
        /// </summary>
        /// <value>The NS file posix permissions.</value>
        public static NSString NSFilePosixPermissions
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFilePosixPermissions, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value identifies the protection level for this file.
        /// </summary>
        /// <value>The NS file protection key.</value>
        public static NSString NSFileProtectionKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileProtectionKey
                    , FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the file’s reference count.
        /// </summary>
        /// <value>The NS file reference count.</value>
        public static NSString NSFileReferenceCount
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileReferenceCount, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the file’s size in bytes.
        /// </summary>
        /// <value>The size of the NS file.</value>
        public static NSString NSFileSize
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileSize, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the file’s filesystem file number.
        /// </summary>
        /// <value>The NS file system file number.</value>
        public static NSString NSFileSystemFileNumber
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileSystemFileNumber, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file system attribute dictionary whose value indicates the number of free nodes in the file system.
        /// </summary>
        /// <value>The NS file system free nodes.</value>
        public static NSString NSFileSystemFreeNodes
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileSystemFreeNodes, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file system attribute dictionary whose value indicates the amount of free space on the file system.
        /// </summary>
        /// <value>The size of the NS file system free.</value>
        public static NSString NSFileSystemFreeSize
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileSystemFreeSize
                    , FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file system attribute dictionary whose value indicates the number of nodes in the file system.
        /// </summary>
        /// <value>The NS file system nodes.</value>
        public static NSString NSFileSystemNodes
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileSystemNodes, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file system attribute dictionary whose value indicates the filesystem number of the file system.
        /// </summary>
        /// <value>The NS file system number.</value>
        public static NSString NSFileSystemNumber
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileSystemNumber, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file system attribute dictionary whose value indicates the size of the file system.
        /// </summary>
        /// <value>The size of the NS file system.</value>
        public static NSString NSFileSystemSize
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileSystemSize, FrameworkName);
            }
        }

        /// <summary>
        /// The key in a file attribute dictionary whose value indicates the file’s type.
        /// </summary>
        /// <value>The type of the NS file.</value>
        public static NSString NSFileType
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => NSFileManager.NSFileType, FrameworkName);
            }
        }

        #endregion

        private static NSFileManager sDefaultManager;
        private NSURL mTemporaryDirectory;
        private iOSObjectProxy mUbiquityIdentityToken;

        internal NSFileManager(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// The shared file manager object for the process.
        /// </summary>
        /// <value>The default manager.</value>
        public static NSFileManager DefaultManager
        {
            get
            {
                if (sDefaultManager == null)
                {
                    IntPtr ptr = C.NSFileManager_defaultManager();
                    sDefaultManager = new NSFileManager(ptr);
                    CFFunctions.CFRelease(ptr);
                }
                return sDefaultManager;
            }
        }

        /// <summary>
        /// Returns the path to either the user’s or application’s home directory, depending on the platform.
        /// </summary>
        /// <returns>The home directory.</returns>
        public static NSString NSHomeDirectory()
        {
            var ptr = C.NSFileManager_NSHomeDirectory();

            if (PInvokeUtil.IsNotNull(ptr))
            {
                var dirPath = new NSString(ptr);
                CFFunctions.CFRelease(ptr);
                return dirPath;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the logon name of the current user.
        /// </summary>
        /// <returns>The user name.</returns>
        public static NSString NSUserName()
        {
            var ptr = C.NSFileManager_NSUserName();

            if (PInvokeUtil.IsNotNull(ptr))
            {
                var name = new NSString(ptr);
                CFFunctions.CFRelease(ptr);
                return name;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a string containing the full name of the current user.
        /// </summary>
        /// <returns>The full user name.</returns>
        public static NSString NSFullUserName()
        {
            var ptr = C.NSFileManager_NSFullUserName();

            if (PInvokeUtil.IsNotNull(ptr))
            {
                var name = new NSString(ptr);
                CFFunctions.CFRelease(ptr);
                return name;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the path to a given user’s home directory.
        /// </summary>
        /// <returns>The home directory for user.</returns>
        /// <param name="userName">User name.</param>
        public static NSString NSHomeDirectoryForUser(NSString userName)
        {
            Util.NullArgumentTest(userName);
            
            var ptr = C.NSFileManager_NSHomeDirectoryForUser(userName.ToPointer());

            if (PInvokeUtil.IsNotNull(ptr))
            {
                var dirPath = new NSString(ptr);
                CFFunctions.CFRelease(ptr);
                return dirPath;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the path of the temporary directory for the current user.
        /// </summary>
        /// <returns>The temporary directory.</returns>
        public static NSString NSTemporaryDirectory()
        {
            var ptr = C.NSFileManager_NSTemporaryDirectory();

            if (PInvokeUtil.IsNotNull(ptr))
            {
                var dirPath = new NSString(ptr);
                CFFunctions.CFRelease(ptr);
                return dirPath;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The temporary directory for the current user.
        /// </summary>
        /// <value>The temporary directory.</value>
        /// <remarks>
        /// This API is available on iOS 11.0+ only.
        /// </remarks>
        public NSURL TemporaryDirectory
        {
            get
            {
                if (mTemporaryDirectory == null)
                {
                    var ptr = C.NSFileManager_temporaryDirectory(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mTemporaryDirectory = new NSURL(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }

                return mTemporaryDirectory;
            }
        }

        /// <summary>
        /// Returns an array of URLs for the specified common directory in the requested domains.
        /// </summary>
        /// <returns>The ls for directory.</returns>
        /// <param name="directory">Directory.</param>
        /// <param name="domainMask">Domain mask.</param>
        public NSArray<NSURL> URLsForDirectory(NSSearchPathDirectory directory, NSSearchPathDomainMask domainMask)
        {
            var ptr = C.NSFileManager_URLsForDirectoryInDomains(SelfPtr(), directory, domainMask);

            if (PInvokeUtil.IsNull(ptr))
            {
                return null;
            }
            else
            {
                var urls = new NSArray<NSURL>(ptr);
                CFFunctions.CFRelease(ptr);
                return urls;
            }
        }

        /// <summary>
        /// Creates a list of directory search paths.
        /// </summary>
        /// <returns>The search path for directories in domains.</returns>
        /// <param name="directory">Directory.</param>
        /// <param name="domainMask">Domain mask.</param>
        /// <param name="expandTilde">If set to <c>true</c> expand tilde.</param>
        public static NSArray<NSString> NSSearchPathForDirectoriesInDomains(NSSearchPathDirectory directory, NSSearchPathDomainMask domainMask, bool expandTilde)
        {
            var ptr = C.NSFileManager_NSSearchPathForDirectoriesInDomains(directory, domainMask, expandTilde);

            if (PInvokeUtil.IsNull(ptr))
            {
                return null;
            }
            else
            {
                var paths = new NSArray<NSString>(ptr);
                CFFunctions.CFRelease(ptr);
                return paths;
            }
        }

        /// <summary>
        /// Returns the root directory of the user’s system.
        /// </summary>
        /// <returns>The open step root directory.</returns>
        public static NSString NSOpenStepRootDirectory()
        {
            var ptr = C.NSFileManager_NSOpenStepRootDirectory();

            if (PInvokeUtil.IsNotNull(ptr))
            {
                var dirPath = new NSString(ptr);
                CFFunctions.CFRelease(ptr);
                return dirPath;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a directory with the given attributes at the specified URL.
        /// </summary>
        /// <returns><c>true</c>, if directory at UR was created, <c>false</c> otherwise.</returns>
        /// <param name="url">URL.</param>
        /// <param name="createIntermediates">If set to <c>true</c> create intermediates.</param>
        /// <param name="attributes">Attributes.</param>
        /// <param name="error">Error.</param>
        public bool CreateDirectoryAtURL(NSURL url, bool createIntermediates, NSDictionary<NSString, iOSObjectProxy> attributes, out NSError error)
        {
            Util.NullArgumentTest(url);

            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_createDirectoryAtURL(
                               SelfPtr(),
                               url.ToPointer(),
                               createIntermediates,
                               attributes != null ? attributes.ToPointer() : IntPtr.Zero,
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Creates a directory with given attributes at the specified path.
        /// </summary>
        /// <returns><c>true</c>, if directory at path was created, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        /// <param name="createIntermediates">If set to <c>true</c> create intermediates.</param>
        /// <param name="attributes">Attributes.</param>
        /// <param name="error">Error.</param>
        public bool CreateDirectoryAtPath(NSString path, bool createIntermediates, NSDictionary<NSString, iOSObjectProxy> attributes, out NSError error)
        {
            Util.NullArgumentTest(path);

            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_createDirectoryAtPath(
                               SelfPtr(),
                               path.ToPointer(),
                               createIntermediates,
                               attributes != null ? attributes.ToPointer() : IntPtr.Zero,
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Creates a file with the specified content and attributes at the given location.
        /// </summary>
        /// <returns><c>true</c>, if file at path was created, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        /// <param name="data">Data.</param>
        /// <param name="attributes">Attributes.</param>
        public bool CreateFileAtPath(NSString path, NSData data, NSDictionary<NSString, iOSObjectProxy> attributes)
        {
            Util.NullArgumentTest(path);
            Util.NullArgumentTest(data);

            return C.NSFileManager_createFileAtPath(
                SelfPtr(),
                path.ToPointer(),
                data.ToPointer(),
                attributes != null ? attributes.ToPointer() : IntPtr.Zero);
        }

        /// <summary>
        /// Removes the file or directory at the specified URL.
        /// </summary>
        /// <returns><c>true</c>, if item at UR was removed, <c>false</c> otherwise.</returns>
        /// <param name="url">URL.</param>
        /// <param name="error">Error.</param>
        public bool RemoveItemAtURL(NSURL url, out NSError error)
        {
            Util.NullArgumentTest(url);

            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_removeItemAtURL(
                               SelfPtr(),
                               url.ToPointer(),
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Removes the file or directory at the specified path.
        /// </summary>
        /// <returns><c>true</c>, if item at path was removed, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        /// <param name="error">Error.</param>
        public bool RemoveItemAtPath(NSString path, out NSError error)
        {
            Util.NullArgumentTest(path);

            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_removeItemAtPath(
                               SelfPtr(),
                               path.ToPointer(),
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Moves an item to the trash.
        /// </summary>
        /// <returns><c>true</c>, if item at UR was trashed, <c>false</c> otherwise.</returns>
        /// <param name="url">URL.</param>
        /// <param name="outResultingURL">Out resulting UR.</param>
        /// <param name="error">Error.</param>
        /// <remarks>
        /// This API is available on iOS 11.0+ only.
        /// </remarks>
        public bool TrashItemAtURL(NSURL url, out NSURL outResultingURL, out NSError error)
        {
            Util.NullArgumentTest(url);

            IntPtr resultUrlPtr = new IntPtr();
            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_trashItemAtURL(
                               SelfPtr(),
                               url.ToPointer(),
                               ref resultUrlPtr,
                               ref errorPtr);

            outResultingURL = null;
            if (PInvokeUtil.IsNotNull(resultUrlPtr))
            {
                outResultingURL = new NSURL(resultUrlPtr);
                CFFunctions.CFRelease(resultUrlPtr);
            }

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Replaces the contents of the item at the specified URL in a manner that ensures no data loss occurs.
        /// </summary>
        /// <returns><c>true</c>, if item at UR was replaced, <c>false</c> otherwise.</returns>
        /// <param name="originalItemURL">Original item UR.</param>
        /// <param name="newItemURL">New item UR.</param>
        /// <param name="backupItemName">Backup item name.</param>
        /// <param name="options">Options.</param>
        /// <param name="resultingURL">Resulting UR.</param>
        /// <param name="error">Error.</param>
        public bool ReplaceItemAtURL(NSURL originalItemURL, NSURL newItemURL, NSString backupItemName, NSFileManagerItemReplacementOptions options, out NSURL resultingURL, out NSError error)
        {
            Util.NullArgumentTest(originalItemURL);
            Util.NullArgumentTest(newItemURL);

            IntPtr resultUrlPtr = new IntPtr();
            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_replaceItemAtURL(
                               SelfPtr(),
                               originalItemURL.ToPointer(),
                               newItemURL.ToPointer(),
                               backupItemName != null ? backupItemName.ToPointer() : IntPtr.Zero,
                               options,
                               ref resultUrlPtr,
                               ref errorPtr);

            resultingURL = null;
            if (PInvokeUtil.IsNotNull(resultUrlPtr))
            {
                resultingURL = new NSURL(resultUrlPtr);
                CFFunctions.CFRelease(resultUrlPtr);
            }

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Copies the file at the specified URL to a new location synchronously.
        /// </summary>
        /// <returns><c>true</c>, if item at UR was copyed, <c>false</c> otherwise.</returns>
        /// <param name="srcURL">Source UR.</param>
        /// <param name="dstURL">Dst UR.</param>
        /// <param name="error">Error.</param>
        public bool CopyItemAtURL(NSURL srcURL, NSURL dstURL, out NSError error)
        {
            Util.NullArgumentTest(srcURL);
            Util.NullArgumentTest(dstURL);

            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_copyItemAtURL(
                               SelfPtr(),
                               srcURL.ToPointer(),
                               dstURL.ToPointer(),
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Copies the item at the specified path to a new location synchronously.
        /// </summary>
        /// <returns><c>true</c>, if item at path was copyed, <c>false</c> otherwise.</returns>
        /// <param name="srcPath">Source path.</param>
        /// <param name="dstPath">Dst path.</param>
        /// <param name="error">Error.</param>
        public bool CopyItemAtPath(NSString srcPath, NSString dstPath, out NSError error)
        {
            Util.NullArgumentTest(srcPath);
            Util.NullArgumentTest(dstPath);

            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_copyItemAtPath(
                               SelfPtr(),
                               srcPath.ToPointer(),
                               dstPath.ToPointer(),
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Moves the file or directory at the specified URL to a new location synchronously.
        /// </summary>
        /// <returns><c>true</c>, if item at UR was moved, <c>false</c> otherwise.</returns>
        /// <param name="srcURL">Source UR.</param>
        /// <param name="dstURL">Dst UR.</param>
        /// <param name="error">Error.</param>
        public bool MoveItemAtURL(NSURL srcURL, NSURL dstURL, out NSError error)
        {
            Util.NullArgumentTest(srcURL);
            Util.NullArgumentTest(dstURL);

            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_moveItemAtURL(
                               SelfPtr(),
                               srcURL.ToPointer(),
                               dstURL.ToPointer(),
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Moves the file or directory at the specified path to a new location synchronously.
        /// </summary>
        /// <returns><c>true</c>, if item at path was moved, <c>false</c> otherwise.</returns>
        /// <param name="srcPath">Source path.</param>
        /// <param name="dstPath">Dst path.</param>
        /// <param name="error">Error.</param>
        public bool MoveItemAtPath(NSString srcPath, NSString dstPath, out NSError error)
        {
            Util.NullArgumentTest(srcPath);
            Util.NullArgumentTest(dstPath);

            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_moveItemAtPath(
                               SelfPtr(),
                               srcPath.ToPointer(),
                               dstPath.ToPointer(),
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// An opaque token that represents the current user’s iCloud identity.
        /// </summary>
        /// <value>The ubiquity identity token.</value>
        public iOSObjectProxy UbiquityIdentityToken
        {
            get
            {
                if (mUbiquityIdentityToken == null)
                {
                    var ptr = C.NSFileManager_ubiquityIdentityToken(SelfPtr());
                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mUbiquityIdentityToken = new iOSObjectProxy(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mUbiquityIdentityToken;
            }
        }

        /// <summary>
        /// Returns the URL for the iCloud container associated with the specified identifier and establishes access to that container.
        /// </summary>
        /// <returns>The for ubiquity container identifier.</returns>
        /// <param name="containerIdentifier">Container identifier.</param>
        public NSURL URLForUbiquityContainerIdentifier(NSString containerIdentifier)
        {
            Util.NullArgumentTest(containerIdentifier);

            var ptr = C.NSFileManager_URLForUbiquityContainerIdentifier(SelfPtr(), containerIdentifier.ToPointer());
            if (PInvokeUtil.IsNotNull(ptr))
            {
                var url = new NSURL(ptr);
                CFFunctions.CFRelease(ptr);
                return url;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a Boolean indicating whether the item is targeted for storage in iCloud.
        /// </summary>
        /// <returns><c>true</c> if this instance is ubiquitous item at UR the specified url; otherwise, <c>false</c>.</returns>
        /// <param name="url">URL.</param>
        public bool IsUbiquitousItemAtURL(NSURL url)
        {
            return url == null ? false : C.NSFileManager_isUbiquitousItemAtURL(SelfPtr(), url.ToPointer());
        }

        /// <summary>
        /// Indicates whether the item at the specified URL should be stored in iCloud.
        /// </summary>
        /// <returns><c>true</c>, if ubiquitous item at UR was set, <c>false</c> otherwise.</returns>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        /// <param name="url">URL.</param>
        /// <param name="destinationURL">Destination UR.</param>
        /// <param name="error">Error.</param>
        public bool SetUbiquitousItemAtURL(bool flag, NSURL url, NSURL destinationURL, out NSError error)
        {
            Util.NullArgumentTest(url);
            Util.NullArgumentTest(destinationURL);

            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_setUbiquitousItemAtURL(
                               SelfPtr(),
                               flag,
                               url.ToPointer(),
                               destinationURL.ToPointer(),
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Starts downloading (if necessary) the specified item to the local system.
        /// </summary>
        /// <returns><c>true</c>, if downloading ubiquitous item at UR was started, <c>false</c> otherwise.</returns>
        /// <param name="url">URL.</param>
        /// <param name="error">Error.</param>
        public bool StartDownloadingUbiquitousItemAtURL(NSURL url, out NSError error)
        {
            Util.NullArgumentTest(url);

            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_startDownloadingUbiquitousItemAtURL(
                               SelfPtr(),
                               url.ToPointer(),
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Removes the local copy of the specified item that’s stored in iCloud.
        /// </summary>
        /// <returns><c>true</c>, if ubiquitous item at UR was evicted, <c>false</c> otherwise.</returns>
        /// <param name="url">URL.</param>
        /// <param name="error">Error.</param>
        public bool EvictUbiquitousItemAtURL(NSURL url, out NSError error)
        {
            Util.NullArgumentTest(url);

            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_evictUbiquitousItemAtURL(
                               SelfPtr(),
                               url.ToPointer(),
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Returns a Boolean value that indicates whether a file or directory exists at a specified path.
        /// </summary>
        /// <returns><c>true</c>, if exists at path was filed, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        public bool FileExistsAtPath(NSString path)
        {
            return path == null ? false : C.NSFileManager_fileExistsAtPath(SelfPtr(), path.ToPointer());
        }

        /// <summary>
        /// Returns a Boolean value that indicates whether a file or directory exists at a specified path.
        /// </summary>
        /// <returns><c>true</c>, if exists at path was filed, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        /// <param name="isDirectory">Is directory.</param>
        public bool FileExistsAtPath(NSString path, out bool isDirectory)
        {
            isDirectory = false;
            return path == null ? false : C.NSFileManager_fileExistsAtPathIsDirectory(SelfPtr(), path.ToPointer(), ref isDirectory);
        }

        /// <summary>
        /// Returns a Boolean value that indicates whether the invoking object appears able to read a specified file.
        /// </summary>
        /// <returns><c>true</c> if this instance is readable file at path the specified path; otherwise, <c>false</c>.</returns>
        /// <param name="path">Path.</param>
        public bool IsReadableFileAtPath(NSString path)
        {
            return path == null ? false : C.NSFileManager_isReadableFileAtPath(SelfPtr(), path.ToPointer());
        }

        /// <summary>
        /// Returns a Boolean value that indicates whether the invoking object appears able to write to a specified file.
        /// </summary>
        /// <returns><c>true</c> if this instance is writable file at path the specified path; otherwise, <c>false</c>.</returns>
        /// <param name="path">Path.</param>
        public bool IsWritableFileAtPath(NSString path)
        {
            return path == null ? false : C.NSFileManager_isWritableFileAtPath(SelfPtr(), path.ToPointer());
        }

        /// <summary>
        /// Returns a Boolean value that indicates whether the operating system appears able to execute a specified file.
        /// </summary>
        /// <returns><c>true</c> if this instance is executable file at path the specified path; otherwise, <c>false</c>.</returns>
        /// <param name="path">Path.</param>
        public bool IsExecutableFileAtPath(NSString path)
        {
            return path == null ? false : C.NSFileManager_isExecutableFileAtPath(SelfPtr(), path.ToPointer());
        }

        /// <summary>
        /// Returns a Boolean value that indicates whether the invoking object appears able to delete a specified file.
        /// </summary>
        /// <returns><c>true</c> if this instance is deletable file at path the specified path; otherwise, <c>false</c>.</returns>
        /// <param name="path">Path.</param>
        public bool IsDeletableFileAtPath(NSString path)
        {
            return path == null ? false : C.NSFileManager_isDeletableFileAtPath(SelfPtr(), path.ToPointer());
        }

        /// <summary>
        /// Returns an array of strings representing the user-visible components of a given path.
        /// </summary>
        /// <returns>The to display for path.</returns>
        /// <param name="path">Path.</param>
        public NSArray<NSString> ComponentsToDisplayForPath(NSString path)
        {
            Util.NullArgumentTest(path);

            NSArray<NSString> comps = null;
            var ptr = C.NSFileManager_componentsToDisplayForPath(SelfPtr(), path.ToPointer());

            if (PInvokeUtil.IsNotNull(ptr))
            {
                comps = new NSArray<NSString>(ptr);
                CFFunctions.CFRelease(ptr);
            }
            return comps;
        }

        /// <summary>
        /// Returns the display name of the file or directory at a specified path.
        /// </summary>
        /// <returns>The name at path.</returns>
        /// <param name="path">Path.</param>
        public NSString DisplayNameAtPath(NSString path)
        {
            Util.NullArgumentTest(path);

            NSString name = null;
            var ptr = C.NSFileManager_displayNameAtPath(SelfPtr(), path.ToPointer());

            if (PInvokeUtil.IsNotNull(ptr))
            {
                name = new NSString(ptr);
                CFFunctions.CFRelease(ptr);
            }
            return name;
        }

        /// <summary>
        /// Returns the attributes of the item at a given path.
        /// </summary>
        /// <returns>The of item at path.</returns>
        /// <param name="path">Path.</param>
        /// <param name="error">Error.</param>
        public NSDictionary<NSString, iOSObjectProxy> AttributesOfItemAtPath(NSString path, out NSError error)
        {
            Util.NullArgumentTest(path);

            NSDictionary<NSString, iOSObjectProxy> attr = null;
            IntPtr errorPtr = new IntPtr();
            var ptr = C.NSFileManager_attributesOfItemAtPath(SelfPtr(), path.ToPointer(), ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            if (PInvokeUtil.IsNotNull(ptr))
            {
                attr = new NSDictionary<NSString, iOSObjectProxy>(ptr);
                CFFunctions.CFRelease(ptr);
            }
            return attr;
        }

        /// <summary>
        /// Returns a dictionary that describes the attributes of the mounted file system on which a given path resides.
        /// </summary>
        /// <returns>The of file system for path.</returns>
        /// <param name="path">Path.</param>
        /// <param name="error">Error.</param>
        public NSDictionary<NSString, iOSObjectProxy> AttributesOfFileSystemForPath(NSString path, out NSError error)
        {
            Util.NullArgumentTest(path);

            NSDictionary<NSString, iOSObjectProxy> attr = null;
            IntPtr errorPtr = new IntPtr();
            var ptr = C.NSFileManager_attributesOfFileSystemForPath(SelfPtr(), path.ToPointer(), ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            if (PInvokeUtil.IsNotNull(ptr))
            {
                attr = new NSDictionary<NSString, iOSObjectProxy>(ptr);
                CFFunctions.CFRelease(ptr);
            }
            return attr;
        }

        /// <summary>
        /// Sets the attributes of the specified file or directory.
        /// </summary>
        /// <returns><c>true</c>, if attributes of item at path was set, <c>false</c> otherwise.</returns>
        /// <param name="attributes">Attributes.</param>
        /// <param name="path">Path.</param>
        /// <param name="error">Error.</param>
        public bool SetAttributesOfItemAtPath(NSDictionary<NSString, iOSObjectProxy> attributes, NSString path, out NSError error)
        {
            Util.NullArgumentTest(attributes);
            Util.NullArgumentTest(path);

            IntPtr errorPtr = new IntPtr();
            bool success = C.NSFileManager_setAttributesOfItemAtPath(
                               SelfPtr(),
                               attributes.ToPointer(),
                               path.ToPointer(),
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);
            }

            return success;
        }

        /// <summary>
        /// Returns the contents of the file at the specified path.
        /// </summary>
        /// <returns>The at path.</returns>
        /// <param name="path">Path.</param>
        public NSData ContentsAtPath(NSString path)
        {
            Util.NullArgumentTest(path);

            NSData data = null;
            var ptr = C.NSFileManager_contentsAtPath(SelfPtr(), path.ToPointer());

            if (PInvokeUtil.IsNotNull(ptr))
            {
                data = new NSData(ptr);
                CFFunctions.CFRelease(ptr);
            }

            return data;
        }

        /// <summary>
        /// Returns a Boolean value that indicates whether the files or directories in specified paths have the same contents.
        /// </summary>
        /// <returns><c>true</c>, if equal at path was contentsed, <c>false</c> otherwise.</returns>
        /// <param name="path1">Path1.</param>
        /// <param name="path2">Path2.</param>
        public bool ContentsEqualAtPath(NSString path1, NSString path2)
        {
            return (path1 == null || path2 == null) ? false : C.NSFileManager_contentsEqualAtPath(SelfPtr(), path1.ToPointer(), path2.ToPointer());
        }

        #region C Wrapper

        private static class C
        {
            // Creating a File Manager
            [DllImport("__Internal")]
            internal static extern /* NSFileManager */IntPtr NSFileManager_defaultManager();

            // Accessing User Directories
            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSFileManager_NSHomeDirectory();

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSFileManager_NSUserName();

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSFileManager_NSFullUserName();

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSFileManager_NSHomeDirectoryForUser(/* NSString */IntPtr userName);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSFileManager_temporaryDirectory(/* NSFileManager */HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSFileManager_NSTemporaryDirectory();

            // Locating System Directories
            [DllImport("__Internal")]
            internal static extern /* NSArray<NSURL *> */IntPtr NSFileManager_URLsForDirectoryInDomains(
                /* NSFileManager */HandleRef selfPointer, 
                                   NSSearchPathDirectory directory, 
                                   NSSearchPathDomainMask domainMask);

            [DllImport("__Internal")]
            internal static extern /* NSArray<NSString *> */IntPtr NSFileManager_NSSearchPathForDirectoriesInDomains(
                NSSearchPathDirectory directory, 
                NSSearchPathDomainMask domainMask, 
                bool expandTilde);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSFileManager_NSOpenStepRootDirectory();

            // Creating and Deleting Items
            [DllImport("__Internal")]
            internal static extern bool NSFileManager_createDirectoryAtURL(
                /* NSFileManager */HandleRef selfPointer, 
                /* NSURL */IntPtr url, bool createIntermediates, 
                /* NSDictionary<NSFileAttributeKey, id> */IntPtr attributes, 
                /* NSError** */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_createDirectoryAtPath(
                /* NSFileManager */HandleRef selfPointer, 
                /* NSString */IntPtr path, bool createIntermediates, 
                /* NSDictionary<NSFileAttributeKey, id> */IntPtr attributes, 
                /* NSError** */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_createFileAtPath(
                /* NSFileManager */HandleRef selfPointer, 
                /* NSString */IntPtr path, 
                /* NSData */IntPtr data, 
                /* NSDictionary<NSFileAttributeKey, id> */IntPtr attr);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_removeItemAtURL(
                /* NSFileManager */HandleRef selfPointer, 
                /* NSURL */IntPtr url, 
                /* NSError** */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_removeItemAtPath(
                /* NSFileManager */HandleRef selfPointer, 
                /* NSString */IntPtr path, 
                /* NSError** */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_trashItemAtURL(
                /* NSFileManager */HandleRef selfPointer, 
                /* NSURL */IntPtr url, 
                /* NSURL** */[In, Out] ref IntPtr outResultingURL, 
                /* NSError** */[In, Out] ref IntPtr error);

            // Replacing Items
            [DllImport("__Internal")]
            internal static extern bool NSFileManager_replaceItemAtURL(
                /* NSFileManager */HandleRef selfPointer,
                /* NSURL */IntPtr originalItemURL,
                /* NSURL */IntPtr newItemURL,
                /* NSString */IntPtr backupItemName,
                                   NSFileManagerItemReplacementOptions options,
                /* NSURL** */[In, Out] ref IntPtr resultingURL,
                /* NSError** */[In, Out] ref IntPtr error);

            // Moving and Copying Items
            [DllImport("__Internal")]
            internal static extern bool NSFileManager_copyItemAtURL(
                /* NSFileManager */HandleRef selfPointer,
                /* NSURL */IntPtr srcURL,
                /* NSURL */IntPtr dstURL,
                /* NSError** */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_copyItemAtPath(
                /* NSFileManager */HandleRef selfPointer,
                /* NSString */IntPtr srcPath,
                /* NSString */IntPtr dstPath,
                /* NSError** */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_moveItemAtURL(
                /* NSFileManager */HandleRef selfPointer,
                /* NSURL */IntPtr srcURL,
                /* NSURL */IntPtr dstURL,
                /* NSError** */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_moveItemAtPath(
                /* NSFileManager */HandleRef selfPointer,
                /* NSString */IntPtr srcPath,
                /* NSString */IntPtr dstPath,
                /* NSError** */[In, Out] ref IntPtr error);

            // Managing iCloud-Based Items
            [DllImport("__Internal")]
            internal static extern /* id<NSObject, NSCopying, NSCoding> */IntPtr NSFileManager_ubiquityIdentityToken(/* NSFileManager */HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSFileManager_URLForUbiquityContainerIdentifier(
                /* NSFileManager */HandleRef selfPointer, 
                /* NSString */IntPtr containerIdentifier);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_isUbiquitousItemAtURL(/* NSFileManager */HandleRef selfPointer, /* NSURL */IntPtr url);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_setUbiquitousItemAtURL(
                /* NSFileManager */HandleRef selfPointer, bool flag,
                /* NSURL */IntPtr url,
                /* NSURL */IntPtr destinationURL,
                /* NSError** */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_startDownloadingUbiquitousItemAtURL(
                /* NSFileManager */HandleRef selfPointer,
                /* NSURL */IntPtr url,
                /* NSError** */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_evictUbiquitousItemAtURL(
                /* NSFileManager */HandleRef selfPointer,
                /* NSURL */IntPtr url,
                /* NSError** */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSFileManager_URLForPublishingUbiquitousItemAtURL(
                /* NSFileManager */HandleRef selfPointer,
                /* NSURL */IntPtr url,
                /* NSDate** */[In, Out] ref IntPtr outExpirationDate,
                /* NSError** */[In, Out] ref IntPtr error);

            // Determining Access to Files
            [DllImport("__Internal")]
            internal static extern bool NSFileManager_fileExistsAtPath(/* NSFileManager */HandleRef selfPointer, /* NSString */IntPtr path);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_fileExistsAtPathIsDirectory(
                /* NSFileManager */HandleRef selfPointer, 
                /* NSString */IntPtr path, 
                                   [In, Out]ref bool isDirectory);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_isReadableFileAtPath(/* NSFileManager */HandleRef selfPointer, /* NSString */IntPtr path);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_isWritableFileAtPath(/* NSFileManager */HandleRef selfPointer, /* NSString */IntPtr path);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_isExecutableFileAtPath(/* NSFileManager */HandleRef selfPointer, /* NSString */IntPtr path);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_isDeletableFileAtPath(/* NSFileManager */HandleRef selfPointer, /* NSString */IntPtr path);

            // Getting and Setting Attributes
            [DllImport("__Internal")]
            internal static extern /* NSArray<NSString *> */IntPtr NSFileManager_componentsToDisplayForPath(
                /* NSFileManager */HandleRef selfPointer, 
                /* NSString */IntPtr path);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSFileManager_displayNameAtPath(/* NSFileManager */HandleRef selfPointer, /* NSString */IntPtr path);

            [DllImport("__Internal")]
            internal static extern /* NSDictionary<NSFileAttributeKey, id> */IntPtr NSFileManager_attributesOfItemAtPath(
                /* NSFileManager */HandleRef selfPointer,
                /* NSString */IntPtr path,
                /* NSError** */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern /* NSDictionary<NSFileAttributeKey, id> */IntPtr NSFileManager_attributesOfFileSystemForPath(
                /* NSFileManager */HandleRef selfPointer,
                /* NSString */IntPtr path,
                /* NSError** */[In, Out] ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_setAttributesOfItemAtPath(
                /* NSFileManager */HandleRef selfPointer,
                /* NSDictionary<NSFileAttributeKey, id> */IntPtr attributes,
                /* NSString */IntPtr path,
                /* NSError** */[In, Out] ref IntPtr error);

            // Getting and Comparing File Contents
            [DllImport("__Internal")]
            internal static extern /* NSData */IntPtr NSFileManager_contentsAtPath(/* NSFileManager */HandleRef selfPointer, /* NSString */IntPtr path);

            [DllImport("__Internal")]
            internal static extern bool NSFileManager_contentsEqualAtPath(
                /* NSFileManager */HandleRef selfPointer, 
                /* NSString */IntPtr path1, 
                /* NSString */IntPtr path2);
        }

        #endregion
    }
}
#endif