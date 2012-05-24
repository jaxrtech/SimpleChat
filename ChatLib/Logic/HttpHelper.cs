using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace ChatLib
{
    public class HttpHelper
    {
        public static string Get(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 5000;
                request.Method = "GET";

                WebResponse responce = request.GetResponse();
                StreamReader reader = new StreamReader(responce.GetResponseStream(), Encoding.UTF8);
                string content = reader.ReadToEnd();
                reader.Close();
                responce.Close();

                return content;
            }
            catch
            {
                return null;
            }
        }
    }
}
