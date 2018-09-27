using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApp
{
    public class HttpLogMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpLogMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        private bool CaptureRequestBody(string contentType)
        {
            /*
             * multipart/form-data 的傳遞內容不進行 Request Body 紀錄
             */
            if (string.IsNullOrWhiteSpace(contentType) == true)
                return true;

            if (contentType.ToLower().StartsWith("multipart/form-data;") == true)
                return false;

            return true;
        }

        public async Task Invoke(HttpContext context)
        {
            Stopwatch _stopwatch = new Stopwatch();
            StringBuilder _builder = new StringBuilder();

            _stopwatch.Start();

            ConnectionInfo _connection = context.Connection;
            HttpRequest _request = context.Request;
            HttpResponse _response = context.Response;


            _builder.AppendLine("HttpContext.Connection");
            _builder.AppendFormat("\tLocalIpAddress: \"{0}\"", _connection.LocalIpAddress).AppendLine();
            _builder.AppendFormat("\tLocalPort: \"{0}\"", _connection.LocalPort).AppendLine();
            _builder.AppendFormat("\tRemoteIpAddress: \"{0}\"", _connection.RemoteIpAddress).AppendLine();
            _builder.AppendFormat("\tRemotePort: \"{0}\"", _connection.RemotePort).AppendLine();
            _builder.AppendLine();

            _builder.AppendLine("HttpContext.User.Identity");
            _builder.AppendFormat("\tIsAuthenticated: \"{0}\"", context.User.Identity.IsAuthenticated).AppendLine();
            _builder.AppendFormat("\tName: \"{0}\"", context.User.Identity.Name).AppendLine();
            _builder.AppendLine();

            _builder.AppendLine("HttpContext.Request");
            _builder.AppendFormat("\tMethod: \"{0}\"", _request.Method).AppendLine();
            _builder.AppendFormat("\tScheme: \"{0}\"", _request.Scheme).AppendLine();
            _builder.AppendFormat("\tPath: \"{0}\"", _request.Path).AppendLine();
            _builder.AppendFormat("\tQueryString: \"{0}\"", _request.QueryString).AppendLine();

            if (_request.ContentType != null)
                _builder.AppendFormat("\tContentType: \"{0}\"", _request.ContentType).AppendLine();

            _builder.AppendFormat("\tContentLength: \"{0}\"", _request.ContentLength).AppendLine();
            _builder.AppendLine();


            /* 取得 Request.Body */
            if(this.CaptureRequestBody(_request.ContentType) == true)
            {
                _request.EnableBuffering();
                _request.Body.Position = 0;

                var _content = await new StreamReader(_request.Body).ReadToEndAsync();

                _request.Body.Position = 0;

                _builder.AppendLine("HttpContext.Request.Body");
                _builder.AppendLine(_content);
                _builder.AppendLine();
            }



            /* 取得 Response.Body */
            Stream _responseStream = _response.Body;
            MemoryStream _stream = new MemoryStream();

            _response.Body = _stream;

            await this._next(context);

            _builder.AppendLine("HttpContext.Response");
            _builder.AppendFormat("\tStatusCode: \"{0}\"", _response.StatusCode).AppendLine();

            if (_response.ContentType != null)
                _builder.AppendFormat("\tContentType: \"{0}\"", _response.ContentType).AppendLine();

            _builder.AppendFormat("\tContentLength: \"{0}\"", _response.ContentLength).AppendLine();
            _builder.AppendLine();

            _stream.Position = 0;

            string _Body = await new StreamReader(_stream).ReadToEndAsync();

            _builder.AppendLine("HttpContext.Response.Body");
            _builder.AppendLine(_Body);
            _builder.AppendLine();

            _stream.Position = 0;

            await _stream.CopyToAsync(_responseStream);

            _response.Body = _responseStream;

            _stopwatch.Stop();

            _builder.AppendLine();
            _builder.AppendFormat("Elapsed: \"{0}\"", _stopwatch.ElapsedMilliseconds);

            Console.WriteLine();
            Console.WriteLine(_builder.ToString());
            Console.WriteLine();
        }
    }
}
