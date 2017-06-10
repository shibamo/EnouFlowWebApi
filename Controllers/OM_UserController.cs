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
  public class OM_UserController : OM_Controller
  {
    // GET: api/OM_User
    public IEnumerable<UserDTO> Get()
    {
      UserHelper userHelper = new UserHelper(db);

      var users = db.users.ToList().Select(user =>
        userHelper.convert2DTO(user, true));
      return users;
    }

    // GET: api/OM_User/5
    [ResponseType(typeof(User))]
    public IHttpActionResult Get(int id)
    {
      var user = db.users.Find(id);
      if (user == null) return NotFound();
      return Ok(user);
    }

    // GET: api/OM_User/GetDepartmentUserRelations/5
    [ResponseType(typeof(DepartmentUserRelation))]
    [HttpGet]
    [Route("api/OM_User/GetDepartmentUserRelations/{id}")]
    public ICollection<DepartmentUserRelation> 
      GetDepartmentUserRelations(int id)
    {
      var result = db.departmentUserRelations.Where(
        r => r.assistUserId == id).ToList();
      return result;
    }

    // POST: api/OM_User
    [ResponseType(typeof(User))]
    public IHttpActionResult Post(User value)
    {
      if (value == null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      UserHelper userHelper = new UserHelper(db);
      try
      {
        userHelper.saveCreatedObject(value);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
      //var dto = OrgMgmtDBHelper.convertUser2DTO(value,db);
      return CreatedAtRoute("DefaultApi", new { id = value.userId }, value);
    }
    
    // PUT: api/OM_User/5
    [ResponseType(typeof(void))]
    public IHttpActionResult Put(int id, User value)
    {
      if (value == null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (id != value.userId)
      {
        return BadRequest();
      }

      UserHelper userHelper = new UserHelper(db);

      if (!userHelper.isObjectExists(id))
      {
        return NotFound();
      }

      try
      {
        if (!userHelper.isObjectChangeAllowed(id, value))
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

    // PUT: api/OM_User/SetDepartment/5/1
    [HttpPut]
    [ResponseType(typeof(void))]
    [Route("api/OM_User/SetDepartment/{departmentId}/{id}")]
    public IHttpActionResult SetDepartment(int departmentId, int id, 
      [FromBody] int userPosition)
    {
      UserHelper userHelper = new UserHelper(db);
      try
      {
        userHelper.setUserDepartment(id, 
          db.departments.Find(departmentId), 
          (UserPositionToDepartment)userPosition);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return StatusCode(HttpStatusCode.NoContent);
    }

    // PUT: api/OM_User/UnsetDepartment/5/1
    [HttpPut]
    [ResponseType(typeof(void))]
    [Route("api/OM_User/UnsetDepartment/{departmentId}/{id}")]
    public IHttpActionResult UnsetDepartment(int departmentId, int id)
    {
      UserHelper userHelper = new UserHelper(db);
      try
      {
        userHelper.unsetUserDepartment(id, db.departments.Find(departmentId));
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return StatusCode(HttpStatusCode.NoContent);
    }

    // PUT: api/OM_User/SetRole/5/1
    [HttpPut]
    [ResponseType(typeof(void))]
    [Route("api/OM_User/SetRole/{roleId}/{id}")]
    public IHttpActionResult SetRole(int roleId, int id)
    {
      try
      {
        new UserHelper(db).setUserRole(id, db.roles.Find(roleId));
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return StatusCode(HttpStatusCode.NoContent);
    }

    // PUT: api/OM_User/UnsetRole/5/1
    [HttpPut]
    [ResponseType(typeof(void))]
    [Route("api/OM_User/UnsetRole/{roleId}/{id}")]
    public IHttpActionResult UnsetRole(int roleId, int id)
    {
      try
      {
        new UserHelper(db).unsetUserRole(id, db.roles.Find(roleId));
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return StatusCode(HttpStatusCode.NoContent);
    }

    // DELETE: api/OM_User/5
    [HttpDelete]
    [ResponseType(typeof(User))]
    public IHttpActionResult Delete(int id)
    {
      var obj = db.users.Find(id);
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
