using Cyb_mcfr.Data;
using Cyb_mcfr.Interfaces;
using Cyb_mcfr.Models;
using Cyb_mcfr.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cyb_mcfr
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("cyberbezpieczenstwo"));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IActivityService, ActivityService>();
            builder.Services.AddScoped<IRepositoryService<Activity>, RepositoryService<Activity>>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

            #region session troubleshooting part 1
            builder.Services.AddDistributedMemoryCache(); // This is used for session state
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(999); // to jest głupie i tylko tu przykleiłem bo stefan mi takie coś dał. wek!
            });
            #endregion

            //builder.Services.Configure<IdentityOptions>(options => //ten fragment kodu powoduje błąd przy wierszu 98 (16.10)
            //{
            //    // grupa 6 (14 znaków, cyfry)
            //    options.Password.RequireDigit = true;
            //    options.Password.RequiredLength = 14;
            //});


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession(); //session troubleshooting part 2

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var roles = new[] { "Operator", "Admin", "Moderator", "User" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await InitUsersStatically(app);

            app.Run();
        }

        private static async Task InitUsersStatically(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                //User 0 (Admin)
                string email = "admin@admin.com";
                string password = "Admin!23";

                if (await userManager.FindByNameAsync(email) == null)
                {
                    var adminUser = new ApplicationUser();
                    adminUser.UserName = email;
                    adminUser.Email = email;
                    adminUser.EmailConfirmed = true;
                    adminUser.PasswordValidity = DateTime.MaxValue;

                    await userManager.CreateAsync(adminUser, password);

                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.AddToRoleAsync(adminUser, "User");
                }

                // User 1 (Moderator)
                string email1 = "moderator1@example.com";
                string password1 = "Moderator1!23";

                if (await userManager.FindByNameAsync(email1) == null)
                {
                    var modUser = new ApplicationUser();
                    modUser.UserName = email1;
                    modUser.Email = email1;
                    modUser.EmailConfirmed = true;
                    modUser.PasswordValidity = DateTime.MaxValue;

                    await userManager.CreateAsync(modUser, password1);
                    await userManager.AddToRoleAsync(modUser, "Moderator");
                }

                // User 2 (Moderator)
                string email2 = "moderator2@example.com";
                string password2 = "Moderator2!23";

                if (await userManager.FindByNameAsync(email2) == null)
                {
                    var modUser = new ApplicationUser();
                    modUser.UserName = email2;
                    modUser.Email = email2;
                    modUser.EmailConfirmed = true;
                    modUser.PasswordValidity = DateTime.MaxValue;

                    await userManager.CreateAsync(modUser, password2);
                    await userManager.AddToRoleAsync(modUser, "Moderator");
                }

                // User 3 (Operator)
                string email3 = "operator@example.com";
                string password3 = "Operator1!23";

                if (await userManager.FindByNameAsync(email3) == null)
                {
                    var OpUser = new ApplicationUser();
                    OpUser.UserName = email3;
                    OpUser.Email = email3;
                    OpUser.EmailConfirmed = true;
                    OpUser.PasswordValidity = DateTime.MaxValue;

                    await userManager.CreateAsync(OpUser, password3);
                    await userManager.AddToRoleAsync(OpUser, "Operator");
                }

                // User 4 (User)
                string email4 = "user4@example.com";
                string password4 = "UserPass123!";

                if (await userManager.FindByNameAsync(email4) == null)
                {
                    var user = new ApplicationUser();
                    user.UserName = email4;
                    user.Email = email4;
                    user.EmailConfirmed = true;
                    user.PasswordValidity = DateTime.MaxValue;

                    await userManager.CreateAsync(user, password4);
                    await userManager.AddToRoleAsync(user, "User");
                }

                // User 5 (User)
                string email5 = "user5@example.com";
                string password5 = "UserPass123!";

                if (await userManager.FindByNameAsync(email5) == null)
                {
                    var user = new ApplicationUser();
                    user.UserName = email5;
                    user.Email = email5;
                    user.EmailConfirmed = true;
                    user.PasswordValidity = DateTime.MaxValue;

                    await userManager.CreateAsync(user, password5);
                    await userManager.AddToRoleAsync(user, "User");
                }

                // User 6 (User)
                string email6 = "user6@example.com";
                string password6 = "UserPass123!";

                if (await userManager.FindByNameAsync(email6) == null)
                {
                    var user = new ApplicationUser();
                    user.UserName = email6;
                    user.Email = email6;
                    user.EmailConfirmed = true;
                    user.PasswordValidity = DateTime.MaxValue;

                    await userManager.CreateAsync(user, password6);
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
        }
    }
}