using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonKey
{
    public static class GameObjectEditorExt
    {
        public static bool IsPrefab(this GameObject obj)
        {
            return PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab ||
                   PrefabUtility.GetPrefabInstanceStatus(obj) != PrefabInstanceStatus.NotAPrefab;
        }

        public static GameObject InstantiateObjectAsInstance(this GameObject obj)
        {
            GameObject returnObj = null;
#if UNITY_2018_3_OR_NEWER
            GameObject prefabParent = (GameObject) PrefabUtility.GetCorrespondingObjectFromSource(obj);

#elif UNITY_2018_2_OR_NEWER
            Type pUtilityType = typeof(PrefabUtility);
            MethodInfo info = pUtilityType.GetMethod("GetCorrespondingObjectFromSource");
            //stupid ass reflection because of some method signature change in the last Unity
            GameObject prefabParent = info.Invoke(null,new object[]{obj}) as GameObject;
#else
            GameObject prefabParent = PrefabUtility.GetPrefabParent(obj) as GameObject;

#endif

            if (prefabParent)
            {
                returnObj = PrefabUtility.InstantiatePrefab(prefabParent) as GameObject;
                PrefabUtility.SetPropertyModifications(returnObj, PrefabUtility.GetPropertyModifications(obj));
            }
            else
            {
                returnObj = Object.Instantiate(obj);
            }

            return returnObj;
        }
    }
}