using Document_Blockchain.Docusign_;
using Document_Blockchain.Entities;
using Document_Blockchain.IInternalServices;
using Document_Blockchain.InternalService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Text;

namespace Document_Blockchain
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
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("54.219.78.153"));
            });
            services.AddHttpClient();
            services.AddControllers().AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.ContractResolver = new DefaultContractResolver();
                o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                o.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                o.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";
            });

            //manual declaration of mysql version
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));
            //Database Dependencies
            services.AddDbContext<BlockChainDbContext>(
            dbContextOptions => dbContextOptions
                .UseMySql(Configuration.GetConnectionString("DefaultConnection"), serverVersion)
                .EnableSensitiveDataLogging() // <-- These two calls are optional but help
                .EnableDetailedErrors());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("DocumentBlockChain",
                    new OpenApiInfo
                    {
                        Title = "DocumentBlockChain",
                        Version = "Phase 1",
                        Description = "DocumentBlockChain",
                        TermsOfService = new Uri("http://DocumentBlockChain.com"),
                        Contact = new OpenApiContact
                        {
                            Name = "Karthik CN",
                            Email = "karthikchiru1995@gmail.com",
                        },
                    }
                 );
                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                });

                //important check the class presence
                c.OperationFilter<AuthOperationFilter>();
            });
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration.GetValue<string>("JwtOptions:Issuer"),
                        ValidAudience = Configuration.GetValue<string>("JwtOptions:Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtOptions:SecurityKey")))
                    };
                });

            //The origins mentioned here will only be access the application
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                      .WithOrigins("http://localhost:3000", "http://contractmint.io", "http://operators.promena.in", "https://contractmint.io", "https://www.contractmint.io")
                      .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowCredentials();
            }));

            //Used for getting the current user who has logged in(Important if you are using the current user)
            services.AddHttpContextAccessor();
            services.AddSession();

            //Service's that are used(Important check whether all the services has been initialized)

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAdminServices, AdminServices>();
            services.AddScoped<IWebUserServices, WebUserServices>();
            services.AddScoped<IDocusign, Docusign>();
            services.AddScoped<IUserServices, UserService>();
            services.AddScoped<IEmailServices, EmailServices>();
            services.AddScoped<IPaymentServices, PaymentServices>();
            services.AddScoped<IStorage, Storage>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();



            //Not needed
            services.AddRazorPages();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHost hostBuilder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //Sends mail to the developer if there is an internal server error along with api name.
            else
            {
                var mail = ActivatorUtilities.CreateInstance<EmailServices>(hostBuilder.Services);

                app.UseExceptionHandler(options =>
                {
                    options.Run(
                        async context =>
                        {
                            var ex = context.Features.Get<IExceptionHandlerFeature>();
                            if (ex != null)
                            {
                                mail.SendExceptionMail(ex, context);
                            }
                        });
                });
            }

            app.UseStatusCodePages("text/plain", "Status code page, status code: {0}");

            app.UseHttpsRedirection();

            //EU LAW (General Data Protecting Regulation)
            app.UseCookiePolicy();

            app.UseRouting();

            //Cross Origin Resource Sharing and must be places between the routing and authentication
            app.UseCors("CorsPolicy");


            app.UseAuthentication();

            app.UseAuthorization();

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseWebSockets();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            //The name must be same that is present in the c.swaggerdoc
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("DocumentBlockChain/swagger.json", "DocumentBlockChain");
            });

            //important
            app.UseEndpoints(endpolongs =>
            {
                endpolongs.MapControllers();
            });

            //When app.Use in used, the application will be redirected to this path when the url is called.
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.HasValue && context.Request.Path.Value != "/")
                {
                    context.Response.ContentType = "text/html";

                    await context.Response.SendFileAsync(
                        env.ContentRootFileProvider.GetFileInfo("wwwroot/index.html")
                    );

                    return;
                }

                await next();
            });
        }
    }
}
