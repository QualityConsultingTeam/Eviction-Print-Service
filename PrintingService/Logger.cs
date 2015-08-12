using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintingService
{
    public static class Logger
    {

        private static LogWriter logWriter;

        private static LogWriter GetLogger()
        {
            var writer = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();

            return writer;
        }


        private static LogEntry GetLogException(Exception ex, string customMessage)
        {

            var StackTrace = ex.StackTrace ?? " ";
            var InnerException = "";
            var innerExceptionStacktrace = "";
            if (ex.InnerException != null)
            {
                InnerException = ex.InnerException.ToString();
                if (ex.InnerException.StackTrace != null)
                    innerExceptionStacktrace = ex.InnerException.StackTrace.ToString() ?? " ";
            }

            return new LogEntry()
            {
                Title = string.Format("{0}", ex.Message),
                Message =
                    string.Format(
                        "Details : {0} StackTrace: {1}\n  InnerException: {2}\n InnerException Stack Trace: {3}\n",
                        customMessage, StackTrace, InnerException, innerExceptionStacktrace),

            };
        }

        public static void WriteLog(this Exception ex, string message = "")
        {
            using (var logger = GetLogger())
            {
                logger.Write(GetLogException(ex, message));
            }

        }

        public static void DebugInfo(string Message)
        {
#if DEBUG
            WriteLog(Message);
#endif
        }

        public static void WriteLog(string Message)
        {
            using (var logger = GetLogger())
            {
                logger.Write(new LogEntry()
                {
                    Message = Message,
                });
            }
        }

        //public static void WriteElapsedTime(this TimeSpan elapsedTime, int count)
        //{
        //    var builder = new StringBuilder();
        //    builder.Append(string.Format("Elapsed Time {0}, Items :{1}", elapsedTime.GetFormattedTime(), count));
        //    WriteLog(builder.ToString());
        //}

        public static void OnExceptionRaised(object sender, UnhandledExceptionEventArgs e)
        {
            // throw new NotImplementedException();
            var ex = e.ExceptionObject as Exception;

            WriteLog(ex, "Unhandled Exception:");
        }
    }
}
