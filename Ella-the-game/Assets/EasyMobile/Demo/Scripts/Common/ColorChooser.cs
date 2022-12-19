using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    [RequireComponent(typeof(Image)), RequireComponent(typeof(Button))]
    public class ColorChooser : MonoBehaviour
    {
        public static event System.Action<Color> colorSelected = delegate{};

        Image imgComp;
        Button btnComp;

        void Start()
        {
            imgComp = GetComponent<Image>();
            btnComp = GetComponent<Button>();
            btnComp.onClick.AddListener(SelectColor);
        }

        public void SelectColor()
        {
            colorSelected(imgComp.color);
        }
    }
}
