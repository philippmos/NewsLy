using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NewsLy.Api.Repositories.Interfaces;
using NewsLy.Api.Settings;
using NewsLy.Api.Services;
using NewsLy.Api.Data;

using DapperRepo = NewsLy.Api.Repositories.Dapper;
using DapperContribRepo = NewsLy.Api.Repositories.DapperContrib;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using NewsLy.Api.Services.Interfaces;

namespace NewsLy.Api
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            AddConfigurationMappings(services);
            AddDependencyInjectionMappings(services);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NewsLy.Api", Version = "v1" });
            });

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(
                jwt => {
                    var key = Encoding.ASCII.GetBytes(Configuration["JwtSettings:Secret"]);

                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = false,
                        ValidateLifetime = true
                    };
                }
            );

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                        .AddEntityFrameworkStores<ApplicationDbContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NewsLy.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddConfigurationMappings(IServiceCollection services)
        {
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
        }

        private void AddDependencyInjectionMappings(IServiceCollection services)
        {
            services.AddScoped<IMailingService, MailingService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<IContactRequestRepository, DapperContribRepo.ContactRequestRepository>();
            services.AddScoped<IRecipientRepository, DapperRepo.RecipientRepository>();
            services.AddScoped<IMailingListRepository, DapperRepo.MailingListRepository>();
            services.AddScoped<ITrackingUrlRepository, DapperRepo.TrackingUrlRepository>();
        }
    }
}
