using System.Linq;
using MonKey.Extensions;
using UnityEditor;
using UnityEngine;

namespace MonKey.Editor
{
    public static class DefaultValuesUtilities
    {
        /// <summary>
        /// Returns the active object from the selection
        /// </summary>
        /// <returns></returns>
        public static Object DefaultActiveObject()
        {
            Object first = null;
            if (Selection.objects.Length > 0)
                first = Selection.activeObject;

            return first;
        }

        public static GameObject DefaultActiveGameObject()
        {
            GameObject first = null;
            if (Selection.gameObjects.Length > 0)
                first = Selection.activeGameObject;

            return first;
        }

        public static Transform DefaultActiveTransform()
        {
            Transform first = null;
            if (Selection.gameObjects.Length > 0)
                first = Selection.activeTransform;

            return first;
        }

        public static GameObject DefaultFirstGameObjectSelected()
        {
            if (MonkeyEditorUtils.OrderedSelectedGameObjects.Any())
                return MonkeyEditorUtils.OrderedSelectedGameObjects.First();
            return null;
        }

        public static Object DefaultFirstObjectSelected()
        {
            if (MonkeyEditorUtils.OrderedSelectedObjects.Any())
                return MonkeyEditorUtils.OrderedSelectedObjects.First();
            return null;
        }

        public static Transform DefaultFirstTransformSelected()
        {
            if (MonkeyEditorUtils.OrderedSelectedTransform.Any())
                return MonkeyEditorUtils.OrderedSelectedTransform.First();
            return null;
        }

        public static GameObject DefaultSecondGameObjectSelected()
        {
            if (MonkeyEditorUtils.OrderedSelectedGameObjects.Count() < 2)
                return null;

            return MonkeyEditorUtils.OrderedSelectedGameObjects.ElementAt(1);
        }

        public static Object DefaultSecondObjectSelected()
        {
            if (MonkeyEditorUtils.OrderedSelectedObjects.Count() < 2)
                return null;

            return MonkeyEditorUtils.OrderedSelectedObjects.ElementAt(1);
        }

        public static Transform DefaultSecondTransformSelected()
        {
            if (MonkeyEditorUtils.OrderedSelectedTransform.Count() < 2)
                return null;
            return MonkeyEditorUtils.OrderedSelectedTransform.ElementAt(1);
        }

        public static GameObject DefaultLastGameObjectSelected()
        {
            return MonkeyEditorUtils.OrderedSelectedGameObjects.Last();
        }

        public static Object DefaultLastObjectSelected()
        {
            return MonkeyEditorUtils.OrderedSelectedObjects.Last();
        }

        public static Transform DefaultLastTransformSelected()
        {
            return MonkeyEditorUtils.OrderedSelectedTransform.Last();
        }

        public static Object[] DefaultAllObjectSelected()
        {
            return Selection.objects;
        }

        public static GameObject[] DefaultAllGameObjectSelected()
        {
            return Selection.gameObjects;
        }

        public static Transform[] DefaultAllTransformSelected()
        {
            return Selection.gameObjects.Convert(_ => _.transform).ToArray();
        }

        public static Object[] DefaultAllObjectSelectedButFirst()
        {
            if (MonkeyEditorUtils.OrderedSelectedObjects.Count() <= 1)
                return null;

            return MonkeyEditorUtils.OrderedSelectedObjects.Skip(1).ToArray();
        }

        public static GameObject[] DefaultAllGameObjectSelectedButFirst()
        {
            if (MonkeyEditorUtils.OrderedSelectedGameObjects.Count() <= 1)
                return null;

            return MonkeyEditorUtils.OrderedSelectedGameObjects.Skip(1).ToArray();
        }

        public static Transform[] DefaultAllTransformSelectedButFirst()
        {
            if (MonkeyEditorUtils.OrderedSelectedTransform.Count() <= 1)
                return null;

            return MonkeyEditorUtils.OrderedSelectedTransform.Skip(1).ToArray();
        }

    }
}
