using UnityEngine;
using System;

namespace EasyMobile
{
    public partial class GameServicesSettings
    {
        /// <summary>
        /// Whether the Saved Games feature is enabled.
        /// </summary>
        /// <value><c>true</c> if Saved Games feature is enabled; otherwise, <c>false</c>.</value>
        public bool IsSavedGamesEnabled { get { return mEnableSavedGames; } }

        /// <summary>
        /// Gets or sets the default strategy for auto conflict resolution.
        /// </summary>
        /// <value>The auto conflict resolution strategy.</value>
        public SavedGameConflictResolutionStrategy AutoConflictResolutionStrategy { get { return mAutoConflictResolutionStrategy; } set { mAutoConflictResolutionStrategy = value; } }

        /// <summary>
        /// [Google Play Games only] Gets or sets the data source for saved games.
        /// </summary>
        /// <value>The GPGS data source.</value>
        public GPGSSavedGameDataSource GPGSDataSource { get { return mGpgsDataSource; } set { mGpgsDataSource = value; } }

        [SerializeField]
        private bool mEnableSavedGames = false;
        [SerializeField]
        private SavedGameConflictResolutionStrategy mAutoConflictResolutionStrategy = SavedGameConflictResolutionStrategy.UseBase;
        [SerializeField]
        private GPGSSavedGameDataSource mGpgsDataSource = GPGSSavedGameDataSource.ReadCacheOrNetwork;
    }
}

