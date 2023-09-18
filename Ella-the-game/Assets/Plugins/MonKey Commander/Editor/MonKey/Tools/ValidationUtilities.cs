using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MonKey
{
    public static class ValidationUtilities
    {

        public static MethodInfo GetValidationMethod(DefaultValidation validation)
        {
            switch (validation)
            {
                case DefaultValidation.NONE:
                    return null;
                case DefaultValidation.AT_LEAST_ONE_GAME_OBJECT:
                    return typeof(ValidationUtilities).GetMethod("AtLeastOneGameObject");
                case DefaultValidation.AT_LEAST_TWO_GAME_OBJECTS:
                    return typeof(ValidationUtilities).GetMethod("AtLeastTwoGameObjects");
                case DefaultValidation.AT_LEAST_ONE_OBJECT:
                    return typeof(ValidationUtilities).GetMethod("AtLeastOneObject");
                case DefaultValidation.AT_LEAST_TWO_OBJECTS:
                    return typeof(ValidationUtilities).GetMethod("AtLeastTwoObjects");
                case DefaultValidation.AT_LEAST_ONE_TRANSFORM:
                    return typeof(ValidationUtilities).GetMethod("AtLeastOneTransform");
                case DefaultValidation.AT_LEAST_TWO_TRANSFORMS:
                    return typeof(ValidationUtilities).GetMethod("AtLeastTwoTransforms");
                case DefaultValidation.IN_PLAY_MODE:
                    return typeof(ValidationUtilities).GetMethod("InPlayMode");
                case DefaultValidation.IN_EDIT_MODE:
                    return typeof(ValidationUtilities).GetMethod("InEditMode");
                case DefaultValidation.IN_PLAY_MODE_AT_LEAST_ONE_GAME_OBJECT:
                    return typeof(ValidationUtilities).GetMethod("InPlayModeAtLeastOneGameObject");
                case DefaultValidation.IN_PLAY_MODE_AT_LEAST_TWO_GAME_OBJECTS:
                    return typeof(ValidationUtilities).GetMethod("InPlayModeAtLeastTwoGameObjects");
                case DefaultValidation.IN_PLAY_MODE_AT_LEAST_ONE_OBJECT:
                    return typeof(ValidationUtilities).GetMethod("InPlayModeAtLeastOneObject");
                case DefaultValidation.IN_PLAY_MODE_AT_LEAST_TWO_OBJECTS:
                    return typeof(ValidationUtilities).GetMethod("InPlayModeAtLeastTwoObjects");
                case DefaultValidation.IN_PLAY_MODE_AT_LEAST_ONE_TRANSFORM:
                    return typeof(ValidationUtilities).GetMethod("InPlayModeAtLeastOneTransform");
                case DefaultValidation.IN_PLAY_MODE_AT_LEAST_TWO_TRANSFORMS:
                    return typeof(ValidationUtilities).GetMethod("InPlayModeAtLeastTwoTransforms");
                case DefaultValidation.IN_EDIT_MODE_AT_LEAST_ONE_GAME_OBJECT:
                    return typeof(ValidationUtilities).GetMethod("InEditModeAtLeastOneGameObject");
                case DefaultValidation.IN_EDIT_MODE_AT_LEAST_TWO_GAME_OBJECTS:
                    return typeof(ValidationUtilities).GetMethod("InEditModeAtLeastTwoGameObjects");
                case DefaultValidation.IN_EDIT_MODE_AT_LEAST_ONE_OBJECT:
                    return typeof(ValidationUtilities).GetMethod("InEditModeAtLeastOneObject");
                case DefaultValidation.IN_EDIT_MODE_AT_LEAST_TWO_OBJECTS:
                    return typeof(ValidationUtilities).GetMethod("InEditModeAtLeastTwoObjects");
                case DefaultValidation.IN_EDIT_MODE_AT_LEAST_ONE_TRANSFORM:
                    return typeof(ValidationUtilities).GetMethod("InEditModeAtLeastOneTransform");
                case DefaultValidation.IN_EDIT_MODE_AT_LEAST_TWO_TRANSFORMS:
                    return typeof(ValidationUtilities).GetMethod("InEditModeAtLeastTwoTransforms");
                case DefaultValidation.AT_LEAST_ONE_ASSET:
                    return typeof(ValidationUtilities).GetMethod("AtLeastOneAsset");
                case DefaultValidation.AT_LEAST_ONE_SCENE_OBJECT:
                    return typeof(ValidationUtilities).GetMethod("AtLeastOneSceneObject");
                case DefaultValidation.AT_LEAST_ONE_FOLDER:
                    return typeof(ValidationUtilities).GetMethod("AtLeastOneFolder");

                default:
                    throw new ArgumentOutOfRangeException("validation", validation, null);
            }
        }

        [CommandValidation("Select at least one GameObject")]
        public static bool AtLeastOneGameObject()
        {
            return Selection.gameObjects.Length > 0;
        }

        [CommandValidation("Select at least two GameObjects")]
        public static bool AtLeastTwoGameObjects()
        {
            return Selection.gameObjects.Length > 1;
        }

        [CommandValidation("Select at least one Object")]
        public static bool AtLeastOneObject()
        {
            return Selection.objects.Length > 0;
        }

        [CommandValidation("Select at least two Objects")]
        public static bool AtLeastTwoObjects()
        {
            return Selection.objects.Length > 1;
        }

        [CommandValidation("Select at least one Transform")]
        public static bool AtLeastOneTransform()
        {
            return Selection.transforms.Length > 0;
        }

        [CommandValidation("Select at least two Transforms")]
        public static bool AtLeastTwoTransforms()
        {
            return Selection.transforms.Length > 1;
        }

        [CommandValidation("Enter Play Mode first")]
        public static bool InPlayMode()
        {
            return Application.isPlaying;
        }

        [CommandValidation("Exit Play Mode first")]
        public static bool InEditMode()
        {
            return !Application.isPlaying;
        }

        [CommandValidation("Enter Play Mode and select at least one GameObject")]
        public static bool InPlayModeAtLeastOneGameObject()
        {
            return InPlayMode() && Selection.gameObjects.Length > 0;
        }

        [CommandValidation("Enter Play Mode and select at least two GameObjects")]
        public static bool InPlayModeAtLeastTwoGameObjects()
        {
            return InPlayMode() && Selection.gameObjects.Length > 1;
        }

        [CommandValidation("Enter Play Mode and select at least one Object")]
        public static bool InPlayModeAtLeastOneObject()
        {
            return InPlayMode() && Selection.objects.Length > 0;
        }

        [CommandValidation("Enter Play Mode and select at least two Objects")]
        public static bool InPlayModeAtLeastTwoObjects()
        {
            return InPlayMode() && Selection.objects.Length > 1;
        }

        [CommandValidation("Enter Play Mode and select at least one Transform")]
        public static bool InPlayModeAtLeastOneTransform()
        {
            return InPlayMode() && Selection.transforms.Length > 0;
        }

        [CommandValidation("Enter Play Mode and select at least two Transforms")]
        public static bool InPlayModeAtLeastTwoTransforms()
        {
            return InPlayMode() && Selection.transforms.Length > 1;
        }

        [CommandValidation("Enter Edit Mode and select at least one GameObject")]
        public static bool InEditModeAtLeastOneGameObject()
        {
            return InEditMode() && Selection.gameObjects.Length > 0;
        }

        [CommandValidation("Enter Edit Mode and select at least two GameObjects")]
        public static bool InEditModeAtLeastTwoGameObjects()
        {
            return InEditMode() && Selection.gameObjects.Length > 1;
        }

        [CommandValidation("Enter Edit Mode and select at least one Object")]
        public static bool InEditModeAtLeastOneObject()
        {
            return InEditMode() && Selection.objects.Length > 0;
        }

        [CommandValidation("Enter Edit Mode and select at least two Objects")]
        public static bool InEditModeAtLeastTwoObjects()
        {
            return InEditMode() && Selection.objects.Length > 1;
        }

        [CommandValidation("Enter Edit Mode and select at least one Transform")]
        public static bool InEditModeAtLeastOneTransform()
        {
            return InEditMode() && Selection.transforms.Length > 0;
        }

        [CommandValidation("Enter Edit Mode and select at least two Transforms")]
        public static bool InEditModeAtLeastTwoTransforms()
        {
            return InEditMode() && Selection.transforms.Length > 1;
        }

        [CommandValidation("Select at least one asset")]
        public static bool AtLeastOneAsset()
        {
            return Selection.objects.Length > 0
                   && Selection.objects.Any(_ => AssetDatabase.Contains(_) && !_.IsDirectory());
        }

        [CommandValidation("Select at least one scene object")]
        public static bool AtLeastOneSceneGameObject()
        {
            return Selection.gameObjects.Length > 0 && Selection.gameObjects.Any(_ => _.scene.IsValid());
        }

        [CommandValidation("Select at least one folder")]
        public static bool AtLeastOneFolder()
        {
            return Selection.objects.Length > 0 && Selection.objects.Any(_ => _.IsDirectory());
        }

    }
}
