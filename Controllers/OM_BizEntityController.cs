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
      return Ok(OrgMgmtDBHelper.convertBizEntity2DTO(obj));
    }

    // POST: api/OM_BizEntity?orgSchemaId=1&bizEntityParentId=3
    [HttpPost]
    [ResponseType(typeof(BizEntityDTO))]
    public IHttpActionResult Post([FromUri] int orgSchemaId, 
      [FromUri] int bizEntityParentId, BizEntity value)
    {
      var bizEntityParent = db.bizEntities.Find(bizEntityParentId);

      if (value==null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (!OrgMgmtDBHelper.isOrgSchemaExists(orgSchemaId, db) ||
        (bizEntityParentId>0 && bizEntityParent==null))
      {
        return BadRequest("数据错误!");
      }

      try
      {
        OrgMgmtDBHelper.saveCreatedBizEntity(
          db.orgSchemas.Find(orgSchemaId), value, bizEntityParent, db);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return Ok(OrgMgmtDBHelper.convertBizEntity2DTO(value));
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

      if (!OrgMgmtDBHelper.isBizEntityExists(id, db))
      {
        return NotFound();
      }

      try
      {
        if (!OrgMgmtDBHelper.isBizEntityChangeAllowed(id, value, db))
          return BadRequest("不允许修改对象!"); ;
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      db.Entry(value).State = EntityState.Modified;

      try
      {
        db.SaveChanges();
      }
      catch (DbUpdateException)
      {
        if (!OrgMgmtDBHelper.isBizEntityExists(id, db))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

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

      try
      {
        OrgMgmtDBHelper.setParentBizEntity(
        id, bizEntityParent, orgSchemaId, db);
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

      try
      {
        OrgMgmtDBHelper.removeBizEntity(id, db);
      }
      catch(Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return Ok(OrgMgmtDBHelper.convertBizEntity2DTO(obj));
    }
  }
}
