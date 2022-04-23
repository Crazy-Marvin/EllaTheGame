using UnityEditor;
using UnityEngine;

namespace MonKey.Editor.Commands
{
    public static class PlayUtilities
    {
        [Command("Play / Pause",QuickName = "PLP",
             Help = "Plays or pauses the game",MenuItemLink = "QuickPause",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Dev")]
        public static void EasyAccessEditorPause()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPaused = !EditorApplication.isPaused;
            }
            else
            {
                EditorApplication.isPlaying = true;
            }
        }

        private static float previousFixedDeltaTime;
        private static bool physicsPaused = false;
        private static PhysicsPauseCommand currentPhysicsCommand;

        [Command("Play / Pause Physics",QuickName = "PP",
            Help = "Pauses / Resumes physics during play mode",
            ValidationMethodName = "ValidatePlayMode",
            MenuItemLink = "PlayPausePhysics",MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Physics")]
        public static void TogglePausePhysics()
        {

            if (currentPhysicsCommand != null)
            {
                currentPhysicsCommand.Stop();
                currentPhysicsCommand = null;
                return;
            }

            currentPhysicsCommand = new PhysicsPauseCommand(0);
            MonkeyEditorUtils.AddSceneCommand(currentPhysicsCommand);
        }

        [Command("Pause Physics Duration",QuickName = "PPD",
            Help = "Pauses Physics and resumes it after some time (or indefinitely by default)",
            ValidationMethodName = "ValidatePlayMode",
            Category = "Physics")]
        public static void PausePhysicsDuration(
            [CommandParameter(Help = "The duration for which the physics will be paused")]
            float duration = 5)
        {

            if (currentPhysicsCommand != null)
            {
                if (Mathf.Approximately(duration, 0))
                    currentPhysicsCommand.Stop();
                else
                {
                    currentPhysicsCommand.Duration = duration;
                    currentPhysicsCommand.CurrentTime = 0;
                }
                return;
            }

            currentPhysicsCommand = new PhysicsPauseCommand(duration);
            MonkeyEditorUtils.AddSceneCommand(currentPhysicsCommand);
        }


        public class PhysicsPauseCommand : TimedSceneCommand
        {
            public bool PreviousAutoSimulation;

            public PhysicsPauseCommand(float duration) : base(duration)
            {
                SceneCommandName = "Physics Pause";
                if (!timeScaleOn)
                    previousFixedDeltaTime = Time.fixedDeltaTime;

                physicsPaused = true;

                HideGUI = true;

#if UNITY_2017_1_OR_NEWER
                PreviousAutoSimulation = Physics.autoSimulation;
#endif

            }

            public override void Update()
            {
#if UNITY_2017_1_OR_NEWER
                Physics.autoSimulation = false;
#endif
                Time.fixedDeltaTime = 0;
                base.Update();
            }

            public override void Stop()
            {
                base.Stop();

#if UNITY_2017_1_OR_NEWER
                Physics.autoSimulation = PreviousAutoSimulation;
#endif
                Time.fixedDeltaTime = previousFixedDeltaTime;
                previousFixedDeltaTime = -1;
                physicsPaused = false;

                currentPhysicsCommand = null;
            }

        }

        private static bool timeScaleOn = false;
        private static TimeScaleSceneCommand currentTimeScaleCommand;

        [Command("Slow Motion",QuickName = "SM",
            Help = "Slows the Game Down during play, Or resumes to normal speed",
            ValidationMethodName = "ValidatePlayMode",
            MenuItemLink = "SlowMotion",MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Time")]
        public static void ToggleSloMo(
            [CommandParameter(Help="The amount of slow motion to apply. " +
                                   "If turning it off, set 1")]
            float amount = .2f)
        {
            if (currentTimeScaleCommand != null)
            {
                if (Mathf.Approximately(amount, 1))
                    currentTimeScaleCommand.Stop();
                else
                    currentTimeScaleCommand.Scale = amount;

            }
            else
                ToggleTimeScale(amount);
        }

        [Command("Time Scale",QuickName = "TS",
            Help = "In play mode, sets a time scale for some duration, or resumes normal time",
            ValidationMethodName = "ValidatePlayMode",
            Category = "Time")]
        public static void ToggleTimeScale(
            [CommandParameter(Help="The time scale to put. If turning off, ignore")]
            float scale = 0.5f,
            [CommandParameter(Help="How long should the time scale apply. If turning off ignore")]
            float duration = 0)
        {
            if (currentTimeScaleCommand != null)
            {
                currentTimeScaleCommand.Stop();
                currentTimeScaleCommand = null;
                return;
            }

            currentTimeScaleCommand = new TimeScaleSceneCommand(duration, scale);
            MonkeyEditorUtils.AddSceneCommand(currentTimeScaleCommand);
        }

        public class TimeScaleSceneCommand : TimedSceneCommand
        {
            public float Scale;
            public TimeScaleSceneCommand(float duration, float scale) : base(duration)
            {
                SceneCommandName = "Time Scale";
                timeScaleOn = true;
                Scale = scale;
                if (physicsPaused)
                    TogglePausePhysics();

                previousFixedDeltaTime = Time.fixedDeltaTime;
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayBoolOption("Toggle Slo Mo", ref timeScaleOn);
                DisplayFloatOption("Time Scale",ref Scale);
            }

            public override void Update()
            {
                base.Update();
                if (timeScaleOn)
                {
                    Time.timeScale = Scale;
                    Time.fixedDeltaTime = previousFixedDeltaTime * Time.timeScale;
                }
                else
                {
                    Time.timeScale = 1;
                    Time.fixedDeltaTime = previousFixedDeltaTime * Time.timeScale;
                }
               
            }

            public override void Stop()
            {
                base.Stop();

                Time.fixedDeltaTime = previousFixedDeltaTime;
                Time.timeScale = 1;

                previousFixedDeltaTime = -1;
                timeScaleOn = false;
                currentTimeScaleCommand = null;
            }
        }

        [CommandValidation("You must enter play mode first")]
        public static bool ValidatePlayMode()
        {
            return Application.isPlaying;
        }

    }

}
