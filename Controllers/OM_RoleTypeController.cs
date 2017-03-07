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
  public class OM_RoleTypeController : OM_Controller
  {
    // GET: api/OM_RoleType
    public IEnumerable<RoleType> Get()
    {
      return db.roleTypes.ToList();
    }

    // GET: api/OM_RoleType/5
    [ResponseType(typeof(RoleTypeDTO))]
    public IHttpActionResult Get(int id)
    {
      var roleType = db.roleTypes.Find(id);
      if (roleType == null) return NotFound();
      return Ok(OrgMgmtDBHelper.convertRoleType2DTO(
        roleType, db));
    }

    // POST: api/OM_RoleType
    [HttpPost]
    [ResponseType(typeof(RoleTypeDTO))]
    public IHttpActionResult Post(RoleType value)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      db.roleTypes.Add(value);
      try
      {
        OrgMgmtDBHelper.saveCreatedRoleType(value, db);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
      return CreatedAtRoute("DefaultApi", new { id = value.roleTypeId },
        OrgMgmtDBHelper.convertRoleType2DTO(value, db));
    }

    // PUT: api/OM_RoleType/5
    [ResponseType(typeof(void))]
    public IHttpActionResult Put(int id, [FromBody]RoleType value)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (id != value.roleTypeId)
      {
        return BadRequest();
      }

      if (!OrgMgmtDBHelper.isRoleTypeExists(id, db))
      {
        return NotFound();
      }

      try
      {
        if (!OrgMgmtDBHelper.isRoleTypeChangeAllowed(id, value, db))
          return BadRequest("不允许修改对象!");
        db.Entry(value).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return StatusCode(HttpStatusCode.NoContent);
    }

    // DELETE: api/OM_RoleType/5
    [HttpDelete]
    [ResponseType(typeof(RoleType))]
    public IHttpActionResult Delete(int id)
    {
      var obj = db.roleTypes.Find(id);
      if (obj == null)
      {
        return NotFound();
      }

      try
      {
        obj.isVisible = false;
        db.SaveChanges();
      }
      catch (Exception ex)
      {
        return InternalServerError(ex);
      }

      return Ok(obj);
    }
  }
}
