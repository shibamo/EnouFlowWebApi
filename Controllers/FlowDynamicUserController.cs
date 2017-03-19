using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using EnouFlowTemplateLib;

namespace EnouFlowWebApi.Controllers
{
  public class FlowDynamicUserController : SharedApiController
  {
    // GET api/<controller>
    public IEnumerable<FlowDynamicUser> Get()
    {
      return FlowTemplateDBHelper.getAllFlowDynamicUsers(true);
    }
  }
}