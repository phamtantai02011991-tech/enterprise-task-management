# HỌC TẬP VÀ PHÂN TÍCH KIẾN THỨC ASP.NET CORE & MVC

> **Tài liệu học tập:** Web Programming Using ASP.NET CORE and MVC (Aptech 2024 - 15 Sessions)  
> **Dự án thực hành:** `day04` (ASP.NET Core MVC Product Management)

---

## 📚 PHẦN 1: TỔNG HỢP KIẾN THỨC 15 SESSIONS (KNOWLEDGE BASE)

### Session 1: Introduction to ASP.NET & ASP.NET Core
* Lịch sử phát triển từ ASP (1996) -> ASP.NET Web Forms (2002) -> ASP.NET MVC -> ASP.NET Core (2016-nay).
* ASP.NET Core là framework mã nguồn mở, đa nền tảng (Cross-platform: Windows, macOS, Linux), tối ưu hiệu năng và linh hoạt nhờ NuGet Packages.
* Các tính năng mới của ASP.NET Core 7.0+: Minimal APIs, Blazor WebAssembly, SignalR real-time, gRPC JSON transcoding, MVC Nullable state checking.

### Session 2: ASP.NET Web Forms, Controls & Events
* Cơ chế Web Forms: file `.aspx` (UI) và file code-behind `.aspx.cs`.
* Các nhóm Server Controls: HTML Server Controls, Web Server Controls, Validation Server Controls.
* Vòng đời trang (Page Lifecycle): `Initialization` -> `Loading` -> `PreRendering` -> `Saving` -> `Rendering` -> `Unloading`.
* Phân biệt Postback (refresh trang) và Callback (gọi AJAX bất đồng bộ).

### Session 3: ADO.NET & Entity Framework (EF Core)
* Các công nghệ truy xuất dữ liệu: DAO -> RDO -> ADO -> ADO.NET -> Entity Framework (EF).
* ADO.NET Components: `SqlConnection`, `SqlCommand`, `SqlDataAdapter`, `SqlDataReader`, `DataSet`.
* EF Workflows:
  1. **Database-First:** Tạo DB trước, tự sinh Code.
  2. **Code-First:** Viết C# Class trước, dùng Migration sinh bảng DB.
  3. **Model-First:** Vẽ sơ đồ EDMX.
* Đăng ký `DbContext` & Migration trong ASP.NET Core: `Add-Migration`, `Update-Database`.

### Session 4: Client-side Development Using ASP.NET Core MVC
* Layout View: Sử dụng `_Layout.cshtml` làm trang khung (Header, Footer, Nav) và `@RenderBody()` để hiển thị trang con.
* Cấu hình Layout mặc định qua `_ViewStart.cshtml`.
* Data Annotations: `[Required]`, `[Range]`, `[MinLength]`, `[MaxLength]`, `[DataType]`.
* Routing & Dependency Injection (DI) cơ bản.
* Phân biệt Client-side Validation (nhanh, dùng JS/jQuery Unobtrusive) và Server-side Validation (an toàn, kiểm tra `ModelState.IsValid`).

### Session 5: More on ASP.NET MVC and Core MVC
* Phân quyền (Authorization): Role-based (`[Authorize(Roles="Admin")]`), View-based (`@inject`), Policy-based.
* Action Selectors: `[ActionName]`, `[NonAction]`, `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`.
* ASP.NET Helpers: Inline Helpers (`@helper`), Built-in Helpers (`Html.TextBoxFor`), Custom Helpers.
* Filters Pipeline: Authorization Filter -> Resource Filter -> Action Filter -> Exception Filter -> Result Filter.
* Partial Views: `@Html.Partial()` & `@Html.RenderPartial()`.

