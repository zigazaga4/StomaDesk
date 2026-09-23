using System.Text;

namespace StomaDesk.Services
{
    /// <summary>Romanian phone numbers: 10 digits starting with 0; mobiles start with 07.</summary>
    public static class PhoneNumber
    {
        public static string Digits(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            var digits = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                if (c >= '0' && c <= '9')
                    digits.Append(c);
            }
            return digits.ToString();
        }

        /// <summary>"+40 722 123 456" and "0040722123456" both become "0722123456".</summary>
        public static string National(string text)
        {
            string digits = Digits(text);
            if (digits.StartsWith("0040"))
                return "0" + digits.Substring(4);
            if (digits.StartsWith("40") && digits.Length == 11)
                return "0" + digits.Substring(2);
            return digits;
        }

        public static bool IsValid(string text)
        {
            string national = National(text);
            return national.Length == 10 && national[0] == '0';
        }

        public static bool IsMobile(string text)
        {
            string national = National(text);
            return national.Length == 10 && national.StartsWith("07");
        }
    }
}
