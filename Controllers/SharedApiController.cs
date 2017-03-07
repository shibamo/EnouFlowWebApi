using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EnouFlowWebApi.Controllers
{
  [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-Custom-Header", SupportsCredentials = true)]
  public class SharedApiController : ApiController
  {
  }
}