/*using System;
using TMPro;
using UnityEngine;
using Unity.Services.Authentication;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class SimpleGoogleSignIn : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;

    private string token;
    private string error;

    private void Start()
    {
        SetStatus("Starting...");

#if UNITY_ANDROID
        PlayGamesPlatform.Activate();
        SetStatus("Google Play Games activated");
        LoginGooglePlayGames();
#else
        SetStatus("Not running on Android");
        Debug.Log("This script runs only on Android builds.");
#endif
    }

    public void LoginGooglePlayGames()
    {
#if UNITY_ANDROID
        if (PlayGamesPlatform.Instance == null)
        {
            error = "PlayGamesPlatform not initialized";
            SetStatus(error);
            Debug.LogError(error);
            return;
        }

        SetStatus("Signing in to Google Play Games...");

        PlayGamesPlatform.Instance.Authenticate(status =>
        {
            if (status == SignInStatus.Success)
            {
                SetStatus("Google Play Games login successful");
                Debug.Log("Google Play Games login successful");

                PlayGamesPlatform.Instance.RequestServerSideAccess(true, async code =>
                {
                    token = code;
                    SetStatus("Auth code received");
                    Debug.Log("Authorization code: " + code);

                    await SignInUnity(code);
                });
            }
            else
            {
                error = "Google Play Games login failed: " + status;
                SetStatus(error);
                Debug.LogError(error);
            }
        });
#endif
    }

    private async System.Threading.Tasks.Task SignInUnity(string authCode)
    {
        try
        {
            SetStatus("Signing in to Unity Authentication...");
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);

            SetStatus("Unity Sign-In SUCCESS");
            Debug.Log("Unity Sign-In SUCCESS");
        }
        catch (Exception e)
        {
            error = e.Message;
            SetStatus("Unity Sign-In FAILED");
            Debug.LogError("Unity Sign-In FAILED: " + e.Message);
        }
    }

    private void SetStatus(string message)
    {
        Debug.Log(message);

        if (statusText != null)
            statusText.text = message;
    }
}*/