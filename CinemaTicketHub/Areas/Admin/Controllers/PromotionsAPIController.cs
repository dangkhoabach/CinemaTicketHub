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
                    km.SoTienGiam,
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
                    khuyenMai.SoTienGiam,
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

                return Ok("Khuyến mãi đã được xóa thành công.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public class KhuyenMaiDTO
        {
            public string IdKM { get; set; }
            public string TenKM { get; set; }
            public int? SoLuong { get; set; }
            public int? PhanTram { get; set; }
            public double? SoTienGiam { get; set; }
            public DateTime? ThoiHan { get; set; }
        }

        [HttpPost]
        [Route("api/PromotionsAPI/Create")]
        public IHttpActionResult Create([FromBody] KhuyenMaiDTO khuyenMaiDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    KhuyenMai khuyenMai = new KhuyenMai
                    {
                        IdKM = khuyenMaiDTO.IdKM,
                        TenKM = khuyenMaiDTO.TenKM,
                        SoLuong = khuyenMaiDTO.SoLuong,
                        PhanTram = khuyenMaiDTO.PhanTram,
                        SoTienGiam = khuyenMaiDTO.SoTienGiam,
                        ThoiHan = khuyenMaiDTO.ThoiHan
                    };

                    _dbContext.KhuyenMai.Add(khuyenMai);
                    _dbContext.SaveChanges();

                    Random random = new Random();
                    for (int i = 1; i <= khuyenMai.SoLuong; i++)
                    {
                        CT_KhuyenMai ctKhuyenMai = new CT_KhuyenMai();
                        ctKhuyenMai.IdKM = khuyenMai.IdKM;
                        ctKhuyenMai.TrangThai = true;

                        // Tạo chuỗi ngẫu nhiên gồm 10 số
                        string randomString = random.Next(1000000000, 2000000000).ToString();
                        ctKhuyenMai.MaKM = randomString;

                        _dbContext.CT_KhuyenMai.Add(ctKhuyenMai);
                        _dbContext.SaveChanges();
                    }

                    return Ok(khuyenMaiDTO);
                }
                else
                {
                    return BadRequest("Dữ liệu không hợp lệ.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/PromotionsAPI/Edit/{id}")]
        public IHttpActionResult Edit(string id, [FromBody] KhuyenMaiDTO khuyenMaiDTO)
        {
            try
            {
                KhuyenMai existingKhuyenMai = _dbContext.KhuyenMai.FirstOrDefault(km => km.IdKM == id);

                if (existingKhuyenMai == null)
                {
                    return NotFound();
                }

                existingKhuyenMai.TenKM = khuyenMaiDTO.TenKM;
                existingKhuyenMai.PhanTram = khuyenMaiDTO.PhanTram;
                existingKhuyenMai.SoTienGiam = khuyenMaiDTO.SoTienGiam;
                existingKhuyenMai.ThoiHan = khuyenMaiDTO.ThoiHan;

                _dbContext.SaveChanges();

                return Ok("Thông tin khuyến mãi đã được cập nhật thành công.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }
}
