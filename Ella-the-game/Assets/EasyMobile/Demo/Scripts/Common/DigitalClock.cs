using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    [AddComponentMenu(""), RequireComponent(typeof(UnityEngine.UI.Text))]
    public class DigitalClock : MonoBehaviour
    {
        Text clockText;

        // Use this for initialization
        void Start()
        {
            clockText = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            clockText.text = System.DateTime.Now.ToString("hh:mm:ss");
        }
    }
}

