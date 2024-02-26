
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using TaskAppl.DataAccess;
using TaskAppl.StartupExtensions;

namespace TaskAppl
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Метод вызывается средой выполнения. Используется для добавления служб в контейнер.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public void ConfigureServices(IServiceCollection services)
        {
            string? connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString, s => s.MigrationsAssembly("TaskAppl.DataAccess")),
                ServiceLifetime.Transient
            );

            services.AddCors();
            services.AddMvc().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

            /*
            // Конфигурация для возможной JWT авторизации
            // AppSettings - настройки с ключами
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                };
            });
            */

            services.AddHttpContextAccessor();
            services.AddServices();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Version = "v1.0",
                        Title = "TASK APPL API",
                        Description = "Web API для приложения TASK APPL",
                    }
                );

                var basePath = Directory.GetCurrentDirectory();
                var xmlPath = Path.Combine(basePath, "taskappl.xml");
                c.IncludeXmlComments(xmlPath);
            });


            services.AddDirectoryBrowser();
            services.AddHttpClient();
        }

        /// <summary>
        /// Метод вызывается средой выполнения. Используется для настройки конвейера HTTP-запросов. 
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="env">IWebHostEnvironment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.DocExpansion(DocExpansion.None);
                });
            }
            else
            {
                #region Start auto migration

                var context = app.ApplicationServices.GetService<ApplicationDbContext>();
                context?.Database.Migrate();

                #endregion

                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }


            app.UseHttpsRedirection();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            PhysicalFileProvider pfp = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload"));
            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                FileProvider = pfp,
                RequestPath = "/upload"
            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = pfp,
                RequestPath = "/upload"
            });

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Xss-Protection", "1");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
