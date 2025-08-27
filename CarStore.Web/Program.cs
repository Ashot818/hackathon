using CityLens.Services.Abstract;
using CityLens.Services.Concrete;
using CityLens.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace CityLens.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ====== Connection string ======
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? "Server=localhost;Database=CityDb;Integrated Security=True;TrustServerCertificate=True;";

            // ====== DbContext ======
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
                )
            );

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // ====== Identity ======
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            // ====== Controllers & Views ======
            builder.Services.AddControllersWithViews();

            // ====== Custom Services ======
            builder.Services.AddScoped<IPostService, PostService>();

            // ====== Swagger ======
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "City Lens API", Version = "v1" });
            });

            var app = builder.Build();

            // ====== HTTP Request Pipeline ======
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "City Lens API V1");
                    c.RoutePrefix = string.Empty; // Swagger на корне
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // ====== Статические файлы ======
            app.UseStaticFiles(); // wwwroot

            // Добавляем явное отображение wwwroot/uploads
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads")),
                RequestPath = "/uploads"
            });

            app.UseRouting();

            app.UseAuthorization();

            // ====== Map Controllers ======
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.MapRazorPages();

            app.Run();
        }
    }
}
