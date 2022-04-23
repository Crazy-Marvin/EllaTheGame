using MonKey.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonKey.Editor.Commands
{
    public static class ConstrainUtilities
    {
        [Command("Look At", QuickName = "LI",
            Help = "Constrains the selected objects to always look " +
                   "at another one (by default, looks at the mouse)"
            , Category = "Transform")]
        public static void LookAtConstrain()
        {

            MonkeyEditorUtils.AddSceneCommand(
                new LookAtSceneCommand(null, Selection.gameObjects, new Vector3(0, 1, 0), new Axis[0]));
        }

        [Command("Look At 2D", QuickName = "LI2",
            Help = "Constrains the selected objects to always look " +
                   "at another one in 2D space(by default the first selected)"
            , DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, Category = "2D/Transform")]
        public static void LookAtConstrain2D()
        {

            MonkeyEditorUtils.AddSceneCommand(
                new LookAt2DSceneCommand(null, Selection.gameObjects));
        }


        [Command("Look At Mouse", QuickName = "LM",
            Help = "Makes the selected objects look at the mouse until confirmed"
            , DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, AlwaysShow = true,
            Category = "Transform")]
        public static void LookAtMouseConstrain()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new LookAtSceneConfirmedCommand(null, Selection.gameObjects, Vector3.up, new Axis[0]));
        }


        [Command("Look At Mouse 2D", QuickName = "LM2",
            Help = "Makes the selected objects look at the mouse until confirmed"
            , DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, AlwaysShow = true
            , Category = "2D/Transform")]
        public static void LookAtMouseConstrain2D()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new LookAt2DSceneConfirmedCommand(null, Selection.gameObjects));
        }

        public static Axis[] DefaultLockedAxis()
        {
            return new[] { Axis.NONE };
        }

        public static GameObject FirstSelected()
        {
            return MonkeyEditorUtils.OrderedSelectedGameObjects.ElementAt(0);
        }

        public static GameObject SecondSelected()
        {
            return MonkeyEditorUtils.OrderedSelectedGameObjects.ElementAt(1);
        }

        public static Vector3 ZeroVector()
        {
            return Vector3.zero;
        }

        public class LookAtSceneCommand : TimedSceneCommand
        {
            private readonly LookAtLogic logic;

            public LookAtSceneCommand(GameObject toLookAt,
                GameObject[] constrained, Vector3 worldUpVector, Axis[] lockedAxes) : base(0)
            {
                SceneCommandName = "Constrain: Look At " + (toLookAt ? toLookAt.name : "Mouse");
                logic = new LookAtLogic(toLookAt, constrained, worldUpVector, lockedAxes);
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();
                logic.OnSceneGUI();
            }

            public override void Update()
            {
                base.Update();
                logic.Update();
            }

            public override void Stop()
            {
                base.Stop();
                logic.Stop();
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayObjectOption("Object To Look At", ref logic.ToLookAt);

                if (!logic.ToLookAt)
                    DisplayMessage("Currently Looking At The Mouse Position");

                DisplayBoolOption("Reverse Direction", ref logic.ReverseDirection);

                DisplayObjectListOption("Objects Looking", logic.Constrained);
                DisplayVectorOption("World Up Vector", ref logic.WorldUpVector);
                DisplayBoolOption("Show Debug Text", ref logic.ShowDebugText);
            }
        }

        public class LookAt2DSceneCommand : TimedSceneCommand
        {
            private readonly LookAt2DLogic logic;

            public LookAt2DSceneCommand(GameObject toLookAt, GameObject[] constrained) : base(0)
            {
                SceneCommandName = "Constrain: Look At " + (toLookAt ? toLookAt.name : "Mouse");
                logic = new LookAt2DLogic(toLookAt, constrained);
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();
                logic.OnSceneGUI();
            }

            public override void Update()
            {
                base.Update();
                logic.Update();
            }

            public override void Stop()
            {
                base.Stop();
                logic.Stop();
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayObjectOption("Object To Look At", ref logic.ToLookAt);

                DisplayBoolOption("Show Debug Text", ref logic.ShowDebugText);
                DisplayObjectListOption("Objects Looking", logic.Constrained);
            }
        }

        public class LookAtSceneConfirmedCommand : ConfirmedCommand
        {
            private readonly LookAtLogic logic;

            public LookAtSceneConfirmedCommand(GameObject toLookAt,
                GameObject[] constrained, Vector3 worldUpVector, Axis[] lockedAxes)
            {
                SceneCommandName = "Constrain: Look At " + (toLookAt ? toLookAt.name : "Mouse");
                logic = new LookAtLogic(toLookAt, constrained, worldUpVector, lockedAxes);
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();
                logic.OnSceneGUI();
            }

            public override void Update()
            {
                base.Update();
                logic.Update();
            }

            public override void Stop()
            {
                base.Stop();
                logic.Stop();
            }
        }

        public class LookAt2DSceneConfirmedCommand : ConfirmedCommand
        {
            private readonly LookAt2DLogic logic;

            public LookAt2DSceneConfirmedCommand(GameObject toLookAt,
                GameObject[] constrained)
            {
                SceneCommandName = "Constrain: Look At " + (toLookAt ? toLookAt.name : "Mouse");
                logic = new LookAt2DLogic(toLookAt, constrained);
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();
                logic.OnSceneGUI();
            }

            public override void Update()
            {
                base.Update();
                logic.Update();
            }

            public override void Stop()
            {
                base.Stop();
                logic.Stop();
            }
        }


        public class LookAtLogic
        {
            public GameObject ToLookAt;
            public List<GameObject> Constrained;
            public Vector3 WorldUpVector;
            public bool ShowDebugText = true;
            public bool ReverseDirection = false;
            private readonly Quaternion[] initialRotations;

            private readonly Axis[] lockedAxes;

            public LookAtLogic(GameObject toLookAt,
                GameObject[] constrained, Vector3 worldUpVector, Axis[] lockedAxes)
            {
                this.ToLookAt = toLookAt;
                this.WorldUpVector = worldUpVector;
                this.Constrained = constrained.Where(_ => _ != toLookAt).ToList();
                this.lockedAxes = lockedAxes;
                initialRotations = this.Constrained.Convert(_ => _.transform.rotation).ToArray();
                Selection.objects = new Object[] { toLookAt };
            }

            public void OnSceneGUI()
            {
                if (!ShowDebugText)
                    return;

                Constrained.RemoveAll(_ => !_);

                foreach (GameObject o in Constrained)
                {
                    Handles.Label(o.transform.position, "Lock Look At '" + ToLookAt + "'");
                }
            }

            public void Update()
            {
                int i = 0;
                Constrained.RemoveAll(_ => !_);
                foreach (var gameObject in Constrained)
                {
                    if (ToLookAt != null)
                    {
                        if (WorldUpVector == Vector3.zero)
                            gameObject.transform.LookAt(ToLookAt.transform);
                        else
                        {
                            gameObject.transform.LookAt(ToLookAt.transform, WorldUpVector);
                        }
                    }
                    else
                    {
                        bool collision;
                        Vector3 normal;
                        Vector3 mouseRaycastPosition =
                            MonkeyEditorUtils.GetMouseRayCastedPosition(new Transform[0], 0, out collision, out normal);
                        if (WorldUpVector == Vector3.zero)
                            gameObject.transform.LookAt(mouseRaycastPosition);
                        else
                        {
                            gameObject.transform.LookAt(mouseRaycastPosition, WorldUpVector);
                        }
                    }

                    if (ReverseDirection)
                    {
                        gameObject.transform.Rotate(gameObject.transform.up, 180);
                    }

                    if (lockedAxes.Length > 0)
                    {
                        Vector3 euler = gameObject.transform.rotation.eulerAngles;
                        Vector3 initialEuler = initialRotations[i].eulerAngles;
                        gameObject.transform.rotation = Quaternion.Euler(
                            lockedAxes.Contains(Axis.X) ? initialEuler.x : euler.x,
                            lockedAxes.Contains(Axis.Y) ? initialEuler.y : euler.y,
                            lockedAxes.Contains(Axis.Z) ? initialEuler.z : euler.z);
                    }

                    i++;
                }
            }

            public void Stop()
            {
                Quaternion[] newRotations = Constrained.Convert(_ => _.transform.rotation).ToArray();

                Constrained.RemoveAll(_ => !_);

                for (int i = 0; i < Constrained.Count; i++)
                {
                    Constrained[i].transform.rotation = initialRotations[i];
                }

                int id = MonkeyEditorUtils.CreateUndoGroup("Look At Constrain");

                for (int i = 0; i < Constrained.Count; i++)
                {
                    Undo.RecordObject(Constrained[i].transform, "Look At Rotation");
                    Constrained[i].transform.rotation = newRotations[i];
                }

                Undo.CollapseUndoOperations(id);
            }
        }

        public class LookAt2DLogic
        {
            public GameObject ToLookAt;
            public List<GameObject> Constrained;
            public bool ShowDebugText = true;

            private readonly Quaternion[] initialRotations;

            public LookAt2DLogic(GameObject toLookAt,
                GameObject[] constrained)
            {
                this.ToLookAt = toLookAt;
                this.Constrained = new List<GameObject>(constrained.Where(_ => _ != toLookAt));
                initialRotations = this.Constrained.Convert(_ => _.transform.rotation).ToArray();
                Selection.objects = new Object[] { toLookAt };
            }

            public void OnSceneGUI()
            {
                if (!ShowDebugText)
                    return;

                Constrained.RemoveAll(_ => !_);

                foreach (GameObject o in Constrained)
                {
                    Handles.Label(o.transform.position, "Lock Look At '" + ToLookAt + "'");
                }
            }

            public void Update()
            {
                Constrained.RemoveAll(_ => !_);

                Vector2 mousePos = HandleUtility.GUIPointToScreenPixelCoordinate(MonkeyEditorUtils.MousePosition);

                foreach (var gameObject in Constrained)
                {
                    Vector2 diff = (ToLookAt != null ? (Vector2)ToLookAt.transform.position : mousePos)
                                   - (Vector2)gameObject.transform.position;
                    diff.Normalize();

                    gameObject.transform.rotation =
                        Quaternion.LookRotation(diff, Vector3.up)
                        * Quaternion.AngleAxis(-90, Vector3.up);

                }
            }

            public void Stop()
            {
                Quaternion[] newRotations = Constrained.Convert(_ => _.transform.rotation).ToArray();

                Constrained.RemoveAll(_ => !_);

                for (int i = 0; i < Constrained.Count; i++)
                {
                    Constrained[i].transform.rotation = initialRotations[i];
                }

                int id = MonkeyEditorUtils.CreateUndoGroup("Look At Constrain");

                for (int i = 0; i < Constrained.Count; i++)
                {
                    Undo.RecordObject(Constrained[i].transform, "Look At Rotation");
                    Constrained[i].transform.rotation = newRotations[i];
                }

                Undo.CollapseUndoOperations(id);
            }
        }



        [Command("Align Axis", QuickName = "AAI",
            Help = "Constrains the objects to align to the axis of another one",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT, Category = "Transform/Align")]
        public static void AlignAxisConstrain()
        {
            MonkeyEditorUtils.AddSceneCommand(new AxisConstrainSceneCommand(null,
               Selection.gameObjects, Axis.X, true));
        }

        public class AxisConstrainSceneCommand : TimedSceneCommand
        {

            public List<GameObject> ToConstrain;
            public GameObject Reference;
            public Axis Axis;
            public bool LocalAxis;

            public AxisConstrainSceneCommand(GameObject reference, GameObject[] toConstrain, Axis axis, bool localAxis)
                : base(0)
            {
                SceneCommandName = "Align Object Axis";
                this.ToConstrain = new List<GameObject>(toConstrain);
                this.Axis = axis;
                this.LocalAxis = localAxis;
                this.Reference = reference;
            }

            public override void Update()
            {
                base.Update();
                if (!Reference)
                    return;

                ToConstrain.RemoveAll(_ => !_);

                foreach (var gameObject in ToConstrain)
                {
                    gameObject.transform.position = MathExt.ProjectPointOnLine(Reference.transform.position
                        , Reference.transform.AxisToVector(Axis, LocalAxis), gameObject.transform.position);
                }
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayObjectOption("Object to Align To", ref Reference);
                Enum axis = Axis;
                DisplayEnumOption("Axis To Align On", ref axis);
                Axis = (Axis)axis;
                DisplayBoolOption("Use Local Axis", ref LocalAxis);
                DisplayObjectListOption("Objects Constrained", ToConstrain);
            }
        }


        [Command("Align On Axis Between", QuickName = "AB",
           Help = "Constrains the objects to align to the axis between two objects",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT, Category = "Transform/Align")]
        public static void AlignBetweenConstrain()
        {

            MonkeyEditorUtils.AddSceneCommand(new DualReferenceAxisConstrainSceneCommand(null,
                null,
                MonkeyEditorUtils.OrderedSelectedGameObjects.ToArray()));
        }


        public class DualReferenceAxisConstrainSceneCommand : TimedSceneCommand
        {
            private TransformUndo undo;
            public List<GameObject> ToConstrain;
            public GameObject Reference;
            public GameObject SecondReference;

            public DualReferenceAxisConstrainSceneCommand(GameObject reference, GameObject secondReference,
                GameObject[] toConstrain) : base(0)
            {
                SceneCommandName = "Align Axis Between Objects";
                this.ToConstrain = new List<GameObject>(toConstrain);
                this.SecondReference = secondReference;
                this.Reference = reference;

                undo = new TransformUndo();
                undo.Register(ToConstrain.Convert(_ => _.transform).ToArray());
            }

            public override void Update()
            {
                base.Update();

                if (!Reference || !SecondReference)
                    return;

                ToConstrain.RemoveAll(_ => !_);

                foreach (var gameObject in ToConstrain)
                {
                    Vector3 distVec = Reference.transform.position - SecondReference.transform.position;
                    gameObject.transform.position = MathExt.ProjectPointOnLine(Reference.transform.position
                        , distVec.normalized, gameObject.transform.position);
                }
            }

            public override void Stop()
            {
                base.Stop();
                var id = MonkeyEditorUtils.CreateUndoGroup("MonKey - Align Between");
                undo.RecordUndo();
                Undo.CollapseUndoOperations(id);
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayObjectOption("First Alignment Object", ref Reference);
                DisplayObjectOption("Second Alignment Object", ref SecondReference);
                DisplayObjectListOption("Objects To Align", ToConstrain);
            }
        }

        [Command("Lock Distance", QuickName = "LD",
            Help = "Constrains the selected objects' distance to a reference one ",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, Category = "Transform/Lock")]
        public static void DistanceConstrain()
        {
            MonkeyEditorUtils.AddSceneCommand(new DistanceConstraintSceneCommand(null,
                Selection.gameObjects, DistanceConstraintSceneCommand.ConstrainOperation.MIN, false, 1));
        }

        [Command("Lock Distance All", QuickName = "LDA",
            Help = "Constrains the selected objects' distance between each other",
            DefaultValidation = DefaultValidation.AT_LEAST_TWO_GAME_OBJECTS, Category = "Transform/Lock")]
        public static void DistanceConstrainAll()
        {
            MonkeyEditorUtils.AddSceneCommand(new DistanceConstraintSceneCommand(null,
                MonkeyEditorUtils.OrderedSelectedGameObjects.ToArray(),
                DistanceConstraintSceneCommand.ConstrainOperation.MIN, true, 1));
        }

        [Command("Lock Distance With Children", QuickName = "LDC",
            Help = "Constrains the selected objects' distance to the reference one's children (by default first selected)",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, Category = "Transform/Lock")]
        public static void ConstrainDistanceAllChildren(
            [CommandParameter(Help = "The first Object who's children will constrain all the others," +
                                     " by default the first object selected",
                DefaultValueMethod = "FirstSelected", DefaultValueNameOverride = "First Object Selected")]
            GameObject reference)
        {
            List<SceneCommand> commands = new List<SceneCommand>();
            foreach (Transform child in reference.GetComponentsInChildren<Transform>())
            {
                commands.Add(new DistanceConstraintSceneCommand(child.gameObject,
                    MonkeyEditorUtils.OrderedSelectedGameObjects.Where(_ => _ != reference && _ != child.gameObject)
                        .ToArray(), DistanceConstraintSceneCommand.ConstrainOperation.MIN, false, 1));
            }
            MonkeyEditorUtils.AddSceneCommand(new TimedMultiSceneCommand(commands.ToArray(), 0)
            { SceneCommandName = "Constrain Distance Children" });
        }

        /// <summary>
        /// Constrains a set of objects to respect a 
        /// certain distance between each other
        /// </summary>
        public class DistanceConstraintSceneCommand : TimedSceneCommand
        {
            /// <summary>
            /// The Mathematical constrains that can be applied
            /// </summary>
            public enum ConstrainOperation
            {
                EQUAL,
                MIN,
                MAX,
            }

            /// <summary>
            /// the objects that will be measured against the others
            /// </summary>
            public GameObject Reference;

            /// <summary>
            /// the objects that will be constrained
            /// </summary>
            public List<GameObject> Constrained;

            /// <summary>
            /// Should the constrained objects 
            /// also respect the mathematical operation between each-other
            /// </summary>
            public bool BetweenEach;

            /// <summary>
            /// The mathematical operation to consider
            /// </summary>
            public ConstrainOperation Operation;

            /// <summary>
            /// Tthe value used for the mathematical operation
            /// </summary>
            public float Value;

            private readonly TransformUndo undo = new TransformUndo();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reference"></param>
            /// <param name="constrained"></param>
            /// <param name="operation"></param>
            /// <param name="betweenEach"></param>
            /// <param name="value"></param>
            public DistanceConstraintSceneCommand(GameObject reference, GameObject[] constrained,
                ConstrainOperation operation, bool betweenEach, float value) : base(0)
            {

                this.Reference = reference;
                this.Constrained = new List<GameObject>(constrained);
                this.BetweenEach = betweenEach;
                this.Value = value;
                Operation = operation;

                SceneCommandName = Operation.ToString().ToLower().NicifyVariableName()
                                   + " Distance Constraint";
                undo.Register(constrained.Convert(_ => _.transform).ToArray());
            }

            /// <summary>
            /// constrain logic
            /// </summary>
            public override void Update()
            {
                base.Update();

                Constrained.RemoveAll(_ => !_);

                for (int i = 0; i < Constrained.Count; i++)
                {
                    if (Reference)
                        DistanceConstrain(Constrained[i], Reference, false);
                    if (!BetweenEach)
                        continue;

                    for (int j = i + 1; j < Constrained.Count; j++)
                    {
                        DistanceConstrain(Constrained[i], Constrained[j], true);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public override void Stop()
            {
                base.Stop();

                int undoID = MonkeyEditorUtils.CreateUndoGroup("Distance Lock");
                undo.RecordUndo();
                Undo.CollapseUndoOperations(undoID);
            }

            private void DistanceConstrain(GameObject obj, GameObject other, bool moveEach)
            {
                Vector3 distance = other.transform.position - obj.transform.position;
                switch (Operation)
                {
                    case ConstrainOperation.EQUAL:
                        if (Mathf.Approximately(distance.magnitude, Value))
                            return;
                        break;
                    case ConstrainOperation.MIN:
                        if (distance.magnitude > Value)
                            return;
                        break;
                    case ConstrainOperation.MAX:
                        if (distance.magnitude < Value)
                            return;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!moveEach)
                    obj.transform.position = other.transform.position - distance.normalized * Value;
                else
                {
                    Vector3 middle = (obj.transform.position + other.transform.position) / 2;
                    obj.transform.position = middle - distance.normalized * Value * .5f;
                    other.transform.position = middle + distance.normalized * Value * .5f;
                }
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayObjectOption("Reference Object", ref Reference);
                Enum constr = Operation;
                DisplayEnumOption("Constrain Type", ref constr);
                Operation = (ConstrainOperation)constr;
                DisplayFloatOption("Constrain Distance", ref Value);
                DisplayBoolOption("Constrain Between All Objects", ref BetweenEach);
                DisplayObjectListOption("Constrained Objects", Constrained);
            }
        }

        [Command("Clamp Position",
            "Clamps the position so that is is a multiple of the specified value",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_SCENE_OBJECT, QuickName = "CLP"
            , Category = "Transform/Clamp")]
        public static void ClampPositionConstrain()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new ClampConstrainSceneCommand(Selection.gameObjects, true, false, false, 1));
        }

        [Command("Clamp Rotation",
            "Clamps the rotation so that is is a multiple of the specified value",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_SCENE_OBJECT, QuickName = "CLR"
            , Category = "Transform/Clamp")]
        public static void ClampRotationConstrain()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new ClampConstrainSceneCommand(Selection.gameObjects, false, true, false, 0, 1));
        }

        [Command("Clamp Scale",
            "Clamps the scale so that is is a multiple of the specified value",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_SCENE_OBJECT, QuickName = "CLS"
            , Category = "Transform/Clamp")]
        public static void ClampScaleConstrain()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new ClampConstrainSceneCommand(Selection.gameObjects, false, false, true, 0, 0, 1));
        }

        public class ClampConstrainSceneCommand : TimedSceneCommand
        {

            public bool ClampPosition;
            public bool ClampRotation;
            public bool ClampScale;

            public float PositionClamp;
            public float RotationClamp;
            public float ScaleClamp;

            public GameObject[] Targets;

            public ClampConstrainSceneCommand(GameObject[] targets, bool clampPosition, bool clampRotation,
                bool clampScale, float positionClamp = 0, float rotationClamp = 0, float scaleClamp = 0) : base(0)
            {
                SceneCommandName = "Clamp Constrain";
                ClampPosition = clampPosition;
                ClampRotation = clampRotation;
                ClampScale = clampScale;
                PositionClamp = positionClamp;
                RotationClamp = rotationClamp;
                ScaleClamp = scaleClamp;
                Targets = targets;
            }

            public override void Update()
            {
                base.Update();
                Targets = Targets.Where(_ => _).ToArray();

                foreach (var gameObject in Targets)
                {

                    if (ClampPosition)
                    {
                        Vector3 position = gameObject.transform.position;
                        position.x = position.x - (position.x % PositionClamp);
                        position.y = position.y - (position.y % PositionClamp);
                        position.z = position.z - (position.z % PositionClamp);

                        gameObject.transform.position = position;
                    }

                    if (ClampRotation)
                    {
                        //will create limitation, good enough for now though
                        Vector3 euler = gameObject.transform.eulerAngles;

                        euler.x = euler.x - (euler.x % RotationClamp);
                        euler.y = euler.y - (euler.y % RotationClamp);
                        euler.z = euler.z - (euler.z % RotationClamp);

                        gameObject.transform.rotation = Quaternion.Euler(euler);
                    }

                    if (ClampScale)
                    {
                        Vector3 scale = gameObject.transform.localScale;
                        scale.x = scale.x - (scale.x % ScaleClamp);
                        scale.y = scale.y - (scale.y % ScaleClamp);
                        scale.z = scale.z - (scale.z % ScaleClamp);

                        gameObject.transform.localScale = scale;
                    }
                }
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayBoolOption("Clamp Position", ref ClampPosition);
                DisplayFloatOption("Position Clamp Value", ref PositionClamp);
                DisplayBoolOption("Clamp Rotation", ref ClampPosition);
                DisplayFloatOption("Rotation Clamp Value", ref RotationClamp);
                DisplayBoolOption("Clamp Scale", ref ClampPosition);
                DisplayFloatOption("Scale Clamp Value", ref ScaleClamp);
            }
        }

        [Command("Lock In Chain", QuickName = "LC",
            Help = "Constrains the selected objects so they act like attached by " +
            "a chain from the first selected to the last",
            DefaultValidation = DefaultValidation.AT_LEAST_TWO_GAME_OBJECTS
            , Category = "Transform/Lock")]
        public static void ChainConstrainObjects()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new ChainConstrain(MonkeyEditorUtils.OrderedSelectedGameObjects
                    .Where(_ => _.scene.IsValid()).ToArray(), 0, 1));
        }

        public class ChainConstrain : TimedSceneCommand
        {
            public List<GameObject> ChainElements;

            public float AngleTolerance;
            public float ChainDistance;

            private readonly Vector3[] previousPositions;
            private readonly Quaternion[] previousRotations;


            //TODO add an align to chain mode

            public ChainConstrain(GameObject[] chainElements, float angleTolerance, float chainDistance) : base(0)
            {
                SceneCommandName = "Chain Constrain";
                ChainElements = new List<GameObject>(chainElements);
                AngleTolerance = angleTolerance;
                ChainDistance = chainDistance;

                previousPositions = chainElements.Convert(_ => _.transform.position).ToArray();
                previousRotations = chainElements.Convert(_ => _.transform.rotation).ToArray();

            }

            public override void Update()
            {
                base.Update();

                if (ChainElements.Count < 2)
                    return;

                ChainElements.RemoveAll(_ => !_);

                GameObject grabbedElement = null;
                if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
                {
                    GameObject[] selection =
                        Selection.gameObjects.Where(_ => ChainElements.Contains(_)).ToArray();
                    if (selection.Any())
                        grabbedElement = selection.First();

                }
                int grabbedIndex = -1;
                if (grabbedElement)
                    grabbedIndex = ChainElements.IndexOf(grabbedElement);

                if (grabbedIndex == -1 || grabbedIndex == 0)
                {
                    for (int i = 0; i < ChainElements.Count; i++)
                    {
                        GrabNextElement(grabbedElement,
                            i > 0 ? ChainElements[i - 1].transform : null,
                            ChainElements[i].transform,
                            i + 1 < ChainElements.Count ? ChainElements[i + 1].transform : null);
                    }
                }
                else
                {
                    if (grabbedElement)
                        GrabNextElement(grabbedElement, null, grabbedElement.transform,
                          ChainElements[grabbedIndex - 1].transform);
                    for (int i = grabbedIndex + 1; i < ChainElements.Count; i++)
                    {
                        GrabNextElement(grabbedElement,
                            i > 0 ? ChainElements[i - 1].transform : null,
                            ChainElements[i].transform,
                            i + 1 < ChainElements.Count ? ChainElements[i + 1].transform : null);
                    }

                    for (int i = grabbedIndex - 1; i >= 0; i--)
                    {
                        GrabNextElement(grabbedElement,
                            i + 1 < ChainElements.Count ? ChainElements[i + 1].transform : null
                            , ChainElements[i].transform,
                            i > 0 ? ChainElements[i - 1].transform : null);
                    }
                }

            }

            public void GrabNextElement(GameObject grabbedObject, Transform elementToFollow, Transform element, Transform previousElement)
            {
                if (elementToFollow == null)
                {
                    if (Tools.pivotRotation == PivotRotation.Local
                        && grabbedObject && grabbedObject.transform == element)
                        return;
                    if (previousElement
                        && (element.position - previousElement.position).normalized.magnitude > 0)
                        element.forward = (element.position - previousElement.position).normalized;
                    return;
                }

                Vector3 distance = elementToFollow.position
                                   - element.transform.position;

                element.position = elementToFollow.position - distance.normalized * ChainDistance;


                Vector3 directionToLookAt;

                if (previousElement)
                {
                    var distanceToPrevious = element.position - previousElement.position;
                    directionToLookAt = (distanceToPrevious.normalized + distance.normalized).normalized;
                }
                else
                {
                    directionToLookAt = distance.normalized;
                }

                float angle = Vector3.Angle(directionToLookAt, element.forward);
                if (angle >= AngleTolerance)
                {
                    //TODO fix back tolerance on angle
                    /*   Quaternion currentLookAt = Quaternion.LookRotation(element.forward,element.up);
                       Quaternion shouldLookAt = Quaternion.LookRotation(directionToLookAt,element.up);
                       Quaternion rotation =
                           Quaternion.RotateTowards(shouldLookAt, currentLookAt, AngleTolerance * .5f);
                       element.forward = rotation * directionToLookAt;*/
                    if (directionToLookAt != Vector3.zero)
                        element.LookAt(element.position + directionToLookAt);
                }

            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayFloatOption("Chaining Length", ref ChainDistance);
                DisplayFloatOption("Angle Tolerance", ref AngleTolerance);

                DisplayObjectListOption("Chain Objects", ChainElements);
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();

                for (int i = 0; i < ChainElements.Count - 1; i++)
                {
                    Handles.DrawDottedLine(ChainElements[i].transform.position,
                        ChainElements[i + 1].transform.position, 1f);
                }
            }

            public override void Stop()
            {
                base.Stop();

                Vector3[] newPositions = ChainElements.Convert(_ => _.transform.position).ToArray();
                Quaternion[] newRotations = ChainElements.Convert(_ => _.transform.rotation).ToArray();

                for (int i = 0; i < previousRotations.Length; i++)
                {
                    ChainElements[i].transform.position = previousPositions[i];
                    ChainElements[i].transform.rotation = previousRotations[i];
                }

                int undoIndex = MonkeyEditorUtils.CreateUndoGroup("Chain Move");
                for (int i = 0; i < ChainElements.Count; i++)
                {
                    Undo.RecordObject(ChainElements[i].transform, "Applying new position");
                    if (newPositions.Length <= i || newRotations.Length <= i)
                        break;
                    ChainElements[i].transform.position = newPositions[i];
                    ChainElements[i].transform.rotation = newRotations[i];
                }
                Undo.CollapseUndoOperations(undoIndex);
            }
        }

        [Command("Move Until Collision", QuickName = "MC",
                Help = "Constrains the selected objects to move on the selected axis " +
                       "until the next raycast hit",
                 DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "Transform")]
        public static void ConstrainRaycastAxis()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new AxisRayCastConstrainSceneCommand(Selection.gameObjects, DirectionAxis.DOWN, DirectionAxis.UP));
        }

        [Command("Move Until Collision 2D", QuickName = "MC2",
            Help = "Constrains the selected objects to move on the selected axis " +
                   "until the next 2D raycast hit",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "2D/Transform")]
        public static void ConstrainRaycastAxis2D()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new AxisRayCast2DConstrainSceneCommand(Selection.gameObjects, DirectionAxis.DOWN, DirectionAxis.UP));
        }

        public class AxisRayCastConstrainSceneCommand : TimedSceneCommand
        {
            public List<GameObject> Objects;
            //private readonly Vector3[] direction;
            private Vector3 direction;

            private readonly Vector3[] previousPositions;
            private readonly Quaternion[] previousRotations;

            private bool alignToCollision;

            private float toleranceDistance = 0.1f;
            private bool ignoreInvisibleColliders;
            private bool directionFromScreen;
            private DirectionAxis alignmentAxis;

            private bool active;

            public AxisRayCastConstrainSceneCommand(GameObject[] objects, DirectionAxis axis,
                DirectionAxis alignmentAxis) : base(0)
            {
                TimeBetweenUpdate = .01f;
                SceneCommandName = "Constrain Axis Raycast";
                Objects = new List<GameObject>(objects);
                this.alignmentAxis = alignmentAxis;
                alignToCollision = true;

                direction = TransformExt.GlobalAxisToVector(axis);
                previousPositions = Objects.Convert(_ => _.transform.position).ToArray();
                previousRotations = Objects.Convert(_ => _.transform.rotation).ToArray();

            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                Enum en = alignmentAxis;

                DisplayButton(active ? "Turn Off" : "Turn On", () =>
                  {
                      active = !active;
                  });

                DisplayDoubleOption("Time Between Updates", ref TimeBetweenUpdate);
                DisplayBoolOption("Ignore Invisible Colliders", ref ignoreInvisibleColliders);
                DisplayBoolOption("Align To Collision", ref alignToCollision);

                if (alignToCollision)
                {
                    DisplayEnumOption("Collision Alignment Axis", ref en);
                    alignmentAxis = (DirectionAxis)en;
                }

                DisplayBoolOption("Direction From Camera", ref directionFromScreen);

                if (!directionFromScreen)
                    DisplayVectorOption("Raycast Direction", ref direction);

                DisplayFloatOption("Raycast Tolerance", ref toleranceDistance);
                DisplayObjectListOption("Objects", Objects);
            }



            public override void Update()
            {
                base.Update();
                int i = 0;

                Objects.RemoveAll(_ => !_);

                if (!active)
                    return;

                foreach (GameObject gameObject in Objects)
                {
                    RaycastDirection(false,
                        gameObject.transform, direction,
                        alignToCollision, previousPositions[i], toleranceDistance, ignoreInvisibleColliders);
                    i++;
                }
            }

            public void RaycastDirection(bool local, Transform t, Vector3 direction,
                bool alignToNormal, Vector3 originalPosition, float tolerance = 10, bool ignoreInvisible = true)
            {
                if (directionFromScreen)
                {
                    var screenPoint = MonkeyEditorUtils.CurrentSceneView.camera.WorldToScreenPoint(t.position);
                    var ray = MonkeyEditorUtils.CurrentSceneView.camera.ScreenPointToRay(screenPoint);
                    direction = ray.direction;
                }

                var hits = Physics.RaycastAll(t.position - direction * tolerance,
                        local ? t.TransformDirection(direction) : direction).Where(_ =>
                        !Objects.Any(s => s.GetComponentsInChildren<Transform>().Contains(_.transform)))
                    .OrderBy(_ => (_.point - (t.position - direction * tolerance)).magnitude)
                    .ToList();

                if (ignoreInvisible)
                {
                    hits.RemoveAll(_ => _.transform.gameObject.GetComponent<Renderer>() &&
                                        !_.transform.gameObject.GetComponent<TerrainCollider>());
                }

                if (hits.Count != 0)
                {
                    t.AlignTransformToCollision(hits[0].point, hits[0].normal, alignToNormal, alignmentAxis);
                }

            }

            public override void Stop()
            {
                base.Stop();

                Objects.RemoveAll(_ => !_);

                Vector3[] newPositions = Objects.Convert(_ => _.transform.position).ToArray();
                Quaternion[] newRotations = Objects.Convert(_ => _.transform.rotation).ToArray();

                for (int i = 0; i < Objects.Count; i++)
                {
                    if (previousPositions.Length <= i)
                        break;
                    Objects[i].transform.position = previousPositions[i];
                    Objects[i].transform.rotation = previousRotations[i];
                }

                int undoIndex = MonkeyEditorUtils.CreateUndoGroup("Raycast down Constrain");
                for (int i = 0; i < Objects.Count; i++)
                {
                    Undo.RecordObject(Objects[i].transform, "Applying new position");
                    Objects[i].transform.position = newPositions[i];
                    Objects[i].transform.rotation = newRotations[i];
                }
                Undo.CollapseUndoOperations(undoIndex);
            }

        }

        public class AxisRayCast2DConstrainSceneCommand : TimedSceneCommand
        {
            public GameObject[] Objects;
            private readonly Vector3[] direction;

            private readonly Vector3[] previousPositions;
            private readonly Quaternion[] previousRotations;

            private DirectionAxis alignmentAxis;

            public AxisRayCast2DConstrainSceneCommand(GameObject[] objects, DirectionAxis axis,
                DirectionAxis alignmentAxis) : base(0)
            {
                SceneCommandName = "Constrain Axis Raycast";
                this.Objects = objects;
                this.alignmentAxis = alignmentAxis;
                HideGUI = true;
                direction = Objects.Convert(_ => _.transform.AxisToVector(axis, false)).ToArray();
                previousPositions = Objects.Convert(_ => _.transform.position).ToArray();
                previousRotations = Objects.Convert(_ => _.transform.rotation).ToArray();
            }

            public override void Update()
            {
                base.Update();
                int i = 0;

                Objects = Objects.Where(_ => _).ToArray();

                foreach (GameObject gameObject in Objects)
                {
                    RaycastDirection(false,
                        gameObject.transform, direction[i], true);
                    i++;
                }

            }

            public void RaycastDirection(bool local, Transform t, Vector3 direction,
                bool alignToNormal, float tolerance = 10)
            {
                Vector2 originalPosition = t.position;

                RaycastHit2D[] hits = Physics2D.RaycastAll(t.position - direction * tolerance,
                        local ? t.TransformDirection(direction) : direction).
                    Where(_ => !Objects.Any(s => s.GetComponentsInChildren<Transform>().Contains(_.transform)))
                    .OrderBy(_ => (_.point - originalPosition).magnitude)
                    .ToArray();


                if (hits.Length != 0)
                {
                    t.AlignTransformToCollision(hits[0].point, hits[0].normal, alignToNormal, alignmentAxis);
                }
                else
                {
                    hits = Physics2D.RaycastAll(t.position - direction * tolerance,
                              local ? t.TransformDirection(direction) : direction).
                          Where(_ => !Objects.Contains(_.transform.gameObject))
                          .OrderBy(_ => (_.point - originalPosition).magnitude)
                          .ToArray();

                    if (hits.Length > 0)
                    {
                        Undo.RecordObject(t, "movement");

                        t.AlignTransformToCollision(hits[0].point, hits[0].normal,
                            alignToNormal, alignmentAxis);
                    }
                }

            }

            public override void Stop()
            {
                base.Stop();

                Objects = Objects.Where(_ => _).ToArray();

                Vector3[] newPositions = Objects.Convert(_ => _.transform.position).ToArray();
                Quaternion[] newRotations = Objects.Convert(_ => _.transform.rotation).ToArray();

                for (int i = 0; i < Objects.Length; i++)
                {
                    Objects[i].transform.position = previousPositions[i];
                    Objects[i].transform.rotation = previousRotations[i];
                }

                int undoIndex = MonkeyEditorUtils.CreateUndoGroup("Raycast down Constrain");
                for (int i = 0; i < Objects.Length; i++)
                {
                    Undo.RecordObject(Objects[i].transform, "Applying new position");
                    Objects[i].transform.position = newPositions[i];
                    Objects[i].transform.rotation = newRotations[i];
                }
                Undo.CollapseUndoOperations(undoIndex);
            }

        }


        [Command("Project On Plane Interactive",
            "Locks the selected objects on a plane defined by a point" +
            " and a normal", QuickName = "PPI",
            Category = "Transform")]
        public static void LockPlane()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new LockPlaneSceneTransform(null, Axis.Y, true,
                    Selection.gameObjects.ToArray()));
        }

        [Command("Lock Transforms", QuickName = "LT",
            Help = "Prevents the selected objects' transforms from moving while the command is active",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT
            , Category = "Transform/Lock")]
        public static void LockTransforms()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new LockTransformsSceneCommand(Selection.gameObjects.Convert(_ => _.transform).ToArray()));
        }

        [Command("Lock Children Transforms", QuickName = "LCT",
            Help = "Prevents the selected objects' children transforms from moving while the command is active",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT
            , Category = "Transform/Lock")]
        public static void LockChildrenTransforms()
        {
            var selection = new System.Collections.Generic.List<Transform>();

            foreach (var gameObject in Selection.gameObjects)
            {
                selection.AddRange(gameObject.GetComponentsInChildren<Transform>()
                    .Where(_ => _ != gameObject.transform));
            }

            MonkeyEditorUtils.AddSceneCommand(new LockTransformsSceneCommand(selection.ToArray()));
        }

        public static List<Transform> GloballyLockedTransforms = new List<Transform>();

        public class LockTransformsSceneCommand : TimedSceneCommand
        {
            public List<Transform> TransformsToLock;

            public bool ShowLockGizmos = false;

            struct TransformData
            {
                public Vector3 Position;
                public Vector3 Scale;
                public Quaternion Rotation;
            }

            private readonly Vector3[] lockedPosition;
            private readonly Vector3[] lockedScale;
            private readonly Quaternion[] lockedRotations;

            public LockTransformsSceneCommand(Transform[] transforms) : base(0)
            {
                HideGUI = true;
                SceneCommandName = "Lock Transforms";
                TransformsToLock = new List<Transform>(transforms);
                lockedPosition = transforms.Convert(_ => _.position).ToArray();
                lockedScale = transforms.Convert(_ => _.lossyScale).ToArray();
                lockedRotations = transforms.Convert(_ => _.rotation).ToArray();

                GloballyLockedTransforms.AddRange(TransformsToLock);
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayBoolOption("Show Lock Gizmos", ref ShowLockGizmos);
                DisplayObjectListOption("Objects To Lock", TransformsToLock);
            }

            public override void Update()
            {
                base.Update();

                LockAllTransforms();
            }

            private void LockAllTransforms()
            {
                for (int i = 0; i < TransformsToLock.Count; i++)
                {
                    if (!TransformsToLock[i])
                        continue;
                    TransformsToLock[i].position = lockedPosition[i];
                    TransformsToLock[i].rotation = lockedRotations[i];
                    TransformsToLock[i].SetLossyGlobalScale(lockedScale[i]);
                }
            }

            private readonly GUIStyle textStyle = new GUIStyle()
            {
                normal = { textColor = Color.red }
            };
            public override void OnSceneGUI()
            {
                base.OnSceneGUI();

                LockAllTransforms();

                if (!ShowLockGizmos)
                    return;

                foreach (var transform in TransformsToLock)
                {
                    if (!transform)
                        continue;
                    Handles.color = Color.red.Alphaed(.2f);
#if UNITY_2017_1_OR_NEWER
                    Handles.DrawWireCube(transform.position, Vector2.one);
#else
                    Handles.DrawWireDisc(transform.position,Vector3.forward, 1);
#endif
                    Handles.Label(transform.position, "Locked", textStyle);
                }
            }

            /// <inheritdoc />
            public override void Stop()
            {
                base.Stop();
                foreach (var transform in TransformsToLock)
                {
                    if (!transform)
                        continue;
                    GloballyLockedTransforms.Remove(transform);
                }
            }
        }

        public class LockPlaneSceneTransform : TimedSceneCommand
        {
            public GameObject PlanePoint;
            public Axis PlaneAxis;
            public bool LocalAxis;
            public List<GameObject> ObjectsToConstrain;

            public TransformUndo Undo;

            public LockPlaneSceneTransform(GameObject planePoint, Axis planeAxis,
                bool localAxis, GameObject[] objectsToConstrain) : base(0)
            {
                PlanePoint = planePoint;
                PlaneAxis = planeAxis;
                LocalAxis = localAxis;
                ObjectsToConstrain = new List<GameObject>(objectsToConstrain);
                SceneCommandName = "Plane Constrain";
                Undo = new TransformUndo();

                Undo.Register(objectsToConstrain.Convert(_ => _.transform).ToArray());
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayObjectOption("Plane Point", ref PlanePoint);
                Enum en = PlaneAxis;
                DisplayEnumOption("Plane Axis", ref en);
                DisplayBoolOption("Local Axis", ref LocalAxis);
                DisplayObjectListOption("Constrained Objects", ObjectsToConstrain);

            }

            /// <inheritdoc />
            public override void Update()
            {
                base.Update();

                foreach (var o in ObjectsToConstrain)
                {
                    Vector3 projection = MathExt.ProjectPointOnPlane(
                        PlanePoint.transform.AxisToVector(PlaneAxis, LocalAxis),
                        PlanePoint.transform.position, o.transform.position);
                    o.transform.position = projection;
                }
            }

            public override void Stop()
            {
                base.Stop();
                int undoID = MonkeyEditorUtils.CreateUndoGroup("Plane Constrain");
                Undo.RecordUndo();
                UnityEditor.Undo.CollapseUndoOperations(undoID);
            }
        }

    }
}
