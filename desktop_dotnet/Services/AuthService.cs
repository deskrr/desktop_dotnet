using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop_dotnet.Services
{
    struct UserInfo
    {
        string firstName;
        string lastName;
        string email;
    }

    public static class AuthService
    {
        public static bool isLoggedIn { get; set; }
        public static string currentUser { get; set; }
        public static string authToken { get; set; }

        public static void InitAuth()
        {
            // check auth
        }

        static void loadToken()
        {
            string token = Properties.Settings.Default.Token;
        }
    }
}
