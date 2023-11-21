using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    public class InvoiceDetailAPIController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;

        public InvoiceDetailAPIController()
        {
            _dbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get()
        {
            try
            {
                List<CT_HoaDon> ctHoaDonList = _dbContext.CT_HoaDon.ToList();

                if (ctHoaDonList.Count == 0)
                {
                    return StatusCode(System.Net.HttpStatusCode.NoContent);
                }

                var ctHoaDonDTOList = ctHoaDonList.Select(ct => new
                {
                    ct.MaHoaDon,
                    ct.SoLuong,
                    ct.MaMon,
                    ct.GiaMon
                }).ToList();

                return Ok(ctHoaDonDTOList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/InvoiceDetailAPI/GetByID/{id}")]
        public IHttpActionResult GetByID(string id)
        {
            try
            {
                List<CT_HoaDon> ctHoaDonList = _dbContext.CT_HoaDon.Where(ct => ct.MaHoaDon == id).ToList();

                if (ctHoaDonList == null || ctHoaDonList.Count == 0)
                {
                    return NotFound();
                }

                var ctHoaDonDTOList = ctHoaDonList.Select(ct => new
                {
                    ct.MaHoaDon,
                    ct.SoLuong,
                    ct.MaMon,
                    ct.GiaMon
                }).ToList();

                return Ok(ctHoaDonDTOList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
