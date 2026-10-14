// Họ và tên: <ĐIỀN TÊN>
// Mã sinh viên: <ĐIỀN MÃ SV>
// Nội dung thực hiện: Module 3 - Nhãn tiếng Việt và màu badge cho trạng thái đăng ký / tiến độ / phân công
namespace QuanLyDeTaiDoAn.Helpers
{
    public static class NhanTrangThai
    {
        public static string DangKy(string? t) => t switch
        {
            TrangThaiDangKy.ChoDuyet => "Chờ duyệt",
            TrangThaiDangKy.DaDuyet => "Đã duyệt",
            TrangThaiDangKy.TuChoi => "Từ chối",
            TrangThaiDangKy.DaHuy => "Đã hủy",
            _ => t ?? ""
        };

        public static string MauDangKy(string? t) => t switch
        {
            TrangThaiDangKy.ChoDuyet => "warning text-dark",
            TrangThaiDangKy.DaDuyet => "success",
            TrangThaiDangKy.TuChoi => "danger",
            _ => "secondary"
        };

        public static string TienDo(string? t) => t switch
        {
            TienDoThucHien.DangThucHien => "Đang thực hiện",
            TienDoThucHien.HoanThanh => "Hoàn thành",
            _ => "Chưa bắt đầu"
        };

        public static string PhanCong(string? t) => t switch
        {
            TrangThaiPhanCong.DangHuongDan => "Đang hướng dẫn",
            TrangThaiPhanCong.DaThayDoi => "Đã thay đổi",
            TrangThaiPhanCong.KetThuc => "Kết thúc",
            _ => t ?? ""
        };
    }
}
