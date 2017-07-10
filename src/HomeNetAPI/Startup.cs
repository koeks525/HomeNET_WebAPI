using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using HomeNetAPI.Repository;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using HomeNetAPI.Services;
using Microsoft.AspNetCore.Mvc;
using HomeNetAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace HomeNetAPI
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; set; }
        
        public Startup(IHostingEnvironment environment)
        {
            var builder = new ConfigurationBuilder().SetBasePath(environment.ContentRootPath).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //Source: PluralSight API Security (https://app.pluralsight.com/player?course=aspdotnetcore-implementing-securing-api&author=shawn-wildermuth&name=aspdotnetcore-implementing-securing-api-m7&clip=3&mode=live )
            services.AddMvc(
                //options => options.Filters.Add(new RequireHttpsAttribute())
                );
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "HomeNET API", Version = "v1.31", Contact = new Swashbuckle.AspNetCore.Swagger.Contact { Name = "Okuhle Ngada", Email = "okuhle.ngada@outlook.com", Url = "http://www.homenet.net.za" }, Description = "HomeNET API for data communication. These calls are consumed by the Android client." }));
            services.AddDbContext<HomeNetContext>(options => options.UseSqlServer(Configuration.GetConnectionString("HomeNetDatabase")));   
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ICountryRepository, CountryRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IHouseRepository, HouseRepository>();
            services.AddTransient<IKeyRepository, KeyRepository>();
            services.AddTransient<IHouseImageRepository, HouseImageRepository>();
            services.AddTransient<IHousePostRepository, HousePostRepository>();
            services.AddTransient<IOrganizationRepository, OrganizationRepository>();
            services.AddTransient<IHouseMemberRepository, HouseMemberRepository>();
            services.AddTransient<IAnnouncementRepository, AnnouncementRepository>();
            services.AddTransient<IHousePostMetaDataRepository, HousePostMetaDataRepository>();
            services.AddTransient<IFlaggedPostRepository, FlaggedPostsRepository>();
            services.AddTransient<ICryptography, Cryptography>();
            services.AddTransient<IImageProcessor, ImageProcessor>();
            services.AddTransient<IMailMessage, MailMessage>();
            services.AddTransient<IFirebaseMessagingService, FirebaseMessagingService>();
            services.AddIdentity<User, IdentityRole<int>>().AddEntityFrameworkStores<HomeNetContext, int>().AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                //Set up the initial password stuff
               options.User.RequireUniqueEmail = true;
               options.Password.RequireDigit = true;
               options.Password.RequiredLength = 8;
               options.Password.RequireLowercase = true;
               options.Password.RequireUppercase = true;
               options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
               options.Lockout.MaxFailedAccessAttempts = 10;
               options.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()
               {
                   OnRedirectToLogin = (context) =>
                   {
                       if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == 200)
                       {
                           context.Response.StatusCode = 401;
                       }

                       return Task.CompletedTask;
                  }, 

                   OnRedirectToAccessDenied = (context) =>
                   {
                       if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == 200)
                      {
                          context.Response.StatusCode = 403;
                      }

                       return Task.CompletedTask;
                   }
               };
            });
            
            services.AddAutoMapper();
            
            //dont forget the transient!


        }   
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, HomeNetContext context)
        {
            //Source: https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro#add-code-to-initialize-the-database-with-test-data 

            app.UseIdentity();
            var certificate = new X509Certificate2("Certificates/homenet.pfx", "Okuhle*1994");
            var key = new SymmetricSecurityKey(certificate.GetPublicKey());
            app.UseJwtBearerAuthentication(new JwtBearerOptions()
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = "https://www.homenet.net.za",
                    ValidAudience = "https://www.homenet.net.za",
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true

                }
            });
            app.UseStaticFiles();
            
            app.UseSwagger();
            app.UseSwaggerUi(c => c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "HomeNet API v1"));
            //DBInitializer.Initialize(context);
            app.UseMvc();
        }
    }
}
