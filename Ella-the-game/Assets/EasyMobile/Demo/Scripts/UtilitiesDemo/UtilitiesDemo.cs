using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EasyMobile;

namespace EasyMobile.Demo
{
    public class UtilitiesDemo : MonoBehaviour
    {
        public GameObject ignoreConstraints;
        public GameObject isDisabled;
        public GameObject annualRemainingRequests;
        public GameObject delayAfterInstallRemainingTime;
        public GameObject coolingOffRemainingTime;
        public DemoUtils demoUtils;

        void Awake()
        {
            // Init EM runtime if needed (useful in case only this scene is built).
            if (!RuntimeManager.IsInitialized())
                RuntimeManager.Init();
        }

        void Update()
        {
            // if (StoreReview.IsDisplayConstraintIgnored())
            //     demoUtils.DisplayBool(ignoreConstraints, true, "Ignore Constraints: TRUE");
            // else
            //     demoUtils.DisplayBool(ignoreConstraints, false, "Ignore Constraints: FALSE");

            // if (!StoreReview.IsRatingRequestDisabled())
            //     demoUtils.DisplayBool(isDisabled, true, "Popup Enabled");
            // else
            //     demoUtils.DisplayBool(isDisabled, false, "Popup Disabled");

            // int remainingRequests = StoreReview.GetThisYearRemainingRequests();
            // int remainingDelayAfterInstallation = StoreReview.GetRemainingDelayAfterInstallation();
            // int remainingCoolingOff = StoreReview.GetRemainingCoolingOffDays();

            // demoUtils.DisplayBool(annualRemainingRequests, remainingRequests > 0, "This Year Remaining Requests: " + remainingRequests);
            // demoUtils.DisplayBool(delayAfterInstallRemainingTime, remainingDelayAfterInstallation <= 0, "Delay-After-Installation Remaining Days: " + remainingDelayAfterInstallation);
            // demoUtils.DisplayBool(coolingOffRemainingTime, remainingCoolingOff <= 0, "Cooling-Off Remaining Days: " + remainingCoolingOff);
        }

        public void RequestRating()
        {
            StoreReview.RequestRating();
            // if (StoreReview.CanRequestRating())
            //     StoreReview.RequestRating();
            // else
            //     NativeUI.Alert("Alert", "The rating popup could not be shown because it was disabled or the display constraints are not satisfied.");
        }

        public void RequestRatingLocalized()
        {
            // if (!StoreReview.CanRequestRating())
            // {
            //     NativeUI.Alert("Alert", "The rating popup could not be shown because it was disabled or the display constraints are not satisfied.");
            //     return;
            // }

            // // For demo purpose, we translated the default content into French using Google Translate!
            // var localized = new RatingDialogContent(
            //                     "Évaluation " + RatingDialogContent.PRODUCT_NAME_PLACEHOLDER,
            //                     "Comment évalueriez-vous " + RatingDialogContent.PRODUCT_NAME_PLACEHOLDER + "?",
            //                     "C'est mauvais. Souhaitez-vous nous donner quelques commentaires à la place?",
            //                     "Impressionnant! Faisons le!",
            //                     "Pas maintenant",
            //                     "Non",
            //                     "Évaluez maintenant!",
            //                     "Annuler",
            //                     "Réaction"
            //                 );


            // StoreReview.RequestRating(localized);
        }
    }
}
