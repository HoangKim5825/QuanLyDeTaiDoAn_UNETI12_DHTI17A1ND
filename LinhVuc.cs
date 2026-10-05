using System.ComponentModel.DataAnnotations;

namespace QuanLyDeTaiDoAn.Models
{
    // Ghi chú: Entity này thuộc phạm vi phụ trách chính của Module 1
    // (Quản lý lĩnh vực đề tài). Module 3 chỉ đọc dữ liệu (qua DeTai.LinhVuc)
    // để hiển thị tên lĩnh vực của đề tài mà sinh viên đăng ký.
    public class LinhVuc
    {
        [Key]
        public int MaLinhVuc { get; set; }

        [Required(ErrorMessage = "Tên lĩnh vực không được để trống")]
        [StringLength(150)]
        [Display(Name = "Tên lĩnh vực")]
        public string TenLinhVuc { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; }

        public ICollection<GiangVien> GiangViens { get; set; } = new List<GiangVien>();
        public ICollection<DeTai> DeTais { get; set; } = new List<DeTai>();
    }
}
