#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RewardedVideoSampleAds : MonoBehaviour
{
    public Text RewardLabel;
    public Text RewardTimer;
    private int TimeRemaining = 5;
    public RectTransform LogoRectTransform;
    private void OnEnable()
    {
        StartCoroutine(StartTimerForReward());

        string[] res = UnityStats.screenRes.Split('x');
        if (int.Parse(res[1]) > int.Parse(res[0]))
        {
            RewardTimer.rectTransform.localScale = new Vector3((float)int.Parse(res[0]) / 1000, (float)int.Parse(res[0]) / 1000, 1);
            LogoRectTransform.anchorMin = new Vector2(0f, 0.5f);
            LogoRectTransform.anchorMax = new Vector2(1, 0.5f);
            LogoRectTransform.pivot = new Vector2(0.5f, 0.5f);
            LogoRectTransform.localScale = new Vector3(1, ((float)int.Parse(res[1]) / (float)int.Parse(res[0])), 1);
            LogoRectTransform.offsetMin = Vector2.zero;
            LogoRectTransform.offsetMax = new Vector2(0, 417);
            LogoRectTransform.localPosition = Vector3.zero;
        }
        else
        {
            float TempVal = float.Parse(res[1]) / float.Parse(res[0]);

            float Value = (float)int.Parse(res[1]) / 1000;
            if (Value > 1)
            {
                Value = 1;
            }
            RewardLabel.rectTransform.localScale = new Vector3(Value, Value, 1);
            LogoRectTransform.localScale = new Vector3(Value, Value, 1);
        }
    }
    private IEnumerator StartTimerForReward()
    {
        RewardTimer.gameObject.SetActive(true);
        RewardTimer.text = TimeRemaining + " seconds remaining";
        yield return new WaitForSecondsRealtime(1.0f);
        TimeRemaining--;

        if (TimeRemaining == 0)
        {
            RewardTimer.gameObject.SetActive(false);
            TimeRemaining = 5;
            RewardLabel.text = "RV shown successfully, and reward granted. Reward callback will send on ad close.";
            Yodo1EditorAds.GetRewardsInEditor();
        }
        else
        {
            StartCoroutine(StartTimerForReward());
        }
    }
    public void CloseRewardedVideoAds()
    {
        StopAllCoroutines();
        Yodo1EditorAds.CloseRewardedVideodsInEditor();
        RewardLabel.text = "Reward not yet granted.";
        TimeRemaining = 5;
    }
}
#endif