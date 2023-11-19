using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    public class PromotionsAPIController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;

        public PromotionsAPIController()
        {
            _dbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get()
        {
            try
            {
                List<KhuyenMai> khuyenMaiList = _dbContext.KhuyenMai.ToList();

                if (khuyenMaiList.Count == 0)
                {
                    return StatusCode(System.Net.HttpStatusCode.NoContent);
                }

                var khuyenMaiDTOList = khuyenMaiList.Select(km => new
                {
                    km.IdKM,
                    km.TenKM,
                    km.SoLuong,
                    km.PhanTram,
                    ThoiHan = km.ThoiHan?.ToString("dd-MM-yyyy")
                }).ToList();

                return Ok(khuyenMaiDTOList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/PromotionsAPI/GetByID/{id}")]
        public IHttpActionResult GetByID(string id)
        {
            try
            {
                KhuyenMai khuyenMai = _dbContext.KhuyenMai.FirstOrDefault(km => km.IdKM == id);

                if (khuyenMai == null)
                {
                    return NotFound();
                }

                var khuyenMaiDTO = new
                {
                    khuyenMai.IdKM,
                    khuyenMai.TenKM,
                    khuyenMai.SoLuong,                   
                    khuyenMai.PhanTram,
                    ThoiHan = khuyenMai.ThoiHan?.ToString("dd-MM-yyyy")
                };

                return Ok(khuyenMaiDTO);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("api/PromotionsAPI/Delete/{id}")]
        public IHttpActionResult Delete(string id)
        {
            try
            {
                KhuyenMai khuyenMai = _dbContext.KhuyenMai.Find(id);

                if (khuyenMai == null)
                {
                    return NotFound();
                }

                //Xóa chi tiết khuyến mãi
                var CTkhuyenmai = _dbContext.CT_KhuyenMai.Where(x => x.IdKM == id).ToList();

                foreach (var CTkm in CTkhuyenmai)
                {
                    _dbContext.CT_KhuyenMai.Remove(CTkm);
                }

                _dbContext.KhuyenMai.Remove(khuyenMai);
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
