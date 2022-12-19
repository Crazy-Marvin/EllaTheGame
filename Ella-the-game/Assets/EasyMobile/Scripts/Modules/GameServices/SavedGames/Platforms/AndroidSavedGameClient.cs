#if UNITY_ANDROID
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using EasyMobile.Internal;

#if EM_GPGS
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif

namespace EasyMobile.Internal.GameServices
{
    internal class AndroidSavedGameClient : ISavedGameClient
    {
        const string SDK_MISSING_MESSAGE = "SDK missing. Please import Google Play Games plugin for Unity.";

        public void OpenWithAutomaticConflictResolution(string name, Action<SavedGame, string> callback)
        {
#if EM_GPGS
            Util.NullArgumentTest(name);
            Util.NullArgumentTest(callback);

            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                name, 
                AsDataSource(EM_Settings.GameServices.GPGSDataSource),
                AsGPGSConflictResolutionStrategy(EM_Settings.GameServices.AutoConflictResolutionStrategy),
                (SavedGameRequestStatus status, ISavedGameMetadata game) =>
                {
                    callback(game != null ? new SavedGame(game) : null, 
                        status == SavedGameRequestStatus.Success ? null : status.ToString());
                });
#else
            Debug.LogError(SDK_MISSING_MESSAGE);
#endif
        }

        public void OpenWithManualConflictResolution(string name, bool prefetchDataOnConflict, 
                                                     SavedGameConflictResolver resolverFunction, 
                                                     Action<SavedGame, string> completedCallback)
        {
#if EM_GPGS
            Util.NullArgumentTest(name);
            Util.NullArgumentTest(resolverFunction);
            Util.NullArgumentTest(completedCallback);

            PlayGamesPlatform.Instance.SavedGame.OpenWithManualConflictResolution(
                name, 
                AsDataSource(EM_Settings.GameServices.GPGSDataSource),
                prefetchDataOnConflict,
                // Internal conflict callback
                (IConflictResolver resolver, 
                 ISavedGameMetadata original, byte[] originalData, 
                 ISavedGameMetadata unmerged, byte[] unmergedData) =>
                {
                    // Invoke the user's conflict resolving function, get their choice
                    var choice = resolverFunction(new SavedGame(original), originalData, new SavedGame(unmerged), unmergedData);
                    ISavedGameMetadata selectedGame = null;

                    switch (choice)
                    {
                        case SavedGameConflictResolutionStrategy.UseBase:
                            selectedGame = original;
                            break;
                        case SavedGameConflictResolutionStrategy.UseRemote:
                            selectedGame = unmerged;
                            break;
                        default:
                            Debug.LogError("Unhandled conflict resolution strategy: " + choice.ToString());
                            break;
                    }

                    // Let the internal client know the selected saved game
                    resolver.ChooseMetadata(selectedGame);
                },
                // Completed callback
                (SavedGameRequestStatus status, ISavedGameMetadata game) =>
                {
                    completedCallback(game != null ? new SavedGame(game) : null, 
                        status == SavedGameRequestStatus.Success ? null : status.ToString());
                });
#else
            Debug.LogError(SDK_MISSING_MESSAGE);
#endif
        }

        public void ReadSavedGameData(SavedGame savedGame, Action<SavedGame, byte[], string> callback)
        {
#if EM_GPGS
            Util.NullArgumentTest(savedGame);
            Util.NullArgumentTest(callback);

            PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(
                savedGame.GPGSSavedGameMetadata, 
                (SavedGameRequestStatus status, byte[] data) =>
                {
                    callback(savedGame, data, status == SavedGameRequestStatus.Success ? null : status.ToString());
                }
            );
#else
            Debug.LogError(SDK_MISSING_MESSAGE);
#endif
        }

        public void WriteSavedGameData(SavedGame savedGame, byte[] data, Action<SavedGame, string> callback)
        {
            SavedGameInfoUpdate infoUpdate = new SavedGameInfoUpdate.Builder().Build();
            WriteSavedGameData(savedGame, data, infoUpdate, callback);
        }

