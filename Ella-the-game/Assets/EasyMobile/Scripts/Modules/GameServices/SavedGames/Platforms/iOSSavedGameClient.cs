#if UNITY_IOS
using UnityEngine;
using System;
using AOT;
using System.Collections.Generic;
using EasyMobile.Internal;
using EasyMobile.Internal.GameServices.iOS;

namespace EasyMobile.Internal.GameServices
{
    internal enum iOSConflictResolutionStrategy
    {
        UseBase,
        UseRemote
    }

    internal delegate void iOSConflictCallback(IIOSConflictResolver resolver,iOSGKSavedGame baseGame,byte[] baseData,
        iOSGKSavedGame remoteGame,byte[] remoteData);

    internal interface IIOSConflictResolver
    {
        void ChooseGKSavedGame(iOSGKSavedGame gkSavedGame, byte[] data);
    }

    internal class iOSSavedGameClient : ISavedGameClient
    {
#region ISavedGameClient Implementation

        public void OpenWithAutomaticConflictResolution(string name, Action<SavedGame, string> callback)
        {
            Util.NullArgumentTest(name);
            Util.NullArgumentTest(callback);

            InternalOpenWithAutomaticConflictResolution(
                name, 
                AsIOSConflictResolutionStrategy(EM_Settings.GameServices.AutoConflictResolutionStrategy),
                (iOSGKSavedGame gkSavedGame, string error) =>
                {
                    callback(gkSavedGame != null ? new SavedGame(gkSavedGame) : null, error);
                }
            );
        }

        public void OpenWithManualConflictResolution(string name, bool prefetchDataOnConflict, 
                                                     SavedGameConflictResolver resolverFunction, 
                                                     Action<SavedGame, string> completedCallback)
        {
            Util.NullArgumentTest(name);
            Util.NullArgumentTest(resolverFunction);
            Util.NullArgumentTest(completedCallback);

            InternalOpenWithManualConflictResolution(
                name,
                prefetchDataOnConflict,
                // Internal conflict callback
                (IIOSConflictResolver resolver,
                 iOSGKSavedGame baseGame, byte[] baseData,
                 iOSGKSavedGame remoteGame, byte[] remoteData) =>
                {
                    // Invoke the user's conflict resolving function, get their choice
                    var choice = resolverFunction(new SavedGame(baseGame), baseData, new SavedGame(remoteGame), remoteData);

                    // Let the internal client know the selected saved game, 
                    // if the passed data is null if will loaded automatically from the chosen iOSGKSavedGame
                    iOSGKSavedGame selectedGame = null;
                    byte[] selectedData = null;

                    switch (choice)
                    {
                        case SavedGameConflictResolutionStrategy.UseBase:
                            selectedGame = baseGame;
                            selectedData = baseData;
                            break;
                        case SavedGameConflictResolutionStrategy.UseRemote:
                            selectedGame = remoteGame;
                            selectedData = remoteData;
                            break;
                        default:
                            Debug.LogError("Unhandled conflict resolution strategy: " + choice.ToString());
                            break;
                    }

                    resolver.ChooseGKSavedGame(selectedGame, selectedData);
                },
                // Completed callback
                (iOSGKSavedGame gkSavedGame, string error) =>
                {
                    completedCallback(gkSavedGame != null ? new SavedGame(gkSavedGame) : null, error);
                }
            );
        }

        public void ReadSavedGameData(SavedGame savedGame, Action<SavedGame, byte[], string> callback)
        {
            Util.NullArgumentTest(savedGame);
            Util.NullArgumentTest(callback);

            InternalLoadSavedGameData(
                savedGame.GKSavedGame,
                (byte[] data, string error) =>
                {
                    callback(savedGame, data, error);
                }
            );
        }

        public void WriteSavedGameData(SavedGame savedGame, byte[] data, Action<SavedGame, string> callback)
        {
            SavedGameInfoUpdate infoUpdate = new SavedGameInfoUpdate.Builder().Build();
            WriteSavedGameData(savedGame, data, infoUpdate, callback);
        }

        // SavedGameInfoUpdate is not really used on iOS.
        public void WriteSavedGameData(SavedGame savedGame, byte[] data, SavedGameInfoUpdate infoUpdate, Action<SavedGame, string> callback)
        {
            Util.NullArgumentTest(savedGame);
            Util.NullArgumentTest(data);
            Util.NullArgumentTest(callback);

            InternalSaveGameData(
                savedGame.GKSavedGame, 
                data, 
                (iOSGKSavedGame gkSavedGame, string error) =>
                {
                    callback(gkSavedGame != null ? new SavedGame(gkSavedGame) : null, error);
                });
        }

