using UnityEngine;
using Yodo1.MAS;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class YodoAdsManager_new : MonoBehaviour
{
    public static YodoAdsManager_new instance;

    [Space(10)]
    [Header("Privacy Popup Settings")]
    [SerializeField]
    private bool privacyPopup = true;

    [Header("Custom Privacy links (optional) ")]
    [Tooltip("Enter your privacy policy link. Leave empty if you do not have a privacy policy")]
    [SerializeField]
    string privacyPolicyLink;
    [Tooltip("Enter your terms of services link. Leave empty if you do not have a terms of services.")]
    [SerializeField]
    string termOfServiceLink;

    [Header("Privacy Popup Colors Settings (optional)")]
    [SerializeField]
    private bool enabledCustomizeColors;

    [SerializeField]
    [ConditionalHide("enabledCustomizeColors", true)]
    Color titleBackgroundColor;
    [SerializeField]
    [ConditionalHide("enabledCustomizeColors", true)]
    Color titleTextColor;
    [SerializeField]
    [ConditionalHide("enabledCustomizeColors", true)]
    Color contentBackgroundColor;
    [SerializeField]
    [ConditionalHide("enabledCustomizeColors", true)]
    Color contentTextColor;
    [SerializeField]
    [ConditionalHide("enabledCustomizeColors", true)]
    Color buttonBackgroundColor;
    [SerializeField]
    [ConditionalHide("enabledCustomizeColors", true)]
    Color buttonTextColor;

    //[Header("Pause game when the interstitial or reward ads are playing(optionl)")]
    //[SerializeField]
    //private bool autoPauseGame = false;

    [Space(10)]
    [Header("MAS SDK Initialization Events")]
    [SerializeField] UnityEvent OnSDKIntialized;
    [SerializeField] UnityEvent OnSDKInitializationFailed;

    bool isInitialized = false;

    private void Awake()
    {
#if UNITY_2023_2_OR_NEWER
        if (FindObjectsByType<YodoAdsManager>(FindObjectsSortMode.None).Length > 1)
#else
        if (FindObjectsOfType<YodoAdsManager>().Length > 1)
#endif
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        Yodo1U3dMasCallback.OnSdkInitializedEvent += (success, error) =>
        {
            if (success)
            {
                Debug.Log(Yodo1U3dMas.TAG + "NoCode The initialization has succeeded");
                OnSDKIntialized.Invoke();
                isInitialized = true;
            }
            else
            {
                OnSDKInitializationFailed.Invoke();
                Debug.Log(Yodo1U3dMas.TAG + "NoCode The initialization has failed with error: " + error.ToString());
            }
        };

        if (privacyPopup)
        {
            Yodo1AdBuildConfig buildConfig = new Yodo1AdBuildConfig();
            if (string.IsNullOrEmpty(privacyPolicyLink) && string.IsNullOrEmpty(termOfServiceLink))
            {
                buildConfig.enableUserPrivacyDialog(true);
            }
            else if (!string.IsNullOrEmpty(privacyPolicyLink) && string.IsNullOrEmpty(termOfServiceLink))
            {
                buildConfig.enableUserPrivacyDialog(true)
                    .privacyPolicyUrl(privacyPolicyLink);
            }
            else if (string.IsNullOrEmpty(privacyPolicyLink) && !string.IsNullOrEmpty(termOfServiceLink))
            {
                buildConfig.enableUserPrivacyDialog(true)
                    .userAgreementUrl(termOfServiceLink);
            }
            else
            {
                buildConfig.enableUserPrivacyDialog(true)
                    .privacyPolicyUrl(privacyPolicyLink)
                    .userAgreementUrl(termOfServiceLink);
            }

            if (enabledCustomizeColors)
            {
                Yodo1MasUserPrivacyConfig userPrivacyConfig = new Yodo1MasUserPrivacyConfig();
                userPrivacyConfig.titleBackgroundColor(titleBackgroundColor);
                userPrivacyConfig.titleTextColor(titleTextColor);
                userPrivacyConfig.contentBackgroundColor(contentBackgroundColor);
                userPrivacyConfig.contentTextColor(contentTextColor);
                userPrivacyConfig.buttonBackgroundColor(buttonBackgroundColor);
                userPrivacyConfig.buttonTextColor(buttonTextColor);

                buildConfig.userPrivacyConfig(userPrivacyConfig);
            }

            Yodo1U3dMas.SetAdBuildConfig(buildConfig);
        }

        if (!isInitialized)
        {
            Yodo1U3dMas.InitializeMasSdk();
            //Yodo1U3dMas.SetAutoPauseGame(autoPauseGame);
        }
    }
}
