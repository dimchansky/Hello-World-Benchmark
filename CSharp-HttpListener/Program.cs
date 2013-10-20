using System;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading;

namespace CSharpHello
{
    class Program
    {
        static void Main()
        {
            Threads();

            var listener = new HttpListener
            {
                // This doesn't seem to ignore all write exceptions, so in WriteResponse(), we still have a catch block.
                IgnoreWriteExceptions = true
            };            
            listener.Prefixes.Add("http://*:8080/");

            try
            {
                listener.Start();

                for (; ; )
                {
                    HttpListenerContext context = null;
                    try
                    {
                        context = listener.GetContext();

                        ThreadPool.QueueUserWorkItem(RequestCallback, context);
                        context = null; // ownership has been transferred to RequestCallback
                    }
                    catch (HttpListenerException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        if (context != null)
                            context.Response.Close();
                    }
                }
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Stop listening for requests.
                listener.Close();
                Console.WriteLine("Done Listening.");
            }
        }

        private static void RequestCallback(Object state)
        {
            var context = (HttpListenerContext)state;
            var response = context.Response;

            try
            {
                var responseString = Plaintext(response);
                WriteResponse(response, responseString);
            }
            finally
            {
                response.Close();
            }
        }

        private static string Plaintext(HttpListenerResponse response)
        {
            response.ContentType = "text/plain";
            return "Hello, World!";
        }

        private static void WriteResponse(HttpListenerResponse response, String responseString)
        {
            response.ContentType = response.ContentType + "; charset=utf-8";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            try
            {
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Win32Exception)
            {
                // Ignore I/O errors
            }
        }

        private static void Threads()
        {
            // To improve CPU utilization, increase the number of threads that the .NET thread pool expands by when
            // a burst of requests come in. We could do this by editing machine.config/system.web/processModel/minWorkerThreads,
            // but that seems too global a change, so we do it in code for just our AppPool. More info:
            //
            // http://support.microsoft.com/kb/821268
            // http://blogs.msdn.com/b/tmarq/archive/2007/07/21/asp-net-thread-usage-on-iis-7-0-and-6-0.aspx
            // http://blogs.msdn.com/b/perfworld/archive/2010/01/13/how-can-i-improve-the-performance-of-asp-net-by-adjusting-the-clr-thread-throttling-properties.aspx

            int newMinWorkerThreads = Convert.ToInt32(ConfigurationManager.AppSettings["minWorkerThreadsPerLogicalProcessor"]);
            if (newMinWorkerThreads > 0)
            {
                int minWorkerThreads, minCompletionPortThreads;
                ThreadPool.GetMinThreads(out minWorkerThreads, out minCompletionPortThreads);
                ThreadPool.SetMinThreads(Environment.ProcessorCount * newMinWorkerThreads, minCompletionPortThreads);
            }
        }
    }
}
