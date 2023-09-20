using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using LocalShipper.Data.Models;
using LocalShipper.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;
using System;
using LocalShipper.Service.Services.Helpers;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using LocalShipper.Data.UnitOfWork;
using AutoMapper;

namespace LSAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {


            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Bearer", policy =>
            //    {
            //        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            //        policy.RequireAuthenticatedUser();
            //    });
            //});


            #region JWT
            var jwtSettings = Configuration.GetSection("JwtAuth");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                };
            });

            services.AddDbContext<LocalShipperCPContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DBLocalShipper"));
            });
            #endregion

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Roles.Admin, policy =>
               {
                   policy.RequireRole(Roles.Admin);
               });
                options.AddPolicy(Roles.Staff, policy =>
               {
                   policy.RequireRole(Roles.Staff);
               });
                options.AddPolicy(Roles.Store, policy =>
               {
                   policy.RequireRole(Roles.Store);
               });
                options.AddPolicy(Roles.Brand, policy =>
               {
                   policy.RequireRole(Roles.Brand);
               });
                options.AddPolicy(Roles.Shipper, policy =>
               {
                   policy.RequireRole(Roles.Shipper);
               });
            });

            services.AddScoped<IGenericRepository<Account>, GenericRepository<Account>>();
            services.AddScoped<IShipperService, ShipperService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IBatchService, BatchService>();
            services.AddScoped<IPackageService, PackageService >();

            services.AddAutoMapper(typeof(Startup));


            // Đăng ký LoginService
            services.AddScoped<LoginService>();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LSAPI", Version = "v1" });

                // Thêm khai báo bảo mật Bearer
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema
                   );

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                        securitySchema,
                    new string[] { "Bearer" }
                    }
                }

                );


            });


        }




        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LSAPI v1"));
            

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
