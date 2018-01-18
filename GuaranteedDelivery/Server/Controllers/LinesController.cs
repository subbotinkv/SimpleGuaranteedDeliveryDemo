using System;
using System.IO;
using System.Net.Http;
using System.Web.Http;

namespace Server.Controllers
{
    public class LinesController : ApiController
    {
        public void Post([FromBody] string line)
        {
            string remoteIpAddress = Request.GetOwinContext().Request.RemoteIpAddress;
            if (remoteIpAddress == "::1")
            {
                remoteIpAddress = "localhost";
            }

            File.AppendAllText(Config.StorageFileName, $"{remoteIpAddress} - [{DateTime.Now:u}] - {line}{Environment.NewLine}");
        }
    }
}