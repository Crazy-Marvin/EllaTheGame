using System;
using System.Collections;

namespace EasyMobile
{
    [Obsolete("This class was deprecated. Please use EasyMobile.AdPlacement instead.")]
    public class AdLocation
    {
        private readonly string name;
        private static Hashtable map = new Hashtable();

        protected AdLocation()
        {
        }

        protected AdLocation(string name, bool addToMap = true)
        {
            this.name = name;
            if (addToMap)
                map.Add(name, this);
        }

        /// <summary>
        /// Returns a String that represents the current AdLocation.
        /// </summary>
        /// <returns>A String that represents the current AdLocation</returns>
        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Default location.
        /// </summary>
        public static readonly AdLocation Default = new AdLocation("Default");

        /// <summary>
        /// Initial startup of your app.
        /// </summary>
        public static readonly AdLocation Startup = new AdLocation("Startup");

        /// <summary>
        /// Home screen the player first sees.
        /// </summary>
        public static readonly AdLocation HomeScreen = new AdLocation("Home Screen");

        /// <summary>
        /// Menu that provides game options.
        /// </summary>
        public static readonly AdLocation MainMenu = new AdLocation("Main Menu");

        /// <summary>
        /// Menu that provides game options.
        /// </summary>
        public static readonly AdLocation GameScreen = new AdLocation("Game Screen");

        /// <summary>
        /// Screen with list achievements in the game.
        /// </summary>
        public static readonly AdLocation Achievements = new AdLocation("Achievements");

        /// <summary>
        /// Quest, missions or goals screen describing things for a player to do.
        /// </summary>
        public static readonly AdLocation Quests = new AdLocation("Quests");

        /// <summary>
        /// Pause screen.
        /// </summary>
        public static readonly AdLocation Pause = new AdLocation("Pause");

        /// <summary>
        /// Start of the level.
        /// </summary>
        public static readonly AdLocation LevelStart = new AdLocation("Level Start");

        /// <summary>
        /// Completion of the level.
        /// </summary>
        public static readonly AdLocation LevelComplete = new AdLocation("Level Complete");

        /// <summary>
        /// Finishing a turn in a game.
        /// </summary>
        public static readonly AdLocation TurnComplete = new AdLocation("Turn Complete");

        /// <summary>
        /// The store where the player pays real money for currency or items.
        /// </summary>
        public static readonly AdLocation IAPStore = new AdLocation("IAP Store");

        /// <summary>
        /// The store where a player buys virtual goods.
        /// </summary>
        public static readonly AdLocation ItemStore = new AdLocation("Item Store");

        /// <summary>
        /// The game over screen after a player is finished playing.
        /// </summary>
        public static readonly AdLocation GameOver = new AdLocation("Game Over");

        /// <summary>
        /// List of leaders in the game.
        /// </summary>
        public static readonly AdLocation LeaderBoard = new AdLocation("Leaderboard");

        /// <summary>
        /// Screen where player can change settings such as sound.
        /// </summary>
        public static readonly AdLocation Settings = new AdLocation("Settings");

        /// <summary>
        /// Screen display right before the player exists an app.
        /// </summary>
        public static readonly AdLocation Quit = new AdLocation("Quit");

        public static AdLocation LocationFromName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return AdLocation.Default;
            else if (map[name] != null)
                return map[name] as AdLocation;
            else
                return new AdLocation(name);
        }
    }

    #pragma warning disable 0618
    [Obsolete]
    public static class AdLocationExtension
    {
        public static AdPlacement ToAdPlacement(this AdLocation location)
        {
            if (location == AdLocation.Default)
                return AdPlacement.Default;
            else
                return AdPlacement.PlacementWithName(location.ToString());
        }
    }
    #pragma warning restore 0618
}

