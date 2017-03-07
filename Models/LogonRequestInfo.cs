using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnouFlowWebApi.Models
{
  public class LogonRequestInfo
  {
    public string userAccount { get; set; }
    public string password { get; set; }
    public string authenticationKey { get; set; }
  }
}