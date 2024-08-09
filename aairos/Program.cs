using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using aairos.Data;
using System.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using aairos.Services;
var builder = WebApplication.CreateBuilder(args);


// Load configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();


// Add services to the container
builder.Services.AddSingleton<FileLoggerService>(provider =>
    new FileLoggerService("log.txt", TimeSpan.FromDays(7)));

/*var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);*/

// Add services to the container
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Issuer"],
        /*IssuerSigningKey = new SymmetricSecurityKey(key)*/
    };
});

// This is for ThresholdContext connection
builder.Services.AddDbContext<ThresholdContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Defaltconnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

// This is for userdevices connection
builder.Services.AddDbContext<UserDeviceContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Defaltconnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

// This is for sensor_data connection
builder.Services.AddDbContext<sensor_dataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Defaltconnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

// This is for devicedetail connection
builder.Services.AddDbContext<devicedetailContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Defaltconnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

// This is for device connection
builder.Services.AddDbContext<deviceContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Defaltconnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

//This is for user profile connection
builder.Services.AddDbContext<userprofileContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Defaltconnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

//This is a Login Connection
builder.Services.AddDbContext<LoginContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("Defaltconnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));


builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
