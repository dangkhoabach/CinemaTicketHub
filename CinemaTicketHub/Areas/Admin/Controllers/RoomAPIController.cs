using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    public class RoomAPIController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;

        public RoomAPIController()
        {
            _dbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get()
        {
            try
            {
                List<PhongChieu> phongChieuList = _dbContext.PhongChieu.ToList();

                if (phongChieuList.Count == 0)
                {
                    return StatusCode(System.Net.HttpStatusCode.NoContent);
                }

                var phongChieuDTOList = phongChieuList.Select(pc => new
                {
                    pc.MaPhong,
                    pc.TenPhong
                }).ToList();

                return Ok(phongChieuDTOList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/RoomAPI/GetByID/{id}")]
        public IHttpActionResult GetByID(int id)
        {
            try
            {
                PhongChieu phongChieu = _dbContext.PhongChieu.FirstOrDefault(pc => pc.MaPhong == id);

                if (phongChieu == null)
                {
                    return NotFound();
                }

                var phongChieuDTO = new
                {
                    phongChieu.MaPhong,
                    phongChieu.TenPhong
                };

                return Ok(phongChieuDTO);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("api/RoomAPI/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                PhongChieu phongChieu = _dbContext.PhongChieu.Find(id);

                if (phongChieu == null)
                {
                    return NotFound();
                }

                _dbContext.PhongChieu.Remove(phongChieu);
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
