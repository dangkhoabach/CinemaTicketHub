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

                return Ok("Suất chiếu đã được xóa thành công.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public class SuatChieuDTO
        {
            public TimeSpan? GioBatDau { get; set; }
            public TimeSpan? GioKetThuc { get; set; }
            public DateTime? NgayChieu { get; set; }
            public int MaPhim { get; set; }
            public int MaPhong { get; set; }
            public List<GheDTO> Ghe { get; set; }
        }

        public class GheDTO
        {
            public int MaGhe { get; set; }
            public string Day { get; set; }
            public int? Cot { get; set; }
            public bool? TrangThai { get; set; }
        }


        [HttpPost]
        [Route("api/ShowtimesAPI/Create")]
        public IHttpActionResult Create(SuatChieuDTO suatChieuDTO)
        {
            try
            {
                if (suatChieuDTO == null)
                {
                    return BadRequest("Invalid data");
                }

                SuatChieu newSuatChieu = new SuatChieu
                {
                    GioBatDau = suatChieuDTO.GioBatDau,
                    GioKetThuc = suatChieuDTO.GioKetThuc,
                    NgayChieu = suatChieuDTO.NgayChieu,
                    MaPhim = suatChieuDTO.MaPhim,
                    MaPhong = suatChieuDTO.MaPhong
                };
                _dbContext.SuatChieu.Add(newSuatChieu);

                //Thêm ghế theo suất chiếu
                for (int i = 1; i <= 5; i++)
                {
                    for (int j = 1; j <= 10; j++)
                    {
                        Ghe ghe = new Ghe
                        {
                            Cot = j,
                            Day = ((char)('A' + i - 1)).ToString(),
                            MaSuatChieu = newSuatChieu.MaSuatChieu,
                            TrangThai = false
                        };

                        _dbContext.Ghe.Add(ghe);
                    }
                }
                _dbContext.SaveChanges();

                var suatChieuResponse = new SuatChieuDTO
                {
                    GioBatDau = newSuatChieu.GioBatDau,
                    GioKetThuc = newSuatChieu.GioKetThuc,
                    NgayChieu = newSuatChieu.NgayChieu,
                    MaPhim = newSuatChieu.MaPhim,
                    MaPhong = newSuatChieu.MaPhong,
                    Ghe = _dbContext.Ghe.Where(g => g.MaSuatChieu == newSuatChieu.MaSuatChieu).Select(g => new GheDTO { MaGhe = g.MaGhe, Day = g.Day, Cot = g.Cot, TrangThai = g.TrangThai }).ToList()
                };
                return Created(new Uri(Request.RequestUri + "/" + newSuatChieu.MaSuatChieu), suatChieuResponse);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/ShowtimesAPI/Edit/{id}")]
        public IHttpActionResult Edit(int id, SuatChieuDTO suatChieuDTO)
        {
            try
            {
                if (suatChieuDTO == null)
                {
                    return BadRequest("Invalid data");
                }

                var existSuatChieu = _dbContext.SuatChieu.Find(id);

                if (existSuatChieu == null)
                {
                    return NotFound();
                }

                existSuatChieu.GioBatDau = suatChieuDTO.GioBatDau;
                existSuatChieu.GioKetThuc = suatChieuDTO.GioKetThuc;
                existSuatChieu.NgayChieu = suatChieuDTO.NgayChieu;
                existSuatChieu.MaPhim = suatChieuDTO.MaPhim;
                existSuatChieu.MaPhong = suatChieuDTO.MaPhong;

                _dbContext.SaveChanges();

                return Ok("Thông tin suất chiếu đã được cập nhật thành công.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
