using API.Middleware;
using BusinessObjectLayer.IService;
using BusinessObjectLayer.Services;
using DataAccessLayer.DbContxts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Lost and Found System API",
        Version = "v1",
        Description = "API for Lost and Found System with JWT Authentication"
    });

    // Map IFormFile to file type for Swagger
    options.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Chỉ cần dán JWT token, không cần gõ 'Bearer '"
    });


    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    // Add file upload operation filter
    options.OperationFilter<API.Swagger.FileUploadOperationFilter>();
});

// Configure Database
builder.Services.AddDbContext<LostAndFoundDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    );

    // Optional: Disable tracking globally nếu cần
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// Configure JWT Authentication
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ========== Register Services from BusinessObjectLayer ==========
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICampusService, CampusService>();
builder.Services.AddScoped<IServiceLocationService, ServiceLocationService>();
builder.Services.AddScoped<IItemService, ItemService>();  // ✅ Thêm dòng này
//builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IReturnRecordService, ReturnRecordService>();
// Thêm các service khác ở đây nếu cần
// builder.Services.AddScoped<ICategoryService, CategoryService>();
// builder.Services.AddScoped<IUserService, UserService>();

// Register generic repository and concrete repositories so DI can supply repositories with the DbContext
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped(typeof(Repository.GenericRepository<>));
builder.Services.AddScoped<Repository.UploadRepository>();
builder.Services.AddScoped<Repository.ItemRepository>();
builder.Services.AddScoped<Repository.ServiceLocationrepository>();



// Configure CORS if needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Global Exception Handler - Must be first
app.UseGlobalExceptionHandler();

// Enable Swagger always (or use if (app.Environment.IsDevelopment()) for production safety)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lost and Found System API v1");
    c.RoutePrefix = "swagger"; // Swagger UI will be available at /swagger
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
    c.EnableValidator();
});

app.UseHttpsRedirection();

// Enable static files for serving uploaded files
app.UseStaticFiles();

// Enable CORS
app.UseCors("AllowAll");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();