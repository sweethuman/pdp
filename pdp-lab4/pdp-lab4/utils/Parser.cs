using System;

namespace pdp_lab4.utils
{
    public class Parser
    {
        public static int PORT = 80; // http default port

        public static string GetRequestString(string hostname, string endpoint)
        {
            return "GET " + endpoint + " HTTP/1.1\r\n" +
                   "Host: " + hostname + "\r\n" +
                   "Content-Length: 0\r\n\r\n";
        }

        public static int GetContentLen(string respContent)
        {
            var contentLen = 0;
            var separators = new[] { '\r', '\n' };
            var respLines = respContent.Split(separators);
            foreach (string respLine in respLines)
            {
                var headDetails = respLine.Split(':');

                if (string.Compare(headDetails[0], "Content-Length", StringComparison.Ordinal) == 0)
                {
                    contentLen = int.Parse(headDetails[1]);
                }
            }

            return contentLen;
        }

        public static bool ResponseHeaderObtained(string responseContent)
        {
            return responseContent.Contains("\r\n\r\n");
        }
    }
}
