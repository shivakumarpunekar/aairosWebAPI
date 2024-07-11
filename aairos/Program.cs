using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using aairos.Data;
using System.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using aairos.Handular;
var builder = WebApplication.CreateBuilder(args);


// Load configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

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


// This is for devicedetail connection
builder.Services.AddDbContext<devicedetailsContext>(options =>
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
builder.Services.AddControllers()
.AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.Converters.Add(new JsonDateConverter());
 });
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
