using MonKey.Editor.Internal;
using MonKey.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MonKey.Editor.Commands
{
    public static class PhysicsUtilities
    {
        [Command("Set MeshColliders Convex",
            "Sets all the mesh colliders  in the selected object and their children as convex",
            ValidationMethodName = "ObjectsSelected", QuickName = "SMC",
            Category = "Physics")]
        public static void SetMeshCollidersConvex()
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                foreach (var collider in gameObject.GetComponentsInChildren<MeshCollider>())
                {
                    collider.convex = true;
                }
            }
        }

        [CommandValidation(DefaultValidationMessages.DEFAULT_SELECTED_GAMEOBJECTS)]
        public static bool ObjectsSelected()
        {
            return Selection.gameObjects.Length > 0;
        }


#if UNITY_2017_1_OR_NEWER
        private static EditorPhysicsSceneCommand currentEditorPhysics;
#endif
        [Command("Editor Physics", "Turn on the physics on all game objects in edit mode",
            DefaultValidation = DefaultValidation.IN_EDIT_MODE, QuickName = "EP", AlwaysShow = true
            , MenuItemLink = "ToggleEditorPhysics", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Physics")]
        public static void ToggleEditorPhysics()
        {
#if UNITY_2017_1_OR_NEWER
            if (currentEditorPhysics != null)
            {
                currentEditorPhysics.Stop();

                return;
            }
            currentEditorPhysics = new EditorPhysicsSceneCommand(Object.FindObjectsOfType<Rigidbody>());
            MonkeyEditorUtils.AddSceneCommand(currentEditorPhysics);
#endif
        }

#if UNITY_2017_1_OR_NEWER
        private static EditorPhysics2DSceneCommand currentEditorPhysics2D;
#endif
        [Command("Editor Physics 2D", "Turn on the physics on all 2D game objects in edit mode",
            DefaultValidation = DefaultValidation.IN_EDIT_MODE, QuickName = "EP2",
            AlwaysShow = true,
            Category = "2D/Physics")]
        public static void ToggleEditorPhysics2D()
        {
#if UNITY_2017_1_OR_NEWER
            if (currentEditorPhysics2D != null)
            {
                currentEditorPhysics2D.Stop();
                return;
            }
            currentEditorPhysics2D = new EditorPhysics2DSceneCommand(Object.FindObjectsOfType<Rigidbody2D>());
            MonkeyEditorUtils.AddSceneCommand(currentEditorPhysics2D);
#endif
        }

        [Command("Editor Physics Selected",
            "Plays the physics on all selected game objects and children",
            DefaultValidation = DefaultValidation.IN_EDIT_MODE_AT_LEAST_ONE_GAME_OBJECT
            , QuickName = "EPS", MenuItemLink = "ToggleEditorPhysicsSelected",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Physics")]
        public static void ToggleEditorPhysicsSelected()
        {
#if UNITY_2017_1_OR_NEWER
            if (currentEditorPhysics != null)
            {
                currentEditorPhysics.Stop();
                return;
            }

            List<Rigidbody> bodies = new List<Rigidbody>();
            List<GameObject> withoutBodies = new List<GameObject>();
            foreach (var gameObject in Selection.gameObjects)
            {
                var rb = gameObject.GetComponentsInChildren<Rigidbody>();
                if (rb.Length == 0)
                    withoutBodies.Add(gameObject);
                bodies.AddRange(rb);
            }

            currentEditorPhysics =
                new EditorPhysicsSceneCommand(bodies.ToArray(), withoutBodies.ToArray());

            MonkeyEditorUtils.AddSceneCommand(currentEditorPhysics);
#endif
        }


#if UNITY_2017_1_OR_NEWER
        public class EditorPhysicsSceneCommand : ConfirmedCommand
        {
            public bool ShowLabel;
            public bool PauseSim;
            public float Speed = 0.5f;
            private readonly List<GameObject> objectsWithAddedBody = new List<GameObject>();
            private readonly List<Rigidbody> cachedBodies = new List<Rigidbody>();
            private readonly List<Collider> addedColliders = new List<Collider>();
            private readonly List<Rigidbody> excludedBodies = new List<Rigidbody>();
            private readonly bool previousAutoSimulation;

            private readonly List<PositionRotation> previousPositionRotations = new List<PositionRotation>();

            private struct PositionRotation
            {
                public readonly Vector3 Position;
                public readonly Quaternion Rotation;

                public PositionRotation(Vector3 position, Quaternion rotation) : this()
                {
                    this.Position = position;
                    this.Rotation = rotation;
                }
            }

            public EditorPhysicsSceneCommand(Rigidbody[] objects, params GameObject[] objectsWithoutBodies) : base()
            {
                ShowActionButton = false;
                ShowLabel = true;
                SceneCommandName = "Editor Physics Updater";
                ConfirmationMode = ActionConfirmationMode.ENTER_AND_ESCAPE;
                cachedBodies.AddRange(objects);
                TimeBetweenUpdate = 0;
                foreach (var o in objectsWithoutBodies)
                {
                    if (o.transform.GetAllParentTransforms()
                        .Any(_ => objectsWithoutBodies.Any(ob => ob.transform == _)))
                        continue;
                    if (o.GetComponent<Rigidbody>())
                    {
                        cachedBodies.Add(o.GetComponent<Rigidbody>());
                        continue;
                    }

                    if (o.GetComponentsInChildren<Collider>().Length == 0)
                    {
                        var renders = o.GetComponentsInChildren<MeshFilter>();
                        foreach (var filter in renders)
                        {
                            var col = filter.gameObject.AddComponent<MeshCollider>();
                            col.sharedMesh = filter.sharedMesh;
                            col.convex = true;
                            addedColliders.Add(col);
                        }
                    }

                    var rb = o.AddComponent<Rigidbody>();
                    objectsWithAddedBody.Add(o);
                    cachedBodies.Add(rb);
                }

                Rigidbody[] allBodies = Object.FindObjectsOfType<Rigidbody>();

                foreach (Rigidbody body in allBodies)
                {
                    if (cachedBodies.Contains(body))
                    {
                        previousPositionRotations.Add(
                            new PositionRotation(body.position, body.rotation));
                        body.WakeUp();
                    }
                    else
                    {
                        excludedBodies.Add(body);
                    }
                }



                previousAutoSimulation = Physics.autoSimulation;
                Physics.autoSimulation = false;
                timer = 0;
            }

            public override string ConfirmationMessage()
            {
                return "Press ENTER to stop the physics simulation";
            }

            public override void Stop()
            {
                base.Stop();

                Physics.autoSimulation = previousAutoSimulation;
                currentEditorPhysics = null;

                foreach (Rigidbody body in cachedBodies)
                {
                    if (!body)
                        continue;
                    body.velocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                    body.Sleep();
                }


                foreach (var body in excludedBodies)
                {
                    if (!body)
                        continue;
                    body.velocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                    body.Sleep();
                }

                int i = 0;

                foreach (var body in cachedBodies)
                {
                    if (!body)
                    {
                        i++;
                        continue;
                    }

                    PositionRotation newPr = new PositionRotation(body.position, body.rotation);
                    body.gameObject.transform.position = previousPositionRotations[i].Position;
                    body.gameObject.transform.rotation = previousPositionRotations[i].Rotation;
                    previousPositionRotations[i] = newPr;
                    i++;
                }


                int id = MonkeyEditorUtils.CreateUndoGroup("Physics Simulation");
                i = 0;
                foreach (var body in cachedBodies)
                {
                    if (!body)
                    {
                        i++;
                        continue;
                    }

                    Undo.RecordObject(body.transform, "position rotation");
                    body.gameObject.transform.position = previousPositionRotations[i].Position;
                    body.gameObject.transform.rotation = previousPositionRotations[i].Rotation;
                    i++;
                }

                foreach (var body in objectsWithAddedBody)
                {
                    if (!body)
                        continue;
                    Object.DestroyImmediate(body.GetComponent<Rigidbody>());
                }

                foreach (var collider in addedColliders)
                {
                    Object.DestroyImmediate(collider);
                }

                Undo.CollapseUndoOperations(id);

            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();

                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
                    ToggleSim();

                if (!ShowLabel)
                    return;

                Color previousColor = GUI.color;

                foreach (Rigidbody body in cachedBodies)
                {
                    if (!body)
                        continue;
                    if (!body.IsSleeping())
                    {
                        GUI.color = MonkeyStyle.Instance.SearchFieldTextColor;
                        Handles.Label(body.transform.position, "Edit Mode Physics");
                    }
                }

                GUI.color = previousColor;
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayBoolOption("Show Labels", ref ShowLabel);
                DisplayFloatOption("Sim Speed", ref Speed);
                DisplayIntOption("Max Frame Simulation Steps", ref maxSimSteps);
                maxSimSteps = Mathf.Max(1, maxSimSteps);
                Speed = Mathf.Max(Speed, 0.01f);
                DisplayButton("Toggle Simulation", ToggleSim);
            }

            private void ToggleSim()
            {
                PauseSim = !PauseSim;
            }

            private float timer;
            private int maxSimSteps = 10;

            public override void Update()
            {
                if (PauseSim)
                    return;

                if (!MonkeyEditorUtils.CurrentSceneView)
                    MonkeyEditorUtils.CurrentSceneView = SceneView.currentDrawingSceneView;

                MonkeyEditorUtils.CurrentSceneView.Focus();

                foreach (var body in excludedBodies)
                {
                    if (!body)
                        continue;
                    body.Sleep();
                }

                timer += (float)MonkeyEditorUtils.EditorDeltaTime;
                int i = 0;

                while (timer >= Time.deltaTime)
                {
                    timer -= Time.deltaTime;
                    Physics.Simulate(Time.fixedDeltaTime * Speed);


                    if (i > maxSimSteps)
                    {

                        break;
                    }


                    i++;

                }

            }


        }
#endif


#if UNITY_2017_1_OR_NEWER
        public class EditorPhysics2DSceneCommand : ConfirmedCommand
        {
            private readonly List<GameObject> objectsWithAddedBody = new List<GameObject>();
            private readonly List<Rigidbody2D> cachedBodies = new List<Rigidbody2D>();
            private readonly List<Rigidbody2D> excludedBodies = new List<Rigidbody2D>();
            private readonly bool previousAutoSimulation;

            private readonly List<PositionRotation> previousPositionRotations = new List<PositionRotation>();

            private struct PositionRotation
            {
                public readonly Vector3 Position;
                public readonly float Rotation;

                public PositionRotation(Vector3 position, float rotation) : this()
                {
                    this.Position = position;
                    this.Rotation = rotation;
                }
            }

            public EditorPhysics2DSceneCommand(Rigidbody2D[] objects, params GameObject[] objectsWithoutBodies) : base()
            {
                SceneCommandName = "Editor Physics Updater";
                ConfirmationMode = ActionConfirmationMode.ENTER;
                cachedBodies.AddRange(objects);

                foreach (var o in objectsWithoutBodies)
                {
                    if (o.transform.GetAllParentTransforms()
                        .Any(_ => objectsWithoutBodies.Any(ob => ob.transform == _)))
                        continue;
                    if (o.GetComponent<Rigidbody2D>())
                    {
                        cachedBodies.Add(o.GetComponent<Rigidbody2D>());
                        continue;
                    }
                    o.AddComponent<Rigidbody2D>();
                    objectsWithAddedBody.Add(o);
                }

                HideGUI = true;

                Rigidbody2D[] allBodies = Object.FindObjectsOfType<Rigidbody2D>();

                foreach (Rigidbody2D body in allBodies)
                {
                    if (!body)
                        continue;
                    if (cachedBodies.Contains(body))
                    {
                        previousPositionRotations.Add(
                            new PositionRotation(body.position, body.rotation));
                        body.WakeUp();
                    }
                    else
                    {
                        excludedBodies.Add(body);
                    }
                }

                previousAutoSimulation = Physics2D.autoSimulation;
                Physics2D.autoSimulation = false;
                timer = 0;
            }

            public override string ConfirmationMessage()
            {
                return "Press ENTER to stop the physics simulation";
            }

            public override void Stop()
            {
                base.Stop();
                foreach (Rigidbody2D body in cachedBodies)
                {
                    if (!body)
                        continue;
                    body.velocity = Vector3.zero;
                    body.angularVelocity = 0;
                    body.Sleep();
                }

                foreach (var body in excludedBodies)
                {
                    if (!body)
                        continue;
                    body.velocity = Vector3.zero;
                    body.angularVelocity = 0;
                    body.Sleep();
                }

                foreach (var body in objectsWithAddedBody)
                {
                    if (!body)
                        continue;
                    Object.DestroyImmediate(body.GetComponent<Rigidbody2D>());
                }

                int i = 0;

                foreach (var body in cachedBodies)
                {
                    if (!body)
                    {
                        i++;
                        continue;

                    }
                    PositionRotation newPr = new PositionRotation(body.position, body.rotation);
                    body.gameObject.transform.position = previousPositionRotations[i].Position;
                    body.rotation = previousPositionRotations[i].Rotation;
                    previousPositionRotations[i] = newPr;
                    i++;
                }


                int id = MonkeyEditorUtils.CreateUndoGroup("Physics Simulation");
                i = 0;
                foreach (var body in cachedBodies)
                {
                    if (!body)
                    {
                        i++;
                        continue;
                    }
                    Undo.RecordObject(body.transform, "position rotation");
                    body.gameObject.transform.position = previousPositionRotations[i].Position;
                    body.rotation = previousPositionRotations[i].Rotation;
                }
                Undo.CollapseUndoOperations(id);

                Physics2D.autoSimulation = previousAutoSimulation;
                currentEditorPhysics2D = null;
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();

                Color previousColor = GUI.color;

                foreach (Rigidbody2D body in cachedBodies)
                {
                    if (!body)
                        continue;

                    if (!body.IsSleeping())
                    {

                        GUI.color = MonkeyStyle.Instance.SearchFieldTextColor;
                        Handles.Label(body.transform.position, "Edit Mode Physics");
                    }
                }

                GUI.color = previousColor;
            }

            private float timer;

            public override void Update()
            {

                if (!MonkeyEditorUtils.CurrentSceneView)
                    MonkeyEditorUtils.CurrentSceneView = SceneView.currentDrawingSceneView;

                MonkeyEditorUtils.CurrentSceneView.Focus();

                foreach (var body in excludedBodies)
                {
                    if (!body)
                        continue;
                    body.Sleep();
                }

                timer += (float)MonkeyEditorUtils.EditorDeltaTime;

                while (timer >= Time.fixedDeltaTime)
                {
                    timer -= Time.fixedDeltaTime;
                    Physics2D.Simulate(Time.deltaTime);

                }

            }


        }
#endif

    }
}

