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
  public class OM_BizEntitySchemaController : OM_Controller
  {
    /* Disable all members list without certain parent information
    // GET: api/OM_BizEntitySchema
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }
    */

    // GET: api/OM_BizEntitySchema/5
    [HttpGet]
    [ResponseType(typeof(BizEntitySchemaDTO))]
    public IHttpActionResult Get(int id)
    {
      var obj = db.bizEntitySchemas.Find(id);
      if (obj == null) return NotFound();
      return Ok(OrgMgmtDBHelper.convertBizEntitySchema2DTO(obj,db));
    }

    // POST: api/OM_BizEntitySchema/Create/1
    [HttpPost]
    [ResponseType(typeof(BizEntitySchemaDTO))]
    [Route("api/OM_BizEntitySchema/Create/{bizEntityId}")]
    public IHttpActionResult Post(BizEntitySchema value, [FromUri] int bizEntityId)
    {
      var bizEntity = db.bizEntities.Find(bizEntityId);

      if (value == null || !ModelState.IsValid ||
        !OrgMgmtDBHelper.isBizEntityExists(bizEntityId, db))
      {
        return BadRequest(ModelState);
      }

      try
      {
        OrgMgmtDBHelper.saveCreatedBizEntitySchema(value, bizEntity, db);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return Ok(OrgMgmtDBHelper.convertBizEntitySchema2DTO(value,db));
    }

    // PUT: api/OM_BizEntitySchema/5
    [ResponseType(typeof(void))]
    public IHttpActionResult Put(int id, BizEntitySchema value)
    {
      if (value == null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (id != value.bizEntitySchemaId)
      {
        return BadRequest();
      }

      if (!OrgMgmtDBHelper.isBizEntitySchemaExists(id, db))
      {
        return NotFound();
      }

      try
      {
        if (!OrgMgmtDBHelper.isBizEntitySchemaChangeAllowed(id, value, db))
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
        if (!OrgMgmtDBHelper.isBizEntitySchemaExists(id, db))
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

    // DELETE: api/OM_BizEntitySchema/5
    [HttpDelete]
    [ResponseType(typeof(BizEntitySchemaDTO))]
    public IHttpActionResult Delete(int id)
    {
      var bizEntitySchema = db.bizEntitySchemas.Find(id);
      if (bizEntitySchema == null)
      {
        return NotFound();
      }

      bizEntitySchema.isVisible = false;
      db.SaveChanges();

      return Ok(OrgMgmtDBHelper.convertBizEntitySchema2DTO(bizEntitySchema,db));
    }
  }
}
