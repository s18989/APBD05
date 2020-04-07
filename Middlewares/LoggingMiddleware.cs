using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APBD05.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            var RequestMethod = string.Empty;
            RequestMethod = httpContext.Request.Method.ToString();
            var Path = string.Empty;
            Path = httpContext.Request.Path.ToString();
            var Body = string.Empty;
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                Body = await reader.ReadToEndAsync();
            }
            var Query = string.Empty;
            Query = httpContext.Request.QueryString.ToString();

            string sciezka = "log.txt";
            if (!File.Exists(sciezka))
            {
                using (StreamWriter sw = File.CreateText(sciezka))
                {
                    sw.WriteLine(RequestMethod);
                    sw.WriteLine(Path);
                    if (Body.Length == 0)
                    {
                        sw.WriteLine("Body jest puste");
                    }
                    else
                    {
                        sw.WriteLine(Body);
                    }
                    if (Query.Length == 0)
                    {
                        sw.WriteLine("Query jest puste");
                    }
                    else
                    {
                        sw.WriteLine(Query);
                    }
                    sw.WriteLine();
                }
            }
            using (StreamWriter sw = File.AppendText(sciezka))
            {
                sw.WriteLine(RequestMethod);
                sw.WriteLine(Path);
                if (Body.Length == 0)
                {
                    sw.WriteLine("Body jest puste");
                }
                else
                {
                    sw.WriteLine(Body);
                }
                if (Query.Length == 0)
                {
                    sw.WriteLine("Query jest puste");
                }
                else
                {
                    sw.WriteLine(Query);
                }
                sw.WriteLine();
            }
            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            await _next(httpContext);
        }
    }

}
