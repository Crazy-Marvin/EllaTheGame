using UnityEngine;
using System.Collections;
using EasyMobile.Internal;

namespace EasyMobile
{
    [System.Serializable]
    public class RatingDialogContent
    {
        // Placeholders for replacable strings.
        public const string PRODUCT_NAME_PLACEHOLDER = "$PRODUCT_NAME";

        public readonly static RatingDialogContent Default = new RatingDialogContent();

        public string Title
        { 
            get { return mTitle; }
            set { mTitle = value; }
        }

        public string Message
        { 
            get { return mMessage; }
            set { mMessage = value; }
        }

        public string LowRatingMessage
        { 
            get { return mLowRatingMessage; }
            set { mLowRatingMessage = value; }
        }

        public string HighRatingMessage
        { 
            get { return mHighRatingMessage; }
            set { mHighRatingMessage = value; }
        }

        public string PostponeButtonText
        {
            get { return mPostponeButtonText; } 
            set { mPostponeButtonText = value; }
        }

        public string RefuseButtonText
        { 
            get { return mRefuseButtonText; }
            set { mRefuseButtonText = value; }
        }

        public string RateButtonText
        { 
            get { return mRateButtonText; }
            set { mRateButtonText = value; }
        }

        public string CancelButtonText
        { 
            get { return mCancelButtonText; }
            set { mCancelButtonText = value; }
        }

        public string FeedbackButtonText
        { 
            get { return mFeedbackButtonText; } 
            set { mFeedbackButtonText = value; }
        }

        [SerializeField][Rename("Title")]
        private string mTitle = "Rate " + PRODUCT_NAME_PLACEHOLDER;
        [SerializeField][Rename("Message")]
        private string mMessage = "How would you rate " + PRODUCT_NAME_PLACEHOLDER + "?";
        [SerializeField][Rename("Low Rating Message")]
        private string mLowRatingMessage = "That's bad. Would you like to give us some feedback instead?";
        [SerializeField][Rename("High Rating Message")]
        private string mHighRatingMessage = "Awesome! Let's do it!";
        [SerializeField][Rename("Postpone Button Title")]
        private string mPostponeButtonText = "Not Now";
        [SerializeField][Rename("Refuse Button Title")]
        private string mRefuseButtonText = "Don't Ask Again";
        [SerializeField][Rename("Rate Button Title")]
        private string mRateButtonText = "Rate Now!";
        [SerializeField][Rename("Cancel Button Title")]
        private string mCancelButtonText = "Cancel";
        [SerializeField][Rename("Feedback Button Title")]
        private string mFeedbackButtonText = "Send Feedback";

        public RatingDialogContent()
        {
        }

        public RatingDialogContent(
            string title,
            string message,
            string lowRatingMessage,
            string highRatingMessage,
            string postponeButtonText,
            string refuseButtonText,
            string rateButtonText,
            string cancelButtonText,
            string feedbackButtonText)
        {
            this.mTitle = title == null ? "" : title;
            this.mMessage = message == null ? "" : message;
            this.mLowRatingMessage = lowRatingMessage == null ? "" : lowRatingMessage;
            this.mHighRatingMessage = highRatingMessage == null ? "" : highRatingMessage;
            this.mPostponeButtonText = postponeButtonText == null ? "" : postponeButtonText;
            this.mRefuseButtonText = refuseButtonText == null ? "" : refuseButtonText;
            this.mRateButtonText = rateButtonText == null ? "" : rateButtonText;
            this.mCancelButtonText = cancelButtonText == null ? "" : cancelButtonText;
            this.mFeedbackButtonText = feedbackButtonText == null ? "" : feedbackButtonText;
        }
    }
}

