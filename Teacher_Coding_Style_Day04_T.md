# BÁO CÁO PHÂN TÍCH PHONG CÁCH CODE CỦA THẦY (TEACHER CODING CONVENTIONS)
## Dự án: ACMF (.NET 8 MVC) | Trọng tâm: `day04_T` & Tiến trình `day02` -> `day03_T` -> `day04_T` -> `day07` -> `day08`

---

## I. XÁC NHẬN: `day04_T` CÓ PHẢI LÀ CÁCH VIẾT CỦA THẦY KHÔNG?

**Khẳng định:** **CHẮC CHẮN 100% ĐÂY LÀ CODE CỦA THẦY (TEACHER BASELINE).**

### Các bằng chứng xác thực trong dự án:
1. **Quy ước đặt tên thư mục:** Hậu tố `_T` đại diện cho **Teacher** (`day03_T`, `day04_T`), đối lập với `_S` đại diện cho **Student** (`day05_S`).
2. **Cấu hình Connection String (`appsettings.json`):** Trong `day04_T/day04/appsettings.json` có hẳn cấu hình:
   ```json
   "TeacherConnection": "Server=.;Database=SaleManager;uid=sa;pwd=123;TrustServerCertificate=true;MultipleActiveResultSets=true"
   ```
3. **Tính nhất quán về phong cách và "Chữ ký code" (Code Signature):** 
   - Cách đặt tên biến private không dùng gạch dưới (`private readonly ApplicationDbContext context;` và gán `this.context = context;`).
   - Cú pháp DataAnnotations đặc trưng (`[Required(ErrorMessage ="Name is required")]` – không có khoảng trắng sau dấu `=`).
   - Quy tắc đặt tên Partial View luôn có tiền tố `_pv` (`_pvProduct.cshtml`, `_pvCreateOrUpdate.cshtml`).
   - Phương thức xử lý File Upload cô đọng, kiểm tra whitelist extension, max size 2MB, `Guid.NewGuid()`.
   - Dùng `[ActionName("Edit")]` và `[HttpPost, ActionName("Delete")]` trong Controller.

---

## II. BẢN ĐỒ TIẾN TRÌNH KIẾN TRÚC CỦA THẦY QUA CÁC BUỔI HỌC

| Buổi | Kiến trúc / Mô hình | Đặc điểm cốt lõi của Thầy |
| :--- | :--- | :--- |
| **`day02`** | **Basic MVC + Scaffolding** | CRUD trực tiếp trong Controller thông qua DbContext. Chưa tách Service/Repo. |
| **`day03_T`** | **Repository Pattern + Service + DTO** | Tách `IStudentRepository`, `IStudentService`, dùng DTOs (`StudentFilterDto`, `StudentScoreDto`), `SelectList`, `TempData["msg"]`. |
| **`day04_T`** | **Service Layer + ViewModel + File Upload** | Bỏ Repository để đơn giản hóa thống kê LINQ, tập trung vào **ViewModel** (`ProductIndexViewModel`, `ProductStatisticsViewModel`), **Partial Views** (`_pv`), **Upload/Delete ảnh**. |
| **`day07`** | **Clean Architecture (Multi-Project)** | Chia tách dự án thành 4 layers: `Domain`, `Infrastructure`, `Services`, `Presentation`. |
| **`day08`** | **Unit of Work + Generic Repository + AutoMapper** | Mô hình chuẩn doanh nghiệp: `IUnitOfWork`, `IMapper`, `Tuple` kết quả `(bool IsSuccess, string Message)`. |

---

## III. PHÂN TÍCH CHI TIẾT PHONG CÁCH CODE CỦA THẦY TẠI `day04_T`

### 1. Cấu trúc tổ chức thư mục (Folder Structure)
Thầy tổ chức mã nguồn rõ ràng theo trách nhiệm:
```text
day04/
├── Controllers/         # Chứa Controller điều hướng (ProductController, HomeController)
├── Data/                # DbContext kết nối SQL Server (ApplicationDbContext)
├── Models/              # Entity biểu diễn bảng Database + DataAnnotations (Product)
├── Services/            # Business Logic + Xử lý File Upload (IProductService, ProductService)
├── ViewModels/          # Chứa dữ liệu hiển thị, thống kê, tìm kiếm (ProductIndexViewModel, ProductStatisticsViewModel)
├── Views/
│   └── Product/         # Razor views (Index, Create, Edit, Delete, _pvProduct, _pvCreateOrUpdate)
└── wwwroot/
    └── uploads/products/# Thư mục lưu trữ hình ảnh vật lý
```

