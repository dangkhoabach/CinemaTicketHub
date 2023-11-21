using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    public class PromotionsDetailAPIController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;

        public PromotionsDetailAPIController()
        {
            _dbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get()
        {
            try
            {
                List<CT_KhuyenMai> ctKhuyenMaiList = _dbContext.CT_KhuyenMai.ToList();

                if (ctKhuyenMaiList.Count == 0)
                {
                    return StatusCode(System.Net.HttpStatusCode.NoContent);
                }

                var ctKhuyenMaiDTOList = ctKhuyenMaiList.Select(ct => new
                {
                    ct.IdKM,
                    ct.MaKM,
                    ct.TrangThai
                }).ToList();

                return Ok(ctKhuyenMaiDTOList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/PromotionsDetailAPI/GetByID/{id}")]
        public IHttpActionResult GetByID(string id)
        {
            try
            {
                CT_KhuyenMai ctKhuyenMai = _dbContext.CT_KhuyenMai.FirstOrDefault(ct => ct.MaKM == id);

                if (ctKhuyenMai == null)
                {
                    return NotFound();
                }

                var ctKhuyenMaiDTO = new
                {
                    ctKhuyenMai.IdKM,
                    ctKhuyenMai.MaKM,
                    ctKhuyenMai.TrangThai
                };

                return Ok(ctKhuyenMaiDTO);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("api/PromotionsDetailAPI/Delete/{id}")]
        public IHttpActionResult Delete(string id)
        {
            try
            {
                CT_KhuyenMai ctKhuyenMai = _dbContext.CT_KhuyenMai.Find(id);

                if (ctKhuyenMai == null)
                {
                    return NotFound();
                }

                _dbContext.CT_KhuyenMai.Remove(ctKhuyenMai);
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
