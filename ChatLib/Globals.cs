using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib
{
    public static class Globals
    {
        public const int Port = 18500;
        private const string AlternatePortUrl = "http://dl.dropbox.com/u/18952769/ClientChat/ports";

        /// <summary>
        /// Gets the alternate port from the web
        /// </summary>
        /// <returns>The port number unless it was unable to get it which then it will return -1</returns>
        public static int GetAlternatePort()
        {
            int port = -1;
            string text = HttpHelper.Get(AlternatePortUrl);
            // Split by line, get the first, and parse
            string[] ports = text.Split(@"\n".ToCharArray());
            string portLine = ports[0];
            int.TryParse(portLine, out port);

            return port;
        }
    }
}
