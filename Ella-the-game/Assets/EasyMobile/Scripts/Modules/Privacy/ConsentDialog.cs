using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using EasyMobile.MiniJSON;
using EasyMobile.Internal;
using EasyMobile.Internal.Privacy;

namespace EasyMobile
{
    [Serializable]
    public class ConsentDialog
    {
        #region Inner Classes

        /// <summary>
        /// This class represents a toogle in the consent dialog.
        /// </summary>
        [Serializable]
        public class Toggle
        {
            /// <summary>
            /// Gets the identifier of this toggle.
            /// </summary>
            /// <value>The identifier.</value>
            public string Id
            {
                get { return id; }
            }

            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            /// <value>The title.</value>
            public string Title
            {
                get { return title; }
                set { title = value; }
            }

            /// <summary>
            /// Returns <see cref="OffDescription"/> if <see cref="shouldToggleDescription"/> is <c>true</c>
            /// and <see cref="isOn"/> is <c>false</c>. Otherwise returns <see cref="OnDescription"/>.
            /// </summary>
            /// <value>The description.</value>
            public string Description
            {
                get { return ShouldToggleDescription ? (isOn ? OnDescription : OffDescription) : OnDescription; }
            }

            /// <summary>
            /// Gets or sets the description of the toggle when is is on.
            /// </summary>
            /// <value>The on description.</value>
            public string OnDescription
            {
                get { return onDescription; }
                set { onDescription = value; }
            }

