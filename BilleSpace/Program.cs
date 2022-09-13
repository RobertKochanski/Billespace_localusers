using BilleSpace.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BilleSpace.Domain.Queries;
using Microsoft.AspNetCore.Identity;
using BilleSpace.Infrastructure.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using BilleSpace.Domain.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using BilleSpace.Domain.Commands.Offices;

namespace BilleSpace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            var tokenKey = builder.Configuration.GetValue<string>("TokenKey");
            char[] tokenKeyArray = tokenKey.ToCharArray();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<BilleSpaceDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<BilleSpaceDbContext>()
                 .AddDefaultTokenProviders();

            builder.Services.AddIdentityServer()
                .AddApiAuthorization<User, BilleSpaceDbContext>()
                .AddDeveloperSigningCredential();

            var authenticationSettings = new AuthenticationSettings();
            builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
            builder.Services.AddSingleton(authenticationSettings);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyArray)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

            // Add services to the container.      

            builder.Services.AddMediatR(typeof(ManageOfficeCommandHandler));
            builder.Services.AddMediatR(typeof(LoadReservationsQueryHandler));
            builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("OnlyReceptionists", policy =>
                    policy.RequireClaim(ClaimTypes.Role, "True"));
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Name = "Authorization",
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                    });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        new string[] { }
                    },
                });
            });

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
            app.UseIdentityServer();
            app.UseAuthorization();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                });
            }

            app.MapControllers();

            app.Run();
        }
    }
}