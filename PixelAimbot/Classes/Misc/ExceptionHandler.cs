using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace PixelAimbot.Classes.Misc
{
    public static class ExceptionHandler
    {
        public static void SendException(Exception ex, string Methodname = "")
        {
            try
            {
                Debug.WriteLine(ex.ToString());
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                string filename = (new StackTrace(ex, true)).GetFrame(0).GetMethod().Name;
                string stacktrace = (new StackTrace(ex, true)).GetFrames().ToString();
                string message = ex.Message;

                var values = new NameValueCollection
                {
                    ["username"] = Config.Load().username,
                    ["filename"] = Config.version,
                    ["line"] = line.ToString(),
                    ["message"] = message,
                    ["stacktrace"] = ex.ToString()

                };
                using (var webClient = new WebClient())
                {
                    webClient.UploadValuesAsync(new Uri("https://admin.symbiotic.link/api/exception"), values);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static string GetCallForExceptionThisMethod(MethodBase methodBase, Exception e)
        {
            StackTrace trace = new StackTrace(e);
            StackFrame previousFrame = null;

            foreach (StackFrame frame in trace.GetFrames())
            {
                if (frame.GetMethod() == methodBase)
                {
                    break;
                }

                previousFrame = frame;
            }

            return previousFrame != null ? previousFrame.GetMethod().Name : null;
        }
    }
}