using Microsoft.EntityFrameworkCore;

namespace Lab5.Infrastructure.Extensions;

public static class ApplicationDbContextExtensions
{
    public static void AddDbEfConnection(this IServiceCollection service, IConfiguration configuration)
    { 
        service.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }
}