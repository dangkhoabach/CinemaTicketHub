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
        /*public static List<Test> testlist = new List<Test>
        {
            new Test {Id = 1, Title = "ABC", Description = "áddasdassad"},
            new Test {Id = 2, Title = "XYZ", Description = "dsfsdfdsfsdf"},
            new Test {Id = 3, Title = "WAD", Description = "tghfhfghgfh"},
            new Test {Id = 4, Title = "QWE", Description = "yutyuytutyu"},
            new Test {Id = 5, Title = "JKL", Description = "bnvbnvbnvbn"}
        };

        public List<Test> Get()
        {
            return testlist;
        }

        public Test Get(int Id)
        {
            return testlist.FirstOrDefault(e => e.Id == Id);
        }*/


        private readonly ApplicationDbContext _dbContext;

        public ShowtimesAPIController()
        {
            _dbContext = new ApplicationDbContext();
        }

        // Phương thức GET để lấy dữ liệu từ bảng SuatChieu
        public IHttpActionResult GetSuatChieuData()
        {
            try
            {
                // Lấy danh sách các SuatChieu từ DbContext
                List<SuatChieu> suatChieuList = _dbContext.SuatChieu.ToList();

                if (suatChieuList.Count == 0)
                {
                    // Trả về 204 No Content nếu không có dữ liệu
                    return StatusCode(System.Net.HttpStatusCode.NoContent);
                }

                // Chỉ lấy các trường cần thiết và tạo danh sách DTO
                var suatChieuDTOList = suatChieuList.Select(sc => new
                {
                    sc.MaSuatChieu,
                    sc.GioBatDau,
                    sc.GioKetThuc,
                    sc.NgayChieu,
                    sc.MaPhim,
                    sc.MaPhong
                }).ToList();

                // Trả về danh sách SuatChieuDTO
                return Ok(suatChieuDTOList);
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ nếu có
                return InternalServerError(ex);
            }
        }
    }
}
