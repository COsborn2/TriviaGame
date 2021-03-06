using TriviaGame.Data;
using IntelliTect.Coalesce;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TriviaGame.Data.Services.Impl;
using TriviaGame.Data.Services.Interfaces;

namespace TriviaGame.Web
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionName = "DefaultConnection";
            string connString = Configuration.GetConnectionString(connectionName);

            services.AddScoped<ITriviaService, TriviaService>();

            // Add Entity Framework services to the services
            services.AddSingleton(Configuration);
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connString));

            services.AddCoalesce<AppDbContext>();

            services
                .AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

#pragma warning disable CS0618 // Type or member is obsolete
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    // Use a slightly-tweaked version of vue-cli's webpack config to work around a few bugs.
                    ConfigFile = "webpack.config.aspnetcore-hmr.js",
                });
#pragma warning restore CS0618 // Type or member is obsolete
            }

            // Routing
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            var containsFileHashRegex = new Regex(@"\.[0-9a-fA-F]{8}\.[^\.]*$", RegexOptions.Compiled);
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // vue-cli puts 8-hex-char hashes before the file extension.
                    // Use this to determine if we can send a long-term cache duration.
                    if (containsFileHashRegex.IsMatch(ctx.File.Name))
                    {
                        ctx.Context.Response.GetTypedHeaders().CacheControl =
                            new CacheControlHeaderValue { Public = true, MaxAge = TimeSpan.FromDays(30) };
                    }
                }
            });

            // For all requests that aren't to static files, disallow caching.
            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new CacheControlHeaderValue { NoCache = true, NoStore = true, };

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // API fallback to prevent serving SPA fallback to 404 hits on API endpoints.
                endpoints.Map("api/{**any}", ctx => Task.FromResult(ctx.Response.StatusCode = StatusCodes.Status404NotFound));

                endpoints.MapFallbackToController("Index", "Home");

                endpoints.MapHub<GameBoardHub>("/gameboardhub");
            });
        }
    }
}