        public void FetchAllSavedGames(Action<SavedGame[], string> callback)
        {
            Util.NullArgumentTest(callback);

            InternalFetchSavedGames(
                (iOSGKSavedGame[] gkSavedGames, string error) =>
                {
                    var savedGames = new List<SavedGame>();

                    if (string.IsNullOrEmpty(error))
                    {
                        for (int i = 0; i < gkSavedGames.Length; i++)
                        {
                            savedGames.Add(new SavedGame(gkSavedGames[i]));
                        }
                    }

                    callback(savedGames.ToArray(), error); 
                });
        }

        public void DeleteSavedGame(SavedGame savedGame)
        {
            Util.NullArgumentTest(savedGame);

            string name = savedGame.Name;
            InternalDeleteSavedGame(
                savedGame.GKSavedGame, 
                (string error) =>
                {
                    if (string.IsNullOrEmpty(error))
                        Debug.Log("Successfully deleted saved game " + name);
                    else
                        Debug.Log("Failed to delete saved game " + name + " due to error: " + error);
                });
        }

        public void ShowSelectSavedGameUI(string uiTitle, uint maxDisplayedSavedGames, bool showCreateSaveUI,
                                          bool showDeleteSaveUI, Action<SavedGame, string> callback)
        {
            string error = "ShowSelectSavedGameUI is not available on this platform.";

            if (callback != null)
                callback(null, error);
            else
                Debug.Log(error);
        }

#endregion

#region Internal Saved Game Client Implementation

        internal static void InternalOpenWithAutomaticConflictResolution(string name, iOSConflictResolutionStrategy resolutionStrategy,
                                                                         Action<iOSGKSavedGame, string> callback)
        {
            Util.NullArgumentTest(name);
            Util.NullArgumentTest(callback);

            InternalOpenWithManualConflictResolution(
                name, false,
                (resolver, baseGame, baseData, remoteGame, remoteData) =>
                {
                    switch (resolutionStrategy)
                    {
                        case iOSConflictResolutionStrategy.UseBase:
                            resolver.ChooseGKSavedGame(baseGame, baseData);
                            return;
                        case iOSConflictResolutionStrategy.UseRemote:
                            resolver.ChooseGKSavedGame(remoteGame, remoteData);
                            return;
                        default:
                            Debug.LogError("Unhandled strategy " + resolutionStrategy);
                            callback(null, "Unhandled conflict resolution strategy " + resolutionStrategy);
                            return;
                    }
                },
                callback
            );
        }

        internal static void InternalOpenWithManualConflictResolution(string name, bool prefetchDataOnConflict,
                                                                      iOSConflictCallback conflictCallback,
                                                                      Action<iOSGKSavedGame, string> completedCallback)
        {
            Util.NullArgumentTest(name);
            Util.NullArgumentTest(conflictCallback);
            Util.NullArgumentTest(completedCallback);

            InternalOpenSavedGame(name, matchingGames =>
                {
                    if (matchingGames.Length == 0)
                    {
                        Debug.LogError("Something really wrong happened: no saved game was opened. Please check.");
                    }
                    else if (matchingGames.Length == 1)
                    {
                        completedCallback(matchingGames[0], null);
                    }
                    else
                    {
                        // There're more than one game: conflict detected!
                        // Sort the saved games from old to new, the oldest one is considered the base
                        var sortedGames = new List<iOSGKSavedGame>(matchingGames);
                        sortedGames.Sort((x, y) => x.ModificationDate.CompareTo(y.ModificationDate));

                        // Run the conflict resolve loop
                        ConflictResolvingLoop(sortedGames.ToArray(), prefetchDataOnConflict, conflictCallback, completedCallback);
                    }
                });
        }

        private static void InternalOpenSavedGame(string name, Action<iOSGKSavedGame[]> callback)
        {
            iOSSavedGameNative.EM_OpenSavedGame(
                name,
                InternalOpenSavedGameCallback,
                PInvokeCallbackUtil.ToIntPtr<OpenSavedGameResponse>(
                    response =>
                    {
                        callback(response.GetResultSavedGames());
                    },
                    OpenSavedGameResponse.FromPointer
                )
            );
        }

