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
  public class OM_RoleController : OM_Controller
  {
    // GET: api/OM_Role
    public IEnumerable<RoleDTO> Get()
    {
      return db.roles.ToList().Select(
        role => OrgMgmtDBHelper.convertRole2DTO(role, db));
    }

    // GET: api/OM_Role/5
    [ResponseType(typeof(RoleDTO))]
    public IHttpActionResult Get(int id)
    {
      var role = db.roles.Find(id);
      if (role == null) return NotFound();
      return Ok(OrgMgmtDBHelper.convertRole2DTO(role, db));
    }

    // POST: api/OM_Role
    [HttpPost]
    [ResponseType(typeof(Role))]
    public IHttpActionResult Post([FromBody] Role value)
    {
      if (value == null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      db.roles.Add(value);
      try
      {
        OrgMgmtDBHelper.saveCreatedRole(value, db);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
      return CreatedAtRoute("DefaultApi", 
        new { id = value.roleId },
        OrgMgmtDBHelper.convertRole2DTO(value, db));
    }

    // PUT: api/OM_Role/5
    [ResponseType(typeof(void))]
    public IHttpActionResult Put(int id, Role value)
    {
      if (value == null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (id != value.roleId)
      {
        return BadRequest();
      }

      if (!OrgMgmtDBHelper.isRoleExists(id, db))
      {
        return NotFound();
      }

      try
      {
        if (!OrgMgmtDBHelper.isRoleChangeAllowed(id, value, db))
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

    // PUT: api/OM_Role/SetRoleType/5/1
    [HttpPut]
    [ResponseType(typeof(void))]
    [Route("api/OM_Role/SetRoleType/{roletypeId}/{id}")]
    public IHttpActionResult SetRoleType(int roletypeId, int id)
    {
      var roleType = db.roleTypes.Find(roletypeId);

      try
      {
        OrgMgmtDBHelper.setRole_RoleType(id, roleType, db);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return StatusCode(HttpStatusCode.NoContent);
    }

    // PUT: api/OM_Role/UnsetRoleType/5/1
    [HttpPut]
    [ResponseType(typeof(void))]
    [Route("api/OM_Role/UnsetRoleType/{roletypeId}/{id}")]
    public IHttpActionResult UnsetRoleType(int roletypeId, int id)
    {
      var roleType = db.roleTypes.Find(roletypeId);

      try
      {
        OrgMgmtDBHelper.unsetRole_RoleType(id, roleType, db);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return StatusCode(HttpStatusCode.NoContent);
    }

    // DELETE: api/OM_Role/5
    [HttpDelete]
    [ResponseType(typeof(Role))]
    public IHttpActionResult Delete(int id)
    {
      var obj = db.roles.Find(id);
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
