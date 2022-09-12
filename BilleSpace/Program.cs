using BilleSpace.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BilleSpace.Domain.Commands;
using BilleSpace.Domain.Queries;

namespace BilleSpace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<BilleSpaceDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add services to the container.      

            builder.Services.AddMediatR(typeof(ManageOfficeCommandHandler));
            builder.Services.AddMediatR(typeof(LoadReservationsQueryHandler));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options => { 
                options.AddPolicy("front", builder => { 
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin(); 
                }); 
            });

            var app = builder.Build();

            app.UseCors("front");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                });

                app.Map("/swagger/index.html", () => "This endpoint requires authorization")
                   .RequireAuthorization(new AuthorizeAttribute
                   {
                       AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme,
                   });
            }


            app.MapControllers();

            app.Run();
        }
    }
}