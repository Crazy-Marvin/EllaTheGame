namespace EasyMobile
{
    public enum AutoAdLoadingMode
    {
        None = 0,

        /// <summary>
        /// Only loads default ads of default networks.
        /// </summary>
        LoadDefaultAds,

        /// <summary>
        /// Automatically loads all ads at defined placements. Defined placements include
        /// default and custom placements registered in the module settings (with valid IDs
        /// where applicable), and whose network SDKs were imported.
        /// </summary>
        LoadAllDefinedPlacements,
    }
}
