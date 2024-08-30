using aairos.Data;
using aairos.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


namespace aairos
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            // Register FileLoggerService with a specific log file path and log retention period
            services.AddSingleton<FileLoggerService>(provider =>
                new FileLoggerService("log.txt", TimeSpan.FromDays(7)));

            //Add JWT authentication
            var secretKey = Configuration["JwtSettings:SecretKey"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "yourdomain.com", // Replace with your actual domain
                    ValidAudience = "yourdomain.com", // Replace with your actual domain
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddAuthorization();

            //This is a cors
            services.AddCors(options =>
            {
                options.AddPolicy("ReactNativeAppPolicy",
                    builder =>
                    {
                        builder.WithOrigins("http://10.0.2.2:8081")
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            services.AddControllers();




            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "GenerateJwtToken",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            // Other service configurationsDefaltconnection
            services.AddDbContext<LoginContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Defaltconnection"),
                new MySqlServerVersion(new Version(8, 0, 21))));

            services.AddDbContext<userprofileContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Defaltconnection"),
                new MySqlServerVersion(new Version(8, 0, 21))));

            services.AddDbContext<deviceContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Defaltconnection"),
                new MySqlServerVersion(new Version(8, 0, 21))));

            services.AddDbContext<devicedetailContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Defaltconnection"),
                new MySqlServerVersion(new Version(8, 0, 21))));

            services.AddDbContext<sensor_dataContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Defaltconnection"),
                new MySqlServerVersion(new Version(8, 0, 21))));

            services.AddDbContext<UserDeviceContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Defaltconnection"),
                new MySqlServerVersion(new Version(8, 0, 21))));

            services.AddDbContext<ThresholdContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Defaltconnection"),
                new MySqlServerVersion(new Version(8, 0, 21))));

            services.AddDbContext<ValveStatusContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Defaltconnection"),
                new MySqlServerVersion(new Version(8, 0, 21))));

            services.AddDbContext<relay_durationsContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Defaltconnection"),
                new MySqlServerVersion(new Version(8, 0, 21))));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //Use CORS
            app.UseCors("ReactNativeAppPolicy");

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
