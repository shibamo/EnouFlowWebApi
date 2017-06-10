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
  public class OM_OrgController : OM_Controller
  {
    // GET: api/OM_Org
    public IEnumerable<OrgDTO> Get()
    {
      var orgs = db.orgs.ToList().Select(org =>
        new OrgHelper().convert2DTO(org)); //OrgMgmtDBHelper.convertOrg2DTO(org));
      return orgs;
    }

    // GET: api/OM_Org/1
    [ResponseType(typeof(Org))]
    public async Task<IHttpActionResult> Get(int id)
    {
      var org = await db.orgs.FindAsync(id);
      if (org == null) return NotFound();
      return Ok(org);
    }

    //GET: api/OM_Org/GetOrgSchemas/1
    [ResponseType(typeof(OrgSchemaDTO))]
    [HttpGet]
    [Route("api/OM_Org/GetOrgSchemas/{orgId}")]
    public async Task<IHttpActionResult> GetOrgSchemas(int orgId)
    {
      var org = await db.orgs.FindAsync(orgId);
      if (org == null) return NotFound();

      var orgHelper = new OrgHelper(db);
      return Ok(orgHelper.getOrgSchemaDTOs(org));
    }

    // POST: api/OM_Org
    [ResponseType(typeof(OrgDTO))]
    public IHttpActionResult Post(Org value)
    {
      if (value == null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var orgHelper = new OrgHelper(db);

      try
      {
        //OrgMgmtDBHelper.saveCreatedOrg(value, db);
        orgHelper.saveCreatedObject(value);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
      var dto = orgHelper.convert2DTO(value); //OrgMgmtDBHelper.convertOrg2DTO(value);
      return CreatedAtRoute("DefaultApi", new { id = value.orgId }, dto);
    }

    // PUT: api/OM_Org/5
    [ResponseType(typeof(void))]
    public IHttpActionResult Put(int id, Org value)
    {
      if (value == null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (id != value.orgId)
      {
        return BadRequest();
      }

      OrgHelper orgHelper = new OrgHelper(db);

      //if (!OrgMgmtDBHelper.isOrgExists(id, db))
      if (!orgHelper.isObjectExists(id))
      {
        return NotFound();
      }

      try
      {
        //if (!OrgMgmtDBHelper.isOrgChangeAllowed(id, value, db))
        if (!orgHelper.isObjectChangeAllowed(id, value))
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
        //if (!OrgMgmtDBHelper.isOrgExists(id, db))
        if (!orgHelper.isObjectExists(id))
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

    // DELETE: api/OM_Org/5
    [HttpDelete]
    [ResponseType(typeof(OrgDTO))]
    public IHttpActionResult Delete(int id)
    {
      //var org = db.orgs.Find(id);
      //if (org == null)
      //{
      //  return NotFound();
      //}

      //org.isVisible = false;
      //db.SaveChanges();

      //return Ok(OrgMgmtDBHelper.convertOrg2DTO(org));
      OrgHelper orgHelper = new OrgHelper(db);

      orgHelper.removeObject(id);

      var org = db.orgs.Find(id);
      return Ok(orgHelper.convert2DTO(org));
    }

  }
}
