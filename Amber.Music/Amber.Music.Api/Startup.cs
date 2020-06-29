using Amber.Music.Api.Services;
using Amber.Music.Domain;
using Amber.Music.Domain.Services;
using Amber.Music.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace Amber.Music.Api
{
    public class Startup
    {
        private readonly string AllowAllSpecificOrigins = "_allowAll";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllSpecificOrigins,
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddControllers();

            // TODO: convention based
            var appInfoOptions = new ApplicationInfoOptions();
            Configuration.GetSection(ApplicationInfoOptions.SectionName).Bind(appInfoOptions);
            services.AddSingleton(appInfoOptions);

            var lyricsApiOptions = new LyricsApiOptions();
            Configuration.GetSection(LyricsApiOptions.SectionName).Bind(lyricsApiOptions);
            services.AddSingleton(lyricsApiOptions);

            services.AddTransient<IArtistService, MusicBrainzService>();
            services.AddTransient<ILyricsService, LyricsService>();
            services.AddTransient<IWordCounterService, WordCounterService>();
            services.AddTransient<IAggregatorProcess, AggregatorProcess>();

            services.AddSingleton(new HttpClient());
            services.AddSingleton<ICacheService, CacheService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(AllowAllSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
