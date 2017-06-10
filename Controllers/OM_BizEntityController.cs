using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using EnouFlowOrgMgmtLib;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace EnouFlowWebApi.Controllers
{
  public class OM_BizEntityController : OM_Controller
  {
    /* Disable all members list without certain parent information
    // GET: api/OM_BizEntity
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }
    */

    // GET: api/OM_BizEntity/5
    [HttpGet]
    [ResponseType(typeof(BizEntityDTO))]
    public IHttpActionResult Get(int id)
    {
      var obj = db.bizEntities.Find(id);
      if (obj == null) return NotFound();
      var bizEntityHelper = new BizEntityHelper();

      return Ok(bizEntityHelper.convert2DTO(obj));
    }

    // POST: api/OM_BizEntity?orgSchemaId=1&bizEntityParentId=3
    [HttpPost]
    [ResponseType(typeof(BizEntityDTO))]
    public IHttpActionResult Post([FromUri] int orgSchemaId,
      [FromUri] int bizEntityParentId, BizEntity value)
    {
      var bizEntityParent = db.bizEntities.Find(bizEntityParentId);

      if (value == null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var orgSchemaHelper = new OrgSchemaHelper(db);
      //if (!OrgMgmtDBHelper.isOrgSchemaExists(orgSchemaId, db) ||
      if (!orgSchemaHelper.isObjectExists(orgSchemaId) ||
        (bizEntityParentId > 0 && bizEntityParent == null))
      {
        ModelState.AddModelError(
          string.Empty,
          "数据错误(invalid orgSchemaId or bizEntityParentId>0 && bizEntityParent==null)!");
        return BadRequest(ModelState);
      }

      var bizEntityHelper = new BizEntityHelper(db);
      try
      {
        //OrgMgmtDBHelper.saveCreatedBizEntity(
        //  db.orgSchemas.Find(orgSchemaId), value, bizEntityParent, db);
        bizEntityHelper.saveCreatedObject(db.orgSchemas.Find(orgSchemaId), value, bizEntityParent);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return Ok(bizEntityHelper.convert2DTO(value));
    }

    // PUT: api/OM_BizEntity/5
    [ResponseType(typeof(void))]
    public IHttpActionResult Put(int id, BizEntity value)
    {
      if (value == null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (id != value.bizEntityId)
      {
        return BadRequest();
      }

      var bizEntityHelper = new BizEntityHelper(db);

      if (!bizEntityHelper.isObjectExists(id))
      {
        return NotFound();
      }

      try
      {
        if (!bizEntityHelper.isObjectChangeAllowed(id, value))
          return BadRequest("不允许修改对象!"); ;
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      db.Entry(value).State = EntityState.Modified;

      db.SaveChanges();

      return StatusCode(HttpStatusCode.NoContent);
    }

    // PUT: api/OM_BizEntity/SetParent/2/0/5
    [HttpPut]
    [ResponseType(typeof(void))]
    [Route("api/OM_BizEntity/SetParent/{orgSchemaId}/{bizEntityIdParent}/{id}")]
    public IHttpActionResult SetParent(
      int orgSchemaId, int id, int bizEntityIdParent)
    {
      var bizEntityParent = db.bizEntities.Find(bizEntityIdParent);
      var bizEntityHelper = new BizEntityHelper(db);

      try
      {
        //OrgMgmtDBHelper.setParentBizEntity(id, bizEntityParent, orgSchemaId, db);
        bizEntityHelper.setParentBizEntity(id, bizEntityParent, orgSchemaId);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return StatusCode(HttpStatusCode.NoContent);
    }


    // DELETE: api/OM_BizEntity/5
    [HttpDelete]
    [ResponseType(typeof(BizEntityDTO))]
    public IHttpActionResult Delete(int id)
    {
      var obj = db.bizEntities.Find(id);
      if (obj == null)
      {
        return NotFound();
      }

      var bizEntityHelper = new BizEntityHelper(db);
      try
      {
        //OrgMgmtDBHelper.removeBizEntity(id, db);
        bizEntityHelper.removeObject(id);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return Ok(bizEntityHelper.convert2DTO(obj));
    }
  }
}
