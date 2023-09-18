using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonKey.Commands;
using MonKey.Editor.Internal;
using MonKey.Extensions;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

#if UNITY_2017_1_OR_NEWER
using UnityEngine.Playables;
#endif

namespace MonKey.Editor.Commands
{

    public static class EditorUtilities
    {
        [Command("Save Selected Objects","Saves the selected objects")]
        public static void SaveSelectedObjects()
        {
            foreach (var obj in Selection.objects)
            {
                EditorUtility.SetDirty(obj);
            }
            AssetDatabase.SaveAssets();
        }


        [Command("Play Timelines", "Plays the selected Timelines", QuickName = "PT",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT, AlwaysShow = true,
            MenuItemLink = "PlayTimelines", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Animation")]
        public static void PlayTimelines(
            [CommandParameter("Should the playable director with a 'hold' behavior be looped anyway")]
            bool loopHoldDirectors = true)
        {
#if UNITY_2017_1_OR_NEWER
            List<PlayableDirector> directors = new List<PlayableDirector>();

            foreach (var gameObject in Selection.gameObjects)
            {
                directors.AddRange(gameObject.GetComponentsInChildren<PlayableDirector>());
            }


            MonkeyEditorUtils.AddSceneCommand(
                new TimelineUpdateEditModeSceneCommand(
                    directors.ToArray(),
                    loopHoldDirectors));
#endif
        }

#if UNITY_2017_1_OR_NEWER
        public class TimelineUpdateEditModeSceneCommand : TimedSceneCommand
        {
            public PlayableDirector[] Directors;
            public DirectorUpdateMode[] PreviousUpdateModes;
            public double[] PreviousTimes;

            public bool LoopHoldModeDirectors;

            public TimelineUpdateEditModeSceneCommand(PlayableDirector[] directors, bool loopHoldModeDirectors) :
                base(0)
            {
                SceneCommandName = "Timeline Play";
                this.Directors = directors;
                this.LoopHoldModeDirectors = loopHoldModeDirectors;
                PreviousUpdateModes = directors.Convert(_ => _.timeUpdateMode).ToArray();
                PreviousTimes = directors.Convert(_ => _.time).ToArray();
                this.Directors.ForEach(_ => _.timeUpdateMode = DirectorUpdateMode.Manual);
            }

            public override void Update()
            {
                base.Update();

                foreach (PlayableDirector director in Directors)
                {
                    director.time += Time.deltaTime;
                    if ((LoopHoldModeDirectors || director.extrapolationMode == DirectorWrapMode.Loop)
                        && director.time > director.duration)
                        director.time = 0;
                    director.Evaluate();
                }
            }

            public override void Stop()
            {
                base.Stop();
                int i = 0;
                foreach (PlayableDirector director in Directors)
                {
                    director.time = PreviousTimes[i];
                    director.Evaluate();
                    director.Stop();
                    director.timeUpdateMode = PreviousUpdateModes[i];
                    i++;
                }
            }
        }

#endif

        [Command("Execute In Edit Mode", "Executes all the selected objects' MonoBehaviours",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, QuickName = "EE",
            Category = "Dev")]
        public static void ExecuteInEditMode()
        {
            List<MonoBehaviour> behaviours = new List<MonoBehaviour>();
            foreach (var go in Selection.gameObjects)
            {
                behaviours.AddRange(go.GetComponents<MonoBehaviour>());
            }

            MonkeyEditorUtils.AddSceneCommand(new ExecuteInEditModeSceneCommand(behaviours.ToArray()));
        }

        public class ExecuteInEditModeSceneCommand : TimedSceneCommand
        {
            private MethodGroup[] selected;

            private class MethodGroup
            {
                public MonoBehaviour Behaviour;
                public readonly List<MethodInfo> updateMethods = new List<MethodInfo>();
            }

            public ExecuteInEditModeSceneCommand(MonoBehaviour[] mono) : base(0)
            {
                SceneCommandName = "Execute In Edit Mode";
                selected = new MethodGroup[mono.Length];

                int i = 0;
                foreach (var behaviour in mono)
                {
                    Type type = behaviour.GetType();

                    MethodInfo awake = type.GetMethod("Awake", BindingFlags.Public
                                                               | BindingFlags.NonPublic
                                                               | BindingFlags.Instance
                                                               | BindingFlags.Default);
                    if (awake != null)
                        awake.Invoke(behaviour, null);
                    MethodInfo onEnable = type.GetMethod("OnEnable", BindingFlags.Public
                                                                     | BindingFlags.NonPublic
                                                                     | BindingFlags.Instance
                                                                     | BindingFlags.Default);
                    if (onEnable != null)
                        onEnable.Invoke(behaviour, null);

                    MethodInfo start = type.GetMethod("Start", BindingFlags.Public
                                                               | BindingFlags.NonPublic
                                                               | BindingFlags.Instance
                                                               | BindingFlags.Default);
                    if (start != null)
                        start.Invoke(behaviour, null);

                    MethodGroup group = new MethodGroup();
                    group.Behaviour = behaviour;

                    MethodInfo fixedUpdate = type.GetMethod("FixedUpdate", BindingFlags.Public
                                                                           | BindingFlags.NonPublic
                                                                           | BindingFlags.Instance
                                                                           | BindingFlags.Default);
                    if (fixedUpdate != null)
                        group.updateMethods.Add(fixedUpdate);


                    MethodInfo update = type.GetMethod("Update", BindingFlags.Public
                                                                 | BindingFlags.NonPublic
                                                                 | BindingFlags.Instance
                                                                 | BindingFlags.Default);
                    if (update != null)
                        group.updateMethods.Add(update);

                    MethodInfo lateUpdate = type.GetMethod("LateUpdate", BindingFlags.Public
                                                                         | BindingFlags.NonPublic
                                                                         | BindingFlags.Instance
                                                                         | BindingFlags.Default);
                    if (lateUpdate != null)
                        group.updateMethods.Add(lateUpdate);
                    selected[i] = group;
                    i++;
                }

            }

            public override void Update()
            {
                base.Update();

                selected = selected.Where(_ => _ != null && _.Behaviour).ToArray();

                foreach (MethodGroup behaviour in selected)
                {
                    foreach (MethodInfo info in behaviour.updateMethods)
                    {
                        info.Invoke(behaviour.Behaviour, null);
                    }
                }
            }
        }



    }


}
