using Daily_Task_Manager.Data;
using Daily_Task_Manager.Hubs;
using Daily_Task_Manager.Models.Data_Models;
using Daily_Task_Manager.Models.Interfaces;
using Daily_Task_Manager.Models.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Daily_Task_Manager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Identity with roles
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
                options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddSignalR();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireClaim("Role", "Admin"));
                options.AddPolicy("UserOnly", policy =>
                    policy.RequireClaim("Role", "User"));
            });

            var app = builder.Build();

            // Configure environment
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string[] roles = { "Admin", "User" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                var adminEmail = "admin@demo.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin, "Admin@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                        await userManager.AddClaimAsync(admin, new System.Security.Claims.Claim("Role", "Admin"));
                    }
                }

                var testEmail = "user@demo.com";
                var testUser = await userManager.FindByEmailAsync(testEmail);

                if (testUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = testEmail,
                        Email = testEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, "User@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                        await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Role", "User"));
                    }
                }
            }



            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapHub<TaskHub>("/taskHub");
            
            app.Run();
        }
    }
}
