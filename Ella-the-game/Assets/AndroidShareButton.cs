using System;
using UnityEngine;

public class AndroidShareButton : MonoBehaviour
{
    static int score = FindObjectOfType<inGameUI>().getCurrentScore();
    static string message = "Hey," + Environment.NewLine + "I just scored " + score + " coins in Ella - The Game. Are you able to beat that? 🐕 " + Environment.NewLine + "Check out this amazing game: https://play.google.com/store/apps/details?id=rocks.poopjournal.Ella";
   
    public string subject = "My Score";
    public string shareMessage = message; //"Your Share Message Here";

    public void ShareText()
    {
        PlayGames.Instance.SharingIsCaring();
        // Create intent
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        // Set action and type
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");

        // Set subject and message
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);

        // Get current activity
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        // Start activity
        AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share via");
        unityActivity.Call("startActivity", chooser);
    }
}