        [MonoPInvokeCallback(typeof(iOSSavedGameNative.OpenSavedGameCallback))]
        private static void InternalOpenSavedGameCallback(IntPtr response, IntPtr callbackPtr)
        {
            PInvokeCallbackUtil.PerformInternalCallback(
                "iOSSavedGameClient#OpenSavedGameCallback",
                PInvokeCallbackUtil.Type.Temporary,
                response,
                callbackPtr);
        }

        // This method will automatically be called repeatedly until no conflict remains.
        private static void ConflictResolvingLoop(iOSGKSavedGame[] conflictingGames, bool prefetchData,
                                                  iOSConflictCallback conflictCallback,
                                                  Action<iOSGKSavedGame, string> completedCallback)
        {
            if (conflictingGames.Length == 1)
            {
                // No more conflict!
                var openedSavedGame = conflictingGames[0];
                completedCallback(openedSavedGame, null);
                return;
            }

            // The 1st element in the array is always the base game.
            // Either it is the game with oldest timestamp to start with,
            // or the resolved game of the previous conflict resolution request.
            var baseGame = conflictingGames[0];
            var remoteGame = conflictingGames[1];

            var resolver = new NativeConflictResolver(
                               baseGame,
                               remoteGame,
                               completedCallback,
                               updatedConflictingGames => 
                               ConflictResolvingLoop(updatedConflictingGames, prefetchData, conflictCallback, completedCallback)
                           );

            // Invoke the conflict callback immediately if we don't need to prefetch data
            if (!prefetchData)
            {
                conflictCallback(resolver, baseGame, null, remoteGame, null);
                return;
            }

            // If we have to prefetch the data, we delegate invoking the conflict resolution
            // callback to the joiner instance (once both callbacks resolve, the joiner will
            // invoke the lambda that we declare here, using the fetched data).
            Prefetcher joiner = new Prefetcher((baseData, remoteData) =>
                        conflictCallback(resolver, baseGame, baseData, remoteGame, remoteData),
                                    completedCallback);

            // Kick off the read calls.
            InternalLoadSavedGameData(baseGame, joiner.OnBaseDataRead);
            InternalLoadSavedGameData(remoteGame, joiner.OnRemoteDataRead);
        }

        internal static void InternalSaveGameData(iOSGKSavedGame savedGame, byte[] data, Action<iOSGKSavedGame, string> callback)
        {
            Util.NullArgumentTest(savedGame);
            Util.NullArgumentTest(data);
            Util.NullArgumentTest(callback);

            iOSSavedGameNative.EM_SaveGameData(
                savedGame.ToPointer(), 
                data,
                data.Length,
                InternalSaveGameDataCallback, 
                PInvokeCallbackUtil.ToIntPtr<SaveGameDataResponse>(
                    response =>
                    {
                        string error = response.GetError();
                        if (string.IsNullOrEmpty(error))
                        {
                            callback(response.GetSavedGame(), error);
                        }
                        else
                        {
                            callback(null, error);
                        }
                    }, 
                    SaveGameDataResponse.FromPointer
                )
            );
        }

        [MonoPInvokeCallback(typeof(iOSSavedGameNative.SaveGameDataCallback))]
        private static void InternalSaveGameDataCallback(IntPtr response, IntPtr callbackPtr)
        {
            PInvokeCallbackUtil.PerformInternalCallback(
                "iOSSavedGameClient#SaveGameDataCallback",
                PInvokeCallbackUtil.Type.Temporary,
                response,
                callbackPtr);
        }

        internal static void InternalLoadSavedGameData(iOSGKSavedGame savedGame, Action<byte[], string> callback)
        {
            Util.NullArgumentTest(savedGame);
            Util.NullArgumentTest(callback);

            iOSSavedGameNative.EM_LoadSavedGameData(
                savedGame.ToPointer(),
                InternalLoadSavedGameDataCallback,
                PInvokeCallbackUtil.ToIntPtr<LoadSavedGameDataResponse>(
                    response =>
                    {
                        callback(response.GetData(), response.GetError());
                    },
                    LoadSavedGameDataResponse.FromPointer
                )
            );
        }

        [MonoPInvokeCallback(typeof(iOSSavedGameNative.LoadSavedGameDataCallback))]
        private static void InternalLoadSavedGameDataCallback(IntPtr response, IntPtr callbackPtr)
        {
            PInvokeCallbackUtil.PerformInternalCallback(
                "iOSSavedGameClient#LoadSavedGameCallback",
                PInvokeCallbackUtil.Type.Temporary,
                response,
                callbackPtr);
        }