---

### 2. Phong cách Thiết kế Entity & DataAnnotations (`Models/Product.cs`)
Thầy sử dụng triệt để DataAnnotations để validation ngay từ Model:
```csharp
[Table("Product")]
public class Product
{
    public int Id { get; set; }
    
    [Required(ErrorMessage ="Name is required")]
    [StringLength(100,MinimumLength =2, ErrorMessage ="Name from 2 to 100")]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Category is required")]
    [StringLength(50,ErrorMessage = "Category max 50 characters")]
    public string? Category { get; set; }

    [Range(1000,100000000,ErrorMessage = "Price from 1,000 to 100,000,000 VND")]
    public decimal Price { get; set; }
    
    [Range(1,100,ErrorMessage = "Quantity from 1 to 100")]
    public int Quantity { get; set; }

    public string? ImagePath { get; set; }
}
```
* **Thói quen của thầy:**
  - Viết `[Table("Tên_Bảng")]` rõ ràng.
  - Thông báo lỗi rõ ràng, trực diện bằng tiếng Anh (`ErrorMessage = "..."`).
  - Cho phép null bằng nullable reference types `string?`.
  - Khai báo khoảng giá trị và số lượng bằng `[Range]`.

---

### 3. Phong cách Tầng Dịch vụ (Service Layer & File Handling)
Tại `day04_T`, toàn bộ xử lý nghiệp vụ, database và file I/O được đóng gói trong `ProductService`:

#### A. Dependency Injection & Fields
- Thầy **không** dùng tiền tố gạch dưới `_` cho private readonly fields, mà dùng từ khóa `this.`:
```csharp
private readonly ApplicationDbContext context;
private readonly IWebHostEnvironment environment;
private readonly string[] fileExtensions = { ".gif", ".png", ".jpg" };

public ProductService(ApplicationDbContext context, IWebHostEnvironment environment)
{
    this.context = context;
    this.environment = environment;
}
```

#### B. Xử lý File Upload chuẩn mực (`SaveImageAsync` & `DeleteImage`)
- **Whitelist file extension** + chuyển chữ thường `.ToLowerInvariant()`.
- **Kiểm tra kích thước file:** `image.Length > 2 * 1024 * 1024` (2MB).
- **Tạo thư mục nếu chưa tồn tại:** `Directory.CreateDirectory(uploadFolder)`.
- **Đổi tên file tránh trùng lặp:** `Guid.NewGuid().ToString() + extension`.
- **Đường dẫn trả về dạng Web path:** `"/uploads/products/" + fileName`.
- **Hàm xóa ảnh cũ độc lập:** Kiểm tra `File.Exists(filePath)` trước khi `File.Delete(filePath)`.

```csharp
private async Task<string> SaveImageAsync(IFormFile image)
{
    var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
    if (!fileExtensions.Contains(extension))
    {
        throw new Exception("Only file extension .gif, .png, .jpg");
    }
    if (image.Length > 2 * 1024 * 1024) {
        throw new Exception("Size only 2MB");
    }
    var uploadFolder = Path.Combine(environment.WebRootPath, "uploads", "products");
    if (!Directory.Exists(uploadFolder))
    {
        Directory.CreateDirectory(uploadFolder);
    }
    var fileName = Guid.NewGuid().ToString() + extension;
    var filePath = Path.Combine(uploadFolder, fileName);
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await image.CopyToAsync(stream);
    }
    return "/uploads/products/" + fileName;
}
```

#### C. LINQ Truy vấn & Thống kê Tối ưu
Thầy luôn áp dụng `.AsNoTracking()` khi đọc dữ liệu để tăng hiệu năng, kết hợp tổng hợp Dashboard trong 1 lượt gọi:
```csharp
public async Task<ProductIndexViewModel> GetIndexDataAsync(string? keyword)
{
    var query = context.Products.AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(keyword))
    {
        query = query.Where(p => p.Name!.Contains(keyword.Trim()) 
                              || p.Category!.Contains(keyword.Trim()));
    }
    var products = await query.OrderBy(p => p.Name).ToListAsync();
    var statistics = new ProductStatisticsViewModel
    {
        TotalProducts = await context.Products.CountAsync(),
        TotalQuantity = await context.Products.SumAsync(p => (int?)p.Quantity) ?? 0,
        TotalInventoryValue = await context.Products.SumAsync(p => (decimal?)p.Price * p.Quantity) ?? 0,
        AveragePrice = await context.Products.AverageAsync(p => (decimal?)p.Price) ?? 0,
        TotalCategories = await context.Products.Select(p => p.Category).Distinct().CountAsync()
    };
    return new ProductIndexViewModel
    {
        Keyword = keyword,
        Products = products,
        Statistics = statistics
    };
}
```

