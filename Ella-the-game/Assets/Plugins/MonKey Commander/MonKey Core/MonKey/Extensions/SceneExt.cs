using System.Collections.Generic;
using MonKey.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonKey.Extensions
{
    public static class SceneExt
    {
        public static List<Transform> GetRootTransformsOfAllOpenScene()
        {
            List<Transform> rootTransforms=new List<Transform>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                rootTransforms
                    .AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects().Convert(_=>_.transform));
            }

            return rootTransforms;
        }

    }
}
