using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Yodo1.MAS
{
    public class Yodo1AdUtils
    {
        /// <summary>
        /// Show Alert
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="positiveButton"></param>
        public static void ShowAlert(string title, string message, string positiveButton)
        {
            if (!string.IsNullOrEmpty(positiveButton))
            {
                int index = EditorUtility.DisplayDialogComplex(title, message, positiveButton, "", "");

            }
            return;
        }
    }
}