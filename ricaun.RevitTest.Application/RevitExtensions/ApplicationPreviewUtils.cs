namespace Autodesk.Revit.ApplicationServices
{
    /// <summary>
    /// ApplicationPreviewUtils
    /// </summary>
    public static class ApplicationPreviewUtils
    {
        /// <summary>
        /// Is PreviewRelease
        /// </summary>
        /// <returns></returns>
        public static bool IsPreviewRelease()
        {
            try
            {
                var type = typeof(Application);
                var method = type.GetMethod("IsPreviewRelease", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                if (method is null) return false;
                return (bool)method.Invoke(null, null);
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Is PreviewRelease and NotLoggedIn
        /// </summary>
        /// <returns></returns>
        public static bool IsPreviewReleaseNotLoggedIn()
        {
            return Application.IsLoggedIn == false && IsPreviewRelease();
        }
    }
}
