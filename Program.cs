using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers().AddJsonOptions(o =>
{
	o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// EF Core - InMemory for local development
builder.Services.AddDbContext<EnterpriseAPI.Data.AppDbContext>(options =>
	options.UseInMemoryDatabase("EnterpriseDb"));

// CORS - allow local frontend
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
		policy.AllowAnyOrigin()
			.AllowAnyHeader()
			.AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseCors();

// No HTTPS redirection to keep http calls simple during development
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// Minimal API sample removed in favor of controllers
