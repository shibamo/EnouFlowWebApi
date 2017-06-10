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
  public class OM_OrgSchemaController : OM_Controller
  {
    /* Disable all members list without certain parent information
     GET: api/OM_OrgSchema
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }
    */

    // GET: api/OM_OrgSchema/5
    [HttpGet]
    [ResponseType(typeof(OrgSchema))]
    public IHttpActionResult Get(int id)
    {
      var obj = db.orgSchemas.Find(id);
      if (obj == null) return NotFound();
      return Ok(obj);
    }

    //

    // POST: api/OM_OrgSchema/Create/1
    [HttpPost]
    [ResponseType(typeof(OrgSchema))]
    [Route("api/OM_OrgSchema/Create/{orgId}")]
    public IHttpActionResult Post(OrgSchema value, [FromUri] int orgId)
    {
      OrgHelper orgHelper = new OrgHelper(db);

      if (value == null || !ModelState.IsValid ||
        !orgHelper.isObjectExists(orgId))
      {
        return BadRequest(ModelState);
      }

      try
      {
        OrgSchemaHelper orgSchemaHelper = new OrgSchemaHelper(db);
        value.Org = orgHelper.getObject(orgId);
        orgSchemaHelper.saveCreatedObject(value);
        //OrgMgmtDBHelper.saveCreatedOrgSchema(db.orgs.Find(orgId), value, db);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return Ok(value);
    }

    // PUT: api/OM_OrgSchema/5
    [ResponseType(typeof(void))]
    public IHttpActionResult Put(int id, OrgSchema value)
    {
      if (value == null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (id != value.orgSchemaId)
      {
        return BadRequest();
      }

      OrgSchemaHelper orgSchemaHelper = new OrgSchemaHelper(db);

      //if (!OrgMgmtDBHelper.isOrgSchemaExists(id, db))
      if (!orgSchemaHelper.isObjectExists(id))
      {
        return NotFound();
      }

      try
      {
        //if (!OrgMgmtDBHelper.isOrgSchemaChangeAllowed(id, value, db))
        if (!orgSchemaHelper.isObjectChangeAllowed(id, value))
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
        //if (!OrgMgmtDBHelper.isOrgSchemaExists(id, db))
        if (!orgSchemaHelper.isObjectExists(id))
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

    // DELETE: api/OM_OrgSchema/5
    [HttpDelete]
    [ResponseType(typeof(OrgSchema))]
    public IHttpActionResult Delete(int id)
    {
      var orgSchema = db.orgSchemas.Find(id);
      if (orgSchema == null)
      {
        return NotFound();
      }

      orgSchema.isVisible = false;
      db.SaveChanges();

      return Ok(orgSchema);
    }
  }
}
