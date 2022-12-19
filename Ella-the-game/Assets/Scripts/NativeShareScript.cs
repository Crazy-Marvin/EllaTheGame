using UnityEngine;
using System.Collections;
using System.IO;
using System;
public class NativeShareScript : MonoBehaviour
{

    private bool isProcessing = false;
    private bool isFocus = false;

    public void ShareBtnPress()
    {
        //if (!isProcessing)
        //{
        //    StartCoroutine(ShareScreenshot());
        //}
        //StartCoroutine(TakeScreenshotAndShare());

        int score = FindObjectOfType<inGameUI>().getCurrentScore();
        string message = "Hey,"+ Environment.NewLine +"I just scored " + score + " coins in Ella - The Game. Are you able to beat that? 🐕 "+ Environment.NewLine +"Check out this amazing game: https://play.google.com/store/apps/details?id=rocks.poopjournal.Ella";

        EasyMobile.Sharing.ShareText(message, "Ella - The Game");
    }
    //private IEnumerator TakeScreenshotAndShare()
    //{
    //    yield return new WaitForEndOfFrame();

    //    Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    //    ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
    //    ss.Apply();

    //    string filePath = Path.Combine(Application.temporaryCachePath, "Ella" + DateTime.Now.ToString() + ".png");
    //    File.WriteAllBytes(filePath, ss.EncodeToPNG());

    //    // To avoid memory leaks
    //    Destroy(ss);

    //    new NativeShare().AddFile(filePath)
    //        .SetSubject("Ella the Game").SetText("Check out this Amazing Game").SetUrl("https://play.google.com/store/apps/details?id=rocks.poopjournal.Ella")
    //        .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
    //        .Share();

    //    // Share on WhatsApp only, if installed (Android only)
    //    //if( NativeShare.TargetExists( "com.whatsapp" ) )
    //    //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    //}
    //IEnumerator ShareScreenshot()
    //{
    //    isProcessing = true;

    //    yield return new WaitForEndOfFrame();

    //    ScreenCapture.CaptureScreenshot("screenshot.png", 2);
    //    string destination = Path.Combine(Application.persistentDataPath, "screenshot.png");

    //    yield return new WaitForSecondsRealtime(0.3f);

    //    if (!Application.isEditor)
    //    {
    //        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
    //        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
    //        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
    //        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
    //        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
    //        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"),
    //            uriObject);
    //        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),
    //            "Can you beat my score?");
    //        intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
    //        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
    //        AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser",
    //            intentObject, "Share your new score");
    //        currentActivity.Call("startActivity", chooser);

    //        yield return new WaitForSecondsRealtime(1);
    //    }

    //    yield return new WaitUntil(() => isFocus);
    //    PlayGames.Instance.SharingIsCaring();
    //    PlayGames.Instance.SendLove();
    //    isProcessing = false;
    //}

    //private void OnApplicationFocus(bool focus)
    //{
    //    isFocus = focus;
    //}
}