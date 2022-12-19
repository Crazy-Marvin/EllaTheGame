using System.Collections.Generic;

namespace EasyMobile.Editor
{
    /// <summary>
    /// Abstract editor module management class.
    /// </summary>
    public abstract class ModuleManager
    {
        /// <summary>
        /// Call this when enable a module.
        /// </summary>
        public virtual void EnableModule()
        {
            InternalEnableModule();
        }

        /// <summary>
        /// Call this when disable a module.
        /// </summary>
        public virtual void DisableModule()
        {
            InternalDisableModule();
        }

        /// <summary>
        /// Gets the module managed by this class.
        /// </summary>
        /// <value>The self module.</value>
        public abstract Module SelfModule { get; }

        /// <summary>
        /// Gets the paths to the AndroidManifest.xml templates for this module.
        /// </summary>
        /// <value>The android manifest template paths.</value>
        public abstract List<string> AndroidManifestTemplatePaths { get; }

        /// <summary>
        /// Gets the android permissions holder.
        /// </summary>
        /// <value>The android permissions holder.</value>
        public abstract IAndroidPermissionRequired AndroidPermissionsHolder { get; }

        /// <summary>
        /// Gets the iOS Info.plist items holder.
        /// </summary>
        /// <value>The i OS info items holder.</value>
        public abstract IIOSInfoItemRequired iOSInfoItemsHolder { get; }

        /// <summary>
        /// Performs necessary editor-level actions when enabling the module, e.g. define symbols.
        /// </summary>
        protected abstract void InternalEnableModule();

        /// <summary>
        /// Performs necessary editor-level actions when disabling the module, e.g. undefine symbols.
        /// </summary>
        protected abstract void InternalDisableModule();
    }
}
