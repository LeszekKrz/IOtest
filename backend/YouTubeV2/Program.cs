using FluentValidation;
using Microsoft.EntityFrameworkCore;
using YouTubeV2.Application;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Validator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString = builder.Configuration.GetConnectionString("Db");
builder.Services.AddDbContext<YTContext>(
    options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<UserService>();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
