using LazyCache;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Test.DataAccess;
using Test.Application.Services;
using Test.DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ConfigurationManager = Test.API.ConfigurationManager;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.Extensions.Logging;
using Test.API.ActionFilter;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("V1", new OpenApiInfo
    {
        Version = "V1",
        Title = "WebAPI",
        Description = "Product WebAPI"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddLogging();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddLazyCache();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddSingleton(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("DefaultLogger"));
builder.Services.AddScoped<LogResponseTimeFilter>();


builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = ConfigurationManager.AppSetting["JWT:ValidIssuer"],
            ValidAudience = ConfigurationManager.AppSetting["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSetting["JWT:Secret"]))
        };
    });

builder.Services.Configure<DataProtectionTokenProviderOptions>(options => { options.TokenLifespan = TimeSpan.FromHours(2); });
builder.Services.AddControllers(options =>
{
    options.Filters.Add<LogResponseTimeFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins("http://localhost:4200", "https://metropolicuatrotechsolutions.com")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/V1/swagger.json", "Product WebAPI");
    }); app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseCors();

app.Run();

