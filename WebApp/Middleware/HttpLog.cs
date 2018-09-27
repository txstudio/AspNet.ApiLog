using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp
{
    public class HttpLog
    {
        public string ApplicationName { get; set; }

        //public string LocalIpAddress { get; set; }
        //public int LocalPort { get; set; }

        public string RemoteIpAddress { get; set; }
        public int RemotePort { get; set; }

        public string IdentityName { get; set; }

        //HttRequest
        public string Method { get; set; }
        public string Scheme { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string RequestContentType { get; set; }
        public long? RequestContentLength { get; set; }
        public string RequestBody { get; set; }

        //HttpResponse
        public string StatusCode { get; set; }
        public string ResponseContentType { get; set; }
        public long? ResponseContentLength { get; set; }
        public string ResponseBody { get; set; }

        public long Elapsed { get; set; }
    }
}
