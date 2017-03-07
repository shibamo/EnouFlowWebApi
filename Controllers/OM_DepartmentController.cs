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
  public class OM_DepartmentController : OM_Controller
  {
    /* Disable all members list without certain parent information
        // GET: api/OM_Department
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

  */

    // GET: api/OM_Department/5
    [HttpGet]
    [ResponseType(typeof(DepartmentDTO))]
    public IHttpActionResult Get(int id)
    {
      var obj = db.departments.Find(id);
      if (obj == null) return NotFound();
      return Ok(OrgMgmtDBHelper.convertDepartment2DTO(obj,db));
    }

    // POST: api/OM_department?bizEntitySchemaId=1&departmentParentId=2
    [HttpPost]
    [ResponseType(typeof(DepartmentDTO))]
    public IHttpActionResult Post([FromUri] int bizEntitySchemaId,
      [FromUri] int departmentParentId, Department value)
    {
      var departmentParent = db.departments.Find(departmentParentId);

      if (value == null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (!OrgMgmtDBHelper.isBizEntitySchemaExists(bizEntitySchemaId, db) ||
        (departmentParentId > 0 && departmentParent == null))
      {
        return BadRequest("数据错误!");
      }

      try
      {
        OrgMgmtDBHelper.saveCreatedDepartment(
          db.bizEntitySchemas.Find(bizEntitySchemaId), value, departmentParent, db);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return Ok(OrgMgmtDBHelper.convertDepartment2DTO(value, db));
    }

    // PUT: api/OM_Department/5
    [ResponseType(typeof(void))]
    public IHttpActionResult Put(int id, Department value)
    {
      if (value == null || !ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (id != value.departmentId)
      {
        return BadRequest();
      }

      if (!OrgMgmtDBHelper.isDepartmentExists(id, db))
      {
        return NotFound();
      }

      try
      {
        if (!OrgMgmtDBHelper.isDepartmentChangeAllowed(id, value, db))
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

    // PUT: api/OM_Department/SetParent/2/0/5
    [HttpPut]
    [ResponseType(typeof(void))]
    [Route("api/OM_Department/SetParent/{bizEntitySchemaId}/{departmentIdParent}/{id}")]
    public IHttpActionResult SetParent(int id, int departmentIdParent, int bizEntitySchemaId)
    {
      var bizDepartmentParent = db.departments.Find(departmentIdParent);

      try
      {
        OrgMgmtDBHelper.setParentDepartment(
        id, bizDepartmentParent, bizEntitySchemaId, db);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return StatusCode(HttpStatusCode.NoContent);
    }

    // DELETE: api/OM_Department/5
    [HttpDelete]
    [ResponseType(typeof(Department))]
    public IHttpActionResult Delete(int id)
    {
      var obj = db.departments.Find(id);
      if (obj == null)
      {
        return NotFound();
      }

      try
      {
        OrgMgmtDBHelper.removeDepartment(id, db);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return Ok(obj);
    }
  }
}
