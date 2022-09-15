using Common.Behaviors;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Configuration;
using Pilotbird.Claim.EFCore.DBModels.DB;
using Pilotbird.Claim.Service.Infrastructure;
using FluentValidation.AspNetCore;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Pilotbird.Claim.EFCore.DBModels;
using DataAccess.Interfaces;
using DataAccess;

namespace Pilotbird.Claim.Service
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private Func<DbConnection> ConnectionFactory =>
           () => new SqlConnection
           (
               Configuration.GetConnectionString(nameof(ApplicationSettings.ConnectionStringKeyNameForQueryDataStore))
           );

        private TConfiguration BindConfiguration<TConfiguration>(string configurationSectionName)
           where TConfiguration : class, new()
        {
            var configuration = new TConfiguration();

            Configuration
                .GetSection(configurationSectionName)
                .Bind(configuration);

            return configuration;
        }

        private void ConfigureApplicationConfiguration(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<ApplicationSettings>(Configuration.GetSection(nameof(ApplicationSettings)));

            services.Configure<JwtConfiguration>(Configuration.GetSection(nameof(JwtConfiguration)));

            services.AddScoped(serviceProvider => serviceProvider.GetService<IOptionsSnapshot<JwtConfiguration>>().Value);
        }

        private void ConfigureJwt(IServiceCollection services)
        {
            var jwtConfiguration = BindConfiguration<JwtConfiguration>(nameof(JwtConfiguration));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = jwtConfiguration.ShouldValidateIssuer,
                        ValidateAudience = jwtConfiguration.ShouldValidateAudience,
                        ValidAudience = jwtConfiguration.ValidAudience,
                        ValidIssuer = jwtConfiguration.ValidIssuer,
                        IssuerSigningKey = jwtConfiguration.GenerateSymmetricSecurityKey()
                    };
                });
        }

        private static void ConfigureMediatr(IServiceCollection services)
        {
            var validationBehaviorType = typeof(ValidationBehavior<,>);

            services.AddMediatR(typeof(Startup), validationBehaviorType);

            validationBehaviorType
                .Assembly
                .ExportedTypes
                .Select(type => type.GetTypeInfo())
                .Where(type => type.IsClass && !type.IsAbstract)
                .ToList()
                .ForEach(type =>
                {
                    type
                        .ImplementedInterfaces
                        .Select(i => i.GetTypeInfo())
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                        .ToList()
                        .ForEach(handlerType => services.AddTransient(handlerType.AsType(), type.AsType()));
                });

            services.AddTransient(typeof(IPipelineBehavior<,>), validationBehaviorType);
        }

        private void ConfigureDataRepositories
            (
                IServiceCollection serviceCollection
            )
        {
            serviceCollection.AddTransient<IDbConnection>(_ => ConnectionFactory());

            //serviceCollection.AddTransient<IUnitOfWork>
            //    (serviceProvider => new UnitOfWork(serviceProvider.GetService<IDbConnection>()));

            //serviceCollection.AddTransient<IQueryRepositoryAsync>
            //(
            //    serviceProvider => new QueryRepositoryAsync
            //    (
            //        serviceProvider.GetService<IDbConnection>()
            //    )
            //);

            serviceCollection.AddTransient<ICommandRepositoryAsync>
            (
                serviceProvider => new CommandRepositoryAsync
                (
                    serviceProvider.GetService<IUnitOfWork>()
                )
            );
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureApplicationConfiguration(services);
            var applicationSettings = BindConfiguration<ApplicationSettings>(nameof(ApplicationSettings));
            services
               .AddCors()
               .AddControllers(options => options.Filters.Add(new ModelValidationFilter()))
                   .AddJsonOptions(options =>
                   {
                       options.JsonSerializerOptions.IgnoreNullValues = true;
                   });
            services.AddSwaggerGen();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.Configure<IISServerOptions>(options => options.AutomaticAuthentication = false)
                .AddMvc(option => option.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddFluentValidation
               (
                   fluentValidationConfiguration =>
                   {
                       fluentValidationConfiguration
                          .RegisterValidatorsFromAssemblyContaining(typeof(Startup))
                          .RegisterValidatorsFromAssembly(Assembly.LoadWithPartialName("Models"))
                          .RegisterValidatorsFromAssemblyContaining(typeof(ValidationBehavior<,>));
                   }
               ); //option => option.EnableEndpointRouting = true

            services
              .AddDbContext<PilotbirdClaimDbContext>(options =>
                              options.UseSqlServer(Configuration.GetConnectionString(applicationSettings.ConnectionStringKeyNameForQueryDataStore)));

            services
                .AddDbContext<PilotbirdClaimIdentityDbContext>(options =>
                                options.UseSqlServer(Configuration.GetConnectionString(applicationSettings.ConnectionStringKeyNameForIdentityDataStore)))
                .AddIdentity<ApplicationUser, ApplicationRole>
                (
                    options =>
                    {
                        options.User.RequireUniqueEmail = true;
                    }
                )
                .AddEntityFrameworkStores<PilotbirdClaimIdentityDbContext>()
                .AddDefaultTokenProviders();

            //  services.AddDbContext<PilotbirdClaimDbContext>(options =>
            // options.UseSqlServer("Data Source=192.168.5.10;Initial Catalog=Pilotbird_Claim;Persist Security Info=True;User ID=sa;Password=CFvgbhnj12#"))
            //.AddDbContext<PilotbirdClaimIdentityDbContext>(options => options.UseSqlServer("Data Source=192.168.5.10;Initial Catalog=Pilotbird_Claim;Persist Security Info=True;User ID=sa;Password=CFvgbhnj12#"))
            //.AddIdentity<ApplicationUser, ApplicationRole>()
            //.AddEntityFrameworkStores<PilotbirdClaimIdentityDbContext>()
            //.AddDefaultTokenProviders();

            #region Enable Swagger   
            //services.AddSwaggerGen(swagger =>
            //{
            //    swagger.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Version = "v1",
            //        Title = "JWT Token Authentication API",
            //        Description = "ASP.NET Core 3.1 Web API"
            //    });
            //    // To Enable authorization using Swagger (JWT)  
            //    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            //    {
            //        Name = "Authorization",
            //        Type = SecuritySchemeType.ApiKey,
            //        Scheme = "Bearer",
            //        BearerFormat = "JWT",
            //        In = ParameterLocation.Header,
            //        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            //    });
            //    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        {
            //              new OpenApiSecurityScheme
            //                {
            //                    Reference = new OpenApiReference
            //                    {
            //                        Type = ReferenceType.SecurityScheme,
            //                        Id = "Bearer"
            //                    }
            //                },
            //                new string[] {}
            //        }
            //    });
            //});
            #endregion
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);


            //services.AddAuthentication(option =>
            //{
            //    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            //}).AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = false,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = Configuration["Jwt:Issuer"],
            //        ValidAudience = Configuration["Jwt:Issuer"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])) //Configuration["JwtToken:SecretKey"]  
            //    };
            //});
            ConfigureJwt(services);

            ConfigureMediatr(services);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        //  public void ConfigureServices(IServiceCollection services)
        //{
        // services.AddDbContext<PilotbirdClaimDbContext>();
        //services.AddControllers();
        //  var connectionString = Configuration["ConnectionStrings::DefaultConnection"].ToString();

        //var optionsBuilder = new DbContextOptionsBuilder<PilotbirdClaimDbContext>();
        //optionsBuilder.UseSqlServer(connectionString);

        //services.AddDbContext<PilotbirdClaimDbContext>(options =>
        //                options.UseSqlServer(connectionString));

        //services
        //    .AddDbContext<PilotbirdClaimIdentityDbContext>(options => options.UseSqlServer(connectionString))
        //    .AddIdentity<ApplicationUser, ApplicationRole>()
        //    .AddEntityFrameworkStores<PilotbirdClaimIdentityDbContext>()
        //    .AddDefaultTokenProviders();

        // }
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //}
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMediator mediator, IConfiguration _config)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();

                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection()
                .UseRouting()
                .UseCors(policy =>
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                )
                .UseAuthorization()
                .UseAuthentication()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            })
            .UseMvcWithDefaultRoute();

            // Swagger Configuration in API  
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
            });
        }
    }
}
