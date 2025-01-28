using LaptopStoreProject_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LaptopStoreProject_MVC;
using Microsoft.AspNetCore.Identity.UI.Services;
using LaptopStoreProject_MVC.Services;

namespace LaptopStoreProject_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Lấy MailSettings từ cấu hình (appsettings.json)
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var result = builder.Configuration.GetSection("Authentication:Google");
                    options.ClientId = result["ClientId"];
                    options.ClientSecret = result["ClientSecret"];
                    options.CallbackPath = "/signInGoogle";
                });
                //.AddFacebook(options =>
                //            { });
            // Đăng ký dịch vụ gửi email
            builder.Services.AddTransient<IEmailSender, SendMailService>();

            //Đăng ký DbContext
            builder.Services.AddDbContext<LaptopStoreProjectContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("LaptopStoreProject")));

            // //Đăng ký Identity
            builder.Services.AddDefaultIdentity<AppUser>()
                .AddEntityFrameworkStores<LaptopStoreProjectContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                //Thiết lập cấu hình Password
                options.Password.RequireDigit = true; //Bắt buộc phải có số
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;//Bắt buộc phải có ký tự đặc biệt
                options.Password.RequiredLength = 12;

                //Cấu hình lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 5; // Sai pass 5 lần thì khóa
                options.Lockout.AllowedForNewUsers = true;

                //Cấu hình User
                options.User.RequireUniqueEmail = true; //Email là duy nhất

                //Cấu hình login 
                /* options.SignIn.RequireConfirmedPhoneNumber = true;*/ //Xác thực sdt
                options.SignIn.RequireConfirmedEmail = true; //Xác thức email
                options.SignIn.RequireConfirmedAccount = true;
            });

            //Đăng ký Identity
            /*
            builder.Services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<LaptopStoreProjectContext>()
            .AddDefaultTokenProviders();
            */

            //Cookie Authentication
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                //Khi người dùng chưa xác thực
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
                //Khi user chọn vào chức năng phải yêu cầu đăng nhập
                options.AccessDeniedPath = "/Login";
                // Gia hạn cookie nếu còn hoạt động
                options.SlidingExpiration = true;
            });
            

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //đường dẫn mặc định là /Home/Index
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"); 

            app.MapRazorPages(); //Map các Razor pages của Identity 

            app.Run();
        }
    }
}
