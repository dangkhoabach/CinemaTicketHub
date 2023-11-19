using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    public class ShowtimesAPIController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;

        public ShowtimesAPIController()
        {
            _dbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get()
        {
            try
            {
                List<SuatChieu> suatChieuList = _dbContext.SuatChieu.ToList();

                if (suatChieuList.Count == 0)
                {
                    return StatusCode(System.Net.HttpStatusCode.NoContent);
                }

                var suatChieuDTOList = suatChieuList.Select(sc => new
                {
                    sc.MaSuatChieu,
                    sc.GioBatDau,
                    sc.GioKetThuc,
                    NgayChieu = sc.NgayChieu?.ToString("dd-MM-yyyy"),
                    sc.MaPhim,
                    sc.MaPhong
                }).ToList();

                return Ok(suatChieuDTOList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/ShowtimesAPI/GetByID/{id}")]
        public IHttpActionResult GetByID(int id)
        {
            try
            {
                SuatChieu suatChieu = _dbContext.SuatChieu.FirstOrDefault(sc => sc.MaSuatChieu == id);

                if (suatChieu == null)
                {
                    return NotFound();
                }

                var suatChieuDTO = new
                {
                    suatChieu.MaSuatChieu,
                    suatChieu.GioBatDau,
                    suatChieu.GioKetThuc,
                    NgayChieu = suatChieu.NgayChieu?.ToString("dd-MM-yyyy"),
                    suatChieu.MaPhim,
                    suatChieu.MaPhong
                };

                return Ok(suatChieuDTO);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("api/ShowtimesAPI/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                SuatChieu suatChieu = _dbContext.SuatChieu.Find(id);

                if (suatChieu == null)
                {
                    return NotFound();
                }

                //Xóa ghế theo suất chiếu
                var gheList = _dbContext.Ghe.Where(x => x.MaSuatChieu == id).ToList();

                foreach (var ghe in gheList)
                {
                    _dbContext.Ghe.Remove(ghe);
                }

                _dbContext.SuatChieu.Remove(suatChieu);
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
