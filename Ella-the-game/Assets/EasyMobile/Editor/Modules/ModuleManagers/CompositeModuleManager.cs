using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Editor
{
    /// <summary>
    /// Abstract editor management class for module made up from multiple features (or submodules).
    /// </summary>
    public abstract class CompositeModuleManager : ModuleManager
    {
        #region ModuleManager Overriding

        /// <summary>
        /// Do not call this for composite module, use <see cref="EnableSubmodule"/> instead.
        /// </summary>
        public new void EnableModule()
        {
            Debug.Log("Composite module " + SelfModule.ToString() + " cannot be enabled wholly but by individual features.");
            throw new System.NotSupportedException();
        }

        /// <summary>
        /// Do not call this for composite module, use <see cref="DisableSubmodule"/> instead.
        /// </summary>
        public new void DisableModule()
        {
            // Composite module is not disabled wholly but by individual features.
            Debug.Log("Composite module " + SelfModule.ToString() + " cannot be disabled wholly but by individual features.");
            throw new System.NotSupportedException(); 
        }

        /// <summary>
        /// Gets the paths to the AndroidManifest.xml templates for this module.
        /// Always returns null for composite modules.
        /// Please use <see cref="AndroidManifestTemplatePathsForSubmodule"/> instead.
        /// </summary>
        /// <value>The android manifest template paths.</value>
        public override List<string> AndroidManifestTemplatePaths{ get { return null; } }

        /// <summary>
        /// Gets the android permissions holder. Always returns null for composite modules.
        /// Please use <see cref="AndroidPermissionHolderForSubmodule"/> instead.
        /// </summary>
        /// <value>The android permissions holder.</value>
        public override IAndroidPermissionRequired AndroidPermissionsHolder { get { return null; } }

        /// <summary>
        /// Gets the iOS Info.plist items holder. Always returns null for composite modules.
        /// Please use <see cref="iOSInfoItemsHolderForSubmodule"/> instead.
        /// </summary>
        /// <value>The i OS info items holder.</value>
        public override IIOSInfoItemRequired iOSInfoItemsHolder { get { return null; } }

        /// <summary>
        /// Performs necessary editor-level actions when enabling the module, e.g. define symbols.
        /// No need to register Android permissions or iOS Info.plist items here.
        /// </summary>
        protected override void InternalEnableModule()
        {
            // Nothing.
        }

        /// <summary>
        /// Performs necessary editor-level actions when disabling the module, e.g. undefine symbols.
        /// No need to unregister Android permissions or iOS Info.plist items here.
        /// </summary>
        protected override void InternalDisableModule()
        {
            // Nothing.
        }

        #endregion

        public abstract List<Submodule> SelfSubmodules { get; }

        public abstract List<string> AndroidManifestTemplatePathsForSubmodule(Submodule submod);

        public abstract IAndroidPermissionRequired AndroidPermissionHolderForSubmodule(Submodule submod);

        public abstract IIOSInfoItemRequired iOSInfoItemsHolderForSubmodule(Submodule submod);

        /// <summary>
        /// Call this to enable a submodule of this module.
        /// </summary>
        /// <param name="submod">Submod.</param>
        public virtual void EnableSubmodule(Submodule submod)
        {
            if (!SelfSubmodules.Contains(submod))
            {
                Debug.LogFormat("Submodule {0} doesn't belong to module {1}.", submod.ToString(), SelfModule.ToString());
                return;
            }

            InternalEnableSubmodule(submod);
        }

        /// <summary>
        /// Call this to disable a submodule of this module.
        /// </summary>
        /// <param name="submod">Submod.</param>
        public virtual void DisableSubmodule(Submodule submod)
        {
            if (!SelfSubmodules.Contains(submod))
            {
                Debug.LogFormat("Submodule {0} doesn't belong to module {1}.", submod.ToString(), SelfModule.ToString());
                return;
            }

            InternalDisableSubmodule(submod);
        }

        /// <summary>
        /// Performs any necessary tasks to enable a submodule.
        /// </summary>
        /// <param name="submod">Submod.</param>
        protected abstract void InternalEnableSubmodule(Submodule submod);

        /// <summary>
        /// Performs any necessary tasks to disable a submodule.
        /// </summary>
        /// <param name="submod">Submod.</param>
        protected abstract void InternalDisableSubmodule(Submodule submod);
    }
}