        public void WriteSavedGameData(SavedGame savedGame, byte[] data, SavedGameInfoUpdate infoUpdate, Action<SavedGame, string> callback)
        {
#if EM_GPGS
            Util.NullArgumentTest(savedGame);
            Util.NullArgumentTest(data);
            Util.NullArgumentTest(callback);

            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();

            if (infoUpdate.IsDescriptionUpdated)
                builder = builder.WithUpdatedDescription(infoUpdate.UpdatedDescription);

            if (infoUpdate.IsPlayedTimeUpdated)
                builder = builder.WithUpdatedPlayedTime(infoUpdate.UpdatedPlayedTime);

            if (infoUpdate.IsCoverImageUpdated && infoUpdate.UpdatedPngCoverImage != null)
                builder = builder.WithUpdatedPngCoverImage(infoUpdate.UpdatedPngCoverImage);

            SavedGameMetadataUpdate updatedMetadata = builder.Build();

            PlayGamesPlatform.Instance.SavedGame.CommitUpdate(
                savedGame.GPGSSavedGameMetadata, 
                updatedMetadata, 
                data,
                (SavedGameRequestStatus status, ISavedGameMetadata game) =>
                {
                    callback(game != null ? new SavedGame(game) : null, 
                        status == SavedGameRequestStatus.Success ? null : status.ToString());
                }
            );
#else
            Debug.LogError(SDK_MISSING_MESSAGE);
#endif
        }

        public void FetchAllSavedGames(Action<SavedGame[], string> callback)
        {
#if EM_GPGS
            Util.NullArgumentTest(callback);

            PlayGamesPlatform.Instance.SavedGame.FetchAllSavedGames(
                AsDataSource(EM_Settings.GameServices.GPGSDataSource),
                (SavedGameRequestStatus status, List<ISavedGameMetadata> games) =>
                {
                    var savedGames = new List<SavedGame>();

                    if (status == SavedGameRequestStatus.Success)
                    {
                        for (int i = 0; i < games.Count; i++)
                        {
                            savedGames.Add(new SavedGame(games[i]));
                        }
                    }

                    callback(savedGames.ToArray(), status == SavedGameRequestStatus.Success ? null : status.ToString());
                }
            );

#else
            Debug.LogError(SDK_MISSING_MESSAGE);
#endif
        }

        public void DeleteSavedGame(SavedGame savedGame)
        {
#if EM_GPGS
            Util.NullArgumentTest(savedGame);
            PlayGamesPlatform.Instance.SavedGame.Delete(savedGame.GPGSSavedGameMetadata);
#else
            Debug.LogError(SDK_MISSING_MESSAGE);
#endif
        }

        public void ShowSelectSavedGameUI(string uiTitle, uint maxDisplayedSavedGames, bool showCreateSaveUI,
                                          bool showDeleteSaveUI, Action<SavedGame, string> callback)
        {
#if EM_GPGS
            Util.NullArgumentTest(uiTitle);
            Util.NullArgumentTest(callback);
            PlayGamesPlatform.Instance.SavedGame.ShowSelectSavedGameUI(uiTitle,
                maxDisplayedSavedGames,
                showCreateSaveUI,
                showDeleteSaveUI,
                (SelectUIStatus status, ISavedGameMetadata game) =>
                {
                    if (status == SelectUIStatus.SavedGameSelected)
                    {
                        // Handle saved game selected
                        callback(new SavedGame(game), null);
                    }
                    else
                    {
                        // Handle cancel or error
                        callback(null, status.ToString());
                    }
                }
            );
#else
            Debug.LogError(SDK_MISSING_MESSAGE);
#endif
        }

#if EM_GPGS
        internal static ConflictResolutionStrategy AsGPGSConflictResolutionStrategy(SavedGameConflictResolutionStrategy strategy)
        {
            switch (strategy)
            {
                case SavedGameConflictResolutionStrategy.UseBase:
                    return ConflictResolutionStrategy.UseOriginal;
                case SavedGameConflictResolutionStrategy.UseRemote:
                    return ConflictResolutionStrategy.UseUnmerged;
                default:
                    return ConflictResolutionStrategy.UseOriginal;
            }
        }

        internal static DataSource AsDataSource(GPGSSavedGameDataSource source)
        {
            switch (source)
            {
                case GPGSSavedGameDataSource.ReadCacheOrNetwork:
                    return DataSource.ReadCacheOrNetwork;
                case GPGSSavedGameDataSource.ReadNetworkOnly:
                    return DataSource.ReadNetworkOnly;
                default:
                    return DataSource.ReadCacheOrNetwork;
            }
        }
#endif
    }
}

#endif