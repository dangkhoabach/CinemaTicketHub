using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    public class GenresAPIController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;

        public GenresAPIController()
        {
            _dbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get()
        {
            try
            {
                List<LoaiPhim> loaiPhimList = _dbContext.LoaiPhim.ToList();

                if (loaiPhimList.Count == 0)
                {
                    return StatusCode(System.Net.HttpStatusCode.NoContent);
                }

                var loaiPhimDTOList = loaiPhimList.Select(lp => new
                {
                    lp.MaLoai,
                    lp.TenLoai
                }).ToList();

                return Ok(loaiPhimDTOList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/GenresAPI/GetByID/{id}")]
        public IHttpActionResult GetByID(int id)
        {
            try
            {
                LoaiPhim loaiPhim = _dbContext.LoaiPhim.FirstOrDefault(lp => lp.MaLoai == id);

                if (loaiPhim == null)
                {
                    return NotFound();
                }

                var loaiPhimDTO = new
                {
                    loaiPhim.MaLoai,
                    loaiPhim.TenLoai
                };

                return Ok(loaiPhimDTO);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("api/GenresAPI/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                LoaiPhim loaiPhim = _dbContext.LoaiPhim.Find(id);

                if (loaiPhim == null)
                {
                    return NotFound();
                }
                _dbContext.LoaiPhim.Remove(loaiPhim);
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
