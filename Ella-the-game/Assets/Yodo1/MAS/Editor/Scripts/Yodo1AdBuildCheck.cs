namespace Yodo1.MAS
{
    using System;
    using UnityEngine;
    using System.IO;
    using UnityEditor.PackageManager.Requests;
    using UnityEditor.PackageManager;

    public static class Yodo1AdBuildCheck
    {
        public static bool HasConflict()
        {
            return IsAdmobAdsExist() || IsFirebaseExist() || IsUnityAdsExist() || IsFBSDKCoreKitExist();
        }

        public static bool IsUnityAdsExist()
        {
            string remove_package_name = "com.unity.ads";
            string manifest_path = Directory.GetCurrentDirectory() + "/Packages/manifest.json";
            if (!File.Exists(manifest_path))
            {
                return false;
            }
            string text = File.ReadAllText(manifest_path);
            return text.Contains(remove_package_name);
        }
        private static RemoveRequest Request;
        public static void RemoveUnityAds()
        {
            if (IsUnityAdsExist())
            {
                Request = Client.Remove("com.unity.ads");
                //EditorApplication.update += Progress;
            }
        }

        public static bool IsFBSDKCoreKitExist()
        {
            string dependencies_path = Directory.GetCurrentDirectory() + "/Assets/FacebookSDK/Plugins/Editor/Dependencies.xml";
            if (File.Exists(dependencies_path))
            {
                string line = null;
                string line_to_delete = "FBSDKCoreKit";
                using (StreamReader reader = new StreamReader(dependencies_path))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains(line_to_delete))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void RemoveFacebookCoreKit()
        {
            if (!IsFBSDKCoreKitExist())
            {
                return;
            }
            string dependencies_path = Directory.GetCurrentDirectory() + "/Assets/FacebookSDK/Plugins/Editor/Dependencies.xml";
            string new_dependencies_path = Directory.GetCurrentDirectory() + "/Assets/FacebookSDK/Plugins/Editor/Dependencies_new.xml";
            string line = null;
            string line_to_delete = "FBSDKCoreKit";
            using (StreamReader reader = new StreamReader(dependencies_path))
            {
                using (StreamWriter writer = new StreamWriter(new_dependencies_path))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains(line_to_delete))
                        {
                            continue;
                        }
                        writer.WriteLine(line);
                    }
                }
            }
            File.Delete(dependencies_path);
            File.Move(new_dependencies_path, dependencies_path);
        }

        public static bool IsAdmobAdsExist()
        {
            //AdmobAds Matching rules
            string admobPath1 = Path.GetFullPath(Application.dataPath + "/GoogleMobileAds");
            string admobPath2 = Path.GetFullPath(Application.dataPath + "/Plugins/Android/GoogleMobileAdsPlugin.androidlib");
            string admobPath3 = Path.GetFullPath(Application.dataPath + "/Plugins/Android/googlemobileads-unity.aar");
            string admobPath4 = Path.GetFullPath(Application.dataPath + "/Plugins/iOS/unity-plugin-library.a");

            return Directory.Exists(admobPath1) && ((Directory.Exists(admobPath2) || File.Exists(admobPath3)) || File.Exists(admobPath4));
        }

        public static void RemoveAdmobAds()
        {
            //AdmobAds Matching rules
            string admobPath1 = Path.GetFullPath(Application.dataPath + "/GoogleMobileAds");
            string admobPath2 = Path.GetFullPath(Application.dataPath + "/Plugins/Android/GoogleMobileAdsPlugin.androidlib");
            string admobPath3 = Path.GetFullPath(Application.dataPath + "/Plugins/Android/googlemobileads-unity.aar");
            string admobPath4 = Path.GetFullPath(Application.dataPath + "/Plugins/iOS/unity-plugin-library.a");

            // common
            DeleteDir(admobPath1);
            DeleteTempFile(admobPath1);

            // android
            DeleteDir(admobPath2);
            DeleteTempFile(admobPath2);


            DeleteFile(admobPath3);
            DeleteTempFile(admobPath3);

            // ios
            DeleteFile(admobPath4);
            DeleteTempFile(admobPath4);
        }

        public static bool IsFirebaseExist()
        {
            //Firebase Matching rules
            string firebaseDir1 = Path.GetFullPath(Application.dataPath + "/Firebase");
            string firebaseDir2 = Path.GetFullPath(Application.dataPath + "/Firebase/m2repository/com/google/firebase/firebase-app-unity");
            string firebasePath1 = Path.GetFullPath(Application.dataPath + "/Firebase/Plugins/iOS/Firebase.App.dll");

            return Directory.Exists(firebaseDir1) && (Directory.Exists(firebaseDir2) || File.Exists(firebasePath1));
        }

        public static void DeleteTempFile(string path)
        {
            string tempPaht = path + ".meta";
            try
            {
                if (File.Exists(tempPaht))
                {
                    File.Delete(tempPaht);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "DeleteTempFile error : tempPaht " + tempPaht + " error: " + e.Message.ToString());
            }
        }

        public static void DeleteFile(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "DeleteFile error : file " + file + " error: " + e.Message.ToString());
            }
        }

        public static void DeleteDir(string file)
        {
            if (!Directory.Exists(file))
            {
                return;
            }

            try
            {
                //Remove the read-only attribute
                System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
                fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;

                //Remove the read-only attribute
                System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);

                if (Directory.Exists(file))
                {
                    foreach (string f in Directory.GetFileSystemEntries(file))
                    {
                        if (File.Exists(f))
                        {
                            File.Delete(f);
                            Console.WriteLine(f);
                        }
                        else
                        {
                            //Delete subfolders recursively in a loop
                            DeleteDir(f);
                        }
                    }

                    //Delete empty dir
                    Directory.Delete(file);
                    Console.WriteLine(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}


