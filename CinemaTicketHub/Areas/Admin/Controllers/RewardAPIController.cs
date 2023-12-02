using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    public class RewardAPIController : ApiController
    {

        private readonly ApplicationDbContext _dbContext;

        public RewardAPIController()
        {
            _dbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get()
        {
            try
            {
                List<PhanThuong> phanThuongList = _dbContext.PhanThuong.ToList();

                if (phanThuongList.Count == 0)
                {
                    return StatusCode(System.Net.HttpStatusCode.NoContent);
                }

                var phanThuongDTOList = phanThuongList.Select(pt => new
                {
                    pt.MaPT,
                    pt.IdKM,
                    pt.Diem,
                    pt.HinhAnhQua
                }).ToList();

                return Ok(phanThuongDTOList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/RewardAPI/GetByID/{id}")]
        public IHttpActionResult GetByID(int id)
        {
            try
            {
                PhanThuong phanThuong = _dbContext.PhanThuong.FirstOrDefault(pt => pt.MaPT == id);

                if (phanThuong == null)
                {
                    return NotFound();
                }

                var phanThuongDTO = new
                {
                    phanThuong.MaPT,
                    phanThuong.IdKM,
                    phanThuong.Diem,
                    phanThuong.HinhAnhQua
                };

                return Ok(phanThuongDTO);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("api/RewardAPI/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                PhanThuong phanThuong = _dbContext.PhanThuong.Find(id);

                if (phanThuong == null)
                {
                    return NotFound();
                }

                _dbContext.PhanThuong.Remove(phanThuong);
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
