using UnityEngine;
using UnityEditor;
using System;
using System.Threading;

namespace EasyMobile.Editor
{
    public static class EM_AndroidLibBuilder
    {
        public class AndroidLibConfig
        {
            public string targetLibFolderName;
            public string targetContentFolderName;
            public string packageName;
            public int versionCode = 1;
            public string versionName = "1.0";
            public int minSdkVersion = 9;
            public int targetSdkVersion = 19;
        }

        private class AndroidLibBuildingJob
        {
            public AndroidLibConfig config;
            public string contentFolder;
            public Action<float> progressCallback;
            public Action<string> newStepCallback;
            public Action completeCallback;
            public Action<string> customManifestMergeProcess;
        }

        public const string AndroidLibTemplateFolder = "AndroidLibTemplate";
        public const string AndroidManifest_Package = "__AndroidManifest_Package__";
        public const string AndroidManifest_VersionCode = "__AndroidManifest_VersionCode__";
        public const string AndroidManifest_VersionName = "__AndroidManifest_VersionName__";
        public const string AndroidManifest_MinSdkVersion = "__AndroidManifest_MinSdkVersion__";
        public const string AndroidManifest_TargetSdkVersion = "__AndroidManifest_TargetSdkVersion__";

        public static void BuildAndroidLibFromFolder(string folderPath, AndroidLibConfig config, Action<float> progressCallback, Action<string> newStepCallback, Action completeCallback, Action<string> customManifestMergeProcess = null)
        {
            if (config == null || string.IsNullOrEmpty(config.targetLibFolderName) || string.IsNullOrEmpty(config.packageName))
            {
                Debug.LogWarning("Failed to build Android library: invalid config.");
                return;
            }
            /* Ignore empty resource folder
            if (string.IsNullOrEmpty(folderPath))
            {
                Debug.LogWarning("Failed to build Android library: invalid content folder.");
                return;
            }
            */
            var job = new AndroidLibBuildingJob();
            job.config = config;
            job.contentFolder = folderPath;
            job.progressCallback = progressCallback;
            job.newStepCallback = newStepCallback;
            job.completeCallback = completeCallback;
            job.customManifestMergeProcess = customManifestMergeProcess;

            /* Initially we intended to run the job on a worker thread,
             * however current implementation employs Unity's FileUtil methods
             * which cannot be called from a non-main thread, forcing us to move the
             * whole thing to the main thread. Given that the library folder is normally
             * small, this shouldn't be a problem.
             * /
            /*ThreadPool.QueueUserWorkItem(new WaitCallback(DoBuildAndroidLibFromFolder), job);*/

            DoBuildAndroidLibFromFolder(job);
        }

        private static void DoBuildAndroidLibFromFolder(System.Object data)
        {
            var job = data as AndroidLibBuildingJob;

            if (job == null)
            {
                Debug.LogWarning("Failed to build Android library: invalid job data.");
                return;
            }
                
            int totalSteps = 3;
            int currentStep = 0;

            // -----------------------------------------------------------
            // Step 1: Create an Android lib template folder at Assets/Plugins/Android.
            // -----------------------------------------------------------
            RaiseNewStepEvent(job.newStepCallback, "Copying library template");

            // Copy the template lib folder to the Plugins/Android folder, removing the old folder if it exists.
            string templateLibFolder = EM_Constants.TemplateFolder + "/" + AndroidLibTemplateFolder;
            string targetLibFolder = EM_Constants.AssetsPluginsAndroidFolder + "/" + job.config.targetLibFolderName + ".androidlib";
            if (!FileIO.FolderExists(EM_Constants.AssetsPluginsAndroidFolder))
            {
                FileIO.CreateFolder(EM_Constants.AssetsPluginsAndroidFolder);
            }
            FileUtil.ReplaceDirectory(templateLibFolder, targetLibFolder);

            // Remove all .meta files in the newly copied folder to force Unity generate new GUIDs.
            foreach (string fPath in FileIO.GetFiles(targetLibFolder, "*.meta"))
            {
                System.IO.File.Delete(fPath);
            }

            RaiseProgressEvent(job.progressCallback, (float)(++currentStep) / totalSteps);

            // -----------------------------------------------------------
            // Step 2: Update AndroidManifest.xml
            // -----------------------------------------------------------
            RaiseNewStepEvent(job.newStepCallback, "Updating AndroidManifest.xml");

            // Read manifest template and replace placeholders with actual values.
            string manifestPath = targetLibFolder + "/" + "AndroidManifest.xml";
            if (job.customManifestMergeProcess == null)
            {
                string manifestContent = FileIO.ReadFile(manifestPath);
                manifestContent = manifestContent.Replace(AndroidManifest_Package, job.config.packageName)
                    .Replace(AndroidManifest_VersionCode, job.config.versionCode.ToString())
                    .Replace(AndroidManifest_VersionName, job.config.versionName)
                    .Replace(AndroidManifest_MinSdkVersion, job.config.minSdkVersion.ToString())
                    .Replace(AndroidManifest_TargetSdkVersion, job.config.targetSdkVersion.ToString());

                // Write back manifest file.
                FileIO.WriteFile(manifestPath, manifestContent);
            }
            else
            {
                job.customManifestMergeProcess(manifestPath);
            }

            RaiseProgressEvent(job.progressCallback, (float)(++currentStep) / totalSteps);

            /*** Update project.properties if needed ***/

            // -----------------------------------------------------------
            // Step 3: Copy the content folder into place.
            // -----------------------------------------------------------
            RaiseNewStepEvent(job.newStepCallback, "Copying content folder into place");
            if (!String.IsNullOrEmpty(job.contentFolder))
            {
                // If a valid target content folder name was provided, use it. Otherwise keep the source folder name.
                string sourceContentFolderName = System.IO.Path.GetDirectoryName(job.contentFolder);
                string targetContentFolderName = string.IsNullOrEmpty(job.config.targetContentFolderName) ? sourceContentFolderName : job.config.targetContentFolderName;
                FileUtil.ReplaceDirectory(job.contentFolder, targetLibFolder + "/" + targetContentFolderName);
            }
            // Make Unity aware of the new folder.
            AssetDatabase.ImportAsset(targetLibFolder, ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);

            RaiseProgressEvent(job.progressCallback, (float)(++currentStep) / totalSteps);
            RaiseCompleteEvent(job.completeCallback);
        }

        private static void RaiseNewStepEvent(Action<string> callback, string step)
        {
            if (callback != null)
                callback(step);
        }

        private static void RaiseProgressEvent(Action<float> callback, float progress)
        {
            if (callback != null)
                callback(progress);
        }

        private static void RaiseCompleteEvent(Action callback)
        {
            if (callback != null)
                callback();
        }
    }
}

