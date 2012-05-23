using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib.Protocol
{
    public class ProtocolParameter
    {
        #region Properties
        public enum Position
        {
            Start,
            Inside
        }

        public Position TextPosition { get; set; }

        private bool _PrefixEnabled = false;
        public bool PrefixEnabled
        {
            get { return _PrefixEnabled; }
            set { _PrefixEnabled = value; }
        }

        public const string Prefix = ":";

        public string Content { get; set; }
        #endregion

        public string Serialize()
        {
            string text = (PrefixEnabled) ? Prefix : string.Empty;
            text += Content;
            return text;
        }

        public bool Deserialize(string text)
        {
            if (PrefixEnabled) // Check for prefix
            {
                if (text.StartsWith(Prefix)) // Remove it if it is there
                { 
                    Content = text.TrimStart(Prefix.ToCharArray());
                    return true;
                }
                else return false;
            }
            else
            {
                Content = text;
                return true;
            }
        }
    }
}
