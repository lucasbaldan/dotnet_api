using System.Text;
using Asp.Versioning;
using dotnet_api.Models;
using dotnet_api.Database;
using dotnet_api.Services;
using dotnet_api.Middlewares;
using dotnet_api.Repositories;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.RateLimiting;
using dotnet_api.Shared.DTOs.AutoMapper;
using dotnet_api.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "apicatalogo", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT ",
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

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("String de conexão não encontrada ou vazia!");
}
builder.Services.AddDbContext<BDContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddIdentity<Usuario, GrupoUsuarios>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
    options.Password.RequiredUniqueChars = 0;

    options.User.RequireUniqueEmail = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = false;

}).AddEntityFrameworkStores<BDContext>().AddDefaultTokenProviders();

var secretKey = builder.Configuration["JWT:SecretKey"] ?? throw new InvalidOperationException("ERRO CRÍTICO! Chave secreta não encontrada!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://google.com").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient("ViaCep", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalServices:ViaCep"]!);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = int.Parse(builder.Configuration["RateLimit:FixedWindow:Limit"] ?? "3");
        options.Window = TimeSpan.FromSeconds(int.Parse(builder.Configuration["RateLimit:FixedWindow:TimeWindowSeconds"] ?? "5"));
        options.QueueProcessingOrder = int.Parse(builder.Configuration["RateLimit:FixedWindow:QueueLimit"] ?? "0") > 0 ? QueueProcessingOrder.OldestFirst : 0;
        options.QueueLimit = int.Parse(builder.Configuration["RateLimit:FixedWindow:QueueLimit"] ?? "0");
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
builder.Services.AddScoped<ITransaction, Transaction>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionService>();
builder.Services.AddScoped<IViaCepService, ViaCepService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandlingMiddleware();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseMiddleware<UserAuthtenticationMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
