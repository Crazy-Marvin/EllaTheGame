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
    /// An object that represents the location of a resource, such as an item on a remote server or the path to a local file.
    /// </summary>
    internal class NSURL : iOSObjectProxy
    {
        private NSString mAbsoluteString;
        private NSURL mAbsoluteURL;
        private NSURL mBaseURL;
        private NSString mFragment;
        private NSString mHost;
        private NSString mLastPathComponent;
        private NSString mParameterString;
        private NSString mPassword;
        private NSString mPath;
        private NSArray<NSString> mPathComponents;
        private NSString mPathExtension;
        private NSString mPort;
        private NSString mQuery;
        private NSString mRelativePath;
        private NSString mRelativeString;
        private NSString mResourceSpecifier;
        private NSString mScheme;
        private NSString mStandardizedURL;
        private NSString mUser;
        private NSURL mFilePathURL;
        private NSURL mURLByDeletingLastPathComponent;
        private NSURL mURLByDeletingPathExtension;
        private NSURL mURLByStandardizingPath;

        internal NSURL(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Creates a NSURL object from a pointer and release that pointer if required.
        /// </summary>
        /// <returns>The NSURL object.</returns>
        /// <param name="pointer">Pointer.</param>
        /// <param name="releasePointer">If set to <c>true</c> release pointer.</param>
        internal static NSURL FromPointer(IntPtr pointer, bool releasePointer)
        {
            if (PInvokeUtil.IsNotNull(pointer))
            {
                var newURL = new NSURL(pointer);
                if (releasePointer)
                    CFFunctions.CFRelease(pointer);
                return newURL;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates and returns an NSURL object initialized with a provided URL string.
        /// </summary>
        /// <returns>The with string.</returns>
        /// <param name="URLString">URL string.</param>
        public static NSURL URLWithString(NSString URLString)
        {
            return URLString == null ? null : FromPointer(C.NSURL_URLWithString(URLString.ToPointer()), true);
        }

        /// <summary>
        /// Creates and returns an NSURL object initialized with a base URL and a relative string.
        /// </summary>
        /// <returns>The with string relative to UR.</returns>
        /// <param name="URLString">URL string.</param>
        /// <param name="baseURL">Base UR.</param>
        public static NSURL URLWithStringRelativeToURL(NSString URLString, NSURL baseURL)
        {
            if (URLString == null || baseURL == null)
                return null;

            return FromPointer(C.NSURL_URLWithStringRelativeToURL(URLString.ToPointer(), baseURL.ToPointer()), true);
        }

        /// <summary>
        /// Initializes and returns a newly created NSURL object as a file URL with a specified path.
        /// </summary>
        /// <returns>The URL with path.</returns>
        /// <param name="path">Path.</param>
        public static NSURL FileURLWithPath(NSString path)
        {
            return path == null ? null : FromPointer(C.NSURL_fileURLWithPath(path.ToPointer()), true);
        }

        /// <summary>
        /// Initializes and returns a newly created NSURL object as a file URL with a specified path.
        /// </summary>
        /// <returns>The URL with path.</returns>
        /// <param name="path">Path.</param>
        /// <param name="isDirectory">If set to <c>true</c> is directory.</param>
        public static NSURL FileURLWithPath(NSString path, bool isDirectory)
        {
            return path == null ? null : FromPointer(C.NSURL_fileURLWithPathIsDirectory(path.ToPointer(), isDirectory), true);
        }

        public static NSURL FileURLWithPathRelativeToURL(NSString path, NSURL baseURL)
        {
            if (path == null || baseURL == null)
                return null;

            return FromPointer(C.NSURL_fileURLWithPathRelativeToURL(path.ToPointer(), baseURL.ToPointer()), true);
        }

        /// <summary>
        /// Files the URL with path relative to UR.
        /// </summary>
        /// <returns>The URL with path relative to UR.</returns>
        /// <param name="path">Path.</param>
        /// <param name="isDirectory">If set to <c>true</c> is directory.</param>
        /// <param name="baseURL">Base UR.</param>
        public static NSURL FileURLWithPathRelativeToURL(NSString path, bool isDirectory, NSURL baseURL)
        {
            if (path == null || baseURL == null)
                return null;
                 
            return FromPointer(C.NSURL_fileURLWithPathIsDirectoryRelativeToURL(path.ToPointer(), isDirectory, baseURL.ToPointer()), true);
        }

        /// <summary>
        /// A boolean value that determines whether the receiver is a file URL.
        /// </summary>
        /// <value><c>true</c> if file URL; otherwise, <c>false</c>.</value>
        public bool FileURL
        {
            get { return C.NSURL_isFileURL(SelfPtr()); }
        }

        /// <summary>
        /// The URL string for the receiver as an absolute URL.
        /// </summary>
        /// <value>The absolute string.</value>
        public NSString AbsoluteString
        {
            get
            {
                if (mAbsoluteString == null)
                {
                    IntPtr strPtr = C.NSURL_absoluteString(SelfPtr());
                    mAbsoluteString = new NSString(strPtr);
                    CoreFoundation.CFFunctions.CFRelease(strPtr);
                }

                return mAbsoluteString;
            }
        }

        /// <summary>
        /// An absolute URL that refers to the same resource as the receiver.
        /// </summary>
        /// <value>The absolute UR.</value>
        public NSURL AbsoluteURL
        {
            get
            {
                if (mAbsoluteURL == null)
                    mAbsoluteURL = FromPointer(C.NSURL_absoluteURL(SelfPtr()), true);

                return mAbsoluteURL;
            }
        }

        /// <summary>
        /// The base URL.
        /// </summary>
        /// <value>The base UR.</value>
        public NSURL BaseURL
        {
            get
            {
                if (mBaseURL == null)
                    mBaseURL = FromPointer(C.NSURL_baseURL(SelfPtr()), true);

                return mBaseURL;
            }
        }

        /// <summary>
        /// The fragment identifier, conforming to RFC 1808. (read-only)
        /// </summary>
        /// <value>The fragment.</value>
        public NSString Fragment
        {
            get
            {
                if (mFragment == null)
                    mFragment = NSString.FromPointer(C.NSURL_fragment(SelfPtr()), true);

                return mFragment;
            }
        }

        /// <summary>
        /// The host, conforming to RFC 1808. (read-only)
        /// </summary>
        /// <value>The host.</value>
        public NSString Host
        {
            get
            {
                if (mHost == null)
                    mHost = NSString.FromPointer(C.NSURL_host(SelfPtr()), true);

                return mHost;
            }
        }

        /// <summary>
        /// The last path component. (read-only)
        /// </summary>
        /// <value>The last path component.</value>
        public NSString LastPathComponent
        {
            get
            {
                if (mLastPathComponent == null)
                    mLastPathComponent = NSString.FromPointer(C.NSURL_lastPathComponent(SelfPtr()), true);

                return mLastPathComponent;
            }
        }

        /// <summary>
        /// The parameter string conforming to RFC 1808. (read-only)
        /// </summary>
        /// <value>The parameter string.</value>
        public NSString ParameterString
        {
            get
            {
                if (mParameterString == null)
                    mParameterString = NSString.FromPointer(C.NSURL_parameterString(SelfPtr()), true);

                return mParameterString;
            }
        }

        /// <summary>
        /// The password conforming to RFC 1808. (read-only)
        /// </summary>
        /// <value>The password.</value>
        public NSString Password
        {
            get
            {
                if (mPassword == null)
                    mPassword = NSString.FromPointer(C.NSURL_password(SelfPtr()), true);

                return mPassword;
            }
        }

        /// <summary>
        /// The path, conforming to RFC 1808. (read-only)
        /// </summary>
        /// <value>The path.</value>
        public NSString Path
        {
            get
            {
                if (mPath == null)
                    mPath = NSString.FromPointer(C.NSURL_path(SelfPtr()), true);

                return mPath;
            }
        }

        /// <summary>
        /// An array containing the path components. (read-only)
        /// </summary>
        /// <value>The path components.</value>
        public NSArray<NSString> PathComponents
        {
            get
            {
                if (mPathComponents == null)
                    mPathComponents = NSArray<NSString>.FromPointer(C.NSURL_pathComponents(SelfPtr()), true);

                return mPathComponents;
            }
        }

        /// <summary>
        /// The path extension. (read-only)
        /// </summary>
        /// <value>The path extension.</value>
        public NSString PathExtension
        {
            get
            {
                if (mPathExtension == null)
                    mPathExtension = NSString.FromPointer(C.NSURL_pathExtension(SelfPtr()), true);

                return mPathExtension;
            }
        }

        /// <summary>
        /// The query string, conforming to RFC 1808.
        /// </summary>
        /// <value>The query.</value>
        public NSString Query
        {
            get
            {
                if (mQuery == null)
                    mQuery = NSString.FromPointer(C.NSURL_query(SelfPtr()), true);

                return mQuery;
            }
        }

        /// <summary>
        /// The relative path, conforming to RFC 1808. (read-only)
        /// </summary>
        /// <value>The relative path.</value>
        public NSString RelativePath
        {
            get
            {
                if (mRelativePath == null)
                    mRelativePath = NSString.FromPointer(C.NSURL_relativePath(SelfPtr()), true);

                return mRelativePath;
            }
        }

        /// <summary>
        /// A string representation of the relative portion of the URL. (read-only)
        /// </summary>
        /// <value>The relative string.</value>
        public NSString RelativeString
        {
            get
            {
                if (mRelativeString == null)
                    mRelativeString = NSString.FromPointer(C.NSURL_relativeString(SelfPtr()), true);

                return mRelativeString;
            }
        }

        /// <summary>
        /// The resource specifier. (read-only)
        /// </summary>
        /// <value>The resource specifier.</value>
        public NSString ResourceSpecifier
        {
            get
            {
                if (mResourceSpecifier == null)
                    mResourceSpecifier = NSString.FromPointer(C.NSURL_resourceSpecifier(SelfPtr()), true);

                return mResourceSpecifier;
            }
        }

        /// <summary>
        /// The scheme. (read-only)
        /// </summary>
        /// <value>The scheme.</value>
        public NSString Scheme
        {
            get
            {
                if (mScheme == null)
                    mScheme = NSString.FromPointer(C.NSURL_scheme(SelfPtr()), true);

                return mScheme;
            }
        }

        /// <summary>
        /// A copy of the URL with any instances of ".." or "." removed from its path. (read-only)
        /// </summary>
        /// <value>The standardized UR.</value>
        public NSString StandardizedURL
        {
            get
            {
                if (mStandardizedURL == null)
                    mStandardizedURL = NSString.FromPointer(C.NSURL_standardizedURL(SelfPtr()), true);

                return mStandardizedURL;
            }
        }

        /// <summary>
        /// The user name, conforming to RFC 1808.
        /// </summary>
        /// <value>The user.</value>
        public NSString User
        {
            get
            {
                if (mUser == null)
                    mUser = NSString.FromPointer(C.NSURL_user(SelfPtr()), true);

                return mUser;
            }
        }

        /// <summary>
        /// A file path URL that points to the same resource as the URL object. (read-only)
        /// </summary>
        /// <value>The file path UR.</value>
        public NSURL FilePathURL
        {
            get
            {
                if (mFilePathURL == null)
                    mFilePathURL = FromPointer(C.NSURL_filePathURL(SelfPtr()), true);

                return mFilePathURL;
            }
        }

        /// <summary>
        /// Returns a new file reference URL that points to the same resource as the receiver.
        /// </summary>
        /// <returns>The reference UR.</returns>
        public NSURL FileReferenceURL()
        {
            return FromPointer(C.NSURL_fileReferenceURL(SelfPtr()), true);
        }

        /// <summary>
        /// Returns a new URL made by appending a path component to the original URL.
        /// </summary>
        /// <returns>The by appending path component.</returns>
        /// <param name="pathComponent">Path component.</param>
        public NSURL URLByAppendingPathComponent(NSString pathComponent)
        {
            return FromPointer(C.NSURL_URLByAppendingPathComponent(SelfPtr(), pathComponent != null ? pathComponent.ToPointer() : IntPtr.Zero), true);
        }

        /// <summary>
        /// Returns a new URL made by appending a path component to the original URL, along with a trailing slash if the component is designated a directory.
        /// </summary>
        /// <returns>The by appending path component.</returns>
        /// <param name="pathComponent">Path component.</param>
        /// <param name="isDirectory">If set to <c>true</c> is directory.</param>
        public NSURL URLByAppendingPathComponent(NSString pathComponent, bool isDirectory)
        {
            return FromPointer(C.NSURL_URLByAppendingPathComponentIsDirectory(SelfPtr(), pathComponent != null ? pathComponent.ToPointer() : IntPtr.Zero, isDirectory), true);
        }

        /// <summary>
        /// Returns a new URL made by appending a path extension to the original URL.
        /// </summary>
        /// <returns>The by appending path extension.</returns>
        /// <param name="pathExtension">Path extension.</param>
        public NSURL URLByAppendingPathExtension(NSString pathExtension)
        {
            return FromPointer(C.NSURL_URLByAppendingPathExtension(SelfPtr(), pathExtension != null ? pathExtension.ToPointer() : IntPtr.Zero), true);
        }

        /// <summary>
        /// A URL created by taking the receiver and removing the last path component. (read-only)
        /// </summary>
        /// <value>The URL by deleting last path component.</value>
        public NSURL URLByDeletingLastPathComponent
        {
            get
            {
                if (mURLByDeletingLastPathComponent == null)
                    mURLByDeletingLastPathComponent = FromPointer(C.NSURL_URLByDeletingLastPathComponent(SelfPtr()), true);

                return mURLByDeletingLastPathComponent;
            }
        }

        /// <summary>
        /// A URL created by taking the receiver and removing the path extension, if any. (read-only)
        /// </summary>
        /// <value>The URL by deleting path extension.</value>
        public NSURL URLByDeletingPathExtension
        {
            get
            {
                if (mURLByDeletingPathExtension == null)
                    mURLByDeletingPathExtension = FromPointer(C.NSURL_URLByDeletingPathExtension(SelfPtr()), true);

                return mURLByDeletingPathExtension;
            }
        }

        /// <summary>
        /// A URL that points to the same resource as the original URL using an absolute path. (read-only)
        /// </summary>
        /// <value>The URL by standardizing path.</value>
        public NSURL URLByStandardizingPath
        {
            get
            {
                if (mURLByStandardizingPath == null)
                    mURLByStandardizingPath = FromPointer(C.NSURL_URLByStandardizingPath(SelfPtr()), true);

                return mURLByStandardizingPath;
            }
        }

        public bool HasDirectoryPath
        {
            get
            {
                return C.NSURL_hasDirectoryPath(SelfPtr());
            }
        }

        public override string ToString()
        {
            return string.Format("[NSURL: AbsoluteString={0}]", AbsoluteString.UTF8String);
        }

        #region C Wrapper

        private static class C
        {
            // Creating an NSURL Object
            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_URLWithString(/* NSString */IntPtr URLString);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_URLWithStringRelativeToURL(/* NSString */IntPtr URLString, /* NSURL */IntPtr baseURL);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_fileURLWithPathIsDirectory(/* NSString */IntPtr path, bool isDir);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_fileURLWithPathRelativeToURL(/* NSString */IntPtr path, /* NSURL */IntPtr baseURL);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_fileURLWithPathIsDirectoryRelativeToURL(/* NSString */IntPtr path, bool isDir, /* NSURL */IntPtr baseURL);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_fileURLWithPath(/* NSString */IntPtr path);

            // Querying an NSURL
            [DllImport("__Internal")]
            internal static extern bool NSURL_isFileURL(HandleRef self);

            // Accessing the Parts of the URL
            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_absoluteString(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_absoluteURL(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_baseURL(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_fragment(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_host(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_lastPathComponent(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_parameterString(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_password(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_path(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSArray<NSString *> */IntPtr NSURL_pathComponents(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_pathExtension(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_query(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_relativePath(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_relativeString(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_resourceSpecifier(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_scheme(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_standardizedURL(HandleRef self);

            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSURL_user(HandleRef self);

            // Modifying and Converting a File URL
            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_filePathURL(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_fileReferenceURL(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_URLByAppendingPathComponent(HandleRef selfPtr, /* NSString */IntPtr pathComponent);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_URLByAppendingPathComponentIsDirectory(HandleRef selfPtr, /* NSString */IntPtr pathComponent, bool isDirectory);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_URLByAppendingPathExtension(HandleRef selfPtr, /* NSString */IntPtr pathExtension);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_URLByDeletingLastPathComponent(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_URLByDeletingPathExtension(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern /* NSURL */IntPtr NSURL_URLByStandardizingPath(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern bool NSURL_hasDirectoryPath(HandleRef selfPtr);
        }

        #endregion
    }
}
#endif