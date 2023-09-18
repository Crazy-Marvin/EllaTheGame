using System;

namespace EasyMobile
{
    /// <summary>
    /// A enum describing where game data can be fetched from when using the Saved Games service of Google Play Games.
    /// </summary>
    public enum GPGSSavedGameDataSource
    {
        /// <summary>
        /// Allow a read from either a local cache, or the network.
        /// </summary>
        /// <remarks> Values from the cache may be
        /// stale (potentially producing more write conflicts), but reading from cache may still
        /// allow reads to succeed if the device does not have internet access and may complete more
        /// quickly (as the reads can occur locally rather requiring network roundtrips).
        /// </remarks>
        ReadCacheOrNetwork,

        /// <summary>
        /// Only allow reads from network.
        /// </summary>
        /// <remarks> This guarantees any returned values were current at the time
        /// the read succeeded, but prevents reads from succeeding if the network is unavailable for
        /// any reason.
        /// </remarks>
        ReadNetworkOnly
    }

    /// <summary>
    /// An enum for the different strategies that can be used to resolve saved game conflicts (i.e.
    /// conflicts produced by two or more separate writes to the same saved game at once).
    /// </summary>
    public enum SavedGameConflictResolutionStrategy
    {
        /// <summary>
        /// Choose the version of the saved game that existed before any conflicting write occurred.
        /// Consider the following case:
        /// - An initial version of a save game ("X") is written from a device ("Dev_A")
        /// - The save game X is downloaded by another device ("Dev_B").
        /// - Dev_A writes a new version of the save game to the cloud ("Y")
        /// - Dev_B does not see the new save game Y, and attempts to write a new save game ("Z").
        /// - Since Dev_B is performing a write using out of date information, a conflict is generated.
        ///
        /// In this situation, we can resolve the conflict by declaring either keeping Y as the
        /// canonical version of the saved game (i.e. choose the "base" version aka <see cref="UseBase"/>),
        /// or by overwriting it with conflicting value, Z (i.e. choose the "remote" version aka
        /// <see cref="UseRemote"/>).
        /// </summary>
        /// 
        UseBase,

        /// <summary>
        /// See the documentation for <see cref="UseBase"/>.
        /// </summary>
        UseRemote
    }

    /// <summary>
    /// A delegate that is invoked when we encounter a conflict during execution of the Open
    /// operation with manual conflict resolution. The passed saved games (base and remote) are all open.
    /// If <see cref="OpenWithManualConflictResolution"/> was invoked with
    /// <c>prefetchDataOnConflict</c> set to <c>true</c>, the <paramref name="baseVersionData"/> and
    /// <paramref name="remoteVersionData"/> will be equal to the binary data of the "base" and
    /// "remote" saved game respectively (and null otherwise). Since conflict files may be generated
    /// by other clients, it is possible that neither of the passed saved games were originally written
    /// by the current device. Consequently, any conflict resolution strategy should not rely on local
    /// data that is not part of the binary data of the passed saved games - this data will not be
    /// present if conflict resolution occurs on a different device. In addition, since a given saved
    /// game may have multiple conflicts, this function must be designed to handle multiple invocations.
    /// Use the return value to determine whether the base or the remote will be chosen as the canonical 
    /// version of the saved game (i.e. <see cref="UseBase"/> or <see cref="UseRemote"/> strategy).
    /// In case you want to merge the data from different versions, simply specify either base or remote
    /// as the chosen version to resolve the conflicts. Once all pending conflicts are resolved, and the 
    /// saved game has been opened successfully, perform a Write operation using the merged data.
    /// </summary>
    public delegate SavedGameConflictResolutionStrategy SavedGameConflictResolver(
        SavedGame baseVersion,byte[] baseVersionData,SavedGame remoteVersion,byte[] remoteVersionData);
    
