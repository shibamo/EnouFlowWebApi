using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using EnouFlowTemplateLib;
using System.Web.Http.Description;

namespace EnouFlowWebApi.Controllers
{
  public class FlowTemplateController : SharedApiController
  {
    // GET api/<controller>
    public IEnumerable<FlowTemplate> Get()
    {
      return FlowTemplateDBHelper.getAllFlowTemplates();
    }

    // POST api/<controller>
    [ResponseType(typeof(FlowTemplate))]
    public IHttpActionResult Post(FlowTemplate value)
    {
      if (value == null) //这里不做ModelState校验,在创建时由创建函数检查
      {
        return BadRequest("不能传空对象");
      }

      Tuple<bool, FlowTemplate, List<string>> result = 
        FlowTemplateDBHelper.createFlowTemplate(
          value.guid, value.name, value.displayName, 
          value.version, value.code, value.flowTemplateJson);
      if (result.Item1) // 成功创建
      {
        return CreatedAtRoute("DefaultApi", 
          new { id = result.Item2.flowTemplateId }, result.Item2);
      }
      else
      {
        var errorInfo = result.Item3.Aggregate(
          (total, next) => { return total + "\n" + next; });
        return BadRequest(errorInfo);
      }
    }

    // PUT api/<controller>/xxxx-xxxx-xxxxxxxxxxx
    [HttpPut]
    [ResponseType(typeof(FlowTemplate))]
    [Route("api/FlowTemplate/{guid}/")]
    public IHttpActionResult Put(string guid, [FromBody]FlowTemplate value)
    {
      //这里不做ModelState校验,在创建时由创建函数检查
      if (value == null || guid != value.guid) 
      {
        return BadRequest("错误: 不能传空对象或guid不匹配");
      }

      Tuple<bool, FlowTemplate, List<string>> result =
        FlowTemplateDBHelper.updateFlowTemplate(
          value.guid, value.name, value.displayName,
          value.version, value.code, value.flowTemplateJson);
      if (result.Item1) // 成功修改
      {
        return Ok(result.Item2);
      }
      else
      {
        var errorInfo = result.Item3.Aggregate(
          (total, next) => { return total + "\n" + next; });
        return BadRequest("错误:" + errorInfo);
      }
    }

  }
}