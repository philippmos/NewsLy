using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using NewsLy.Api.Repositories.Interfaces;
using NewsLy.Api.Settings;
using NewsLy.Api.Services;
using NewsLy.Api.Data;

using DapperRepo = NewsLy.Api.Repositories.Dapper;
using DapperContribRepo = NewsLy.Api.Repositories.DapperContrib;

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

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));

            AddDependencyInjectionMappings(services);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NewsLy.Api", Version = "v1" });
            });
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddDependencyInjectionMappings(IServiceCollection services)
        {
            services.AddScoped<IMailingService, MailingService>();

            services.AddScoped<IContactRequestRepository, DapperContribRepo.ContactRequestRepository>();
            services.AddScoped<IRecipientRepository, DapperRepo.RecipientRepository>();
            services.AddScoped<IMailingListRepository, DapperRepo.MailingListRepository>();
            services.AddScoped<ITrackingUrlRepository, DapperRepo.TrackingUrlRepository>();
        }
    }
}