### Session 6: Action Methods & Advanced Concepts
* So sánh Web API (RESTful - JSON/XML, nhẹ, linh hoạt) và SOAP (XML, WS-Security, nặng).
* Route Config: Dạng mẫu `/{area}/{controller}/{action}/{id}`.
* Bundling & Minification: Gom nhóm và nén file CSS/JS để tối ưu tốc độ tải trang.
* Areas: Phân chia dự án lớn thành các module độc lập (Student Area, Admin Area, etc.).
* Truyền dữ liệu Controller -> View: `ViewData` (Dictionary), `ViewBag` (Dynamic wrapper), `TempData` (chuyển tiếp qua Redirect), Strongly Typed View.

### Session 7: Enhancements in ASP.NET Core
* Razor Pages (`@page`): Đơn giản hóa việc tạo trang UI mà không cần Controller cầu kỳ.
* MVC Model Binding: Tự động ánh xạ HTTP request data vào tham số C# (`[BindNever]`, `[BindRequired]`).
* C# Record Types: `public record Product(...)` cho truyền dữ liệu immutable.
* `System.Text.Json`: Thư viện xử lý JSON mã hóa UTF-8 tốc độ cao.
* Garbage Collection (GC) & Dump Debugging (`dotnet-dump`).

### Session 8: .NET Core Architecture & Kestrel Web Server
* Entry Point: Hàm `Main()` trong `Program.cs` khởi tạo `WebApplicationBuilder` và chạy Web Host.
* Kestrel Web Server: Web Server nội bộ, đa nền tảng, hiệu năng cao, thường chạy sau Reverse Proxy (IIS/Nginx).
* OWIN (Open Web Interface for .NET): Tách rời ứng dụng khỏi Web Server.
* GraphQL vs REST API.

### Session 9 & 10: Onion Architecture (Kiến trúc Củ Hành I & II)
* Giải quyết triệt để Tight Coupling (phụ thuộc chặt) bằng Dependency Inversion Principle (DIP) & IoC.
* Cấu trúc 4 lớp chính:
  1. **Domain Layer (Core):** POCO Entities (`User`, `BaseEntity`), Interfaces.
  2. **Infrastructure Layer:** `DbContext`, `GenericRepository<T>`, Data Access.
  3. **Service Layer:** Business Logic (`UserService`).
  4. **UI / Web API Layer:** Controllers, DTOs, ViewModel.
* Security & Monitoring cho từng lớp (Parametrized Queries, Model Validation, Serilog/ELK).

### Session 11: Overview of Fluent Model & AutoMapper
* Fluent API: Ghi đè `OnModelCreating` trong `DbContext` để cấu hình Mapping tỉ mỉ hơn Data Annotations (`HasKey`, `HasOne`, `WithMany`, `HasForeignKey`).
* AutoMapper: Thư viện tự động ánh xạ giữa Model & DTO/ViewModel. Đăng ký `builder.Services.AddAutoMapper(...)`, inject `IMapper`.

### Session 12: Token Authentication (JWT)
* Cấu trúc JWT: Header (Thuật toán) . Payload (Claims) . Signature (Chữ ký).
* Cấu hình JwtBearer trong `Program.cs`: `AddAuthentication().AddJwtBearer(...)`.
* Tạo Token bằng `JwtSecurityTokenHandler` và bảo vệ API bằng `[Authorize]`.

### Session 13: Deployment and Unit of Work (UoW) Patterns
* Repository Pattern: Lớp trung gian giữa Data Layer và Business Layer.
* Unit of Work (UoW): Quản lý tập hợp các Repository chung một `DbContext` để đảm bảo **Atomic Transaction** (tất cả cùng thành công hoặc rollback).

### Session 14: User Login and ASP.NET Core Identity
* ASP.NET Core Identity: Khung quản lý User, Role, Password Hashing (PBKDF2/SHA256 + Salt), Claims.
* Quản lý Session State (`AddSession`, `UseSession`).
* Xây dựng luồng Đăng ký (`RegisterVM`), Đăng nhập (`LoginVM`), Đăng xuất.

### Session 15: Publishing and Deploying ASP.NET Core Applications
* Deploy lên IIS (Windows), Azure App Service, Docker Container.
* Cài đặt **.NET Core Hosting Bundle** trên IIS server để xử lý `AspNetCoreModuleV2`.
* Đăng ký Virtual Host và cấu hình file `hosts`.