            /// <summary>
            /// Gets or sets the description of the toggle when it is off.
            /// </summary>
            /// <value>The off description.</value>
            public string OffDescription
            {
                get { return offDescription; }
                set { offDescription = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this toggle is on.
            /// Default value is <c>false</c>.
            /// </summary>
            /// <value><c>true</c> if this instance is on; otherwise, <c>false</c>.</value>
            public bool IsOn
            {
                get { return isOn; }
                set { isOn = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this toggle is interactable.
            /// Default value is <c>true</c>.
            /// </summary>
            /// <value><c>true</c> if this toggle is interactable; otherwise, <c>false</c>.</value>
            public bool IsInteractable
            {
                get { return interactable; }
                set { interactable = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this toggle 
            /// should update its description when its on/off state changes.
            /// If this value is <c>false</c>, the <see cref="OnDescription"/> will always show.
            /// Default is <c>false</c>.
            /// </summary>
            /// <value><c>true</c> if should toggle description; otherwise, <c>false</c>.</value>
            public bool ShouldToggleDescription
            {
                get { return shouldToggleDescription; }
                set { shouldToggleDescription = value; }
            }

            [SerializeField]
            private string id;
            [SerializeField]
            private string title;
            [SerializeField]
            private string onDescription;
            [SerializeField]
            private string offDescription;
            [SerializeField]
            private bool isOn = false;
            [SerializeField]
            private bool interactable = true;
            [SerializeField]
            private bool shouldToggleDescription = false;

            public Toggle(string id)
            {
                this.id = id;
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }
        }

        /// <summary>
        /// This class represents an action button in the consent dialog.
        /// </summary>
        [Serializable]
        public class Button
        {
            /// <summary>
            /// Gets the identifier of this button.
            /// </summary>
            /// <value>The identifier.</value>
            public string Id
            {
                get { return id; }
            }

            /// <summary>
            /// Gets or sets the button title.
            /// </summary>
            /// <value>The title.</value>
            public string Title
            {
                get { return title; }
                set { title = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this button is interactable.
            /// Default value is <c>true</c>.
            /// </summary>
            /// <value><c>true</c> if this button is interactable; otherwise, <c>false</c>.</value>
            public bool IsInteractable
            {
                get { return interactable; }
                set { interactable = value; }
            }

            /// <summary>
            /// Gets or sets the color of the button title when <see cref="IsInteractable"/> is <c>true</c>.
            /// </summary>
            /// <value>The color of the title.</value>
            public Color TitleColor
            {
                get { return titleColor; }
                set { titleColor = value; }
            }

            /// <summary>
            /// Gets or sets the color of the button title when <see cref="IsInteractable"/> is <c>false</c>.
            /// </summary>
            /// <value>The color of the disabled title.</value>
            public Color DisabledTitleColor
            {
                get { return uninteractableTitleColor; }
                set { uninteractableTitleColor = value; }
            }

            /// <summary>
            /// Gets or sets the color of the button body when <see cref="IsInteractable"/> is <c>true</c>.
            /// </summary>
            /// <value>The color of the body.</value>
            public Color BodyColor
            {
                get { return backgroundColor; }
                set { backgroundColor = value; }
            }

            /// <summary>
            /// Gets or sets the color of the button body when <see cref="IsInteractable"/> is <c>false</c>.
            /// </summary>
            /// <value>The color of the disabled body.</value>
            public Color DisabledBodyColor
            {
                get { return uninteractableBackgroundColor; }
                set { uninteractableBackgroundColor = value; }
            }

            [SerializeField]
            private string id;
            [SerializeField]
            private string title;
            [SerializeField]
            private bool interactable = true;
            [SerializeField]
            private Color titleColor = Color.white;
            [SerializeField]
            private Color backgroundColor = Color.blue;
            [SerializeField]
            private Color uninteractableTitleColor = Color.white;
            [SerializeField]
            private Color uninteractableBackgroundColor = Color.gray;

            public Button(string id)
            {
                this.id = id;
            }

            /// <summary>
            /// Return <see cref="TitleColor"/> if <see cref="IsInteractable"/> is <c>true</c>,
            /// otherwise return <see cref="DisabledTitleColor"/>.
            /// </summary>
            public Color GetCurrentTitleColor()
            {
                return IsInteractable ? TitleColor : DisabledTitleColor;
            }

            /// <summary>
            /// Return <see cref="BodyColor"/> if <see cref="IsInteractable"/> is <c>true</c>,
            /// otherwise return <see cref="DisabledBodyColor"/>.
            /// </summary>
            public Color GetCurrentBodyColor()
            {
                return IsInteractable ? BodyColor : DisabledBodyColor;
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }
        }

        /// <summary>
        /// This class represents the results when a consent dialog is completed.
        /// </summary>
        public class CompletedResults
        {
            /// <summary>
            /// The identifier of the selected button.
            /// </summary>
            public string buttonId;

            /// <summary>
            /// The values of the toggles in the dialog. This can be null if the dialog
            /// doesn't have any toggle.
            /// </summary>
            public Dictionary<string, bool> toggleValues;
        }

        #endregion  // Inner Classes

        #region Public API

        // Full pattern </EM_CONSENT_TOGGLE. Id = TOGGLE_ID_HERE>
        public const string TogglePattern = "</EM_CONSENT_TOGGLE. ";
        // Full pattern </EM_CONSENT_TOGGLE. Id = BUTTON_ID_HERE>
        public const string ButtonPattern = "</EM_CONSENT_BUTTON. ";
        public const string ToggleSearchPattern = "</EM_CONSENT_TOGGLE. Id = (.*?)>";
        public const string ButtonSearchPattern = "</EM_CONSENT_BUTTON. Id = (.*?)>";
        public const string UrlPattern = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)";

        /// <summary>
        /// The consent dialog being shown if any. 
        /// Returns null if no dialog is shown at the moment.
        /// </summary>
        /// <value>The active dialog.</value>
        public static ConsentDialog ActiveDialog { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this dialog is showing.
        /// </summary>
        /// <value><c>true</c> if this dialog is showing; otherwise, <c>false</c>.</value>
        public bool IsShowing
        {
            get { return ActiveDialog == this; }
        }

        /// <summary>
        /// The dialog title.
        /// </summary>
        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; }
        }

        /// <summary>
        /// The HTML-tagged text used to construct the main body of the dialog.
        /// </summary>
        public string Content
        {
            get { return mContent; }
            set { mContent = value; }
        }

        /// <summary>
        /// Array containing the toogles to be inserted into the dialog.
        /// </summary>
        public Toggle[] Toggles
        {
            get { return mToggles; }
            set { mToggles = value; }
        }

        /// <summary>
        /// Array containing the action buttons to be inserted into the dialog.
        /// Each dialog must have at least one button or the <see cref="Completed"/>
        /// event will never be fired, and the dialog can't be closed unless it is
        /// shown with the dismissible option being <c>true</c>.
        /// </summary>
        public Button[] ActionButtons
        {
            get { return mActionButtons; }
            set { mActionButtons = value; }
        }

        [SerializeField]
        private string mContent;

        [SerializeField][Rename("Title")]
        private string mTitle = "Privacy Consent";

        [SerializeField][Rename("Toggles")]
        private Toggle[] mToggles;

        [SerializeField][Rename("Action Buttons")]
        private Button[] mActionButtons;

        /// <summary>
        /// The 1st param is the dialog being completed.
        /// The 2nd param is the results of the dialog.
        /// </summary>
        public delegate void CompletedHandler(ConsentDialog dialog, CompletedResults results);

        /// <summary>
        /// The 1st param is the dialog whose toggle is being updated.
        /// The 2nd param is the ID of the toggle whose state is updated.
        /// The 3rd param is the new value of the toggle.
        /// </summary>
        public delegate void ToggleStateUpdatedHandler(ConsentDialog dialog, string toggleId, bool isOn);

        /// <summary>
        /// Occurs when the state of any toggle in the dialog is changed.
        /// This event is always raised on main thread.
        /// </summary>
        public event ToggleStateUpdatedHandler ToggleStateUpdated;

        /// <summary>
        /// Occurs when the dialog is completed, which means it is closed by the
        /// user clicking an action button inside the dialog.
        /// This event is always raised on main thread.
        /// </summary>
        public event CompletedHandler Completed;

        /// <summary>
        /// Occurs when the dialog is dismissed, which means it is closed by the
        /// user clicking the default 'Cancel' button on iOS or the 'Back' button on Android.
        /// This event is always raised on main thread.
        /// </summary>
        public event Action<ConsentDialog> Dismissed;

        // The common platform-specific dialog access point.
        private static IPlatformConsentDialog PlatformDialog
        {
            get
            {
                if (sPlatformDialog == null)
                {
                    #if UNITY_EDITOR
                    sPlatformDialog = new EditorConsentDialog();
                    #elif UNITY_IOS
                    sPlatformDialog = iOSConsentDialog.Instance;
                    #elif UNITY_ANDROID
                    sPlatformDialog = AndroidConsentDialog.Instance;
                    #else
                    sPlatformDialog = new UnsupportedConsentDialog();
                    #endif
                }

                return sPlatformDialog;
            }
        }

        private static IPlatformConsentDialog sPlatformDialog = null;

        public ConsentDialog()
        {
            mContent = string.Empty;
            mToggles = new Toggle[] { };
            mActionButtons = new Button[] { };
        }

        /// <summary>
        /// Determines if any consent dialog is being shown.
        /// </summary>
        /// <returns><c>true</c> if is showing any dialog; otherwise, <c>false</c>.</returns>
        public static bool IsShowingAnyDialog()
        {
            return ActiveDialog != null;
        }

        /// <summary>
        /// Show the consent dialog.
        /// </summary>
        /// <param name="isDismissible">If set to <c>true</c> the dialog can be dismissed by the user. 
        /// On iOS it will have a system-provided 'Cancel' button, while on Android it can be
        /// dismissed using the 'Back' button. Never enable this if the user consent is required
        /// (an explicit action needs to be made by the user). Default value is <c>false</c>.</param>
        public void Show(bool isDismissible = false)
        {
            if (ActiveDialog == null)
            {
                AssignAsActiveDialog();
                PlatformDialog.Show(Title, new ConsentDialogContentSerializer(this).SerializedContent, isDismissible);
            }
            else
            {
                Debug.Log("Another consent dialog is being shown. Ignoring this call.");
            }
        }

        /// <summary>
        /// Appends the text to the end of the main content (see <see cref="Content"/>).
        /// </summary>
        /// <param name="text">Text.</param>
        public void AppendText(string text)
        {
            Content += text;
        }

        /// <summary>
        /// Appends the toggle to the end of the main content (see <see cref="Content"/>).
        /// The toggle will also be added to the <see cref="Toggles"/> array if it is not
        /// included in that array yet.
        /// </summary>
        /// <param name="toggle">Toggle.</param>
        public void AppendToggle(Toggle toggle)
        {
            if (toggle == null)
                return;

            // Append the toggle-represent text into the main content.
            string element = TogglePattern + "Id = " + toggle.Id + ">";
            AppendText(element);

            // Add the toggle into the array if needed.
            if (!Toggles.Contains(toggle))
            {
                var list = new List<Toggle>(Toggles);
                list.Add(toggle);
                Toggles = list.ToArray();
            }
        }

        /// <summary>
        /// Appends the button to the end of the main content (see <see cref="Content"/>).
        /// The button will also be added to the <see cref="ActionButtons"/> array if it is not
        /// included in that array yet.
        /// </summary>
        /// <param name="button">Button.</param>
        public void AppendButton(Button button)
        {
            if (button == null)
                return;

            // Append the button-represent text into the main content.
            string element = ButtonPattern + "Id = " + button.Id + ">";
            AppendText(element);

            // Add the button into the array if needed.
            if (!ActionButtons.Contains(button))
            {
                var list = new List<Button>(ActionButtons);
                list.Add(button);
                ActionButtons = list.ToArray();
            }
        }

        /// <summary>
        /// Removes the toggle with the specified ID from the dialog if any.
        /// </summary>
        /// <param name="toggleId">Toggle identifier.</param>
        public void RemoveToggle(string toggleId)
        {
            var list = new List<Toggle>(Toggles);
            list.RemoveAll(t => t.Id == toggleId);
            Toggles = list.ToArray();
        }

        /// <summary>
        /// Removes the button with the specified ID from the dialog if any.
        /// </summary>
        /// <param name="buttonId">Button identifier.</param>
        public void RemoveButton(string buttonId)
        {
            var list = new List<Button>(ActionButtons);
            list.RemoveAll(b => b.Id == buttonId);
            ActionButtons = list.ToArray();
        }

        /// <summary>
        /// Returns the toggle with the given ID if one exists in the <see cref="Toggles"/> array.
        /// </summary>
        /// <returns>The toggle with identifier.</returns>
        /// <param name="id">Identifier.</param>
        public Toggle FindToggleWithId(string id)
        {
            if (id == null || Toggles == null)
                return default(Toggle);

            return Toggles.Where(toggle => toggle.Id.Equals(id)).FirstOrDefault();
        }

        /// <summary>
        /// Returns the button with the give ID if one exists in the <see cref="Buttons"/> array.
        /// </summary>
        /// <returns>The button with identifier.</returns>
        /// <param name="id">Identifier.</param>
        public Button FindButtonWithId(string id)
        {
            if (id == null || ActionButtons == null)
                return default(Button);

            return ActionButtons.Where(button => button.Id.Equals(id)).FirstOrDefault();
        }

        /// <summary>
        /// Sets the interactable state of the button with the specified ID.
        /// Use this to update the button's interactable setting while the dialog is showing.
        /// </summary>
        /// <param name="buttonId">Button identifier.</param>
        /// <param name="interactable">If set to <c>true</c> interactable.</param>
        public void SetButtonInteractable(string buttonId, bool interactable)
        {
            Button button = FindButtonWithId(buttonId);

            if (button != null)
            {
                button.IsInteractable = interactable;

                // Make sure we're the active dialog since we don't want
                // to mess with the settings of another dialog that is the active one.
                if (this.IsShowing)
                    PlatformDialog.SetButtonInteractable(buttonId, interactable);
            }
        }

        /// <summary>
        /// Sets the interactable state of the toggle with the specified ID.
        /// Use this to update the toggle's interactable setting while the dialog is showing.
        /// </summary>
        /// <param name="toggleId">Toggle identifier.</param>
        /// <param name="interactable">If set to <c>true</c> interactable.</param>
        public void SetToggleInteractable(string toggleId, bool interactable)
        {
            Toggle toggle = FindToggleWithId(toggleId);

            if (toggle != null)
            {
                toggle.IsInteractable = interactable;

                // Only talk to the native dialog if we're the active one.
                if (this.IsShowing)
                    PlatformDialog.SetToggleInteractable(toggleId, interactable);
            }
        }

        /// <summary>
        /// Sets the value of toggle with the specified ID.
        /// Use this to update the toggle state from script while the dialog is showing.
        /// </summary>
        /// <param name="toggleId">Toggle identifier.</param>
        /// <param name="isOn">If set to <c>true</c> turn on the toggle.</param>
        /// <param name="animated">If set to <c>true</c> animate the transition.</param>
        public void SetToggleIsOn(string toggleId, bool isOn, bool animated = true)
        {
            // Only talk to the native dialog if we're the active one.
            // Once the native toggle's value is updated, our event
            // handler will automatically update the counterpart toggle on managed side.
            if (this.IsShowing)
                PlatformDialog.SetToggleIsOn(toggleId, isOn, animated);
        }

        /// <summary>
        /// Find all the urls that can be found in <see cref="Content"/>.
        /// </summary>
        public IEnumerable<string> GetAllUrlsInContent()
        {
            var match = new Regex(UrlPattern);
            var matches = match.Matches(Content);
            foreach (Match url in matches)
            {
                yield return url.Value;
            }
        }

        public List<string> GetSplittedContents()
        {
            List<string> splitedContents = new List<string>();
            List<string> allPatterns = GetAllPatternsInContent();

            /// If we couldn't find any pattern in the content,
            /// we don't need to split it so we return the whole content here.
            if (allPatterns == null || allPatterns.Count < 1)
                return new List<string> { Content };

            string remainedContent = Content;
            for (int i = 0; i < allPatterns.Count; i++)
            {
                string pattern = allPatterns[i];
                string[] splited = remainedContent.Split(new string[] { pattern }, 2, StringSplitOptions.None);
                string firstPart = splited[0];
                string secondPart = splited[1];

                if (!string.IsNullOrEmpty(firstPart))
                    splitedContents.Add(firstPart);
                    
                splitedContents.Add(pattern);
                remainedContent = secondPart;          
            }

            /// If the end of the dialog is a toggle or button pattern, "remainedContent" will be an empty string, so we ignore it.
            /// Otherwise it's gonna be a valid plain text so we have to include it in splited result.
            if (!string.IsNullOrEmpty(remainedContent))
                splitedContents.Add(remainedContent);

            return splitedContents;
        }

        /// <summary>
        /// Get all defined toggle ids.
        /// </summary>
        public List<string> GetAllToggleIds()
        {
            if (Toggles == null)
                return null;

            return Toggles.Select(toggle => toggle.Id).ToList();
        }

        /// <summary>
        /// Get all defined button ids.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllButtonIds()
        {
            if (ActionButtons == null)
                return null;

            return ActionButtons.Select(button => button.Id).ToList();
        }

        /// <summary>
        /// Check if a string is a <see cref="TogglePattern"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsTogglePattern(string source)
        {
            return string.IsNullOrEmpty(source) ? false : Regex.IsMatch(source, ToggleSearchPattern);
        }

        /// <summary>
        /// Check if a string is a <see cref="ButtonPattern"/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsButtonPattern(string source)
        {
            return string.IsNullOrEmpty(source) ? false : Regex.IsMatch(source, ButtonSearchPattern);
        }

        /// <summary>
        /// Return the id in a toggle pattern if available.
        /// </summary>
        /// For example, if the input string is "</EM_CONSENT_TOGGLE. Id = 0002>", this method will return "0002".
        public static string SearchForIdInTogglePattern(string source)
        {
            return Regex.Match(source, ToggleSearchPattern).Groups[1].Value;
        }

        /// <summary>
        /// Return the id in a button pattern if available.
        /// </summary>
        /// For example, if the input string is "</EM_CONSENT_BUTTON. Id = 0002>", this method will return "0002".
        public static string SearchForIdInButtonPattern(string source)
        {
            return Regex.Match(source, ButtonSearchPattern).Groups[1].Value;
        }

        #endregion // Public API

        #region Private Methods

        /// <summary>
        /// All patterns that can be found in dialog content, from top to bottom;
        /// </summary>
        private List<string> GetAllPatternsInContent()
        {
            /// Find all patterns in dialog content, remove duplicated values.
            HashSet<string> allPatterns = new HashSet<string>();
            SearchPatternInContent(ButtonSearchPattern).ForEach(pattern => allPatterns.Add(pattern));
            SearchPatternInContent(ToggleSearchPattern).ForEach(pattern => allPatterns.Add(pattern));

            /// Find indexes of them in the dialog content.
            var resultDict = new Dictionary<int, string>();
            foreach (string pattern in allPatterns)
            {
                foreach (var item in IndexesOf(Content, pattern))
                {
                    resultDict.Add(item.Key, item.Value);
                }
            }

            /// Sort the result and return, new we have all the pattern in the dialog, ordered from top to bottom.
            return resultDict.OrderBy(item => item.Key).Select(item => item.Value).ToList();
        }

        /// <summary>
        /// Find all indexes of <paramref name="value"/> in <paramref name="source"/>
        /// </summary>
        private Dictionary<int, string> IndexesOf(string source, string value)
        {
            var indexesAndValues = new Dictionary<int, string>();
            value = Regex.Escape(value);
            foreach (Match match in Regex.Matches(source, value))
            {
                indexesAndValues.Add(match.Index, match.Value);
            }
            return indexesAndValues;
        }

        /// <summary>
        /// Search for a specific pattern in dialog content.
        /// </summary>
        private List<string> SearchPatternInContent(string regexPattern)
        {
            List<string> patterns = new List<string>();

            if (string.IsNullOrEmpty(Content))
                return patterns;

            Regex regex = new Regex(regexPattern);
            var matches = regex.Matches(Content);

            foreach (Match match in matches)
            {
                patterns.Add(match.ToString());
            }

            return patterns;
        }

        private void OnNativeToggleStateUpdated(IPlatformConsentDialog platformDialog, string toggleId, bool isOn)
        {
            // Update the state of the managed counterpart.
            var toggle = FindToggleWithId(toggleId);

            if (toggle != null)
                toggle.IsOn = isOn;

            // Fire event.
            if (ToggleStateUpdated != null)
                ToggleStateUpdated(this, toggleId, isOn);
        }

        private void OnNativeDialogCompleted(IPlatformConsentDialog platformDialog, string buttonId, Dictionary<string, bool> toggles)
        {
            var results = new CompletedResults();
            results.buttonId = buttonId;
            results.toggleValues = toggles;
            
            if (Completed != null)
                Completed(this, results);

            ResignAsActiveDialog();
        }

        private void OnNativeDialogDismissed(IPlatformConsentDialog platformDialog)
        {
            if (Dismissed != null)
                Dismissed(this);

            ResignAsActiveDialog();
        }

        // Explicitly subscribe to platform-specific dialog's events and forward them to outside world.
        private void AssignAsActiveDialog()
        {
            if (ActiveDialog != this)
            {
                ActiveDialog = this;
                PlatformDialog.ToggleStateUpdated += OnNativeToggleStateUpdated;
                PlatformDialog.Completed += OnNativeDialogCompleted;
                PlatformDialog.Dismissed += OnNativeDialogDismissed;
            }
        }

        // Unsubscribe to platform-specific dialog's events.
        private void ResignAsActiveDialog()
        {
            if (ActiveDialog == this)
            {
                ActiveDialog = null;
                PlatformDialog.ToggleStateUpdated -= OnNativeToggleStateUpdated;
                PlatformDialog.Completed -= OnNativeDialogCompleted;
                PlatformDialog.Dismissed -= OnNativeDialogDismissed;
            }
        }

        #endregion // Private Methods
    }
}
