using MonKey.Editor.Internal;
using MonKey.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonKey.Editor.Commands
{
    public static class MoveUtilities
    {
        [Command("Move Under Mouse",
            Help = "Moves the selected objects to the position raycasted from the mouse",
            QuickName = "MM", AlwaysShow = true,
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "MoveMouseRaycast",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform")]
        public static void MoveObjectUnderMouse()
        {
            MonkeyEditorUtils.AddSceneCommand(new RaycastSceneCommand(Selection.gameObjects, 0,
                DirectionAxis.UP));
        }

        public class RaycastSceneCommand : ConfirmedCommand
        {
            public GameObject[] CachedObjectsToMove;
            public float Offset = 0;
            public float AngleOffset = 0;
            public DirectionAxis AlignToNormal;
            public DirectionAxis ForwardAlign;
            public bool IgnoreInvisibleColliders;

            private Transform[] selfAndChildren;

            struct PositionRotation
            {
                public readonly Vector3 Position;
                public readonly Quaternion Rotation;

                public PositionRotation(Vector3 position, Quaternion rotation)
                {
                    this.Position = position;
                    this.Rotation = rotation;
                }
            }

            private readonly List<PositionRotation> previousPosRot = new List<PositionRotation>();

            public RaycastSceneCommand(GameObject[] cachedObjectsToMove, float offset,
                DirectionAxis alignToNormal)
            {
                //  HideGUI = true;
                ConfirmationMode = ActionConfirmationMode.ENTER_AND_CLICK;
                SceneCommandName = "RayCast Mouse Position";
                IgnoreInvisibleColliders = true;
                CachedObjectsToMove = cachedObjectsToMove;
                Offset = offset;
                AlignToNormal = alignToNormal;
                ForwardAlign = DirectionAxis.FORWARD;
                List<Transform> allChildren = new List<Transform>();
                foreach (GameObject gameObject in cachedObjectsToMove)
                {
                    previousPosRot.Add(
                        new PositionRotation(gameObject.transform.position,
                            gameObject.transform.rotation));
                    allChildren.AddRange(gameObject.GetComponentsInChildren<Transform>());
                }

                selfAndChildren = allChildren.ToArray();
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayFloatOption("Collision Offset", ref Offset);
                DisplayFloatOption("Angle Offset", ref AngleOffset);
                Enum en = AlignToNormal;

                DisplayEnumOption("Up Axis", ref en);
                AlignToNormal = (DirectionAxis) en;

                en = ForwardAlign;
                DisplayEnumOption("Forward Axis", ref en);
                ForwardAlign = (DirectionAxis) en;

                DisplayBoolOption("Ignore Invisible Colliders", ref IgnoreInvisibleColliders);
            }

            public override void Update()
            {
                base.Update();

                bool collision = false;
                Vector3 normal;

                selfAndChildren = selfAndChildren.Where(_ => _).ToArray();

                var position = MonkeyEditorUtils.GetMouseRayCastedPosition(selfAndChildren, Offset,
                    out collision, out normal, IgnoreInvisibleColliders);

                if (collision)
                {
                   // CachedObjectsToMove = CachedObjectsToMove.Where(_ => _).ToArray();

                    foreach (GameObject gameObject in CachedObjectsToMove)
                    {
                        if (!gameObject)
                            continue;
                        
                        gameObject.transform.AlignTransformToCollision(position, normal, true, AlignToNormal,
                            ForwardAlign);
                        gameObject.transform.rotation =
                            Quaternion.AngleAxis(AngleOffset, normal) * gameObject.transform.rotation;
                    }
                }
            }

            public override void Stop()
            {
                base.Stop();
                List<PositionRotation> newPosRot =
                    new List<PositionRotation>(CachedObjectsToMove.Length);

                foreach (var gameObject in CachedObjectsToMove)
                {
                    if (!gameObject)
                        continue;

                    newPosRot.Add(
                        new PositionRotation(gameObject.transform.position,
                            gameObject.transform.rotation));
                }

                for (int i = 0; i < previousPosRot.Count; i++)
                {
                    if (! CachedObjectsToMove[i])
                        continue;

                    CachedObjectsToMove[i].transform.position = previousPosRot[i].Position;
                    CachedObjectsToMove[i].transform.rotation = previousPosRot[i].Rotation;
                }

                int undoIndex = MonkeyEditorUtils.CreateUndoGroup("Mouse Raycast");
                for (int i = 0; i < newPosRot.Count; i++)
                {
                    if (! CachedObjectsToMove[i])
                        continue;
                    
                    Undo.RecordObject(CachedObjectsToMove[i].transform, "Applying new position");
                    CachedObjectsToMove[i].transform.position = newPosRot[i].Position;
                    CachedObjectsToMove[i].transform.rotation = newPosRot[i].Rotation;
                }

                CachedObjectsToMove = CachedObjectsToMove.Where(_ => _).ToArray();
                Selection.objects = CachedObjectsToMove.Convert(_ => _ as Object).ToArray();
                Undo.CollapseUndoOperations(undoIndex);
            }
        }


        [Command("Copy Position",
            Help = "Copies the position of an object, by default the first object selected",
            QuickName = "CP", DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "CopyPosition",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform")]
        public static void CopyPosition()
        {
            MonkeyEditorUtils.AddSceneCommand(new TransformCopyCommand(Selection.gameObjects) {CopyPosition = true,});
        }

        public class TransformCopyCommand : TimedSceneCommand
        {
            public Transform TransformToCopy;

            public bool LocalValuesOnly;

            public bool CopyPosition;
            public bool CopyRotation;
            public bool CopyScale;

            public List<GameObject> Targets = new List<GameObject>();

            public TransformCopyCommand(params GameObject[] objs) : base(0)
            {
                Targets = new List<GameObject>(objs);
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();

                SceneCommandName = "Copy Transform";

                DisplayObjectOption("Transform To Copy", ref TransformToCopy);
                DisplayBoolOption("Local Values Only", ref LocalValuesOnly);
                DisplayBoolOption("Copy Position", ref CopyPosition);
                DisplayBoolOption("Copy Rotation", ref CopyRotation);
                DisplayBoolOption("Copy Scale", ref CopyScale);

                DisplayObjectListOption("Targets", Targets);
            }

            public override void Update()
            {
                base.Update();

                if (!TransformToCopy)
                    return;

                foreach (GameObject o in Targets)
                {
                    if (!o)
                        continue;

                    if (CopyPosition)
                    {
                        if (!LocalValuesOnly)
                            o.transform.position = TransformToCopy.position;
                        else
                            o.transform.localPosition = TransformToCopy.localPosition;
                    }

                    if (CopyRotation)
                    {
                        if (!LocalValuesOnly)
                            o.transform.rotation = TransformToCopy.rotation;
                        else
                            o.transform.localRotation = TransformToCopy.localRotation;
                    }

                    if (CopyScale)
                    {
                        if (!LocalValuesOnly)
                            o.transform.SetLossyGlobalScale(TransformToCopy.lossyScale);
                        else
                            o.transform.localScale = TransformToCopy.localScale;
                    }
                }
            }
        }


        [Command("Copy Rotation",
            Help = "Copies the rotation of an object, by default the first object selected",
            QuickName = "CR", DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "CopyRotation", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform")]
        public static void CopyRotation()
        {
            MonkeyEditorUtils.AddSceneCommand(new TransformCopyCommand(Selection.gameObjects) {CopyRotation = true});
        }


        [Command("Copy Transform",
            Help = "Copies the transform (scale ignored) of an object," +
                   " by default the first object selected",
            QuickName = "CT", DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "CopyTransform", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform")]
        public static void CopyTransform()
        {
            MonkeyEditorUtils.AddSceneCommand(new TransformCopyCommand(Selection.gameObjects)
                {CopyPosition = true, CopyRotation = true, CopyScale = true});
        }


        private static GameObject ActiveObject()
        {
            return DefaultValuesUtilities.DefaultFirstGameObjectSelected();
        }


        public static void RaycastDirection(bool local, Transform t, Vector3 direction,
            DirectionAxis axisToHitNormal)
        {
            RaycastHit hit;
            if (Physics.Raycast(t.position, local ? t.TransformDirection(direction) : direction,
                out hit))
            {
                Undo.RecordObject(t, "movement");
                t.AlignTransformToCollision(hit.point, hit.normal, true, axisToHitNormal);
            }
        }

        public static void RaycastDirection2D(bool local, Transform t, Vector3 direction,
            DirectionAxis axisToHitNormal, GameObject[] objectsToIgnore)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(t.position, local ? t.TransformDirection(direction) : direction)
                .OrderByDescending(_ => (_.point - (Vector2) t.position).magnitude).ToArray();
            foreach (var hit in hits)
            {
                if (objectsToIgnore.Contains(hit.transform.gameObject)
                    || objectsToIgnore.Any(_ => _.GetComponentsInChildren<Transform>().Contains(hit.transform)))
                    continue;

                if (hit)
                {
                    Undo.RecordObject(t, "movement");
                    t.AlignTransformToCollision(hit.point, hit.normal, true, axisToHitNormal);
                }
                else
                {
                    Debug.Log("Nope");
                }
            }
        }


        public static Axis[] DefaultLockedAxis()
        {
            return new[] {Axis.NONE};
        }

        public static GameObject FirstSelected()
        {
            if (!MonkeyEditorUtils.OrderedSelectedGameObjects.Any())
                return null;
            return MonkeyEditorUtils.OrderedSelectedGameObjects.ElementAt(0);
        }

        public static Vector3 ZeroVector()
        {
            return Vector3.zero;
        }

        [Command("Rotate Around",
            "Rotates the selected objects around a specified one (by default the first selected)," +
            " on one of its local axes", QuickName = "RA",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            Category = "Transform")]
        public static void RotateAround()
        {
            MonkeyEditorUtils.AddSceneCommand(new RotateAroundSceneCommand(Selection.gameObjects.ToList()));
        }

        public class RotateAroundSceneCommand : TimedSceneCommand
        {
            public float Angle = 90;
            public GameObject ReferenceObject;
            public Axis RotationAxis = Axis.Y;
            public List<GameObject> ObjectsToRotate;


            public RotateAroundSceneCommand(List<GameObject> objectsToRotate) : base(0)
            {
                ObjectsToRotate = objectsToRotate;
                SceneCommandName = "Rotate Around";
            }

            public void Rotate()
            {
                int undoID = MonkeyEditorUtils.CreateUndoGroup("Rotate Around");

                foreach (var gameObject in ObjectsToRotate)
                {
                    if (gameObject == ReferenceObject)
                        continue;

                    Undo.RecordObject(gameObject.transform, "rotated");
                    gameObject.transform.RotateAround(ReferenceObject.transform.position,
                        ReferenceObject.transform.AxisToVector(RotationAxis, true), Angle);
                }

                Undo.CollapseUndoOperations(undoID);
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayObjectOption("Rotation Center", ref ReferenceObject);
                Enum en = RotationAxis;
                DisplayEnumOption("Rotation Axis", ref en);
                RotationAxis = (Axis) en;
                DisplayFloatOption("Angle", ref Angle);
                DisplayObjectListOption("Objects To Rotate", ObjectsToRotate);

                DisplayButton("Rotate", Rotate);
            }
        }

        [Command("Rotate",
            "Rotates the selected objects on their own axes", QuickName = "RO",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            Category = "Transform/Set")]
        public static void Rotate(
            [CommandParameter("The axis of rotation, as the local axis of the object")]
            Axis rotationAxis = Axis.Y,
            [CommandParameter("The angle of rotation, in degrees")]
            float angle = 90,
            [CommandParameter("Should the rotation be according to local axes")]
            bool local = true)
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Rotate");

            foreach (GameObject o in Selection.gameObjects)
            {
                Undo.RecordObject(o.transform, "rotated");

                o.transform.Rotate(o.transform.AxisToVector(rotationAxis, local), angle);
            }

            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Reset Transforms",
            "Sets all the local values of the selected transform to default",
            QuickName = "RT", DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "ResetTransforms", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform/Set")]
        public static void ResetTransforms()
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Reset Transforms");
            foreach (Transform transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "reset");
                transform.Reset();
            }

            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Reset Rotation",
            "Sets all the local rotations of the selected transform to default",
            QuickName = "RR", DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "ResetRotations", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform/Set")]
        public static void ResetRotations()
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Reset Transforms");
            foreach (Transform transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "reset");
                transform.rotation = Quaternion.identity;
            }

            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Set Parent", "Sets the parent of the selected objects", QuickName = "SP",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "SetParent", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform/Set")]
        public static void SetParent(
            [CommandParameter(Help = "The new parent of the selected objects",
                ForceAutoCompleteUsage = true, PreventDefaultValueUsage = true)]
            GameObject newParent)
        {
            int undoGroup = MonkeyEditorUtils.CreateUndoGroup("Change Parent");

            foreach (var go in Selection.gameObjects)
            {
                Undo.SetTransformParent(go.transform, newParent.transform, "Reparent");
            }

            Undo.CollapseUndoOperations(undoGroup);
            EditorGUIUtility.PingObject(newParent);
        }

        [Command("Set Parent Dynamic", "Sets the parent of the selected objects, as a scene command", QuickName = "SPD",
            Category = "Transform/Set")]
        public static void SetParentDynamic()
        {
            MonkeyEditorUtils.AddSceneCommand(new SetParentCommand(Selection.activeGameObject));
        }

        public class SetParentCommand : SceneCommand
        {
            public GameObject Parent;

            public bool PreservePositionOnParenting = true;

            public SetParentCommand(GameObject parent)
            {
                Parent = parent;
                SceneCommandName = "Set Parent Dynamic";
            }

            public override void Update()
            {
                //do nothing
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                base.DisplayObjectOption("Parent Object", ref Parent);
                DisplayBoolOption("Preserve Position On Parenting", ref PreservePositionOnParenting);
                DisplayMessage("Reparents Selected Objects under the specified parent");
                DisplayButton("Reparent", ApplyFunction);
            }

            public void ApplyFunction()
            {
                foreach (var gameObject in Selection.gameObjects)
                {
                    gameObject.transform.SetParent(Parent ? Parent.transform : null, PreservePositionOnParenting);
                }
            }
        }


        [Command("Set Pivot On Transform", QuickName = "MP",
            Help = "set the position and rotation of  the selected transforms as pivot, " +
                   "which means that their children will not move or rotate",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "MovePivot", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform")]
        public static void MoveAsPivot()
        {
            MonkeyEditorUtils.AddSceneCommand(new PivotMoveSceneCommand(Selection.gameObjects));
        }

        public class PivotMoveSceneCommand : TimedSceneCommand
        {
            public List<GameObject> PivotsToMove = new List<GameObject>();

            public Transform ToCopyOn;
            public bool CopyRotation;

            private Vector3 lastPosition;
            private Quaternion lastRotation;

            public PivotMoveSceneCommand(params GameObject[] pivots) : base(0)
            {
                SceneCommandName = "Move Pivot";
                PivotsToMove.AddRange(pivots);
                lastRotation = Quaternion.identity;
            }

            public override void DisplayParameters()
            {
                Transform previous = ToCopyOn;
                DisplayObjectOption("Transform To Copy", ref ToCopyOn);
                if (!ToCopyOn == previous)
                {
                    Copy();
                }

                DisplayBoolOption("Copy Rotation", ref CopyRotation);
                DisplayObjectListOption("Pivots To Move", PivotsToMove);
            }

            public override void Update()
            {
                if (!ToCopyOn)
                    return;

                Vector3 distance = ToCopyOn.position - lastPosition;
                var angle = Quaternion.Angle(ToCopyOn.rotation, lastRotation);
                if (!ToCopyOn || !ToCopyOn.hasChanged ||
                    (distance.magnitude < 0.001f && (!CopyRotation || angle < 0.01f)))
                    return;

                Copy();
            }

            private void Copy()
            {
                lastPosition = ToCopyOn.position;
                lastRotation = ToCopyOn.rotation;

                int undoID = MonkeyEditorUtils.CreateUndoGroup("Move Pivot");
                foreach (var go in Selection.gameObjects)
                {
                    if (go == ToCopyOn)
                        continue;

                    Transform[] children = go.GetComponentsInChildren<Transform>()
                        .Where(_ => _ != go.transform).ToArray();
                    foreach (Transform child in children)
                    {
                        Vector3 position = child.position;
                        Quaternion rotation = child.rotation;

                        Undo.SetTransformParent(child, null, "movedObjectsOut");
                        child.position = position;
                        child.rotation = rotation;
                    }

                    Undo.RecordObject(go.transform, "moving pivot");

                    go.transform.position = ToCopyOn.transform.position;
                    if (CopyRotation)
                        go.transform.rotation = ToCopyOn.transform.rotation;

                    foreach (Transform child in children)
                    {
                        Vector3 position = child.position;
                        Quaternion rotation = child.rotation;

                        Undo.SetTransformParent(child, go.transform, "movedObjectsOut");

                        child.position = position;
                        child.rotation = rotation;
                    }
                }

                Undo.CollapseUndoOperations(undoID);
            }
        }

        [Command("Center Pivot", QuickName = "CPI",
            Help = "Moves the selected transforms as pivot " +
                   "to the center of all their children transform",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            MenuItemLink = "CenterPivot", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Transform")]
        public static void MovePivotToChildrenCenter()
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Move Pivot");
            foreach (var go in Selection.gameObjects)
            {
                Transform[] children = go.GetComponentsInChildren<Transform>()
                    .Where(_ => _ != go.transform).ToArray();
                Vector3[] childrenPos = children.Convert(_ => _.position).ToArray();
                Vector3 weightCenter = new Vector3();

                foreach (Transform child in children)
                {
                    weightCenter += child.position;
                }

                weightCenter /= children.Length;

                Undo.RecordObject(go.transform, "moving pivot");

                go.transform.position = weightCenter;

                for (int i = 0; i < children.Length; i++)
                {
                    Undo.RecordObject(children[i], "moving");
                    children[i].position = childrenPos[i];
                }
            }

            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Move Assets To Folder", "Moves the selected asset to the specified folder",
            QuickName = "MA", DefaultValidation = DefaultValidation.AT_LEAST_ONE_ASSET,
            Category = "Assets")]
        public static void MoveAssetToFolder(
            [CommandParameter(Help = "The name of the folder to which the selection will be moved",
                ForceAutoCompleteUsage = true, PreventDefaultValueUsage = true,
                AutoCompleteMethodName = "FolderAutoComplete")]
            string folderName)
        {
            foreach (var o in Selection.objects.Where(AssetDatabase.Contains))
            {
                string objectName = AssetDatabase.GetAssetPath(o).GetAssetNameFromPath(true);

                string newFolder = folderName + "/" + objectName;

                if (AssetDatabase.ValidateMoveAsset(AssetDatabase.GetAssetPath(o), newFolder).IsNullOrEmpty())
                {
                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(o), newFolder);
                    EditorGUIUtility.PingObject(o);
                }
                else
                {
                    Debug.LogWarningFormat("Monkey Warning: The asset {0} " +
                                           "could not be moved to {1}.", o.name, newFolder);
                    Debug.LogWarning(AssetDatabase.ValidateMoveAsset(AssetDatabase.GetAssetPath(o), folderName));
                }
            }
        }

        [Command("Position In Circle",
            Help =
                "Positions all the selected objects in circle around the specified object (by default the first selected)",
            QuickName = "PIC",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            Category = "Transform")]
        public static void PositionInCircle()
        {
            MonkeyEditorUtils.AddSceneCommand(new PositionInCircleSceneCommand(
                MonkeyEditorUtils.OrderedSelectedGameObjects.ToList()
            ));
        }

        [Command("Resolve Intersections", "Moves the selected objects so that they do not overlap each other",
            QuickName = "RI"
            , DefaultValidation = DefaultValidation.AT_LEAST_TWO_GAME_OBJECTS)]
        public static void ResolveIntersections(int iterations = 5)
        {
            var addedColliders = new List<Collider>();

            TransformUndo undo = new TransformUndo();
            undo.Register(Selection.gameObjects.Convert(_ => _.transform).ToArray());

            foreach (var gameObject in Selection.gameObjects)
            {
                var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();

                if (renderers.Length == 0)
                    continue;

                foreach (var renderer in renderers)
                {
                    if (!renderer.GetComponent<Collider>())
                    {
                        var col = renderer.gameObject.AddComponent<MeshCollider>();
                        col.sharedMesh = renderer.GetComponent<MeshFilter>().sharedMesh;
                        col.convex = true;
                        addedColliders.Add(col);
                    }
                }
            }

            for (int i = 0; i < iterations; i++)
            {
                EditorUtility.DisplayProgressBar("Resolving Intersection...",
                    "MonKey is resolving the intersections", (float) i / iterations);

                foreach (var gameObject in Selection.gameObjects)
                {
                    var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
                    foreach (var otherObj in Selection.gameObjects)
                    {
                        var otherRenderers = otherObj.GetComponentsInChildren<MeshRenderer>();

                        foreach (var renderer in renderers)
                        {
                            if (!renderer.gameObject.activeInHierarchy)
                                continue;

                            var collider = renderer.GetComponent<Collider>();
                            foreach (var otherRenderer in otherRenderers)
                            {
                                if (otherRenderer == renderer)
                                    continue;

                                if (!otherRenderer.gameObject.activeInHierarchy)
                                    continue;

                                var otherCollider = otherRenderer.GetComponent<Collider>();
                                if (Physics.ComputePenetration(collider, renderer.transform.position,
                                    renderer.transform.rotation,
                                    otherCollider, otherRenderer.transform.position, otherRenderer.transform.rotation,
                                    out var dir, out var distance))
                                {
                                    gameObject.transform.position += dir * distance;
                                }
                            }
                        }
                    }
                }

                EditorUtility.ClearProgressBar();
            }

            foreach (var collider in addedColliders)
            {
                Object.DestroyImmediate(collider);
            }

            int id = MonkeyEditorUtils.CreateUndoGroup("MonKey - Resolve Intersections");
            undo.RecordUndo();
            Undo.CollapseUndoOperations(id);
        }

        public class PositionInCircleSceneCommand : TimedSceneCommand
        {
            public GameObject CircleCenter;

            public Axis CircleAxis;
            public float CircleRadius = 1;

            public float StartAngle = 0;
            public float EndAngle = 360;
            public bool LookAtCenter = false;
            public Vector3 UpVector;
            public Vector3 Offset;

            public bool useLocal;

            public List<GameObject> ObjectsToPosition;

            public PositionInCircleSceneCommand(List<GameObject> objectsToPosition) : base(0)
            {
                SceneCommandName = "Position In Circle";
                ObjectsToPosition = objectsToPosition;
                TimeBetweenUpdate = 0.1f;
                EndAngle = 360;
                UpVector = Vector3.up;
            }

            public override void Update()
            {
                base.Update();

                if (!CircleCenter || Mathf.Approximately(CircleRadius, 0))
                    return;

                float angle = StartAngle * Mathf.Deg2Rad;

                foreach (var gameObject in ObjectsToPosition)
                {
                    if (gameObject == CircleCenter)
                        continue;
                    float xValue = CircleRadius * Mathf.Cos(angle);
                    float yValue = CircleRadius * Mathf.Sin(angle);
                    Vector3 radius = Vector3.zero;
                    Vector3 offset = Offset;
                    switch (CircleAxis)
                    {
                        case Axis.X:
                            radius = new Vector3(0, yValue, xValue);
                            break;
                        case Axis.Y:
                            radius = new Vector3(xValue, 0, yValue);
                            break;
                        case Axis.Z:
                            radius = new Vector3(xValue, yValue, 0);
                            break;
                        case Axis.NONE:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("circleAxis", CircleAxis, null);
                    }

                    offset += Offset;
                    gameObject.transform.position = CircleCenter.transform.position +
                                                    (useLocal
                                                        ? CircleCenter.transform.TransformDirection(radius)
                                                        : radius) + (useLocal
                                                        ? CircleCenter.transform.TransformDirection(offset)
                                                        : offset);
                    if (LookAtCenter)
                    {
                        gameObject.transform.LookAt(CircleCenter.transform.position,
                            (useLocal ? CircleCenter.transform.TransformDirection(UpVector) : UpVector));
                    }

                    angle += (EndAngle * Mathf.Deg2Rad) / (ObjectsToPosition.Count);
                }
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayObjectOption("Circle Center", ref CircleCenter);
                Enum en = CircleAxis;
                DisplayEnumOption("Circle Axis", ref en);
                DisplayBoolOption("Use Local Axes", ref useLocal);
                CircleAxis = (Axis) en;
                DisplayFloatOption("Circle Radius", ref CircleRadius);

                if (CircleRadius <= 0)
                {
                    CircleRadius = 0.1f;
                }

                DisplayVectorOption("Offset", ref Offset);

                DisplayBoolOption("Look At Center", ref LookAtCenter);
                if (LookAtCenter)
                    DisplayVectorOption("Look At Up Axis", ref UpVector);

                DisplayFloatOption("Start Angle", ref StartAngle);
                DisplayFloatOption("End Angle", ref EndAngle);
                DisplayObjectListOption("Objects To Position", ObjectsToPosition);
            }
        }

        public static GameObject DefaultCenter()
        {
            return MonkeyEditorUtils.OrderedSelectedGameObjects.ElementAt(0);
        }

        /*
        [Command("Move To Mouse Nearest Vertex",
            "Moves the selected objects to the closest vertex to the mouse")]
        public static void MoveToNearestVector()
        {
            MonkeyEditorUtils.AddSceneCommand(new VertexMoveSceneCommand(Selection.gameObjects));
        }

        public class VertexMoveSceneCommand : ConfirmedCommand
        {
            private Transform[] toMove;
            private MethodInfo vertexFind;

            public VertexMoveSceneCommand(GameObject[] toMove)
            {
                this.toMove = toMove.Convert(_=>_.transform).ToArray();
                vertexFind = typeof(HandleUtility).GetMethod("FindNearestVertex",
                    BindingFlags.NonPublic | BindingFlags.Static);
            }

            public override void Update()
            {
                base.Update();
                Vector3 nearestVertex=new Vector3();
                //TODO not working: try to find a solution
                object[] parameters = { MonkeyEditorUtils.MousePosition, toMove, nearestVertex };
                object result = vertexFind.Invoke(null, parameters);
                bool blResult = (bool)result;

                if (blResult)
                {
                    nearestVertex = (Vector3)parameters[2];
                    toMove = toMove.Where(_ => _).ToArray();
                    foreach (var transform in toMove)
                    {
                        transform.position = nearestVertex;
                    }
                }
            }
        }
        */

        [Command("Move Mirror",
            "allows you to move  selected objects mirrored relative to the distance of the active object to the centroid",
            Category = "Transform")]
        public static void MoveRelatedToCentroid()
        {
            MonkeyEditorUtils.AddSceneCommand(new MirrorRelatedToCentroidSceneCommand());
        }

        public class MirrorRelatedToCentroidSceneCommand : TimedSceneCommand
        {
            public GameObject ReferenceObject;

            public List<GameObject> InfluencedObjects;

            public Vector3 Centroid;

            public Vector3 ReferenceDistance;

            public MirrorRelatedToCentroidSceneCommand() : base(-1)
            {
                InfluencedObjects = new List<GameObject>(Selection.gameObjects);
                Centroid = Vector3.zero;
                for (int i = 0; i < InfluencedObjects.Count; i++)
                {
                    Centroid += InfluencedObjects[i].transform.position;
                }

                Centroid /= InfluencedObjects.Count;
            }

            public override void Update()
            {
                base.Update();
                if (Selection.activeGameObject != ReferenceObject)
                {
                    ReferenceObject = Selection.activeGameObject;
                    ReferenceDistance = ReferenceObject.transform.position - Centroid;
                    return;
                }

                if (!ReferenceObject)
                    return;
                var newDistance = ReferenceObject.transform.position - Centroid;
                ReferenceDistance = newDistance;


                for (var index = 0; index < InfluencedObjects.Count; index++)
                {
                    var gameObject = InfluencedObjects[index];
                    if (gameObject != ReferenceObject && gameObject)
                    {
                        var distance = -ReferenceDistance;
                        gameObject.transform.position = Centroid + distance;
                    }
                }
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();
                Centroid = Handles.PositionHandle(Centroid, Quaternion.identity);
                Handles.Label(Centroid + Vector3.up * 0.1f, "Centroid");
                foreach (var gameObject in InfluencedObjects)
                {
                    Handles.DrawDottedLine(gameObject.transform.position, Centroid, 0.1f);
                }
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayObjectListOption("Objects to Move", InfluencedObjects);
            }
        }


        public static AssetNameAutoComplete FolderAutoComplete()
        {
            return new AssetNameAutoComplete() {DirectoryMode = true};
        }

        [MenuItem("Tools/MonKey Commander/Commands/Game Object/Move Up %&-")]
        [Command("Move Siblings Up", Category = "BFT", QuickName = "MU")]
        public static void MoveObjectUpInSiblings()
        {
            foreach (var o in Selection.gameObjects)
            {
                o.transform.SetSiblingIndex(Mathf.Max(0, o.transform.GetSiblingIndex() - 1));
            }
        }

        [MenuItem("Tools/MonKey Commander/Commands/Game Object/Move Down %&=")]
        [Command("Move Siblings Down", Category = "BFT", QuickName = "MD")]
        public static void MoveObjectDownInSiblings()
        {
            foreach (var o in Selection.gameObjects)
            {
                o.transform.SetSiblingIndex(Mathf.Min(
                    o.transform.parent ? o.transform.parent.childCount - 1 : o.scene.rootCount - 1,
                    o.transform.GetSiblingIndex() + 1));
            }
        }

        [Command("Move Pivot On Bounding Box",
            "Moves the pivot transform anywhere within the bounding box of the object", QuickName = "MPB",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT)]
        public static void MovePivotOnBoundingBox()
        {
            if (Selection.activeGameObject.transform.childCount == 0)
            {
                Debug.LogWarning("MonKey Warning:" +
                                 " You are trying to move a a game object as a pivot " +
                                 "but it is not a pivot since it doesn't have children objects: " +
                                 "this command will not be executed");
                return;
            }

            MonkeyEditorUtils.AddSceneCommand(new MovePivotSceneCommand());
        }

        public class MovePivotSceneCommand : TimedSceneCommand
        {
            public enum BoundingBoxSide
            {
                TOP,
                BOTTOM,
                LEFT,
                RIGHT,
                FRONT,
                BACK
            }

            public enum BoundingBoxCorner
            {
                TOP_LEFT,
                TOP_RIGHT,
                TOP_CENTER,
                BOTTOM_LEFT,
                BOTTOM_RIGHT,
                BOTTOM_CENTER,
                CENTER_RIGHT,
                CENTER_LEFT,
                CENTER
            }

            public GameObject PivotToMove;

            private Vector3 previousPosition;

            // public BoundingBoxSide BoxSide;
            public float XPercent = 0.5f;
            public float YPercent = 0.5f;
            public float ZPercent = 0.5f;

            private Vector3 newPivot;

            private Bounds bounds;

            public MovePivotSceneCommand() : base(0)
            {
                SceneCommandName = "Move Pivot On BBox";
                PivotToMove = Selection.activeGameObject;
                TimeBetweenUpdate = 0;
            }

            public override void Update()
            {
                base.Update();

                if (!PivotToMove)
                    return;
                bounds = GetMaxBounds(PivotToMove);

                var children = new List<Transform>(PivotToMove.GetComponentsInChildren<Transform>());
                children.Remove(PivotToMove.transform);
                List<Vector3> previousPos = new List<Vector3>(children.Count);

                foreach (var transform in children)
                {
                    previousPos.Add(transform.position);
                }

                newPivot = bounds.center + bounds.size.x * (XPercent - 0.5f) * Vector3.right +
                           bounds.size.y * (YPercent - 0.5f) * Vector3.up
                           + bounds.size.z * (ZPercent - 0.5f) * Vector3.forward;
                PivotToMove.transform.position = newPivot;

                for (var i = 0; i < children.Count; i++)
                {
                    var transform = children[i];
                    transform.position = previousPos[i];
                }
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();
                Handles.color = Color.blue;
                Handles.SphereHandleCap(-1, newPivot, Quaternion.identity, .1f, EventType.Ignore);
                Handles.DrawWireCube(bounds.center, bounds.size);
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayObjectOption("Pivot To Move", ref PivotToMove);
                DisplayFloatPercentOption("X Percent", ref XPercent);
                DisplayFloatPercentOption("Y Percent", ref YPercent);
                DisplayFloatPercentOption("Z Percent", ref ZPercent);
            }

            private Bounds GetMaxBounds(GameObject g)
            {
                var renderers = g.GetComponentsInChildren<Renderer>();

                if (renderers.Length == 0)
                {
                    return new Bounds(g.transform.position, Vector3.zero);
                }

                var first = renderers.First();
                var b = first.bounds;
                foreach (Renderer r in renderers)
                {
                    if (r == first)
                        continue;

                    b.Encapsulate(r.bounds);
                }

                return b;
            }
        }
    }
}