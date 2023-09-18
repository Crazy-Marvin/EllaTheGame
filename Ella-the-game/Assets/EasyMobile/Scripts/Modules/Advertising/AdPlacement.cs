using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyMobile
{
    #pragma warning disable 0618
    /// <summary>
    /// An ad placement represents an exact location to serve an ad unit in your app.
    /// </summary>
    [Serializable]
    public class AdPlacement : AdLocation
    {
        // Stores all custom placements, built-in or user-created.
        private static Dictionary<string, AdPlacement> sCustomPlacements = new Dictionary<string, AdPlacement>();

        [SerializeField]
        protected string mName;

        /// <summary>
        /// The name of the placement.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get { return mName; } }

        // These members are to be used by the custom drawer.
        #if UNITY_EDITOR
        // Whether the user wants to enter a custom placement name,
        // instead of selecting one from the built-in placements.
        [SerializeField]
        protected bool mUseCustomName = false;
        #endif

        protected AdPlacement()
        {
        }

        private AdPlacement(string name, bool isDefault = false)
            : base(name, false)
        {
            this.mName = name;

            if (!isDefault)
                sCustomPlacements[name] = this;
        }

        #region Built-in Placements.

        /// <summary>
        /// A placement with an empty name. Whenever you attempt to create a new
        /// placement with a null or empty name, this one will be returned.
        /// </summary>
        new public static readonly AdPlacement Default = new AdPlacement(string.Empty, true);

        /// <summary>
        /// A placement with the name "Startup".
        /// </summary>
        new public static readonly AdPlacement Startup = new AdPlacement("Startup");
        
        /// <summary>
        /// A placement with the name "Home_Screen".
        /// </summary>
        new public static readonly AdPlacement HomeScreen = new AdPlacement("Home_Screen");
        
        /// <summary>
        /// A placement with the name "Main_Menu".
        /// </summary>
        new public static readonly AdPlacement MainMenu = new AdPlacement("Main_Menu");
        
        /// <summary>
        /// A placement with the name "Game_Screen".
        /// </summary>
        new public static readonly AdPlacement GameScreen = new AdPlacement("Game_Screen");
        
        /// <summary>
        /// A placement with the name "Achievements".
        /// </summary>
        new public static readonly AdPlacement Achievements = new AdPlacement("Achievements");
        
        /// <summary>
        /// A placement with the name "Level_Start".
        /// </summary>
        new public static readonly AdPlacement LevelStart = new AdPlacement("Level_Start");
        
        /// <summary>
        /// A placement with the name "Level_Complete".
        /// </summary>
        new public static readonly AdPlacement LevelComplete = new AdPlacement("Level_Complete");
        
        /// <summary>
        /// A placement with the name "Turn_Complete".
        /// </summary>
        new public static readonly AdPlacement TurnComplete = new AdPlacement("Turn_Complete");
        
        /// <summary>
        /// A placement with the name "Quests".
        /// </summary>
        new public static readonly AdPlacement Quests = new AdPlacement("Quests");
        
        /// <summary>
        /// A placement with the name "Pause".
        /// </summary>
        new public static readonly AdPlacement Pause = new AdPlacement("Pause");
        
        /// <summary>
        /// A placement with the name "IAP_Store".
        /// </summary>
        new public static readonly AdPlacement IAPStore = new AdPlacement("IAP_Store");
        
        /// <summary>
        /// A placement with the name "Item_Store".
        /// </summary>
        new public static readonly AdPlacement ItemStore = new AdPlacement("Item_Store");
        
        /// <summary>
        /// A placement with the name "Game_Over".
        /// </summary>
        new public static readonly AdPlacement GameOver = new AdPlacement("Game_Over");
        
        /// <summary>
        /// A placement with the name "Leaderboard".
        /// </summary>
        public static readonly AdPlacement Leaderboard = new AdPlacement("Leaderboard");
        
        /// <summary>
        /// A placement with the name "Settings".
        /// </summary>
        new public static readonly AdPlacement Settings = new AdPlacement("Settings");
        
        /// <summary>
        /// A placement with the name "Quit".
        /// </summary>
        new public static readonly AdPlacement Quit = new AdPlacement("Quit");

        #endregion  // Built-in Placements

        /// <summary>
        /// Gets all existing placements including <c>AdPlacement.Default</c>.
        /// </summary>
        /// <returns>The all placements.</returns>
        public static AdPlacement[] GetAllPlacements()
        {
            var list = new List<AdPlacement>();
            list.Add(Default);
            list.AddRange(sCustomPlacements.Values);
            return list.ToArray();
        }

        /// <summary>
        /// Gets all existing placements excluding <c>AdPlacement.Default</c>.
        /// </summary>
        /// <returns>The custom placements.</returns>
        public static AdPlacement[] GetCustomPlacements()
        {
            var list = new List<AdPlacement>(sCustomPlacements.Values);
            return list.ToArray();
        }

        /// <summary>
        /// Returns a new placement with the given name, or an
        /// existing placement with that name if one exists.
        /// If a null or empty name is given, the <c>AdPlacement.Default</c> placement will be returned.
        /// </summary>
        /// <returns>The placement.</returns>
        /// <param name="name">Name.</param>
        public static AdPlacement PlacementWithName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return AdPlacement.Default;

            if (sCustomPlacements.ContainsKey(name))
                return sCustomPlacements[name] as AdPlacement;

            return new AdPlacement(name);
        }

        public static string GetPrintableName(AdPlacement placement)
        {
            return placement == null ? "null" : placement == Default ? "[Default]" : placement.ToString();
        }

        public override string ToString()
        {
            return mName;
        }

        public override bool Equals(object obj)
        {
            var other = obj as AdPlacement;

            if (other == null)
                return false;

            if (string.IsNullOrEmpty(this.Name))
                return string.IsNullOrEmpty(other.Name);

            return this.Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public static bool operator ==(AdPlacement placementA, AdPlacement placementB)
        {
            if (ReferenceEquals(placementA, null))
                return ReferenceEquals(placementB, null);

            return placementA.Equals(placementB);
        }

        public static bool operator !=(AdPlacement placementA, AdPlacement placementB)
        {
            return !(placementA == placementB);
        }
    }
    #pragma warning restore 0618
}

