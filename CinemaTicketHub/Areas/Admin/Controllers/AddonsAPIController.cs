using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    public class AddonsAPIController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;

        public AddonsAPIController()
        {
            _dbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get()
        {
            try
            {
                List<BapNuoc> bapNuocList = _dbContext.BapNuoc.ToList();

                if (bapNuocList.Count == 0)
                {
                    return StatusCode(System.Net.HttpStatusCode.NoContent);
                }

                var bapNuocDTOList = bapNuocList.Select(bn => new
                {
                    bn.MaMon,
                    bn.TenMon,
                    bn.GiaMon,
                    bn.MoTa,
                    bn.HinhAnhMon
                }).ToList();

                return Ok(bapNuocDTOList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/AddonsAPI/GetByID/{id}")]
        public IHttpActionResult GetByID(int id)
        {
            try
            {
                BapNuoc bapNuoc = _dbContext.BapNuoc.FirstOrDefault(bn => bn.MaMon == id);

                if (bapNuoc == null)
                {
                    return NotFound();
                }

                var bapNuocDTO = new
                {
                    bapNuoc.MaMon,
                    bapNuoc.TenMon,
                    bapNuoc.GiaMon,
                    bapNuoc.MoTa,
                    bapNuoc.HinhAnhMon
                };

                return Ok(bapNuocDTO);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("api/AddonsAPI/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                BapNuoc bapNuoc = _dbContext.BapNuoc.Find(id);

                if (bapNuoc == null)
                {
                    return NotFound();
                }

                _dbContext.BapNuoc.Remove(bapNuoc);
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
