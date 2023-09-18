using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Extensions
{
    public static class HttpContextExtension
    {
        private static string RequestOriginApp = "App";
        private static string RequestOriginWeb = "Web";
        private static string RequestOrigin = "request-origin";

        public static string GetBodyPatch(this HttpContext httpContext)
        {
            string body = null;

            if (HttpMethods.IsPatch(httpContext.Request.Method))
            {
                body = (string)httpContext.Items[$"Body{HttpMethods.Patch}"];
            }

            return body;
        }

        public static bool IsRequestOriginApp(this HttpContext httpContext)
        {
            return httpContext?.Request?.Headers[RequestOrigin].ToString() == RequestOriginApp;
        }

        public static bool IsRequestOriginWeb(this HttpContext httpContext)
        {
            return httpContext?.Request?.Headers[RequestOrigin].ToString() == RequestOriginWeb;
        }
    }
}