---

## 🛠️ PHẦN 2: BÁO CÁO PHÂN TÍCH VÀ SỬA LỖI CODE BÀI DAY04

Dự án `day04` là ứng dụng ASP.NET Core MVC quản lý sản phẩm (Product Management System) theo mô hình 3 lớp (Data -> Service -> Controller/View).

### Các lỗi phát hiện và đã được khắc phục:
1. **Khôi phục DbContext chuẩn EF Core (`Data/ApplicationDbContext.cs`):**
   * *Lỗi ban đầu:* Tệp bị gõ sai tên `AplicationDbContext`, kế thừa từ lớp `DbContext` tự tạo ngớ ngẩn làm đè mất EF Core.
   * *Đã sửa:* Đã xóa các class dummy, chuyển sang kế thừa `Microsoft.EntityFrameworkCore.DbContext` và khai báo `DbSet<Product> Products { get; set; }`.
2. **Sửa Interface Service (`Service/IProductService.cs`):**
   * *Lỗi ban đầu:* Khai báo là `public class IProductService` thay vì `public interface`, tên hàm bị gõ sai `GetbuyIdAsync`.
   * *Đã sửa:* Đã chuyển thành `public interface IProductService` đầy đủ các hàm CRUD async.
3. **Sửa Logic xử lý trong `ProductService.cs`:**
   * *Lỗi ban đầu:*
     * Hàm `DeleteImage` bị lỗi logic phủ định `if (!string.IsNullOrEmpty) return;` làm ảnh không bao giờ bị xóa.
     * Hàm `DeleteAsync` và `UpdateAsync` bị ngược điều kiện checking `product != null` gây lỗi crash.
     * Hàm `GetByIdAsync` bị gõ sai cú pháp `FirstOrDefaultAsnync` và phép gán `p.Id = id`.
     * Hàm `GetIndexDataAsync` bị gõ thiếu dấu đóng mở ngoặc `()` ở `AsNoTracking()`, `Keyword.Trim()`.
   * *Đã sửa:* Đã cập nhật lại toàn bộ logic xử lý ảnh, tìm kiếm, lọc theo Keyword, tính toán thống kê (Tổng số lượng, Tổng giá trị kho, Giá trung bình, Số danh mục).
4. **Sửa Controller (`Controllers/ProductController.cs`):**
   * *Lỗi ban đầu:* Sai Namespace (`day4` thay vì `day04`), action `Delete` bị chép nhầm code của `Update`, thiếu đối tượng tham số.
   * *Đã sửa:* Đã chuẩn hóa đầy đủ các Action: `Index`, `Details`, `Create`, `Edit`, `Delete`, `DeleteConfirmed`.
5. **Cấu hình `Program.cs` & Thư viện Test:**
   * Đã bổ sung thư viện **EF Core InMemory Database** để ứng dụng có thể chạy và test trực tiếp ngay lập tức mà không phụ thuộc vào SQL Server cục bộ.
   * Đã đăng ký `AddDbContext` và `AddScoped<IProductService, ProductService>()` vào DI Container.
   * Đã thêm dữ liệu mẫu Seed Data (Laptop Dell XPS 13, iPhone 15 Pro, Bàn phím cơ Keychron K2) để khi ứng dụng khởi chạy lên là có ngay dữ liệu hiển thị.
6. **Bổ sung & Tối ưu hóa Giao diện Views (`Views/Product/`):**
   * Đã sửa file `Index.cshtml` hiển thị bảng chuẩn Bootstrap, thống kê card qua PartialView `_pvProduct.cshtml`, ô tìm kiếm sản phẩm.
   * Đã khởi tạo đầy đủ các View tương tác còn thiếu: `Create.cshtml`, `Edit.cshtml`, `Details.cshtml`, `Delete.cshtml`.

### Kết quả Biên dịch & Kiểm thử (Build & Run Result):
* **Lệnh Build:** `dotnet build` -> **SUCCESS (0 Errors, 0 Warnings)**.