---

### 4. Phong cách Tầng Controller (`ProductController.cs`)
Controller của thầy viết rất gãy gọn, chỉ giữ vai trò điều hướng:
```csharp
[HttpGet]
public async Task<IActionResult> Index(string? keyword)
{
    var model = await productService.GetIndexDataAsync(keyword);
    return View(model);
}

[HttpPost]
public async Task<IActionResult> Create(Product newProduct, IFormFile? image)
{
    if (!ModelState.IsValid) 
    {
        return View(newProduct);
    }
    try
    {
        await productService.CreateAsync(newProduct, image);
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("images", ex.Message);
        return View(newProduct);
    }
}
```
* **Điểm nổi bật:**
  - Nhận `IFormFile? image` riêng biệt trong parameter của Action thay vì nhồi vào Model.
  - Bắt lỗi từ Service qua khối `try-catch` và đẩy vào `ModelState.AddModelError(...)`.
  - Sử dụng `[ActionName("Edit")]` cho hàm `Update` và `[HttpPost, ActionName("Delete")]` cho `DeleteConfirm`.

---

### 5. Phong cách Tầng Giao diện (Razor Views & Partial Views)

#### A. Đặt tên và tái sử dụng qua Partial View:
- Tiền tố `_pv`: 
  - `_pvProduct.cshtml`: Thống kê Card KPI tổng hợp (Total Products, Total Quantity, Inventory Value, Total Categories).
  - `_pvCreateOrUpdate.cshtml`: Dùng chung các ô nhập liệu `Name`, `Category`, `Price`, `Quantity` cho cả Form Create và Form Edit.
- Cú pháp nhúng hiện đại:
  ```html
  <partial name="_pvProduct" model="Model.Statistics"/>
  <partial name="_pvCreateOrUpdate"/>
  ```

#### B. Định dạng giao diện:
- Dùng Bootstrap 5 Cards, Tables, Buttons (`btn-success`, `btn-warning`, `btn-danger`, `btn-primary`).
- Định dạng tiền tệ trực tiếp: `@Model.TotalInventoryValue.ToString("N0") VNĐ`.
- Kiểm tra ảnh tồn tại trước khi render thẻ `<img>`, nếu không có hiển thị `<span>No Image</span>`.

---

## IV. BẢNG TỔNG HỢP CÁC NGUYÊN TẮC CODE CHUẨN CỦA THẦY ĐỂ TỰ ÁP DỤNG

| Hạng mục | Quy tắc của Thầy | Ví dụ mẫu |
| :--- | :--- | :--- |
| **Field Injection** | `private readonly T name;` + gán `this.name = name;` | `this.context = context;` |
| **Async/Await** | Luôn dùng `Task<T>`, hậu tố `Async` cho mọi I/O | `GetByIdAsync(int id)` |
| **Query EF Core** | Dùng `.AsNoTracking()` cho các truy vấn Read-only | `context.Products.AsNoTracking()` |
| **Validation** | DataAnnotations đầy đủ `[Required]`, `[StringLength]`, `[Range]` | `[Required(ErrorMessage ="...")]` |
| **File Upload** | Whitelist ext + 2MB + Guid + WebRootPath + `DeleteImage` | `Guid.NewGuid().ToString() + ext` |
| **ViewModel** | Tách riêng ViewModel cho View phức tạp / thống kê | `ProductIndexViewModel` |
| **Partial View** | Đặt tên theo dạng `_pv[Feature].cshtml` | `_pvCreateOrUpdate.cshtml` |
| **Action Mapping** | Dùng `[ActionName("...")]` để khớp URL MVC chuẩn RESTful | `[HttpPost, ActionName("Delete")]` |
| **Error Handling** | Service ném `Exception`, Controller bắt đẩy vào `ModelState` | `ModelState.AddModelError("images", ex.Message)` |

---
*Tài liệu được tổng hợp và đối chiếu tự động từ các source code `day02`, `day03_T`, `day04_T`, `day07`, `day08` trong workspace ACMF.*
