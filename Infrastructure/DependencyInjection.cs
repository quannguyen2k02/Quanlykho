using Application.IRepositories;
using Application.IServices;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 1. DbContext (MySQL)
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                }
            });

            // 2. Identity (Dùng AddIdentityCore thay vì AddIdentity cho API)
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>() // Bắt buộc phải có để dùng RoleManager
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders(); // Lỗi sẽ biến mất sau khi thêm FrameworkReference

            // 3. Repositories & Services
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ISlipRepository, SlipRepository>();
            services.AddScoped<ISlipService, SlipService>();

            return services;
        }
    }
}