        // Note that if there're multiple saved games with the same name,
        // only one instance of them will be returned.
        internal static void InternalFetchSavedGames(Action<iOSGKSavedGame[], string> callback)
        {
            Util.NullArgumentTest(callback);

            iOSSavedGameNative.EM_FetchSavedGames(
                InternalFetchSavedGamesCallback,
                PInvokeCallbackUtil.ToIntPtr<FetchSavedGamesResponse>(
                    response =>
                    {
                        string error = response.GetError();
                        if (string.IsNullOrEmpty(error))
                        {
                            callback(response.GetFetchedSavedGames(), error);
                        }
                        else
                        {
                            callback(new iOSGKSavedGame[0], error);
                        }
                    },
                    FetchSavedGamesResponse.FromPointer
                )
            );
        }

        [MonoPInvokeCallback(typeof(iOSSavedGameNative.FetchSavedGamesCallback))]
        private static void InternalFetchSavedGamesCallback(IntPtr response, IntPtr callbackPtr)
        {
            PInvokeCallbackUtil.PerformInternalCallback(
                "iOSSavedGameClient#FetchSavedGamesCallback",
                PInvokeCallbackUtil.Type.Temporary,
                response,
                callbackPtr);
        }

        internal static void InternalResolveConflictingSavedGames(iOSGKSavedGame[] conflictingSavedGames, byte[] data, Action<iOSGKSavedGame[], string> callback)
        {
            var savedGamePtrs = new IntPtr[conflictingSavedGames.Length];

            for (int i = 0; i < conflictingSavedGames.Length; i++)
            {
                savedGamePtrs[i] = conflictingSavedGames[i].ToPointer();
            }

            iOSSavedGameNative.EM_ResolveConflictingSavedGames(
                savedGamePtrs,
                savedGamePtrs.Length,
                data,
                data.Length,
                InternalResolveConflictingSavedGamesCallback,
                PInvokeCallbackUtil.ToIntPtr<ResolveConflictResponse>(
                    response =>
                    {
                        callback(response.GetSavedGames(), response.GetError());
                    },
                    ResolveConflictResponse.FromPointer
                )
            );
        }

        [MonoPInvokeCallback(typeof(iOSSavedGameNative.ResolveConflictingSavedGamesCallback))]
        private static void InternalResolveConflictingSavedGamesCallback(IntPtr response, IntPtr callbackPtr)
        {
            PInvokeCallbackUtil.PerformInternalCallback(
                "iOSSavedGameClient#ResolveConflictingSavedGamesCallback",
                PInvokeCallbackUtil.Type.Temporary,
                response,
                callbackPtr);
        }

        internal static void InternalDeleteSavedGame(iOSGKSavedGame savedGame, Action<string> callback)
        {
            Util.NullArgumentTest(savedGame);
            Util.NullArgumentTest(callback);

            iOSSavedGameNative.EM_DeleteSavedGame(
                savedGame.Name, 
                InternalDeleteSavedGameCallback, 
                PInvokeCallbackUtil.ToIntPtr<DeleteSavedGameResponse>(
                    response =>
                    {
                        callback(response.GetError());
                    },
                    DeleteSavedGameResponse.FromPointer
                )
            );
        }

        [MonoPInvokeCallback(typeof(iOSSavedGameNative.DeleteSavedGameCallback))]
        private static void InternalDeleteSavedGameCallback(IntPtr response, IntPtr callbackPtr)
        {
            PInvokeCallbackUtil.PerformInternalCallback(
                "iOSSavedGameClient#DeleteSavedGameCallback",
                PInvokeCallbackUtil.Type.Temporary,
                response,
                callbackPtr);
        }

        internal static iOSConflictResolutionStrategy AsIOSConflictResolutionStrategy(SavedGameConflictResolutionStrategy strategy)
        {
            switch (strategy)
            {
                case SavedGameConflictResolutionStrategy.UseBase:
                    return iOSConflictResolutionStrategy.UseBase;
                case SavedGameConflictResolutionStrategy.UseRemote:
                    return iOSConflictResolutionStrategy.UseRemote;
                default:
                    return iOSConflictResolutionStrategy.UseBase;
            }
        }

#endregion

#region IIOSConflictResolver implementation

        private class NativeConflictResolver : IIOSConflictResolver
        {
            private readonly iOSGKSavedGame _base;
            private readonly iOSGKSavedGame _remote;
            private readonly Action<iOSGKSavedGame, string> _completeCallback;
            private readonly Action<iOSGKSavedGame[]> _repeatResolveConflicts;

