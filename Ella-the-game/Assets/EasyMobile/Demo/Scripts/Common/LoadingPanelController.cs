using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    public class LoadingPanelController : MonoBehaviour
    {
        [Header("Child Objects")]
        public Image spinner;
        public Text message;
        public Button button;

        [Header("Config")]
        public float spinningAngle = 10;
        public float spinningFillSpeed = 0.002f;

        private bool mIsShowing = false;
        private Coroutine mSpinCoroutine;

        public bool IsShowing
        {
            get { return mIsShowing; }
        }

        public void Show()
        {
            if (!mIsShowing)
            {
                gameObject.SetActive(true);
                mIsShowing = true;
            }
        }

        public void Hide()
        {
            mIsShowing = false; 
            gameObject.SetActive(false);
        }

        public void SetMessageText(string msg)
        {
            message.text = msg;
        }

        public void SetButtonLabel(string label)
        {
            button.GetComponentInChildren<Text>().text = label;
        }

        public void RegisterButtonCallback(UnityEngine.Events.UnityAction callback)
        {
            button.onClick.AddListener(callback);
        }

        void OnEnable()
        {
            mSpinCoroutine = StartCoroutine(CRSpin());
        }

        void OnDisable()
        {
            if (mSpinCoroutine != null)
            {
                StopCoroutine(mSpinCoroutine);
                mSpinCoroutine = null;
            }
        }

        private IEnumerator CRSpin()
        {
            float fillAmount = 0f;
            while (true)
            {
                fillAmount += spinningFillSpeed;
                spinner.fillAmount = Mathf.Repeat(fillAmount, 1);
                spinner.rectTransform.Rotate(0, 0, spinningAngle);
                yield return null;
            }
        }
    }
}