using System;
using System.Text;
using log4net;

namespace Toems_Service
{
    public class StringManipulationServices
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StringManipulationServices));

        public static string Decode(string encoded, string parameter)
        {
            string decoded = null;
            try
            {
                var dBytes = Convert.FromBase64String(encoded);
                decoded = Encoding.UTF8.GetString(dBytes);
            }
            catch (Exception ex)
            {
                log.Error(parameter + " Base64 Decoding Failed. " + ex.Message);
            }

            return decoded;
        }

        public static string Encode(string decoded)
        {
            string encoded = null;
            try
            {
                var bytes = Encoding.UTF8.GetBytes(decoded);
                encoded = Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                log.Error("Base64 Encoding Failed. " + ex.Message);
            }

            return encoded;
        }

        public static string EscapeCharacter(string str, string[] charArray)
        {
            string escapedString = null;
            foreach (var c in charArray)
            {
                escapedString = str.Replace(c, "\\" + c);
                str = escapedString;
            }
            return escapedString;
        }

      

      

      

     
    }
}