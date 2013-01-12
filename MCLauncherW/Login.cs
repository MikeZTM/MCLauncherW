using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net;
using System.IO;

namespace MCLauncherW
{
    class Login
    {
        public static String login(String username, String password)
        {
            String result="";
            try
            {
                string url = "https://login.minecraft.net/?user=" + username + "&password=" + password + "&version=14";

                WebClient client = new WebClient();

                // Add a user agent header in case the  
                // requested URI contains a query.

                Stream data = client.OpenRead(url);
                StreamReader reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                Console.WriteLine(s);
                data.Close();
                reader.Close();

                string[] loginResult = s.Split(':');
                result = "\"" + loginResult[2] + "\" \"" + loginResult[3] + "\"";
            }
            catch (Exception)
            {

            }
            return result;
        }
    }
}