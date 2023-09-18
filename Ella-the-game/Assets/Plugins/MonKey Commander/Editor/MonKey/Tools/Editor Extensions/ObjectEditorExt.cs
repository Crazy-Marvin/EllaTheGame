
using MonKey.Extensions;
using UnityEditor;
using UnityEngine;

namespace MonKey
{
    public static class ObjectEditorExt
    {
        public static bool IsDirectory(this Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            return !path.IsNullOrEmpty() && !path.Contains(".");
        }

        private static readonly string temptTermForRename = " - Monkey Asset Name Duplicate";

        public static void RenameObjectAndAsset(this Object o, string newName)
        {
            o.name = o.name.Replace(temptTermForRename, "");
            newName = newName.Replace(temptTermForRename, "");
            string path = AssetDatabase.GetAssetPath(o);

            string newPath = path.Replace(o.name + ".", newName + ".");
            Object oldAtPath = AssetDatabase.LoadAssetAtPath<Object>(newPath);
            if (oldAtPath && oldAtPath != o)
                AssetDatabase.RenameAsset(newPath, oldAtPath.name + temptTermForRename);

            o.name = newName;

            if (AssetDatabase.Contains(o))
            {
                AssetDatabase.RenameAsset(path, o.name);
            }
        }
    }
}