    public interface ISavedGameClient
    {
        /// <summary>
        /// Opens the saved game with the specified name, or creates a new one if none exists.
        /// If the saved game has outstanding conflicts, they will be resolved automatically
        /// using the conflict resolution strategy specified in the module settings.
        /// The saved game returned by this method will be open, which means it can be used
        /// for Read and Write operations.
        /// On Android/Google Play Games Services platform, this method uses the data source specified
        /// in the module settings.
        /// </summary>
        /// <param name="name">The name of the saved game, must consist of
        /// only non-URL reserved characters (i.e. a-z, A-Z, 0-9, or the symbols "-", ".", "_", or "~")
        /// and has between 1 and 100 characters in length (inclusive).</param>
        /// <param name="callback">The callback that is invoked when this operation finishes. If the operation
        /// succeeds, a non-null, opened saved game will be returned and the error string will be null. Otherwise,
        /// the returned saved game will be null and the string contains the error description. This callback will
        /// always execute on the game thread.</param>
        void OpenWithAutomaticConflictResolution(string name, Action<SavedGame, string> callback);

        /// <summary>
        /// Opens the saved game with the specified name, or creates a new one if none exists.
        /// If the saved game has outstanding conflicts, they will be resolved manually using
        /// the passed conflict resolving function. The completed callback will be invoked once
        /// all pending conflicts are resolved, or an error occurs.
        /// The saved game returned by this method will be open, which means it can be used
        /// for Read and Write operations.
        /// On Android/Google Play Games Services platform, this method uses the data source specified
        /// in the module settings.
        /// Use this method if you want to implement a custom conflict resolution strategy.
        /// </summary>
        /// <param name="name">The name of the saved game, must consist of
        /// only non-URL reserved characters (i.e. a-z, A-Z, 0-9, or the symbols "-", ".", "_", or "~")
        /// and has between 1 and 100 characters in length (inclusive).</param>
        /// <param name="prefetchDataOnConflict">If set to <c>true</c>, the data for the two
        /// conflicting saved games will be loaded automatically and passed as parameters in
        /// <paramref name="resolverFunction"/>. If set to <c>false</c>, <c>null</c> binary data
        /// will be passed instead and the caller will have to fetch it manually.</param>
        /// <param name="resolverFunction">The function that will be invoked if one or more conflict is
        /// encountered while executing this method. Note that more than one conflict may be present
        /// and that this function might be executed more than once to resolve multiple conflicts.
        /// This function is always executed on the game thread.</param>
        /// <param name="callback">The callback that is invoked when this operation finishes. If the operation
        /// succeeds, a non-null, opened saved game will be returned and the error string will be null. Otherwise,
        /// the returned saved game will be null and the string will contain the error description. This callback will
        /// always execute on the game thread.</param>
        void OpenWithManualConflictResolution(string name, bool prefetchDataOnConflict, 
                                              SavedGameConflictResolver resolverFunction, 
                                              Action<SavedGame, string> completedCallback);

        /// <summary>
        /// Reads the binary data of the specified saved game, which must be opened (i.e.
        /// <see cref="SavedGame.IsOpen"/> returns true).
        /// </summary>
        /// <param name="savedGame">The saved game to read data. It must be opened or the operation will fail.</param>
        /// <param name="callback">The callback that is invoked when the read finishes. If the
        /// operation succeeds, the byte array will contain the saved game binary data, and the string error will be null.
        /// Otherwise, the returned array will be null and the string will contain the error description.
        /// In case the saved game has just been opened and has no data written yet, both the byte array and the error string will be null.
        /// This callback always executes on the game thread.</param>
        void ReadSavedGameData(SavedGame savedGame, Action<SavedGame, byte[], string> callback);

        /// <summary>
        /// Writes the binary data to the specified saved game. When this method returns successfully,
        /// the data is durably persisted to disk and will eventually be uploaded to the cloud (in
        /// practice, this will happen very quickly unless the device does not have a network
        /// connection). If an update to the saved game has occurred after it was retrieved
        /// from the cloud, this commit will produce a conflict (this commonly occurs if two different
        /// devices are writing to the cloud at the same time). All conflicts must be handled the next
        /// time this saved game is opened.
        /// </summary>
        /// <param name="savedGame">The saved game to write data to. It must be open or the operation will fail.</param>
        /// <param name="data">The new binary content of the saved game.</param>
        /// <param name="callback">The callback that is invoked when this operation finishes. If the operation
        /// succeeds, the error string will be null and a non-null saved game will be returned. Note that this saved game
        /// will NOT be open (so you must open it again before performing further read & write operations). If the operation fails,
        /// the returned saved game will be null and the string will contain the error description. This callback will
        /// always execute on the game thread.</param>
        void WriteSavedGameData(SavedGame savedGame, byte[] data, Action<SavedGame, string> callback);

