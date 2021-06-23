using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hatra.Messenger.Controllers;

namespace Hatra.Messenger.Web.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ServerInfoController : MessengerControllerBase
    {
        [HttpGet]
        public ActionResult<TimeZoneInfo> GetServerTimeZone()
        {
            return TimeZoneInfo.Local;
        }
        [HttpGet]
        public ActionResult<DateTime> GetServerDateTime()
        {
            return DateTime.Now;
        }
    }
}
