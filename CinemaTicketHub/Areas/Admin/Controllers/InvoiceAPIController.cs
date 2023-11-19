using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    public class InvoiceAPIController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;

        public InvoiceAPIController()
        {
            _dbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get()
        {
            try
            {
                List<HoaDon> hoaDonList = _dbContext.HoaDon.ToList();

                if (hoaDonList.Count == 0)
                {
                    return StatusCode(System.Net.HttpStatusCode.NoContent);
                }

                var hoaDonDTOList = hoaDonList.Select(hd => new
                {
                    hd.MaHoaDon,
                    NgayLap = hd.NgayLap?.ToString("dd-MM-yyyy"),
                    hd.TongTien,
                    hd.Id,
                    hd.Payment
                }).ToList();

                return Ok(hoaDonDTOList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/InvoiceAPI/GetByID/{id}")]
        public IHttpActionResult GetByID(string id)
        {
            try
            {
                HoaDon hoaDon = _dbContext.HoaDon.FirstOrDefault(hd => hd.MaHoaDon == id);

                if (hoaDon == null)
                {
                    return NotFound();
                }

                var hoaDonDTO = new
                {
                    hoaDon.MaHoaDon,
                    NgayLap = hoaDon.NgayLap?.ToString("dd-MM-yyyy"),
                    hoaDon.TongTien,
                    hoaDon.Id,
                    hoaDon.Payment
                };

                return Ok(hoaDonDTO);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
