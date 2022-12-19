using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    [System.Serializable]
    public class NotificationCategory
    {
        /// <summary>
        /// The category identifier.
        /// </summary>
        [Tooltip("The category identifier.")]
        public string id;

        /// <summary>
        /// The identifier of the category group this category belongs to.
        /// </summary>
        [Tooltip("The identifier of the category group this category belongs to. Only visible on Android devices.")]
        public string groupId;

        /// <summary>
        /// The category name, only visible on Android devices.
        /// </summary>
        [Tooltip("The category name, only visible on Android devices.")]
        public string name;

        /// <summary>
        /// The category description, this is optional and only visible on Android devices.
        /// </summary>
        [Tooltip("The category description, this is optional and only visible on Android devices.")]
        public string description;

        /// <summary>
        /// [Android only] The importance level of this category, which is
        /// the level of interruption of the notifications posted to this category.
        /// </summary>
        [Tooltip("[Android only] The importance level of this category, which is " +
            "the level of interruption of the notifications posted to this category.")]
        public Importance importance;

        /// <summary>
        /// [Android only] Determines whether notifications posted to this category 
        /// can appear as badges in a Launcher application.
        /// </summary>
        [Tooltip("[Android only] Determines whether notifications posted to this category " +
            "can appear as badges in a Launcher application.")]
        public bool enableBadge = true;

        /// <summary>
        /// [Android only] Determines how notifications posted to this category 
        /// should display notification lights, on devices that support that feature.
        /// </summary>
        [Tooltip("[Android only] Determines how notifications posted to this category " +
            "should display notification lights, on devices that support that feature.")]
        public LightOptions lights = LightOptions.Default;

        /// <summary>
        /// [Android only] The light color for notifications posted to this category,
        /// if <see cref="lights"/> was set to <see cref="LightOptions.Custom"/>
        /// </summary>
        [Tooltip("[Android only] The light color for notifications posted to this category, " +
            "if Lights was set to Custom.")]
        public Color lightColor = Color.white;

        /// <summary>
        /// [Android only] Determines how notifications posted to this category should vibrate.
        /// </summary>
        [Tooltip("[Android only] Determines how notifications posted to this category should vibrate.")]
        public VibrationOptions vibration = VibrationOptions.Default;

        /// <summary>
        /// [Android only] The vibration pattern for notifications posted to this category,
        /// if <see cref="vibration"/> was set to <see cref="VibrationOptions.Custom"/>
        /// </summary>
        [Tooltip("[Android only] The vibration pattern for notifications posted to this category, " +
            "if Vibration was set to Custom.")]
        public int[] vibrationPattern;

        /// <summary>
        /// [Android only] Determines how notifications posted to this category should
        /// be displayed on lockscreen.
        /// </summary>
        [Tooltip("[Android only] Determines how notifications posted to this category should " +
            "be displayed on lockscreen.")]
        public LockScreenVisibilityOptions lockScreenVisibility = LockScreenVisibilityOptions.Public;

        /// <summary>
        /// Determines how sound should be played when notifications posted to this category
        /// are delivered.
        /// </summary>
        [Tooltip("Determines how sound should be played when notifications posted to this category " +
            "are delivered.")]
        public SoundOptions sound = SoundOptions.Default;

        /// <summary>
        /// The filename (with extension) of the sound to be played when notifications posted to this category
        /// are delivered, if <see cref="sound"/> was set to <see cref="SoundOptions.Custom"/>.
        /// The sound must reside locally on the device.
        /// </summary>
        [Tooltip("The filename (with extension) of the sound to be played when notifications posted to this category " +
            "are delivered, if Sound was set to Custom. " +
            "The sound must reside locally on the device.")]
        public string soundName;

        /// <summary>
        /// The custom action buttons for notifications posted to this category.
        /// iOS supports up to 4 buttons. Android supports up to 3 buttons.
        /// Excessive buttons will be ignored.
        /// </summary>
        [Tooltip("The custom action buttons for notifications posted to this category. " +
            "iOS supports up to 4 buttons. Android supports up to 3 buttons." +
            " Excessive buttons will be ignored.")]
        public ActionButton[] actionButtons;

        public enum Importance
        {
            /// <summary>
            /// Default notification importance: shows everywhere, makes noise, but does not visually intrude.
            /// </summary>
            Default = 0,

            /// <summary>
            /// Higher notification importance: shows everywhere, makes noise and peeks.
            /// </summary>
            High = 1,

            /// <summary>
            /// Low notification importance: shows everywhere, but is not intrusive.
            /// </summary>
            Low = 2,

            /// <summary>
            /// Min notification importance: only shows in the shade, below the fold.
            /// </summary>
            Min = 4,

            /// <summary>
            /// A notification with no importance: does not show in the shade.
            /// </summary>
            None = 5,

            /// <summary>
            /// Value signifying that the user has not expressed an importance.
            /// </summary>
            Unspecified = 6
        }

        public enum LightOptions
        {
            Off = 0,
            Default = 1,
            Custom = 2
        }

        public enum SoundOptions
        {
            Off = 0,
            Default = 1,
            Custom = 2
        }

        public enum VibrationOptions
        {
            Off = 0,
            Default = 1,
            Custom = 2
        }

        public enum LockScreenVisibilityOptions
        {
            /// <summary>
            /// Notification does not show on the lockscreen at all.
            /// </summary>
            Secret = 0,

            /// <summary>
            /// Hides notification content on the lockscreen unless system set to "Show all".
            /// </summary>
            Private = 1,

            /// <summary>
            /// Always show notification content on the lockscreen.
            /// </summary>
            Public = 2
        }

        [System.Serializable]
        public struct ActionButton
        {
            public string id;
            public string title;
        }
    }

    [System.Serializable]
    public class NotificationCategoryGroup
    {
        public string id;
        public string name;
    }
}