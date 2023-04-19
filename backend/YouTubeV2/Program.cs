using Azure.Storage.Blobs;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using YouTubeV2.Api.Middleware;
using YouTubeV2.Application;
using YouTubeV2.Application.Configurations.BlobStorage;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Providers;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Services.BlobServices;
using YouTubeV2.Application.Services.JwtFeatures;
using YouTubeV2.Application.Services.VideoServices;
using YouTubeV2.Application.Validator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hmmmm", Version = "1.0" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Insert Token provided on successful login",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                new OpenApiSecurityScheme
                {
                Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,

                },
                new List<string>()
                }
            });
});
builder.Services.AddOptions<BlobStorageImagesConfig>().Bind(builder.Configuration.GetSection("BlobStorageImagesConfig"));
builder.Services.AddOptions<BlobStorageVideosConfig>().Bind(builder.Configuration.GetSection("BlobStorageVideosConfig"));

string connectionString = builder.Configuration.GetConnectionString("Db")!;
builder.Services.AddDbContext<YTContext>(
    options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddSingleton(x => new BlobServiceClient(Environment.GetEnvironmentVariable("AZURE_BLOB_STORAGE_CONNECTION_STRING")));
builder.Services.AddTransient<ISubscriptionService, SubscriptionService>();
builder.Services.AddTransient<IPlaylistService, PlaylistService>();
builder.Services.AddSingleton<IBlobImageService, BlobImageService>();
builder.Services.AddSingleton<IBlobVideoService, BlobVideoService>();
builder.Services.AddTransient<IVideoService, VideoService>();
builder.Services.AddTransient<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddSingleton<IVideoProcessingService, VideoProcessingService>();
builder.Services.AddHostedService(serviceProvider => (VideoProcessingService)serviceProvider.GetRequiredService<IVideoProcessingService>());

builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<YTContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Allow ALL",
        policyBuilder => policyBuilder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin()
            .WithExposedHeaders("Content-Disposition"));
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
       options.TokenLifespan = TimeSpan.FromHours(2));

var jwtSettings = new JwtSettings(builder.Configuration.GetSection("JWTSettings"));
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<JwtHandler>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.ValidIssuer,
        ValidAudience = jwtSettings.ValidAudience,
        IssuerSigningKey = new SymmetricSecurityKey(jwtSettings.SecurityKey)
    };
});
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Allow ALL");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }