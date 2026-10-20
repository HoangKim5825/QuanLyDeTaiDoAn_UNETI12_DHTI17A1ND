# QuanLyDeTaiDoAn_UNETI12_DHTI17A1ND

Hệ thống quản lý đề tài đồ án và phân công giảng viên hướng dẫn  
Bài tập lớn môn Thực hành lập trình .NET – Đề tài 12 – Nhóm 12, lớp DHTI17A1ND

**Công nghệ:** .NET 10, ASP.NET Core 10 MVC, Entity Framework Core 10 (Code First), SQL Server, LINQ, Razor, Bootstrap, Chart.js

## Thành viên và phân công

| STT | Họ và tên | Mã sinh viên | Module |
|---|---|---|---|
| 1 | Nguyễn Minh Đức | 23203100078 | Module 1: Tài khoản, đăng nhập, phân quyền, lĩnh vực |
| 2 | Nguyễn Quang Huy | 23203100080 | Module 2: Giảng viên, đề tài, tìm kiếm/lọc/sắp xếp/phân trang |
| 3 | Thành Hưng | 23203100011 | Module 3: Sinh viên, đăng ký đề tài |
| 4 | Hoàng Kim | 23203100056 | Module 4: Duyệt đăng ký, phân công GVHD, Dashboard, thống kê |

## Chức năng chính

**Admin:** quản lý tài khoản, lĩnh vực, giảng viên, sinh viên, đề tài; duyệt/từ chối đăng ký; phân công và thay đổi GVHD (giữ lịch sử); cập nhật tiến độ; Dashboard; thống kê.

**Sinh viên:** tra cứu đề tài (tìm kiếm, lọc, sắp xếp, phân trang), đăng ký đề tài, hủy đăng ký chờ duyệt, theo dõi kết quả, xem GVHD hiện tại và lịch sử phân công của mình, cập nhật email/số điện thoại cá nhân.

**Nâng cao:** upload file mô tả đề tài, biểu đồ Chart.js, AJAX, xuất Excel/PDF, gửi email thông báo.

**Hai giá trị luôn tính từ dữ liệu, không lưu:**

- Số chỗ còn = `SoSinhVienToiDa` − số đăng ký `DaDuyet`
- Tải giảng viên = số phân công `DangHuongDan`

## Yêu cầu cài đặt

- .NET 10 SDK
- SQL Server hoặc SQL Server LocalDB (đi kèm Visual Studio)
- Visual Studio 2022+ hoặc VS Code
- Kết nối Internet khi xem biểu đồ (Chart.js tải từ CDN)

## Hướng dẫn chạy

1. Clone repository:

   ```bash
   git clone <URL-repository>
   cd QuanLyDeTaiDoAn_UNETI12_DHTI17A1ND
   ```

2. Cấu hình chuỗi kết nối trong `appsettings.json` (mặc định dùng LocalDB):

   ```json
   "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=QuanLyDeTaiDoAn_UNETI12;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   ```

   Dùng SQL Server thường thì đổi `Server=` cho phù hợp.

3. Tạo cơ sở dữ liệu từ Migration (có thể bỏ qua, vì `Program.cs` tự gọi `Database.Migrate()` và nạp dữ liệu mẫu khi khởi động):

   ```bash
   dotnet ef database update
   ```

4. Chạy ứng dụng:

   ```bash
   dotnet build
   dotnet watch run
   ```

## Cấu hình email (tùy chọn)

Nếu không cấu hình, hệ thống vẫn chạy bình thường và chỉ bỏ qua việc gửi email (có ghi log cảnh báo). Để bật, dùng Gmail App Password và lưu bằng User Secrets, không ghi vào `appsettings.json`:

```bash
dotnet user-secrets set "Email:Username" "your-account@gmail.com"
dotnet user-secrets set "Email:Password" "app-password-16-ky-tu"
```

## Tài khoản kiểm thử

| Tài khoản | Mật khẩu | Vai trò / Ghi chú |
|---|---|---|
| admin1, admin2 | admin123 | Admin |
| sv001 … sv029 | 123456 | Sinh viên |
| sv030 | 123456 | Tài khoản bị khóa (dùng kiểm thử) |

## Dữ liệu mẫu

5 lĩnh vực, 12 giảng viên, 20 đề tài, 30 sinh viên, 2 Admin, 27 đăng ký (15 đã duyệt, 6 chờ duyệt, 3 từ chối, 3 đã hủy), phân công đủ các tình huống: giảng viên còn tải, còn 1 suất, đạt giới hạn, có 1 trường hợp đổi GVHD và 1 đăng ký hoàn thành. Dữ liệu chỉ nạp khi bảng `TaiKhoans` còn trống.

Muốn nạp lại từ đầu: `dotnet ef database drop` rồi chạy lại ứng dụng.

## Cấu trúc thư mục

```
Controllers/     Controller theo từng module
Models/          Entity (Data Annotation)
ViewModels/      ViewModel cho danh sách, form, thống kê
Views/           Razor View
Data/            ApplicationDbContext, SeedData
Migrations/      Lịch sử Migration (InitialCreate, ThemTrangThaiThucHienDangKy, ThemFileMoTaDeTai)
Services/        NghiepVuService (LINQ nghiệp vụ), XuatFileService, EmailService
Filters/         YeuCauDangNhap, YeuCauAdmin
Middleware/      KiemTraPhienDangNhapMiddleware
Helpers/         Hằng số trạng thái, nhãn hiển thị
Extensions/      Truy vấn dùng chung, upload file, xuất Excel
```

## Quy ước làm việc nhóm

- Commit theo mẫu: `[Mã SV] [Module] Nội dung công việc`
- Mỗi file mã nguồn ghi đầu file: họ tên, mã sinh viên, nội dung thực hiện
- Thay đổi Entity phải tạo Migration mới, không xóa lịch sử Migration

## Ghi chú

- Mật khẩu hiện lưu dạng thô (phục vụ bài tập); hướng cải tiến là băm mật khẩu hoặc dùng ASP.NET Core Identity.
- Thư mục `UploadedFiles/` (file mô tả đề tài) nằm ngoài `wwwroot`, chỉ tải được qua action `TaiFile` khi đã đăng nhập.
