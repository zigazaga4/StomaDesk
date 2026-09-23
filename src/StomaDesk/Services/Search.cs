using System.Globalization;
using System.Text;

namespace StomaDesk.Services
{
    public static class Search
    {
        /// <summary>
        /// "Șerban Ștefăniță" becomes "Serban Stefanita". Works for both ș (comma below)
        /// and the older ş (cedilla) that many keyboards and old databases still produce.
        /// </summary>
        public static string StripDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            string decomposed = text.Normalize(NormalizationForm.FormD);
            var result = new StringBuilder(decomposed.Length);
            foreach (char c in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    result.Append(c);
            }
            return result.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>The form used to compare search text: no diacritics, lower case, trimmed.</summary>
        public static string Fold(string text)
        {
            return StripDiacritics(text).Trim().ToLowerInvariant();
        }
    }
}
