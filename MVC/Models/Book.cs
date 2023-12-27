using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace MVC.Models
{
    [Table("Book")]

    public class Book
    {
        [Key]
        // Ràng buộc dữ liệu.
        [Required(ErrorMessage = "Mã sách không được để trống.")]
        public int IdBook { get; set; }
        [Required(ErrorMessage = "Tên sách không được để trống.")]
        public string? NameBook { get; set; }
        [Required(ErrorMessage = "Số lượng không được để trống.")]
        public int Number { get; set; }
        public string? NhaXuatBan { get; set; }
        [Required(ErrorMessage = "Năm xuất bản không được để trống.")]
        public int Year { get; set; }
    }
    public class BookWithBookViewModel
    {
        public int? IdBook { get; set; }
        public string? NameBook { get; set; }
        public int Number { get; set; }
        public string? NhaXuatBan { get; set; }
        public int Year { get; set; }
    }

}
