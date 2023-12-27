using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using OfficeOpenXml;

namespace MVC.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            List<Book> books = _context.Book.ToList();
            ViewBag.BookList = books;
            return View();
        }

        /// <summary>
        /// Tạo mới sinh viên mượn sách
        /// </summary>
        /// <param name="sinhVien"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSV,NameSV,Khoa,ClassName,PhoneSV,IdBook,BorrowDate,PayDate")] SinhVien sinhVien)
        {
            // Lấy thông tin sách từ bảng Book dựa trên IdBook
            var book = await _context.Book.FindAsync(sinhVien.IdBook);
            if (ModelState.IsValid)
            {
                // Giảm số lượng sách đi 1
                book.Number--;

                // Cập nhật thông tin sách trong cơ sở dữ liệu
                _context.Update(book);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Insert thông tin sinh viên mượn sách
                sinhVien.Status = 0;
                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sinhVien);
        }
        /// <summary>
        /// Xem chi tiết
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0 || _context.SinhVien == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhVien
                .Where(m => m.Id == id)
                .Join(
                    _context.Book,
                    sv => sv.IdBook,
                    book => book.IdBook,
                    (sv, book) => new SinhVienWithBookViewModel
                    {
                        IdSV = sv.IdSV,
                        NameSV = sv.NameSV,
                        Khoa = sv.Khoa,
                        ClassName = sv.ClassName,
                        PhoneSV = sv.PhoneSV,
                        BorrowDate = sv.BorrowDate,
                        PayDate = sv.PayDate,
                        NameBook = book.NameBook,
                        Status = sv.Status
                    }
                )
                .FirstOrDefaultAsync();

            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // Delete
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0 || _context.SinhVien == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhVien
                .Where(m => m.Id == id)
                .Join(
                    _context.Book,
                    sv => sv.IdBook,
                    book => book.IdBook,
                    (sv, book) => new SinhVienWithBookViewModel
                    {
                        IdSV = sv.IdSV,
                        NameSV = sv.NameSV,
                        Khoa = sv.Khoa,
                        ClassName = sv.ClassName,
                        PhoneSV = sv.PhoneSV,
                        BorrowDate = sv.BorrowDate,
                        PayDate = sv.PayDate,
                        NameBook = book.NameBook,
                        Status = sv.Status
                    }
                )
                .FirstOrDefaultAsync();

            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SinhVien == null)
            {
                return Problem("Không tìm thấy sinh viên");
            }
            var nhanVien = await _context.SinhVien.FindAsync(id);
            if (nhanVien != null)
            {
                _context.SinhVien.Remove(nhanVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
 public async Task<IActionResult> Index(SinhVienWithBookViewModel searchModel)
        {
            if (searchModel.ClearFilter)
            {
                // Đặt lại mô hình tìm kiếm về trạng thái ban đầu
                searchModel = new SinhVienWithBookViewModel();
            }
            List<SinhVienWithBookViewModel> result = await (
            from sinhVien in _context.SinhVien
            join Book in _context.Book on sinhVien.IdBook equals Book.IdBook
            select new SinhVienWithBookViewModel
            {
                Id = sinhVien.Id,
                IdSV = sinhVien.IdSV,
                NameSV = sinhVien.NameSV,
                Khoa = sinhVien.Khoa,
                ClassName = sinhVien.ClassName,
                PhoneSV = sinhVien.PhoneSV,
                BorrowDate = sinhVien.BorrowDate,
                PayDate = sinhVien.PayDate,
                NameBook = Book.NameBook,
                Status = sinhVien.Status
            }
            ).ToListAsync();

            // Áp dụng điều kiện tìm kiếm
            if (!string.IsNullOrEmpty(searchModel.NameSV))
            {
                result = result.Where(s => s.NameSV.Contains(searchModel.NameSV)).ToList();
            }

            if (!string.IsNullOrEmpty(searchModel.IdSV))
            {
                result = result.Where(s => s.IdSV == searchModel.IdSV).ToList();
            }
            if (searchModel.Status == 0)
            {

            }
            else if (searchModel.Status == 1)
            {
                result = result.Where(s => s.Status == 0).ToList();
            }
            else if (searchModel.Status == 2)
            {
                result = result.Where(s => s.Status == 1).ToList();
            }

            return result.Count > 0 ? View(result) : View();
        }
        
        }
        

    }