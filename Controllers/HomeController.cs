using EnouFlowWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;


namespace EnouFlowWebApi.Controllers
{
  public class HomeController : SharedApiController
  {
    [HttpPost]
    [ResponseType(typeof(CurrentUserInfo))]
    [Route("api/Home/Logon")]
    public IHttpActionResult Logon(
      [FromBody]LogonRequestInfo value)
    {
      return Ok(new CurrentUserInfo());
    }
  }
}
