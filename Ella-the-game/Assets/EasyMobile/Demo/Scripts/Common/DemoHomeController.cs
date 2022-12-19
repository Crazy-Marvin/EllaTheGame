using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyMobile.Internal;

namespace EasyMobile.Demo
{
    public class DemoHomeController : MonoBehaviour
    {
        [Header("Object References")]
        public Text installationTime;

        void Awake()
        {
            // Initialize EM runtime.
            if (!RuntimeManager.IsInitialized())
                RuntimeManager.Init();
        }

        void Start()
        {
            var installTime = RuntimeHelper.GetAppInstallationTime();
            installationTime.text = "Install Date: " + installTime.ToShortDateString() + " " + installTime.ToShortTimeString();
        }

        void Update()
        {
            #if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape))
            {   
                // Ask if user wants to exit
                NativeUI.AlertPopup alert = NativeUI.ShowTwoButtonAlert("Exit App",
                                                "Do you want to exit?",
                                                "Yes", 
                                                "No");

                if (alert != null)
                    alert.OnComplete += delegate (int button)
                    { 
                        if (button == 0)
                            Application.Quit();
                    };
            }

            #endif
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

