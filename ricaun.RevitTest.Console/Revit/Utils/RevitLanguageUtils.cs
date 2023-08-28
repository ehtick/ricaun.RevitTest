using System.Collections.Generic;
using System.Linq;

namespace ricaun.RevitTest.Console.Revit.Utils
{
    /// <summary>
    /// RevitLanguageUtils
    /// </summary>
    public static class RevitLanguageUtils
    {
        /// <summary>
        /// Convert language to Revit language argument (default: "/language ENU")
        /// </summary>
        /// <param name="language">The language in Revit argument format or CultureInfo</param>
        /// <returns></returns>
        public static string GetArgument(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                language = "ENU";

            language = TryCultureToLanguage(language);

            return $"/language {language}";
        }

        public static string TryCultureToLanguage(string cultureName)
        {
            var similarCultureName = CultureNameLanguage.Keys.FirstOrDefault(e => e.StartsWith(cultureName));

            if (similarCultureName is not null)
                cultureName = similarCultureName;

            if (CultureNameLanguage.TryGetValue(cultureName, out var language))
            {
                return language;
            }

            return cultureName;
        }

        public static Dictionary<string, string> CultureNameLanguage = new Dictionary<string, string>
        {
            { "en-US", "ENU" },     // English - United States
            { "en-GB", "ENG" },     // English - United Kingdom
            { "fr-FR", "FRA" },     // French
            { "de-DE", "DEU" },     // German
            { "it-IT", "ITA" },     // Italian
            { "ja-JP", "JPN" },     // Japanese
            { "ko-KR", "KOR" },     // Korean
            { "pl-PL", "PLK" },     // Polish
            { "es-ES", "ESP" },     // Spanish
            { "zh-CH", "CHS" },     // Simplified Chinese
            { "zh-TW", "CHT" },     // Traditional Chinese
            { "pt-BR", "PTB" },     // Brazilian Portuguese
            { "ru-RU", "RUS" },     // Russian
            { "cs-CZ", "CSY" },     // Czech
        };
    }
}