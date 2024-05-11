using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email đã được sử dụng chưa
                if (_context.AplicationUser.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(model);
                }

                // Tạo một đối tượng người dùng mới
                var user = new ApplicationUser
                {
                    Name = model.Name,
                    UserName = model.UserName,
                    Email = model.Email,
                    /*PasswordHash = HashPassword(model.Password)*/ // Hash mật khẩu trước khi lưu vào cơ sở dữ liệu
                    PasswordHash = model.Password // Hash mật khẩu trước khi lưu vào cơ sở dữ liệu
                };

                // Lưu người dùng mới vào cơ sở dữ liệu
                _context.AplicationUser.Add(user);
                await _context.SaveChangesAsync();

                // Redirect đến trang đăng nhập hoặc thực hiện các hành động khác sau khi đăng ký thành công
                return RedirectToAction("Login", "Account"); // Thay "Login" và "Account" bằng tên action và controller tương ứng trong ứng dụng của bạn
            }
            return View(model);
        }

        // Hash mật khẩu sử dụng MD5 với salt
        private string HashPassword(string password)
        {
            // Tạo một salt ngẫu nhiên
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Tạo một đối tượng hash MD5
            using (MD5 md5 = MD5.Create())
            {
                // Kết hợp salt và mật khẩu thành một mảng byte
                byte[] combinedBytes = new byte[salt.Length + password.Length];
                Array.Copy(salt, 0, combinedBytes, 0, salt.Length);
                Array.Copy(Encoding.UTF8.GetBytes(password), 0, combinedBytes, salt.Length, password.Length);

                // Mã hóa mảng byte đã kết hợp thành một chuỗi hash
                byte[] hashData = md5.ComputeHash(combinedBytes);

                // Chuyển đổi salt thành chuỗi hexa
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < salt.Length; i++)
                {
                    stringBuilder.Append(salt[i].ToString("x2"));
                }
                string saltString = stringBuilder.ToString();

                // Chuyển đổi mảng byte đã băm thành chuỗi hexa
                stringBuilder.Clear();
                for (int i = 0; i < hashData.Length; i++)
                {
                    stringBuilder.Append(hashData[i].ToString("x2"));
                }

                // Trả về salt và chuỗi hash đã băm, cách nhau bởi dấu hai chấm
                return saltString + ":" + stringBuilder.ToString();
            }
        }

        // Kiểm tra mật khẩu đã nhập có khớp với mật khẩu đã lưu không
        private bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            // Tách salt và hash từ chuỗi đã lưu
            string[] parts = hashedPassword.Split(':', 2);
            byte[] salt = new byte[parts[0].Length / 2];
            for (int i = 0; i < salt.Length; i++)
            {
                salt[i] = Convert.ToByte(parts[0].Substring(i * 2, 2), 16);
            }
            string hash = parts[1];

            // Kết hợp salt và mật khẩu đã nhập
            byte[] combinedBytes = new byte[salt.Length + providedPassword.Length];
            Array.Copy(salt, 0, combinedBytes, 0, salt.Length);
            Array.Copy(Encoding.UTF8.GetBytes(providedPassword), 0, combinedBytes, salt.Length, providedPassword.Length);

            // Mã hóa mảng byte đã kết hợp thành một chuỗi hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashData = md5.ComputeHash(combinedBytes);

                // So sánh chuỗi hash đã tính toán với chuỗi hash đã lưu
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hashData.Length; i++)
                {
                    stringBuilder.Append(hashData[i].ToString("x2"));
                }
                return stringBuilder.ToString() == hash;
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra thông tin đăng nhập
                var user = _context.AplicationUser.FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);

                if (user != null)
                {
                    // Tạo các claims cho người dùng
                    var claims = new[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    // Các claims khác nếu cần thiết
                    };

                    // Tạo identity cho người dùng
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Tạo principal từ identity
                    var principal = new ClaimsPrincipal(identity);

                    // Đăng nhập người dùng
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Điều hướng người dùng đến trang chính sau khi đăng nhập thành công
                    return RedirectToAction("Index", "Home"); // Thay "Index" và "Home" bằng tên action và controller tương ứng trong ứng dụng của bạn
                }

                // Hiển thị thông báo lỗi nếu thông tin đăng nhập không hợp lệ
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            // Hiển thị lại trang đăng nhập nếu dữ liệu không hợp lệ
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chính sau khi logout
        }
    }
}
