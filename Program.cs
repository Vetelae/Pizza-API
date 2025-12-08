using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Services;
using Scalar.AspNetCore;

namespace Pizza_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Services
            builder.Services.AddScoped<IPizzaService, PizzaService>();
            builder.Services.AddScoped<IOrderService, OrderService>();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                
                app.MapScalarApiReference(options =>
                {
                    options.Title = "Pizza API";
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

