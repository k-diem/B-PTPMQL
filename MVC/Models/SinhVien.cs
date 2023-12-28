using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace MVC.Models
{
    [Table("SinhVien")]

    public class SinhVien
    {
        [Key]
        // tự sinh ra id khi create
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Mã sinh viên không được để trống.")]
        public string? IdSV { get; set; }
        [Required(ErrorMessage = "Hãy chọn loại sách cần mượn")]
        public int? IdBook { get; set; }
        [Required(ErrorMessage = "Họ và tên sinh viên không được để trống.")]
        public string? NameSV { get; set; }
        [Required(ErrorMessage = "Khoá sinh viên không được để trống.")]
        public string? Khoa { get; set; }
        [Required(ErrorMessage = "Số điện thoại sinh viên không được để trống.")]
        public string? PhoneSV { get; set; }
        [Required(ErrorMessage = "Lớp sinh viên không được để trống.")]
        public string? ClassName { get; set; }
        [Required(ErrorMessage = "Ngày mượn không được để trống.")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? BorrowDate { get; set; }
        [Required(ErrorMessage = "Hạn trả không được để trống.")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? PayDate { get; set; }
        public int Status { get; set; }

    }
    public class SinhVienWithBookViewModel
    {
        public int Id { get; set; }
        public string? IdSV { get; set; }
        public string? NameSV { get; set; }
        public string? Khoa { get; set; }
        public string? ClassName { get; set; }
        public string? PhoneSV { get; set; }
        public DateTime? BorrowDate { get; set; }
        public string? NameBook { get; set; }
        public DateTime? PayDate { get; set; }
        public int DelayDays { get; set; }
        public int Status { get; set; }
        public int? IdBook { get; set; }
    }
}
