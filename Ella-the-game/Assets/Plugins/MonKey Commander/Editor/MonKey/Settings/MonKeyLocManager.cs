using UnityEngine;
using MonKey.Extensions;
using MonKey.Settings.Internal;

namespace MonKey.Internal
{
    public  static class MonKeyLocManager 
    {

     //   private static readonly string LANGUAGE_KEY_ID = "QCOMM_LOC";

       /* private static string currentLanguageKey;
        private static int currentLanguageID;*/

        //private static readonly List<string> Languages = new List<string>();
      //  private static Dictionary<string, LocalizationFile> localizationFiles;



        public static MonkeyLocalizationFile DefaultLocalization => MonkeyLocalizationFile.Instance;
     
        public static MonkeyLocalizationFile CurrentLoc => DefaultLocalization;
    }
}
