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
      BizEntitySchemaHelper bizEntitySchemaHelper = 
        new BizEntitySchemaHelper(db);

      return Ok(bizEntitySchemaHelper.convert2DTO(obj));
    }

    // POST: api/OM_BizEntitySchema/Create/1
    [HttpPost]
    [ResponseType(typeof(BizEntitySchemaDTO))]
    [Route("api/OM_BizEntitySchema/Create/{bizEntityId}")]
    public IHttpActionResult Post(BizEntitySchema value, [FromUri] int bizEntityId)
    {
      var bizEntityHelper = new BizEntityHelper(db);

      if (value == null || !ModelState.IsValid ||
        !bizEntityHelper.isObjectExists(bizEntityId))
      {
        return BadRequest(ModelState);
      }

      var bizEntity = db.bizEntities.Find(bizEntityId);
      BizEntitySchemaHelper bizEntitySchemaHelper = new BizEntitySchemaHelper(db);
      try
      {
        value.BizEntity = bizEntity;
        bizEntitySchemaHelper.saveCreatedObject(value);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return Ok(bizEntitySchemaHelper.convert2DTO(value));
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

      BizEntitySchemaHelper bizEntitySchemaHelper = new BizEntitySchemaHelper(db);

      if (!bizEntitySchemaHelper.isObjectExists(id))
      {
        return NotFound();
      }

      try
      {
        if (!bizEntitySchemaHelper.isObjectChangeAllowed(id, value))
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

      BizEntitySchemaHelper bizEntitySchemaHelper = new BizEntitySchemaHelper(db);

      bizEntitySchema.isVisible = false;
      db.SaveChanges();

      return Ok(bizEntitySchemaHelper.convert2DTO(bizEntitySchema));
    }
  }
}
