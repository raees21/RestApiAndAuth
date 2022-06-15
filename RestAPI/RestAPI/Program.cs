using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using RestAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestAPI.DataAccess;
using RestAPI.Models;
using Serilog;
using Serilog.Events;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure logging with Serilog
builder.Logging.ClearProviders();

// Set log levels
var logger = new LoggerConfiguration()
    .WriteTo.File("Logs/event-logs.txt")
    .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
    .MinimumLevel.Information()
    .CreateLogger();

// Register Serilog
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers().AddJsonOptions( op => {
    // Use string converted values for Enums over index values
    op.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();

// Configure OAuth2 authentication (NOT SECURE: FAKE ISSUER ONLY FOR DEV USER)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])
            )
    };
});

// Configure swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "C# Level up", Version = "v1.0.0" });

    // Enable authentication from Swagger docs
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "Using the Authorization header with the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    options.AddSecurityDefinition("Bearer", securitySchema);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securitySchema, 
            new[] { "Bearer" }
        }
    });
    
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.EnableAnnotations();
});

builder.Services.AddHttpContextAccessor();

// Add DB context to dependency container
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
        sqlOptions => sqlOptions.EnableRetryOnFailure(maxRetryCount: 5))
);

// Register business logic services with dependency container
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IProductVariantService, ProductVariantService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Two Left Feet API v1"));
}

// Use ErrorController to handle exceptions
app.UseExceptionHandler("/error");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();