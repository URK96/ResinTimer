using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Net;

namespace ResinTimer.Services
{
    public static class WebService
    {
        public static void Get(string uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
        }

        private static JsonDocument ToJson(string content)
        {
            try
            {
                return JsonDocument.Parse(content);
            }
            catch
            {
                return null;
            }
        }
    }
}
