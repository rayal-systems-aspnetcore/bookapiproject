using BookApiProject.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace BookApiProject {
    public class Startup {
        public static IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            var connectionString = Configuration["connectionStrings:bookDbConnection"];
            services.AddDbContext<BookDbContext>(c => c.UseSqlServer(connectionString));

            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IReviewerRepository, ReviewerRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            BookDbContext context) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            if (context.Books.Count() == 0) { 
                context.SeedDataContext();
            }
        }
    }
}