            internal NativeConflictResolver(iOSGKSavedGame baseGame, iOSGKSavedGame remoteGame,
                                            Action<iOSGKSavedGame, string> completeCallback, Action<iOSGKSavedGame[]> repeatAction)
            {
                this._base = Util.NullArgumentTest(baseGame);
                this._remote = Util.NullArgumentTest(remoteGame);
                this._completeCallback = Util.NullArgumentTest(completeCallback);
                this._repeatResolveConflicts = Util.NullArgumentTest(repeatAction);
            }

            public void ChooseGKSavedGame(iOSGKSavedGame chosenGame, byte[] data)
            {
                if (chosenGame != _base && chosenGame != _remote)
                {
                    _completeCallback(null, "Attempted to choose a saved game that was not part of the conflict.");
                    return;
                }

                var conflictingGames = new iOSGKSavedGame[]{ _base, _remote };

                if (data != null)
                {
                    RequestResolveConflict(conflictingGames, data);
                }
                else
                {
                    // Load data of the chosen game
                    InternalLoadSavedGameData(chosenGame, (byte[] chosenData, string loadDataError) =>
                        {
                            if (!string.IsNullOrEmpty(loadDataError))
                            {
                                // Loading data failed -> cannot resolve conflict
                                _completeCallback(null, loadDataError);
                            }
                            else
                            {   
                                // Loading data succeeded -> now resolve conflict using the chosen game data
                                RequestResolveConflict(conflictingGames, chosenData);
                            }
                        });
                }
            }

            private void RequestResolveConflict(iOSGKSavedGame[] conflictingGames, byte[] data)
            {
                InternalResolveConflictingSavedGames(conflictingGames, data, 
                    (iOSGKSavedGame[] resolvedGames, string resolveError) =>
                    {
                        // Conflict resolution failed, propagate the error
                        if (!string.IsNullOrEmpty(resolveError))
                        {
                            _completeCallback(null, resolveError);
                            return;
                        }

                        // Otherwise, repeat the resolution process until there's no conflict remaining.
                        // Note that the first element in the array is the resolved game (base),
                        // unresolved games are appended at the end of the list.
                        _repeatResolveConflicts(resolvedGames);
                    });
            }
        }

#endregion  // IIOSConflictResolver implementation

#region Saved Game Data Prefetcher

        private class Prefetcher
        {
            private readonly object _lock = new object();
            private bool _baseDataFetched;
            private byte[] _baseData;
            private bool _remoteDataFetched;
            private byte[] _remoteData;
            private Action<iOSGKSavedGame, string> _completedCallback;
            private readonly Action<byte[], byte[]> _dataFetchedCallback;

            internal Prefetcher(Action<byte[], byte[]> dataFetchedCallback,
                                Action<iOSGKSavedGame, string> completedCallback)
            {
                this._dataFetchedCallback = Util.NullArgumentTest(dataFetchedCallback);
                this._completedCallback = Util.NullArgumentTest(completedCallback);
            }

            internal void OnBaseDataRead(byte[] data, string error)
            {
                lock (_lock)
                {
                    // If the request doesn't succeed, report the error and make the callback a noop
                    // so that we don't invoke the callback twice if both reads fail.
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError("Encountered error while prefetching base game data.");
                        _completedCallback(null, error);
                        _completedCallback = delegate
                        {
                        };
                    }
                    else
                    {
                        Debug.Log("Successfully fetched remote game data.");
                        _baseDataFetched = true;
                        _baseData = data;
                        MaybeProceed();
                    }
                }
            }

            internal void OnRemoteDataRead(byte[] data, string error)
            {
                lock (_lock)
                {
                    // If the request doesn't succeed, report the error and make the callback a noop
                    // so that we don't invoke the callback twice if both reads fail.
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError("Encountered error while prefetching remote data.");
                        _completedCallback(null, error);
                        _completedCallback = delegate
                        {
                        };
                    }
                    else
                    {
                        Debug.Log("Successfully fetched remote data.");
                        _remoteDataFetched = true;
                        _remoteData = data;
                        MaybeProceed();
                    }
                }
            }

            private void MaybeProceed()
            {
                if (_baseDataFetched && _remoteDataFetched)
                {
                    Debug.Log("Fetched data for base game and remote game, proceeding...");
                    _dataFetchedCallback(_baseData, _remoteData);
                }
                else
                {
                    Debug.Log("Not all data fetched - base:" + _baseDataFetched +
                        " remote:" + _remoteDataFetched);
                }
            }
        }

#endregion  // Saved Game Data Prefetcher
    }
}
#endif
