using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using transaction_infrastructure.Persistence;
using transaction_ms.Configure;
using transaction_ms.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.EnableAnnotations();
    var xmlName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlName);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Transaction API",
        Version = "v1",
        Description = "API para crear y consultar transacciones (CQRS + MediatR).",
    });
    c.EnableAnnotations();
});

var app = builder.Build();

app.UseSwagger(c => {
    c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transaction API v1");
    c.RoutePrefix = "swagger";
});

var application = app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
await Task.Delay(1000 * 3);
var pendingMigrations = await application.Database.GetPendingMigrationsAsync();
if (pendingMigrations != null)
    await application.Database.MigrateAsync();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
