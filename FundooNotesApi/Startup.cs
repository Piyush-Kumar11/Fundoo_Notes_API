using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerLayer.Interfaces;
using ManagerLayer.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;

namespace FundooNotesApi
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
            // Adds support for MVC controllers to the service container.
            services.AddControllers();

            /*
             * Registers the FundooDBContext with the dependency injection container.
             * Configures the DbContext to use SQL Server with the connection string specified in the configuration file.
             */
            services.AddDbContext<FundooDBContext>(a => a.UseSqlServer(Configuration["ConnectionStrings:DBConnection"]));

            // Registers IUserRepository and UserRepository for dependency injection with transient lifetime.
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserManager, UserManager>();

            // Registers INotesRepository and NotesRepository for dependency injection with Scoped lifetime.
            services.AddScoped<INotesRepository, NotesRepository>();
            services.AddScoped<INotesManager, NotesManager>();

            // Registers ILabelRepository and LabelRepository for dependency injection with Scoped lifetime.
            services.AddScoped<ILabelRepository, LabelRepository>();
            services.AddScoped<ILabelManager, LabelManager>();

            // Register Collaborator Dependencies
            services.AddScoped<ICollaboratorRepository, CollaboratorRepository>();
            services.AddScoped<ICollaboratorManager, CollaboratorManager>();

            /*
             * Configures Swagger for API documentation generation.
             * Defines API version and title for Swagger documentation.
             * Adds a security definition for JWT-based authentication in Swagger UI.
             */
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JWTToken_Auth_API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
                });
            });

            /*
             * Configures MassTransit to use RabbitMQ as the message broker.
             * Sets up the RabbitMQ host and credentials.
             * Enables health checks for the RabbitMQ connection.
             */
            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.UseHealthCheck(provider);
                    config.Host(new Uri("rabbitmq://localhost"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                }));
            });

            // Adds MassTransit hosted service for managing the lifecycle of MassTransit components.
            services.AddMassTransitHostedService();

            // Configures authentication services using JWT bearer tokens.
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Default scheme for authentication
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;    // Default scheme for challenges
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;             // Default scheme for token handling
            })
            .AddJwtBearer(options => {
                options.SaveToken = true;                     // Saves the token once validated
                options.RequireHttpsMetadata = false;         // Allows non-HTTPS requests during development
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,                   // Disables issuer validation for development
                    ValidateAudience = false,                 // Disables audience validation for development
                    ValidAudience = Configuration["Jwt:Audience"], // Expected audience of the token
                    ValidIssuer = Configuration["Jwt:Issuer"],     // Expected issuer of the token
                    IssuerSigningKey = new SymmetricSecurityKey(   // Key used for token signing
                        Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });
            services.AddStackExchangeRedisCache(options => { options.Configuration = Configuration["RedisCacheUrl"]; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Checks if the application is in the development environment.
            // If true, it enables the developer exception page for detailed error information.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enables authentication middleware to validate and process authentication tokens.
            app.UseAuthentication();

            // Redirects all HTTP requests to HTTPS for secure communication.
            app.UseHttpsRedirection();

            // Adds routing middleware to the pipeline for matching HTTP requests to routes.
            app.UseRouting();

            // Serves the generated Swagger document as a JSON endpoint for API documentation.
            app.UseSwagger();

            // Serves the Swagger UI, which provides an interactive interface for API documentation.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fundoo API V1");
            });

            // Enables authorization middleware to enforce authorization policies on endpoints.
            app.UseAuthorization();

            // Configures the endpoints for the application, mapping controller actions to routes.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Maps controller endpoints.
            });
        }

    }
}
