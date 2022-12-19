using System;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.MiniJSON;

namespace EasyMobile.Internal.Privacy
{
    /// <summary>
    /// Use this class to construct parameters when calling native methods.
    /// </summary>
    [Serializable]
    internal class ConsentDialogContentSerializer
    {
        [Serializable]
        internal class SplitContent
        {
            /// <summary>
            /// If <see cref="type"/> equals to this, <see cref="content"/> is plain text.
            /// </summary>
            public const string PlainTextType = "plain_text";

            /// <summary>
            /// If <see cref="type"/> equals to this, <see cref="content"/> is a serialized <see cref="ConsentDialog.Toggle"/>.
            /// </summary>
            public const string ToggleType = "toggle";

            /// <summary>
            /// If <see cref="type"/> equals to this, <see cref="content"/> is a serialized <see cref="ConsentDialog.Button"/>.
            /// </summary>
            public const string ButtonType = "button";

            public string type;
            public string content;

            public SplitContent(string type = "", string content = "")
            {
                this.type = type;
                this.content = content;
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }

            public bool IsPlainText()
            {
                return type != null && type.Equals(PlainTextType);
            }

            public bool IsToggle()
            {
                return type != null && type.Equals(ToggleType);
            }

            public bool IsButton()
            {
                return type != null && type.Equals(ButtonType);
            }
        }

        /// <summary>
        /// A serialized array, contains all <see cref="SplitContent"/> infomations.
        /// </summary>
        internal string SerializedContent { get; private set; }

        internal ConsentDialogContentSerializer(ConsentDialog consentDialog)
        {
            if (consentDialog == null)
                throw new ArgumentNullException("Tried to pass a null consent dialog into ConsentDialogNativeAdapter's constructor.");

            SerializedContent = GenerateSplitedContents(consentDialog);
        }

        private string GenerateSplitedContents(ConsentDialog consentDialog)
        {
            List<SplitContent> result = new List<SplitContent>();
            List<string> splittedContents = consentDialog.GetSplittedContents();

            foreach (var content in splittedContents)
            {
                if (string.IsNullOrEmpty(content))
                    continue;

                /// If this content is a button.
                if (ConsentDialog.IsButtonPattern(content))
                {
                    string id = ConsentDialog.SearchForIdInButtonPattern(content);
                    var button = consentDialog.FindButtonWithId(id);

                    /// This mean the id is not defined, so we just skip it.
                    if (button == null)
                        continue;

                    result.Add(new SplitContent(SplitContent.ButtonType, button.ToString()));

                    continue;
                }

                /// If this content is a toggle.
                if (ConsentDialog.IsTogglePattern(content))
                {
                    string id = ConsentDialog.SearchForIdInTogglePattern(content);
                    var toggle = consentDialog.FindToggleWithId(id);        

                    /// This mean the id is not defined, so we just skip it.
                    if (toggle == null)
                        continue;

                    result.Add(new SplitContent(SplitContent.ToggleType, toggle.ToString()));

                    continue;
                }
                
                /// Otherwise this content should be plain text.
                result.Add(new SplitContent(SplitContent.PlainTextType, content));
            }

            return Json.Serialize(result.ToArray());
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
