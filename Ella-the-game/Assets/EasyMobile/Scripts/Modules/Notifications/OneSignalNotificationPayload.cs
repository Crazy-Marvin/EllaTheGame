using UnityEngine;
using System;
using System.Collections.Generic;
using EasyMobile.Internal;
using EasyMobile.MiniJSON;

namespace EasyMobile
{
    public class OneSignalNotificationPayload
    {
        public string notificationID;
        public string sound;
        public string title;
        public string body;
        public string subtitle;
        public string launchURL;
        public Dictionary<string, object> additionalData;
        public Dictionary<string, object> actionButtons;
        public bool contentAvailable;
        public int badge;
        public string smallIcon;
        public string largeIcon;
        public string bigPicture;
        public string smallIconAccentColor;
        public string ledColor;
        public int lockScreenVisibility = 1;
        public string groupKey;
        public string groupMessage;
        public string fromProjectNumber;

        // Construct the payload from a dictionary (converted from OneSignal's JSON payload format).
        public static OneSignalNotificationPayload FromJSONDict(string id, Dictionary<string, object> jsonDict)
        {
            /****************************************************************
             * Build the payload object from OneSignal's JSON payload format.
             * Sample OneSignal JSON payload:
             * 
             {
                  "att":
                  {
                      "id":"media-path"
                  },

                  "buttons":
                      [{
                          "i":"btn_test","n":"Test"
                      },
                      {
                          "i":"btn_forget","n":"Forget"
                      }],
                      
                  "custom":
                  {
                      "ti":"0c235fdb-ff6f-4e65-802c-f26b534dxxxx",
                      "i":"5e488f9c-bee4-484d-9e98-074d5393xxxx",
                      "u":"http://google.com",
                      "tn":"Test_Notification",
                      "a":
                      {
                          "newUpdate":"true"
                      }
                  },

                  "aps":
                  {
                      "sound":"default",
                      "content-available":1,
                      "alert":
                      {
                          "subtitle":"test-subtitle",
                          "title":"Test",
                          "body":"Testing notification from OneSignal!"
                      },
                      "mutable-content":1,
                      "category":"test-ios-notif-category",
                      "badge":1
                  }
                }
             *
             ****************************************************************/

            if (jsonDict == null || jsonDict.Count == 0)
                return null;

            var payload = new OneSignalNotificationPayload();

            payload.notificationID = id;

            if (jsonDict.ContainsKey("aps"))
            {
                var aps = jsonDict["aps"] as Dictionary<string, object>;

                if (aps.ContainsKey("sound"))
                    payload.sound = aps["sound"] as string;
                if (aps.ContainsKey("content-available"))
                    payload.contentAvailable = Convert.ToInt32(aps["content-available"]) != 0;

                if (aps.ContainsKey("alert"))
                {
                    var alert = aps["alert"] as Dictionary<string, object>;

                    if (alert.ContainsKey("title"))
                        payload.title = alert["title"] as string;
                    if (alert.ContainsKey("body"))
                        payload.body = alert["body"] as string;
                    if (alert.ContainsKey("subtitle"))
                        payload.subtitle = alert["subtitle"] as string;
                }

                if (aps.ContainsKey("badge"))
                    payload.badge = Convert.ToInt32(aps["badge"]);
            }

            if (jsonDict.ContainsKey("custom"))
            {
                var custom = jsonDict["custom"] as Dictionary<string, object>;

                if (custom != null)
                {
                    // launchURL as "u"
                    if (custom.ContainsKey("u"))
                        payload.launchURL = custom["u"] as string;  

                    // additionalData as "a"
                    if (custom.ContainsKey("a"))
                        payload.additionalData = custom["a"] as Dictionary<string, object>;
                }
            }

            if (jsonDict.ContainsKey("buttons"))
            {
                // Buttons comes as array deserialized as a list.
                var buttons = jsonDict["buttons"] as List<object>;

                if (buttons != null)
                {
                    payload.actionButtons = new Dictionary<string, object>();

                    foreach (var btn in buttons)
                    {
                        var btnDict = btn as Dictionary<string, object>;

                        if (btnDict != null)
                            payload.actionButtons.Add(btnDict["i"] as string, btnDict["n"]);    // id as "i", text as "n"
                    }
                }
            }

            return payload;
        }

        internal NotificationContent ToNotificationContent()
        {
            var content = new NotificationContent();
            content.title = this.title;
            content.subtitle = this.subtitle;
            content.body = this.body;
            content.badge = this.badge;
            content.userInfo = this.additionalData;
            content.smallIcon = this.smallIcon;
            content.largeIcon = this.largeIcon;

            return content;
        }
    }
}

