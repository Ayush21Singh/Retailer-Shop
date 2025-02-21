using AshishGeneralStore.Data;
using Microsoft.EntityFrameworkCore;
using AshishGeneralStore.Services.Admin;
using AshishGeneralStore.Config;
using AshishGeneralStore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureAuthentication(); 
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.RegisterApplicationServices();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();