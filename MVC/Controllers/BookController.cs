using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.Models.Process;
namespace MVC.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }
        private ExcelProcess _excelPro = new ExcelProcess();
        /// <summary>
        /// Lấy ra toàn bộ danh sách sinh viên mượn sách
        /// </summary>
        /// <returns></returns>
        // public async Task<IActionResult> Index()
        // {
        //     return _context.Book != null ?
        //                   View(await _context.Book.ToListAsync()) :
        //                   View();
        // }
        public async Task<IActionResult> Index(int? page, int? PageSize)
        {
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem() {Value="3", Text="3"},
                new SelectListItem() {Value="5", Text="5"},
                new SelectListItem() {Value="10", Text="10"},
                new SelectListItem() {Value="15", Text="15"},
                new SelectListItem() {Value="25", Text="25"},
                new SelectListItem() {Value="50", Text="50"},  
            };
            int pagesize = (PageSize ?? 10);
            ViewBag.psize = pagesize;
            var model = _context.Book.ToList().ToPagedList(page ?? 1, pagesize);
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Tạo mới sinh viên mượn sách
        /// </summary>
        /// <param name="sinhVien"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdBook,NameBook,Number,Year,NhaXuatBan")] Book book)
        {
            // Check if a book with the same IdBook already exists
            if (BookExists(book.IdBook))
            {
                ModelState.AddModelError("IdBook", "Mã sách đã tồn tại, vui lòng chọn mã sách khác");
                return View(book);
            }
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }
        // Check if a book with the same IdBook already exists
        private bool BookExists(int id)
        {
            // Check if a book with the given IdBook already exists in the database
            return _context.Book.Any(e => e.IdBook == id);
        }
        /// <summary>
        /// Chi tiết của sách
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0 || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.IdBook == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        /// <summary>
        /// Hiển thị màn confirm xoá sách vừa chọn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0 || _context.Book == null)
            {
                return NotFound();
            }

            var bookDetail = await _context.Book
                .Where(m => m.IdBook == id).FirstOrDefaultAsync();

            if (bookDetail == null)
            {
                return NotFound();
            }

            return View(bookDetail);
        }

        /// <summary>
        /// Xác nhận xoá
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Không tìm thấy nhân viên");
            }
            var bookDelete = await _context.Book.FindAsync(id);
            if (bookDelete != null)
            {
                _context.Book.Remove(bookDelete);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0 || _context.Book == null)
            {
                return NotFound();
            }

            var bookDetail = await _context.Book
                .Where(m => m.IdBook == id).FirstOrDefaultAsync();

            if (bookDetail == null)
            {
                return NotFound();
            }
            return View(bookDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdBook,NameBook,Number,NhaXuatBan,Year")] Book book)
        {
            if (id != book.IdBook)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }





        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file!=null)
                {
                    string fileExtension = Path.GetExtension(file.FileName);
                    if (fileExtension != ".xls" && fileExtension != ".xlsx")
                    {
                        ModelState.AddModelError("", "Please choose excel file to upload!");
                    }
                    else
                    {
                        //rename file when upload to server
                        var filePath = Path.Combine(Directory.GetCurrentDirectory() + "/Uploads/Excels", "File" + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Millisecond + fileExtension);
                        var fileLocation = new FileInfo(filePath).ToString();
                        if (file.Length > 0)
                        {
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                //save file to server
                                await file.CopyToAsync(stream);
                                //read data from file and write to database
                                var dt = _excelPro.ExcelToDataTable(fileLocation);
                                for(int i = 0; i < dt.Rows.Count; i++)
                                {
                                    var book = new Book();
                                    book.IdBook = Convert.ToInt16(dt.Rows[i][0].ToString());
                                    book.NameBook = dt.Rows[i][1].ToString();
                                    book.Number = Convert.ToInt16(dt.Rows[i][2].ToString());
                                    book.NhaXuatBan = dt.Rows[i][3].ToString();
                                    book.Year = Convert.ToInt16(dt.Rows[i][4].ToString());
                                    _context.Add(book);
                                }
                                await _context.SaveChangesAsync();
                                return RedirectToAction(nameof(Index));
                            }
                        }
                    }
                }
            
            return View();
        }
        

    }
}