        /// <summary>
        /// Writes the binary data to the specified saved game. When this method returns successfully,
        /// the data is durably persisted to disk and will eventually be uploaded to the cloud (in
        /// practice, this will happen very quickly unless the device does not have a network
        /// connection). If an update to the saved game has occurred after it was retrieved
        /// from the cloud, this commit will produce a conflict (this commonly occurs if two different
        /// devices are writing to the cloud at the same time). All conflicts must be handled the next
        /// time this saved game is opened.
        /// </summary>
        /// <param name="savedGame">The saved game to write data to. It must be open or the operation will fail.</param>
        /// <param name="data">The new binary content of the saved game.</param>
        /// <param name="infoUpdate">All updates that should be applied to the saved game metadata.
        /// Applicable on Android/Google Play Games platform only.</param>
        /// <param name="callback">The callback that is invoked when this operation finishes. If the operation
        /// succeeds, the error string will be null and a non-null saved game will be returned. Note that this saved game
        /// will NOT be open (so you must open it again before performing further read & write operations). If the operation fails,
        /// the returned saved game will be null and the string will contain the error description. This callback will
        /// always execute on the game thread.</param>
        void WriteSavedGameData(SavedGame savedGame, byte[] data, SavedGameInfoUpdate infoUpdate, Action<SavedGame, string> callback);

        /// <summary>
        /// Retrieves all known saved games. All returned saved games are
        /// not open, and must be opened before they can be used for read or write operations.
        /// On Android/Google Play Games Services platform, this method uses the data source specified
        /// in the module settings.
        /// </summary>
        /// <param name="callback">The callback that is invoked when this operation finishes.
        /// If the operation succeeds, the saved game array will be non-empty and the error string
        /// will be null. Otherwise, an empty array will be returned and the string will contain the
        /// error description. Note that the returned saved games (if any) will NOT be "Open".
        /// This callback will always execute on the game thread.</param>
        void FetchAllSavedGames(Action<SavedGame[], string> callback);

        /// <summary>
        /// Deletes the specified saved game.
        /// This will delete the data of the saved game locally and on the cloud.
        /// </summary>
        /// <param name="savedGame">The saved game to delete.</param>
        void DeleteSavedGame(SavedGame savedGame);

        /// <summary>
        /// On Android/Google Play Games Services platform, this method shows the select saved game UI
        /// with the indicated configuration. If the user selects a
        /// saved game in that UI, it will be returned in the passed callback. This saved game will be
        /// unopened and must be passed to <see cref="OpenWithAutomaticConflictResolution"/> or <see cref="OpenWithManualConflictResolution"/>
        /// before it can be used for read or write operations.
        /// </summary>
        /// <param name="uiTitle">The user-visible title of the displayed selection UI.</param>
        /// <param name="maxDisplayedSavedGames">The maximum number of saved games the UI may display.
        /// This value must be greater than 0.</param>
        /// <param name="showCreateSaveUI">If set to <c>true</c>, show UI that will allow the user to
        /// create a new saved game.</param>
        /// <param name="showDeleteSaveUI">If set to <c>true</c> show UI that will allow the user to
        /// delete a saved game.</param>
        /// <param name="callback">The callback that is invoked when an error occurs or if the user
        /// finishes interacting with the UI. If the user selected a saved game, this will be passed
        /// into the callback, and the error message will be null. This returned saved game
        /// will NOT be open, and must be opened before it can be used for Read and Write operations.
        /// If the user backs out of the UI without selecting a saved game, or some error occurs, this callback will
        /// receive a null saved game and an error message. This callback will always execute on the game thread.</param>
        void ShowSelectSavedGameUI(string uiTitle, uint maxDisplayedSavedGames, bool showCreateSaveUI,
                                   bool showDeleteSaveUI, Action<SavedGame, string> callback);
    }
}

