using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Amber.Music.Api.Services;
using Amber.Music.Domain.Services;
using Amber.Music.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amber.Music.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
            services.AddTransient<AggregatorProcess>();            
            
            services.AddSingleton(new HttpClient());
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
