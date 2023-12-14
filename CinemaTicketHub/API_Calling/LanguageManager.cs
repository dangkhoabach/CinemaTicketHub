using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaTicketHub.API_Calling
{
    public class LanguageManager
    {
        private Dictionary<string, string> languageDictionary;

        public LanguageManager()
        {
            languageDictionary = new Dictionary<string, string>
        {
            { "en", "Tiếng Anh" },
    { "vi", "Tiếng Việt" },
    { "th", "Tiếng Thái" },
    { "fr", "Tiếng Pháp" },
    { "es", "Tiếng Tây Ban Nha" },
    { "de", "Tiếng Đức" },
    { "ja", "Tiếng Nhật" },
    { "it", "Tiếng Ý" },
    { "ru", "Tiếng Nga" },
    { "ko", "Tiếng Hàn" },
    { "ar", "Tiếng Ả Rập" },
    { "hi", "Tiếng Hin-đi" },
        };
        }

        public string GetLanguageName(string languageCode)
        {
            if (languageDictionary.ContainsKey(languageCode))
            {
                return languageDictionary[languageCode];
            }
            return languageCode; // Nếu không tìm thấy, trả về mã ngôn ngữ
        }
    }
}