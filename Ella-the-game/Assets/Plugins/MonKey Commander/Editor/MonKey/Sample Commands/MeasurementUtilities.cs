
using MonKey.Editor.Internal;
using MonKey.Extensions;
using MonKey.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace MonKey.Editor.Commands
{

    public static class MeasurementUtilities
    {
        [Command("Measure Distances Between",
            "Displays the distances between the selected objects in the scene view", AlwaysShow = true,
            DefaultValidation = DefaultValidation.AT_LEAST_TWO_TRANSFORMS, QuickName = "MCB",
            Category = "Measurement")]
        public static void ShowDistances(
            [CommandParameter(Help = "The color of the debug on the scene view",
                DefaultValueMethod = "DefaultDebugColor",
                DefaultValueNameOverride = "Debug Green")]
            Color debugColor)
        {
            MonkeyEditorUtils.
                AddSceneCommand(new DistanceShowingSceneCommand(Selection.gameObjects, debugColor, 0));
        }

        public static Color DefaultDebugColor()
        {
            // return ColorUtils.HTMLColor("#228B22");
            return Color.green;
        }


        public class DistanceShowingSceneCommand : TimedSceneCommand
        {
            private List<GameObject> objectsToMeasure;

            private readonly Color color;

            public DistanceShowingSceneCommand(GameObject[] objectsToMeasure, Color color,
                float duration)
                : base(duration)
            {

                SceneCommandName = "Measurement";

                this.objectsToMeasure = new List<GameObject>(objectsToMeasure);
                this.color = color;
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();

                DisplayObjectListOption("Objects to Measure", objectsToMeasure);

            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();

                objectsToMeasure.RemoveAll(_ => !_);

                for (int i = 0; i < objectsToMeasure.Count - 1; i++)
                {
                    GameObject gameObject = objectsToMeasure[i];

                    for (int j = i + 1; j < objectsToMeasure.Count; j++)
                    {
                        GameObject otherObject = objectsToMeasure[j];
                        if (otherObject == gameObject)
                            continue;
                        Handles.color = color;

                        Handles.DrawDottedLine(gameObject.transform.position,
                            otherObject.transform.position, 10);

                        Vector3 distance = gameObject.transform.position - otherObject.transform.position;

                        Handles.Label((gameObject.transform.position + otherObject.transform.position) / 2,
                            "Distance: " + distance.magnitude, new GUIStyle()
                            {
                                normal = { textColor = color ,
                                    background = MonkeyStyle.Instance.WindowBackgroundTex}
                            });

                        Handles.color = Color.white;
                    }
                }
            }
        }

        [Command("Measure Collision Distance", QuickName = "MCD",
            Help = "Displays the distance of objects to any collision" +
            " in the axis specified (default down)",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "Measurement")]
        public static void MeasureCollisionDistance(
           [CommandParameter(Help = "The color of the debug on the scene view",
                DefaultValueMethod = "DefaultDebugColor",
                DefaultValueNameOverride = "Debug Green")]
            Color debugColor)
        {
            MonkeyEditorUtils.
                AddSceneCommand(new DistanceAxisShowingSceneCommand(
                    Selection.gameObjects, debugColor, Vector3.down, false, 0));
        }

        [Command("Measure Collision Distance 2D", QuickName = "MCD2",
            Help = "Displays the distance of  2d objects to any collision" +
                   " in the axis specified (default down)",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "Measurement")]
        public static void MeasureCollisionDistance2D(
            [CommandParameter(Help = "The color of the debug on the scene view",
                DefaultValueMethod = "DefaultDebugColor",
                DefaultValueNameOverride = "Debug Green")]
            Color debugColor)
        {
            MonkeyEditorUtils.
                AddSceneCommand(new DistanceAxisShowingSceneCommand2D(
                    Selection.gameObjects, debugColor, Vector3.down, false, 0));
        }

        [Command("Show Position Handles",
            "Shows handles on all the specified objects (by default the selection)", QuickName = "SH",
            Category = "Measurement")]
        public static void ShowHandles(
            [CommandParameter("The Objects to show the handle on",
                DefaultValueMethod = "SelectionDefault",
                ForceAutoCompleteUsage = true)]
            GameObject[] objects)
        {
            MonkeyEditorUtils.AddSceneCommand(new TransformHandleShowingSceneCommand(objects));
        }

        private static GameObject[] SelectionDefault()
        {
            return Selection.gameObjects;
        }

        [Command("Measure Collision Distance All Axis",
            "Displays the distance of an objects to any collision" +
            " in all axis (global by default)", QuickName = "MCA",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "Measurement")]
        public static void MeasureAllCollisionDistance(
            [CommandParameter(Help = "if true, considers the local axis instead" +
                                     " of the global one",
                DefaultValueMethod = "DefaultLocal",DefaultValueNameOverride = "Global")]
            AxisReference localAxisReference)
        {
            MonkeyEditorUtils.
                AddSceneCommand(new DistanceAxisShowingSceneCommand(
                    Selection.gameObjects, Color.green.DarkerBrighter(-0.2f), Vector3.up, localAxisReference == AxisReference.LOCAL, 0));
            MonkeyEditorUtils.
                AddSceneCommand(new DistanceAxisShowingSceneCommand(
                    Selection.gameObjects, Color.green.DarkerBrighter(-0.2f), Vector3.down, localAxisReference == AxisReference.LOCAL, 0));
            MonkeyEditorUtils.
                AddSceneCommand(new DistanceAxisShowingSceneCommand(
                    Selection.gameObjects, Color.blue, Vector3.forward, localAxisReference == AxisReference.LOCAL, 0));
            MonkeyEditorUtils.
                AddSceneCommand(new DistanceAxisShowingSceneCommand(
                    Selection.gameObjects, Color.blue, Vector3.back, localAxisReference == AxisReference.LOCAL, 0));
            MonkeyEditorUtils.
                AddSceneCommand(new DistanceAxisShowingSceneCommand(
                    Selection.gameObjects, Color.red, Vector3.left, localAxisReference == AxisReference.LOCAL, 0));
            MonkeyEditorUtils.
                AddSceneCommand(new DistanceAxisShowingSceneCommand(
                    Selection.gameObjects, Color.red, Vector3.right, localAxisReference == AxisReference.LOCAL, 0));
        }

        private static AxisReference DefaultLocal()
        {
            return AxisReference.GLOBAL;
        }

        private static Vector3 DefaultAxis()
        {
            return Vector3.down;
        }

        public class TransformHandleShowingSceneCommand : TimedSceneCommand
        {
            private GameObject[] objectsToHandle;

            public TransformHandleShowingSceneCommand(GameObject[] objectsToHandle) : base(0)
            {
                this.objectsToHandle = objectsToHandle;
                SceneCommandName = "Handles";
                HideGUI = true;
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();

                foreach (GameObject gameObject in objectsToHandle)
                {
                    if (!gameObject)
                        continue;

                    Vector3 position = Handles.DoPositionHandle(gameObject.transform.position,
                        gameObject.transform.localRotation);
                    Handles.Label(gameObject.transform.position + Vector3.up, gameObject.name);

                    if ((position - gameObject.transform.position).magnitude > Mathf.Epsilon)
                    {
                        Undo.RecordObject(gameObject.transform, "move");
                    }
                    gameObject.transform.position = position;

                }

            }

        }

        public class DistanceAxisShowingSceneCommand : TimedSceneCommand
        {
            private List<GameObject> objectsToMeasure;
            private Vector3 axis;
            private bool local;
            private Color color;

            public DistanceAxisShowingSceneCommand(GameObject[] objectsToMeasure,
                Color color, Vector3 axis, bool local, float duration)
                : base(duration)
            {

                SceneCommandName = "Distance Measurement";

                this.objectsToMeasure = objectsToMeasure.ToList();
                this.color = color;
                this.local = local;
                this.axis = axis;
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();

                DisplayVectorOption("Direction", ref axis);
                DisplayBoolOption("Local Axis", ref local);
                DisplayObjectListOption("Objects To Measure", objectsToMeasure);
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();

                objectsToMeasure = objectsToMeasure.Where(_ => _).ToList();

                for (int i = 0; i < objectsToMeasure.Count; i++)
                {
                    GameObject gameObject = objectsToMeasure[i];

                    if (!gameObject)
                        return;

                    Ray ray = new Ray(gameObject.transform.position,
                        (local) ? gameObject.transform.TransformDirection(axis) : axis);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        Handles.color = color;
#if UNITY_2017_1_OR_NEWER
                        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
#endif
                        Handles.DrawDottedLine(gameObject.transform.position,
                            hit.point, 5);
                        Handles.DrawWireDisc(hit.point, hit.normal, 0.5f);

#if UNITY_2017_1_OR_NEWER
                        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
#endif
                        Vector3 distance = gameObject.transform.position - hit.point;
                        Handles.Label((gameObject.transform.position + hit.point) / 2,
                            "Distance: " + distance.magnitude, new GUIStyle()
                            {
                                margin = new RectOffset(2, 2, 2, 2),
                                normal =
                                {
                                    textColor = color,
                                    background = MonkeyStyle.Instance.WindowBackgroundTex
                                }
                            });

                        Handles.color = color.Inverted();

#if UNITY_2017_1_OR_NEWER
                        Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
#endif
                        Handles.DrawDottedLine(gameObject.transform.position,
                            hit.point, 5);
                        Handles.DrawWireDisc(hit.point + hit.normal * 0.1f, hit.normal, 0.5f);

                        Handles.color = Color.white;
                    }
                }
            }
        }

        public class DistanceAxisShowingSceneCommand2D : TimedSceneCommand
        {
            private List<GameObject> objectsToMeasure;
            private Vector2 axis;
            private bool local;
            private Color color;

            public DistanceAxisShowingSceneCommand2D(GameObject[] objectsToMeasure,
                Color color, Vector3 axis, bool local, float duration)
                : base(duration)
            {

                SceneCommandName = "Distance Measurement";

                this.objectsToMeasure = objectsToMeasure.ToList();
                this.color = color;
                this.local = local;
                this.axis = axis;
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();

                DisplayVector2Option("Direction", ref axis);
                DisplayBoolOption("Local Axis", ref local);
                DisplayObjectListOption("Objects To Measure", objectsToMeasure);
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();

                objectsToMeasure = objectsToMeasure.Where(_ => _).ToList();

                for (int i = 0; i < objectsToMeasure.Count; i++)
                {
                    GameObject gameObject = objectsToMeasure[i];

                    if (!gameObject)
                        return;


                    RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position,
                        (local) ? (Vector2)gameObject.transform.TransformDirection(axis) : axis);

                    if (hit.collider != null)
                    {
                        Handles.color = color;
#if UNITY_2017_1_OR_NEWER
                        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
#endif
                        Handles.DrawDottedLine(gameObject.transform.position,
                            hit.point, 5);
                        Handles.DrawWireDisc(hit.point, hit.normal, 0.5f);

#if UNITY_2017_1_OR_NEWER
                        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
#endif
                        Vector2 distance = (Vector2)gameObject.transform.position - hit.point;
                        Handles.Label(((Vector2)gameObject.transform.position + hit.point) / 2,
                            "Distance: " + distance.magnitude, new GUIStyle()
                            {
                                margin = new RectOffset(2, 2, 2, 2),
                                normal =
                                {
                                    textColor = color,
                                    background = MonkeyStyle.Instance.WindowBackgroundTex
                                }
                            });

                        Handles.color = color.Inverted();

#if UNITY_2017_1_OR_NEWER
                        Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
#endif
                        Handles.DrawDottedLine(gameObject.transform.position,
                            hit.point, 5);
                        Handles.DrawWireDisc(hit.point + hit.normal * 0.1f, hit.normal, 0.5f);

                        Handles.color = Color.white;
                    }
                }
            }
        }


        [Command("Count Selected",
            "Counts the amount of objects selected and outputs the result in the console",
            QuickName = "COS",
            Category = "Measurement")]
        public static void CountSelectedObject()
        {
            Debug.LogFormat("MonKey Counted:".Colored(MonkeyStyle.Instance.SelectedResultFieldColor).Bold() +
                            "\n {0} Objects Selected" +
                            ", {1} GameObjects Selected " +
                            ", {2} Transforms Selected", Selection.objects.Length,
                Selection.gameObjects.Length,
                Selection.transforms.Length);
        }

        [Command("Randomize Scale",
            "Randomizes the local scale of the selected objects " +
            "within a specified range (by default .8 to 1.2)", QuickName = "RAS",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            IgnoreHotKeyConflict = true,
            MenuItemLink = "RandomizeScale", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform/Set")]
        public static void RandomizeScale()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new InteractiveRandomScale(Selection.gameObjects));
        }

        [Command("Set Local Scale", "Set the specified scale on the selected objects",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, QuickName = "SLS",
            MenuItemLink = "SetLocalScale", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform/Set")]
        public static void SetScale(
            [CommandParameter(Help = "The local scale to set on the selected objects")]
            Vector3 scale)
        {
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                gameObject.transform.localScale = scale;
            }
        }

        [Command("Set Local Rotation", "Set the specified rotation on the selected objects",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, QuickName = "SLR",
            MenuItemLink = "SetLocalRotation", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform/Set")]
        public static void SetRotation(
            [CommandParameter(Help = "The local rotation to set on the selected objects")]
            Vector3 rotation)
        {
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                gameObject.transform.localRotation = Quaternion.Euler(rotation);
            }
        }

        [Command("Set Local Position", "Set the specified rotation on the selected objects",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, QuickName = "SLP",
            MenuItemLink = "SetLocalPosition", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform/Set")]
        public static void SetPosition(
            [CommandParameter(Help = "The local position to set on the selected objects")]
            Vector3 position)
        {
            int id = MonkeyEditorUtils.CreateUndoGroup("MonKey : Local Position Set");
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                Undo.RecordObject(gameObject.transform, " tr");
                gameObject.transform.localPosition = position;
            }
            Undo.CollapseUndoOperations(id);
        }


        private static Vector3 OneVector()
        {
            return Vector3.one;
        }


        [Command("Randomize Rotation",
            "Randomizes the local rotation of the selected objects " +
            "within a specified range (by default 0 to 360)", QuickName = "RAR",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            IgnoreHotKeyConflict = true,
            MenuItemLink = "RandomizeRotation", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform/Set")]
        public static void RandomizeRotation()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new InteractiveRandomRotation(Selection.gameObjects));
        }


        [Command("Randomize Position",
            "Randomizes the local position of the selected objects " +
            "within a specified range (by default -1 to 1)", QuickName = "RAP",
            IgnoreHotKeyConflict = true,
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "RandomizePosition", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform/Set")]
        public static void RandomizePosition()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new InteractiveRandomPosition(Selection.gameObjects));
        }


        public static Vector2 DefaultScaleRange()
        {
            return new Vector2(.8f, 1.2f);
        }


        public static Vector2[] RandomRotationRangeDefaultValue()
        {
            return new Vector2[] { new Vector2(-180, 180f) };
        }

        public static CommandParameterAutoComplete<Vector2> RandomRotationRangeAutoComplete()
        {
            return new CommandParameterAutoComplete<Vector2>().
                AddValue("-180 to 180", new Vector2(-180, 180f))
                .AddValue("0 to 180", new Vector2(0, 180))
                .AddValue("-90 to +90", new Vector2(-90, 90))
                .AddValue("0 to 360", new Vector2(0, 360));
        }

        public static CommandParameterAutoComplete<Vector2> RandomScaleRangeAutoComplete()
        {
            return new CommandParameterAutoComplete<Vector2>().AddValue("0 to 1", Vector2.up)
            .AddValue("1 to 2", new Vector2(1, 2))
            .AddValue("1 to 5", new Vector2(1, 5))
            .AddValue("0.5 to 1.5", new Vector2(.5f, 1.5f))
            .AddValue("0.8 to 1.2", new Vector2(.8f, 1.2f));
        }

        public static CommandParameterAutoComplete<Vector2> RandomPositionRangeAutoComplete()
        {
            return new CommandParameterAutoComplete<Vector2>()
                .AddValue("-0.25 to 0.25", new Vector2(-.25f, .25f))
                .AddValue("-0.5 to 0.5", new Vector2(-.5f, .5f))
                .AddValue("-1 to 1", new Vector2(-1, 1))
                .AddValue("-2 to 2", new Vector2(-2, 2))
                .AddValue("-5 to 5", new Vector2(-5, 5))
                .AddValue("-10 to 10", new Vector2(-10, 10))
                .AddValue("-50 to 50", new Vector2(-50, 50));
        }


        public class InteractiveRandomScale : InteractiveCommand
        {

            private Vector2 scaleRandom;
            private List<GameObject> toRandomize;
            public float Multiplier = 1;

            public InteractiveRandomScale(GameObject[] toRandomize)
            {
                ConfirmationMode = ActionConfirmationMode.ESCAPE;
                SceneCommandName = "Transform Randomization";
                ActionOnSpace = "to randomize transforms";
                this.scaleRandom = new Vector2(0.8f, 1.1f);
                this.toRandomize = toRandomize.ToList();
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                float min = scaleRandom.x;
                float max = scaleRandom.y;
                DisplayFloatOption("Min Scale", ref min);
                DisplayFloatOption("Max Scale", ref max);
                scaleRandom = new Vector2(min, max);
                DisplayFloatOption("Multiplier", ref Multiplier);
                DisplayObjectListOption("Objects To Randomize", toRandomize);
            }

            public override void ApplyFunction()
            {
                OnSpaceDownAction();
            }

            protected override void OnSpaceDownAction()
            {
                base.OnSpaceDownAction();
                int undoID = MonkeyEditorUtils.CreateUndoGroup("Randomization");

                toRandomize.RemoveAll(_ => !_);

                foreach (GameObject gameObject in toRandomize)
                {
                    Undo.RecordObject(gameObject.transform, "Randomization trans");

                    gameObject.transform.localScale = Vector3.one * (Random.Range(scaleRandom.x, scaleRandom.y) * Multiplier);


                }
                Undo.CollapseUndoOperations(undoID);
            }
        }

        public class InteractiveRandomRotation : InteractiveCommand
        {


            private readonly Axis[] randomAxes = { Axis.X, Axis.Y, Axis.Z };

            private readonly Vector2[] randomPerAxis;

            private List<GameObject> toRandomize;

            private readonly Quaternion[] previousAngles;

            public float Multiplier = 1f;

            public InteractiveRandomRotation(GameObject[] toRandomize)
            {
                ConfirmationMode = ActionConfirmationMode.ESCAPE;
                SceneCommandName = "Transform Randomization";
                ActionOnSpace = "to randomize transforms";

                this.randomPerAxis = new Vector2[] { new Vector2(-180, 180), new Vector2(-180, 180), new Vector2(-180, 180) };

                this.toRandomize = new List<GameObject>(toRandomize);

                previousAngles = toRandomize.Convert(_ => _.transform.localRotation).ToArray();

            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();

                DisplayVector2Option("X Axis Random Range", ref randomPerAxis[0]);
                DisplayVector2Option("Y Axis Random Range", ref randomPerAxis[1]);
                DisplayVector2Option("Z Axis Random Range", ref randomPerAxis[2]);
                DisplayFloatOption("Multiplier", ref Multiplier);
                DisplayObjectListOption("Objects To Randomize", toRandomize);
            }

            public override void ApplyFunction()
            {
                OnSpaceDownAction();
            }

            protected override void OnSpaceDownAction()
            {
                base.OnSpaceDownAction();
                int undoID = MonkeyEditorUtils.CreateUndoGroup("Randomization");
                int objectID = 0;

                toRandomize.RemoveAll(_ => !_);

                foreach (GameObject gameObject in toRandomize)
                {
                    Undo.RecordObject(gameObject.transform, "Randomization trans");

                    if (randomPerAxis != null && randomPerAxis.Length > 0)
                    {
                        int i = 0;

                        gameObject.transform.localRotation = previousAngles[objectID];

                        Vector3 previousUp = gameObject.transform.up;
                        Vector3 previousRight = gameObject.transform.right;
                        Vector3 previousForward = gameObject.transform.forward;

                        foreach (Axis axis in randomAxes)
                        {
                            int j = i;
                            if (i >= randomPerAxis.Length)
                            {
                                j = 0;
                            }
                            float angle = Random.Range(randomPerAxis[j].x, randomPerAxis[j].y) * Multiplier;
                            switch (axis)
                            {
                                case Axis.X:
                                    gameObject.transform.Rotate(previousRight, angle, Space.World);
                                    break;
                                case Axis.Y:
                                    gameObject.transform.Rotate(previousUp, angle, Space.World);
                                    break;
                                case Axis.Z:
                                    gameObject.transform.Rotate(previousForward, angle, Space.World);
                                    break;
                            }
                            i++;
                        }
                    }

                    objectID++;
                }
                Undo.CollapseUndoOperations(undoID);
            }
        }

        public class InteractiveRandomPosition : InteractiveCommand
        {

            private readonly Axis[] randomAxes;

            private readonly Vector2[] randomPerAxis;

            private List<GameObject> toRandomize;

            private readonly Vector3[] previousPositions;

            public float Multiplier = 1f;

            public bool LocalSpace = true;

            public InteractiveRandomPosition(GameObject[] toRandomize)
            {
                ConfirmationMode = ActionConfirmationMode.ESCAPE;
                SceneCommandName = "Transform Randomization";
                ActionOnSpace = "to randomize transforms";

                randomAxes = new[] { Axis.X, Axis.Y, Axis.Z };
                this.randomPerAxis = new[] { new Vector2(-1, 1), new Vector2(-1, 1), new Vector2(-1, 1) }; ;
                this.toRandomize = new List<GameObject>(toRandomize);
                LocalSpace = true;

                previousPositions = toRandomize.Convert(_ => _.transform.position).ToArray();
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();

                DisplayBoolOption("Local Space", ref LocalSpace);


                DisplayVector2Option("X Axis Random Range", ref randomPerAxis[0]);
                DisplayVector2Option("Y Axis Random Range", ref randomPerAxis[1]);
                DisplayVector2Option("Z Axis Random Range", ref randomPerAxis[2]);

                DisplayFloatOption("Multiplier", ref Multiplier);

                DisplayObjectListOption("Objects To Randomize", toRandomize);

            }

            public override void ApplyFunction()
            {
                OnSpaceDownAction();
            }

            protected override void OnSpaceDownAction()
            {
                base.OnSpaceDownAction();
                int undoID = MonkeyEditorUtils.CreateUndoGroup("Randomization");
                int objectID = 0;

                toRandomize.RemoveAll(_ => !_);

                foreach (GameObject gameObject in toRandomize)
                {
                    Undo.RecordObject(gameObject.transform, "Randomization trans");


                    if (randomPerAxis != null && randomPerAxis.Length > 0)
                    {
                        int i = 0;

                        gameObject.transform.position = previousPositions[objectID];

                        Vector3 previousUp = LocalSpace? gameObject.transform.up:Vector3.up;
                        Vector3 previousRight = LocalSpace ? gameObject.transform.right : Vector3.right;
                        Vector3 previousForward = LocalSpace ? gameObject.transform.forward : Vector3.forward;

                        foreach (Axis axis in randomAxes)
                        {
                            int j = i;
                            if (i >= randomPerAxis.Length)
                            {
                                j = 0;
                            }
                            float positionJitter = Random.Range(randomPerAxis[j].x, randomPerAxis[j].y)*Multiplier;
                            switch (axis)
                            {
                                case Axis.X:
                                    gameObject.transform.position =
                                        gameObject.transform.position + positionJitter * previousRight;
                                    break;
                                case Axis.Y:
                                    gameObject.transform.position =
                                        gameObject.transform.position + positionJitter * previousUp;
                                    break;
                                case Axis.Z:
                                    gameObject.transform.position =
                                        gameObject.transform.position + positionJitter * previousForward;
                                    break;
                            }
                            i++;
                        }
                    }

                    objectID++;

                }
                Undo.CollapseUndoOperations(undoID);
            }
        }

        public class InteractiveRandomScaleRotationPosition : InteractiveCommand
        {
            public bool randomizeScale = false;
            private Vector2 scaleRandom;

            private readonly Axis[] randomAxes;

            private readonly Vector2[] randomPerAxis;

            private List<GameObject> toRandomize;

            private readonly Quaternion[] previousAngles;

            private readonly Vector3[] previousPositions;

            private bool randomizePosition;

            public float Multiplier = 1f;

            public InteractiveRandomScaleRotationPosition(GameObject[] toRandomize, Vector2 scaleRandom,
                Vector2[] randomPerAxis, bool randomizePosition = false)
            {
                ConfirmationMode = ActionConfirmationMode.ESCAPE;
                SceneCommandName = "Transform Randomization";
                ActionOnSpace = "to randomize transforms";
                this.scaleRandom = scaleRandom;
                if (scaleRandom != Vector2.zero)
                    randomizeScale = true;
                this.randomAxes = new Axis[] { Axis.X, Axis.Y, Axis.Z };
                this.randomPerAxis = randomPerAxis;
                this.toRandomize = new List<GameObject>(toRandomize);
                this.randomizePosition = randomizePosition;
                previousAngles = toRandomize.Convert(_ => _.transform.localRotation).ToArray();
                previousPositions = toRandomize.Convert(_ => _.transform.position).ToArray();
            }

            public override void DisplayParameters()
            {

                base.DisplayParameters();

                float min = scaleRandom.x;
                float max = scaleRandom.y;
                DisplayFloatOption("Min Scale", ref min);
                DisplayFloatOption("Max Scale", ref max);
                scaleRandom = new Vector2(min, max);
                DisplayFloatOption("Multiplier", ref Multiplier);
                DisplayObjectListOption("Objects To Randomize", toRandomize);

            }

            protected override void OnSpaceDownAction()
            {
                base.OnSpaceDownAction();
                int undoID = MonkeyEditorUtils.CreateUndoGroup("Randomization");
                int objectID = 0;

                toRandomize.RemoveAll(_ => !_);

                foreach (GameObject gameObject in toRandomize)
                {
                    Undo.RecordObject(gameObject.transform, "Randomization trans");
                    if (randomizeScale)
                        gameObject.transform.localScale = Vector3.one *
                                                          Random.Range(scaleRandom.x, scaleRandom.y) * Multiplier;
                    if (randomizePosition)
                    {
                        if (randomPerAxis != null && randomPerAxis.Length > 0)
                        {
                            int i = 0;

                            gameObject.transform.position = previousPositions[objectID];

                            Vector3 previousUp = gameObject.transform.up;
                            Vector3 previousRight = gameObject.transform.right;
                            Vector3 previousForward = gameObject.transform.forward;

                            foreach (Axis axis in randomAxes)
                            {
                                int j = i;
                                if (i >= randomPerAxis.Length)
                                {
                                    j = 0;
                                }
                                float positionJitter = Random.Range(randomPerAxis[j].x, randomPerAxis[j].y) * Multiplier;
                                switch (axis)
                                {
                                    case Axis.X:
                                        gameObject.transform.position =
                                            gameObject.transform.position + positionJitter * previousRight;
                                        break;
                                    case Axis.Y:
                                        gameObject.transform.position =
                                            gameObject.transform.position + positionJitter * previousUp;
                                        break;
                                    case Axis.Z:
                                        gameObject.transform.position =
                                            gameObject.transform.position + positionJitter * previousForward;
                                        break;
                                }
                                i++;
                            }
                        }
                    }
                    else
                    {
                        if (randomPerAxis != null && randomPerAxis.Length > 0)
                        {
                            int i = 0;

                            gameObject.transform.localRotation = previousAngles[objectID];

                            Vector3 previousUp = gameObject.transform.up;
                            Vector3 previousRight = gameObject.transform.right;
                            Vector3 previousForward = gameObject.transform.forward;

                            foreach (Axis axis in randomAxes)
                            {
                                int j = i;
                                if (i >= randomPerAxis.Length)
                                {
                                    j = 0;
                                }
                                float angle = Random.Range(randomPerAxis[j].x, randomPerAxis[j].y);
                                switch (axis)
                                {
                                    case Axis.X:
                                        gameObject.transform.Rotate(previousRight, angle, Space.World);
                                        break;
                                    case Axis.Y:
                                        gameObject.transform.Rotate(previousUp, angle, Space.World);
                                        break;
                                    case Axis.Z:
                                        gameObject.transform.Rotate(previousForward, angle, Space.World);
                                        break;
                                }
                                i++;
                            }
                        }
                    }

                    objectID++;

                }
                Undo.CollapseUndoOperations(undoID);
            }
        }

        [Command("Spread And Align Between", QuickName = "SA",
            Help = "Spreads equally and aligns selected objects between two others",
            DefaultValidation = DefaultValidation.AT_LEAST_TWO_GAME_OBJECTS,
            Category = "Transform/Align")]
        public static void SpreadBetween()
        {
            GameObject[] selected =
                MonkeyEditorUtils.OrderedSelectedGameObjects.ToArray();
            MonkeyEditorUtils.AddSceneCommand(new SpreadBetweenSceneCommand(null, null, selected));
        }

        public static GameObject FirstSelected()
        {
            return MonkeyEditorUtils.OrderedSelectedGameObjects.First();
        }

        public static GameObject SecondSelected()
        {
            return MonkeyEditorUtils.OrderedSelectedGameObjects.ElementAt(1);
        }

        public class SpreadBetweenSceneCommand : TimedSceneCommand
        {
            private GameObject first;
            private GameObject second;
            private List<GameObject> selected;

            private List<Vector3> previousPos;
            private List<Quaternion> previousRot;

            public SpreadBetweenSceneCommand(GameObject first, GameObject second,
                GameObject[] selected) : base(0)
            {
                SceneCommandName = "Spread Between";
                this.first = first;
                this.second = second;
                this.selected = selected.ToList();
                previousPos = selected.Convert(_ => _.transform.position).ToList();
                previousRot = selected.Convert(_ => _.transform.rotation).ToList();
            }

            public override void DisplayParameters()
            {
                DisplayObjectOption("Start Object", ref first);
                DisplayObjectOption("End Object", ref second);

                int id = DisplayObjectListOption("Objects To Spread", selected);
                if (id != -1)
                {
                    if (previousPos.Count <= id)
                    {
                        previousPos.Add(Vector3.zero);
                        previousRot.Add(Quaternion.identity);
                    }

                    previousPos[id] = selected[id].transform.position;
                    previousRot[id] = selected[id].transform.rotation;
                }
            }

            public override void Update()
            {
                base.Update();

                if (!first || !second)
                    return;

                Vector3 direction = second.transform.position - first.transform.position;
                float distance = direction.magnitude / (selected.Count + 1);
                Vector3 currentPosition = first.transform.position + distance * direction.normalized;

                selected = selected.Where(_ => _).ToList();

                foreach (var gameObject in selected)
                {
                    gameObject.transform.position = currentPosition;
                    currentPosition += distance * direction.normalized;
                }
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();

                if (!first || !second)
                    return;

                Handles.DrawDottedLine(first.transform.position,
                    second.transform.position, 1f);
            }

            public override void Stop()
            {
                base.Stop();

                Vector3[] newPositions = selected.Convert(_ => _.transform.position).ToArray();
                Quaternion[] newRotations = selected.Convert(_ => _.transform.rotation).ToArray();

                for (int i = 0; i < previousRot.Count; i++)
                {
                    selected[i].transform.position = previousPos[i];
                    selected[i].transform.rotation = previousRot[i];
                }

                int undoIndex = MonkeyEditorUtils.CreateUndoGroup("Spread Between");
                for (int i = 0; i < selected.Count; i++)
                {
                    Undo.RecordObject(selected[i].transform, "Applying new position");
                    selected[i].transform.position = newPositions[i];
                    selected[i].transform.rotation = newRotations[i];
                }
                Undo.CollapseUndoOperations(undoIndex);
            }
        }

        [Command("Visualize Colliders", "Adds temporary visualizations to all the colliders selected",
            QuickName = "VC",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            Category = "Physics")]
        public static void VisualizeColliders(
            [CommandParameter("The material to set for the debug meshes (by default the default material)"
                ,DefaultValueMethod = "DefaultMat")]
            Material mat)
        {
            List<GameObject> objToDebug = new List<GameObject>();

            foreach (var o in Selection.gameObjects)
            {
                var cols = o.GetComponentsInChildren<Collider>();
                objToDebug.AddRange(cols.Convert(_ => _.gameObject));
            }

            MonkeyEditorUtils.AddSceneCommand(new TemporaryMeshSceneCommand(objToDebug.ToArray(), mat, 0));
        }

        private static Material DefaultMat()
        {
            return AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
        }


        public class TemporaryMeshSceneCommand : TimedSceneCommand
        {
            private readonly List<GameObject> addedRenderers = new List<GameObject>();

            private readonly Dictionary<BoxCollider, GameObject> transformsPerBoxCollider
                = new Dictionary<BoxCollider, GameObject>();
            private readonly Dictionary<SphereCollider, GameObject> transformsPerSphereCollider
                = new Dictionary<SphereCollider, GameObject>();
            private readonly Dictionary<CapsuleCollider, GameObject> transformsPerCapsuleCollider
                = new Dictionary<CapsuleCollider, GameObject>();

            public TemporaryMeshSceneCommand(GameObject[] objects, Material mat, float duration) : base(duration)
            {
                TimeBetweenUpdate = 0.2;
                SceneCommandName = "Collider Visualization";
                foreach (GameObject o in objects)
                {

                    Collider col = o.GetComponent<Collider>();
                    GameObject renderer = null;

                    //ugly type checking but heh, it works.
                    if (col is BoxCollider box)
                    {
                        renderer = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        renderer.transform.SetParent(o.transform, false);
                        UpdateBoxScale(renderer, box);
                        transformsPerBoxCollider.Add(box, renderer);
                    }
                    else if (col is SphereCollider sphere)
                    {
                        renderer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        renderer.transform.SetParent(o.transform, false);
                        UpdateSphereScale(renderer, sphere);
                        transformsPerSphereCollider.Add(sphere, renderer);

                    }
                    else if (col is CapsuleCollider caps)
                    {
                        renderer = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        renderer.transform.SetParent(o.transform, false);
                        UpdateCapsScale(renderer, caps);
                        transformsPerCapsuleCollider.Add(caps, renderer);
                    }
                    else if (col is MeshCollider mesh)
                    {
                        renderer = new GameObject();
                        MeshFilter filter = renderer.AddComponent<MeshFilter>();
                        filter.sharedMesh = mesh.sharedMesh;
                        renderer.AddComponent<MeshRenderer>();
                    }


                    if (renderer != null)
                    {
                        renderer.name = "MonKey Debug " + renderer.name;
                        renderer.GetComponent<MeshRenderer>().material = mat;
                        Object.DestroyImmediate(renderer.GetComponent<Collider>());

                        addedRenderers.Add(renderer);
                    }

                }
            }

            private static void UpdateCapsScale(GameObject renderer, CapsuleCollider capsuleCollider)
            {
                renderer.transform.localScale = capsuleCollider.radius * 2 * Vector3.one;
                //for capsules the mesh is going to stretch:
                //could be possible to generate a proper mesh, but to isolated of a problem to care
                renderer.transform.localScale += Vector3.up * (capsuleCollider.height - 2);
            }

            private static void UpdateSphereScale(GameObject renderer, SphereCollider sphereCollider)
            {
                renderer.transform.localScale = sphereCollider.radius * 2 * Vector3.one;
            }

            private static void UpdateBoxScale(GameObject renderer, BoxCollider boxCollider)
            {
                renderer.transform.localScale = boxCollider.size;
            }

            public override void Update()
            {
                base.Update();
                foreach (var tpb in transformsPerBoxCollider)
                {
                    UpdateBoxScale(tpb.Value, tpb.Key);
                }

                foreach (var tps in transformsPerSphereCollider)
                {
                    UpdateSphereScale(tps.Value, tps.Key);
                }

                foreach (var tpc in transformsPerCapsuleCollider)
                {
                    UpdateCapsScale(tpc.Value, tpc.Key);
                }
            }

            public override void Stop()
            {
                foreach (var renderer in addedRenderers)
                {
                    Object.DestroyImmediate(renderer);
                }

                base.Stop();

            }
        }

    }
}
