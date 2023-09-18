using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMobile.ManifestGenerator.Elements
{
    public enum AndroidManifestElementStyles
    {
        None = 0,

        /// <summary>
        /// Adds an action to an intent filter.
        /// </summary>
        Action,

        /// <summary>
        /// Declares an activity component.
        /// </summary>
        Activity,

        /// <summary>
        /// Declares an alias for an activity.
        /// </summary>
        ActivityAlias,

        /// <summary>
        /// The declaration of the application.
        /// </summary>
        Application,

        /// <summary>
        /// Adds a category name to an intent filter.
        /// </summary>
        Category,

        /// <summary>
        /// Adds a data specification to an intent filter.
        /// </summary>
        Data,

        /// <summary>
        /// Specifies the subsets of app data that the parent content provider has permission to access.
        /// </summary>
        GrantUriPermission,

        /// <summary>
        /// Declares an Instrumentation class that enables you to monitor an application's interaction with the system.
        /// </summary>
        Instrumentation,

        /// <summary>
        /// Specifies the types of intents that an activity, service, or broadcast receiver can respond to.
        /// </summary>
        IntentFilter,

        /// <summary>
        /// The root element of the AndroidManifest.xml file.
        /// </summary>
        Manifest,

        /// <summary>
        /// A name-value pair for an item of additional, arbitrary data that can be supplied to the parent component.
        /// </summary>
        MetaData,

        /// <summary>
        /// Defines the path and required permissions for a specific subset of data within a content provider.
        /// </summary>
        PathPermission,

        /// <summary>
        /// Declares a security permission that can be used to limit access 
        /// to specific components or features of this or other applications.
        /// </summary>
        Permission,

        /// <summary>
        /// Declares a name for a logical grouping of related permissionss.
        /// </summary>
        PermissionGroup,

        /// <summary>
        /// Declares the base name for a tree of permissions.
        /// </summary>
        PermissionTree,

        /// <summary>
        /// Declares a content provider component.
        /// </summary>
        Provider,

        /// <summary>
        /// Declares a broadcast receiver component.
        /// </summary>
        Receiver,

        /// <summary>
        /// Declares a service component.
        /// </summary>
        Service,

        /// <summary>
        /// Declares a single GL texture compression format that the app supports.
        /// </summary>
        SupportsGlTexture,

        /// <summary>
        /// Declares the screen sizes your app supports and enables screen compatibility mode 
        /// for screens larger than what your app supports
        /// </summary>
        SupportsScreens,

        /// <summary>
        /// Indicates specific input features the application requires.
        /// </summary>
        UsesConfiguration,

        /// <summary>
        /// Declares a single hardware or software feature that is used by the application.
        /// </summary>
        UsesFeature,

        /// <summary>
        /// Specifies a shared library that the application must be linked against.
        /// </summary>
        UsesLibrary,

        /// <summary>
        /// Specifies a system permission that the user must grant in order for the app to operate correctly.
        /// </summary>
        UsesPermission,

        /// <summary>
        /// Specifies that an app wants a particular permission,
        /// but only if the app is installed on a device running Android 6.0 (API level 23) or higher.
        /// </summary>
        UsesPermissionSdk23,

        /// <summary>
        /// Lets you express an application's compatibility with one or more versions of the Android platform,
        /// by means of an API level integer.
        /// </summary>
        UsesSdk,
    }

    public static class AndroidManifestElementStyleExtension
    {
        public static string ToAndroidManifestFormat(this AndroidManifestElementStyles elementStyle)
        {
            switch (elementStyle)
            {
                case AndroidManifestElementStyles.Action: return "action";

                case AndroidManifestElementStyles.Activity: return "activity";

                case AndroidManifestElementStyles.ActivityAlias: return "activity-alias";

                case AndroidManifestElementStyles.Application: return "application";

                case AndroidManifestElementStyles.Category: return "category";

                //case AndroidManifestElementStyles.CompatibleScreens: return "compatible-screens";

                case AndroidManifestElementStyles.Data: return "data";

                case AndroidManifestElementStyles.GrantUriPermission: return "grant-uri-permission";

                case AndroidManifestElementStyles.Instrumentation: return "instrumentation";

                case AndroidManifestElementStyles.IntentFilter: return "intent-filter";

                case AndroidManifestElementStyles.Manifest: return "manifest";

                case AndroidManifestElementStyles.MetaData: return "meta-data";

                case AndroidManifestElementStyles.PathPermission: return "path-permission";

                case AndroidManifestElementStyles.Permission: return "permission";

                case AndroidManifestElementStyles.PermissionGroup: return "permission-group";

                case AndroidManifestElementStyles.PermissionTree: return "permission-tree";

                case AndroidManifestElementStyles.Provider: return "provider";

                case AndroidManifestElementStyles.Receiver: return "receiver";

                case AndroidManifestElementStyles.Service: return "service";

                case AndroidManifestElementStyles.SupportsGlTexture: return "supports-gl-texture";

                case AndroidManifestElementStyles.SupportsScreens: return "supports-screens";

                case AndroidManifestElementStyles.UsesConfiguration: return "uses-configuration";

                case AndroidManifestElementStyles.UsesFeature: return "uses-feature";

                case AndroidManifestElementStyles.UsesLibrary: return "uses-library";

                case AndroidManifestElementStyles.UsesPermission: return "uses-permission";

                case AndroidManifestElementStyles.UsesPermissionSdk23: return "uses-permission-sdk-23";

                case AndroidManifestElementStyles.UsesSdk: return "uses-sdk";

                default: return "none";
            }
        }

        public static AndroidManifestElement CreateElementClass(this AndroidManifestElementStyles style)
        {
            switch (style)
            {
                case AndroidManifestElementStyles.Action: return new ActionElement();

                case AndroidManifestElementStyles.Activity: return new ActivityElement();
                    
                case AndroidManifestElementStyles.ActivityAlias: return new ActivityAliasElement();
                    
                case AndroidManifestElementStyles.Application: return new ApplicationElement();
                    
                case AndroidManifestElementStyles.Category: return new CategoryElement();
                    
                case AndroidManifestElementStyles.Data: return new DataElement();
                    
                case AndroidManifestElementStyles.GrantUriPermission: return new GrantUriPermissionElement();
                    
                case AndroidManifestElementStyles.Instrumentation: return new InstrumentationElement();
                    
                case AndroidManifestElementStyles.IntentFilter: return new IntentFilterElement();
                    
                case AndroidManifestElementStyles.Manifest: return new ManifestElement();
                        
                case AndroidManifestElementStyles.MetaData: return new MetaDataElement();
                    
                case AndroidManifestElementStyles.PathPermission: return new PathPermissionElement();
                    
                case AndroidManifestElementStyles.Permission: return new PermissionElement();
                    
                case AndroidManifestElementStyles.PermissionGroup: return new PermissionGroupElement();
                    
                case AndroidManifestElementStyles.PermissionTree: return new PermissionTreeElement();
                    
                case AndroidManifestElementStyles.Provider: return new ProviderElement();
                    
                case AndroidManifestElementStyles.Receiver: return new ReceiverElement();
                    
                case AndroidManifestElementStyles.Service: return new ServiceElement();
                    
                case AndroidManifestElementStyles.SupportsGlTexture: return new SupportsGlTextureElement();
                    
                case AndroidManifestElementStyles.SupportsScreens: return new SupportsScreensElement();
                    
                case AndroidManifestElementStyles.UsesConfiguration: return new UsesConfigurationElement();
                    
                case AndroidManifestElementStyles.UsesFeature: return new UsesFeatureElement();
                    
                case AndroidManifestElementStyles.UsesLibrary: return new UsesLibraryElement();
                    
                case AndroidManifestElementStyles.UsesPermission: return new UsesPermissionElement();
                    
                case AndroidManifestElementStyles.UsesPermissionSdk23: return new UsesPermissionSdk23Element();
                    
                case AndroidManifestElementStyles.UsesSdk: return new UsesSdkElement();
                    
                case AndroidManifestElementStyles.None:
                default:
                        return null;
            }
        }

        public static bool IsAndroidAttribute(this string attribute)
        {
            return attribute.StartsWith("android:");
        }
    }
}
